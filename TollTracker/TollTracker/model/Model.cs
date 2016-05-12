using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TollTracker.model
{
    /// <summary>
    /// Obecný model s daty celé aplikace
    /// </summary>
    public class Model : DBConnector
    {
        private Action<int, string> oneTollErrorCallback; //funkce, která bude zavolána v případě chyby při parsování jednoho záznamu mýta (jako parametry má funkce pořadí mýta a chybovou hlášku)
        private string errMes; //poslední chybová hláška ke které došlo při práci s databází
        private Action<int> showNumberOfProcessedTolls; //funkce, která bude zavolána v případě zpracování jednoho mýta (jako parametr má funkce počet zpracovaných mýt)
        private int count = 0; //počet právě zpracovaných záznamů při vkládání do databáze v jednom bundlu
        private List<string> roads = new List<string>(); //seznam připravených hodnot silnic pro vložení do datbáze
        private List<string> cars = new List<string>(); //seznam připravených hodnot aut pro vložení do datbáze
        private List<string> gates = new List<string>(); //seznam připravených hodnot bran pro vložení do datbáze
        private List<string> gpsGates = new List<string>(); //seznam připravených hodnot gps bran pro vložení do datbáze
        private List<string> tolls = new List<string>(); //seznam připravených hodnot mýt pro vložení do datbáze
        Action<string> errorCallback; //callback v případě závažné chyby

        public Model()
        {
            if (openConnection())
            {
                deleteAll();
                inicializeDB();
                closeConnection();
            }
        }

        /// <summary>
        /// Přečte daný soubor a nahraje jeho obsah do databáze
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při čtení souboru a parsování souboru (jako parametr má funkce chybovou hlášku)</param>
        /// <param name="oneTollErrorCallback">funkce, která bude zavolána v případě chyby při parsování jednoho konkrétního mýta (jako parametr má funkce chybovou hlášku a pořadí mýta)</param>
        /// <param name="showNumberOfProcessedTolls">funkce, která bude zavolána v případě zpracování jednoho mýta (jako parametr má funkce počet zpracovaných mýt)</param>
        /// <returns>false pokud nastala při čtení a parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        public bool readFile(string pathToFile, Action<string> errorCallback, Action<int, string> oneTollErrorCallback, Action<int> showNumberOfProcessedTolls)
        {
            this.errorCallback = errorCallback;
            this.oneTollErrorCallback = oneTollErrorCallback;
            this.showNumberOfProcessedTolls = showNumberOfProcessedTolls;
            if (openConnection())
            {
                try
                {
                    char firstChar;
                    try
                    {
                        firstChar = readFirstNotWhiteSpaceChar(pathToFile);
                    }
                    catch (IOException ex)
                    {
                        errorCallback("Došlo k chybě při čtení souboru: " + ex.Message);
                        return false;
                    }

                    if (firstChar == '<')
                    {
                        if (!parseXML(pathToFile, errorCallback))
                        {
                            return false;
                        }
                    }
                    else if (firstChar == '{')
                    {
                        if (!parseJSON(pathToFile, errorCallback))
                        {
                            return false;
                        }
                    }
                    else {
                        errorCallback("Obsah souboru není v čitelném formátu (XML nebo JSON)");
                        return false;
                    }
                    return true;
                }
                finally
                {
                    closeConnection();
                }
            }
            else {
                errorCallback("Nepodařilo se připojit k databázi");
                return false;
            }
        }

        /// <summary>
        /// Získá z databáze informace o zvolené bráně záznam o mýtném s mýtnou branou
        /// </summary>
        /// <param name="gateId">id mýtné brány</param>
        /// <returns>průjezdy aut zvolenou mýtnou branou</returns>
        public List<List<String>> getGateReport(String gateId)
        {
            int outvalue;
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT car.spz, car.type, whenn FROM toll ");
            builder.Append("JOIN car ON toll.car_spz = car.spz ");
            if (Int32.TryParse(gateId, out outvalue))
            {
                builder.Append("WHERE((toll.gps_gate_id IS NOT NULL) AND (toll.gps_gate_id = '");
                builder.Append(gateId);
                builder.Append("'))");
            }
            else
            {
                builder.Append("WHERE((toll.toll_gate_id IS NOT NULL) AND (toll.toll_gate_id = '");
                builder.Append(gateId);
                builder.Append("'))");
            }

            return getSelectResults(builder.ToString());
        }

        /// <summary>
        /// Získá z databáze informace o sumě vybraných peněz pro každý druh vozidla
        /// </summary>
        /// <returns>obnosy vybraných peněz</returns>
        public List<List<String>> getTollsSummary()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT 'Auta pod 3.5t' AS type, SUM(price) FROM toll ");
            builder.Append("LEFT JOIN car ON car.spz = toll.car_spz ");
            builder.Append("WHERE car.type = 'below 3.5t' ");
            builder.Append("UNION ");
            builder.Append("SELECT 'Auta nad 3.5t' AS type,SUM(price) FROM toll ");
            builder.Append("LEFT JOIN car ON car.spz = toll.car_spz ");
            builder.Append("WHERE car.type = 'over 3.5t'");

            return getSelectResults(builder.ToString());
        }

        /// <summary>
        /// Získá z databáze informace o penězích zaplacených daným vozidlem za zvolenou dobu
        /// pro každý druh silnice
        /// </summary>
        /// /// <param name="spz">spz vozidla</param>
        /// /// <param name="from">termín od kdy</param>
        /// /// <param name="to">termín do kdy</param>
        /// <returns>obnosy vybraných peněz</returns>
        public List<List<String>> getVehicleToll(String spz, DateTime from, DateTime to)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COALESCE(r1.type, r2.type) AS type, SUM(price) AS s FROM toll ");
            builder.Append("LEFT JOIN gps_gate ON gps_gate.id = toll.gps_gate_id ");
            builder.Append("LEFT JOIN road r1 ON gps_gate.road_number = r1.number ");
            builder.Append("LEFT JOIN toll_gate ON toll_gate.id = toll.toll_gate_id ");
            builder.Append("LEFT JOIN road r2 ON toll_gate.road_number = r2.number ");
            builder.Append("WHERE car_spz = '");
            builder.Append(spz);
            builder.Append("' AND ((toll.gps_gate_id IS NOT NULL) OR(toll.toll_gate_id IS NOT NULL)) ");
            builder.Append("AND whenn >= '");
            builder.Append(new NpgsqlTypes.NpgsqlDateTime(from));
            builder.Append("' AND whenn < '");
            builder.Append(new NpgsqlTypes.NpgsqlDateTime(to));
            builder.Append("' GROUP BY COALESCE(r1.type, r2.type)");

            return getSelectResults(builder.ToString());
        }

        /// <summary>
        /// Získá z databáze informace o pohybu vozidla
        /// </summary>
        /// <param name="spz">id mýtné brány</param>
        /// <returns>seznam pozic a časů, kde se vozidlo pohybovalo</returns>
        public List<List<String>> getVehicleTrackingData(String spz)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT whenn, gps_gate.road_number, toll_gate.road_number, price FROM toll ");
            builder.Append("LEFT JOIN gps_gate ON gps_gate.id = toll.gps_gate_id ");
            builder.Append("LEFT JOIN toll_gate ON toll_gate.id = toll.toll_gate_id ");
            builder.Append("WHERE (toll.gps_gate_id IS NOT NULL  OR toll.toll_gate_id IS NOT NULL) ");
            builder.Append("AND car_spz = '");
            builder.Append(spz);
            builder.Append("' ORDER BY whenn");

            return getSelectResults(builder.ToString());
        }

        /// <summary>
        /// Získá z databáze všechna vozidla
        /// </summary>
        /// <returns>List obsahující spz všech aut</returns>
        public List<List<String>> getAllVehicles()
        {
            return getSelectResults("SELECT spz FROM car");
        }

        /// <summary>
        /// Získá z databáze všechny brány
        /// </summary>
        /// <returns>List všech mýtných bran</returns>
        public List<List<String>> getAllGates()
        {
            return getSelectResults("SELECT id FROM toll_gate UNION SELECT CAST(id as text) FROM gps_gate");
        }

        /// <summary>
        /// Exportuje výsledky vehicleTrackingReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportVehicleTrackingReportToXML(string pathToFile, ListView.ListViewItemCollection data)
        {
            XMLParser parser = new XMLParser();
            parser.exportVehicleTrackingReport(pathToFile, data);
        }

        /// <summary>
        /// Exportuje výsledky vehicleTollReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportVehicleTollReportToXML(string pathToFile, ListView.ListViewItemCollection data)
        {
            XMLParser parser = new XMLParser();
            parser.exportVehicleTollReport(pathToFile, data);
        }

        /// <summary>
        /// Exportuje výsledky tollsSummaryReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportTollsSummaryReportToXML(string pathToFile, ListView.ListViewItemCollection data)
        {
            XMLParser parser = new XMLParser();
            parser.exportTollsSummaryReport(pathToFile, data);
        }

        /// <summary>
        /// Exportuje výsledky gateReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportGateReportToXML(string pathToFile, ListView.ListViewItemCollection data)
        {
            XMLParser parser = new XMLParser();
            parser.exportGateReport(pathToFile, data);
        }

        /****************************************************************PRIVATE*******************************************************/

        /// <summary>
        /// smaže obsah všech tabulek, do kterých se nahrávají data
        /// </summary>
        private void deleteAll()
        {
            deleteAllFromTable("toll", false);
            deleteAllFromTable("gps_gate", true);
            deleteAllFromTable("car", true);
            deleteAllFromTable("road", true);
        }

        /// <summary>
        /// smaže obsah dané tabulky
        /// </summary>
        /// <param name="tableName">název tabulky</param>
        /// <param name="cascade">zda kaskádově mazat z ostatních tabulek</param>
        /// <returns></returns>
        private void deleteAllFromTable(string tableName, bool cascade)
        {
            string query = "TRUNCATE TABLE " + tableName;
            if (cascade)
            {
                query += " CASCADE";
            }
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Nahraje do databáze typy aut, silnic a bran
        /// </summary>
        private void inicializeDB()
        {
            insertType("car_type", "over 3.5t");
            insertType("car_type", "below 3.5t");
            insertType("gate_type", "large");
            insertType("gate_type", "medium");
            insertType("road_type", "Dalnice");
            insertType("road_type", "II.Tr");
            insertType("road_type", "I.Tr");
        }

        /// <summary>
        /// Vloží typ něčeho do databáze
        /// </summary>
        /// <param name="tableName">jméno tabulky typu</param>
        /// <param name="typeName">hodnota typu</param>
        private void insertType(string tableName, string typeName)
        {
            try
            {
                string query = "INSERT INTO " + tableName + " (name) VALUES('" + typeName + "')";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Rozparsování JSON dat a uložení do databáze
        /// </summary>
        /// <param name="pathToFile">cesta k JSON souboru</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        private bool parseJSON(string pathToFile, Action<string> errorCallback)
        {
            JSONParser parser = new JSONParser();
            if (!parser.loadData(pathToFile, errorCallback, oneTollErrorCallback, insertTollWithGPS))
            {
                return false;
            }
            insertGPSBundle();
            return true;
        }

        /// <summary>
        /// Rozparsování XML dat a uložení do databáze
        /// </summary>
        /// <param name="pathToFile">cesta k XML souboru</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        private bool parseXML(string pathToFile, Action<string> errorCallback)
        {
            XMLParser parser = new XMLParser();
            if (!parser.loadData(pathToFile, errorCallback, oneTollErrorCallback, insertTollWithTollGate))
            {
                return false;
            }
            insertGateBundle();
            return true;
        }

        /// <summary>
        /// Přečte všechny bílé znaky a vrátí až první nebílý znak
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <returns>první nebílý znak</returns>
        private char readFirstNotWhiteSpaceChar(string pathToFile)
        {
            using (StreamReader streamReader = new StreamReader(pathToFile, Encoding.UTF8))
            {
                char[] c = new char[1];
                do
                {
                    streamReader.Read(c, 0, 1);
                } while (Char.IsWhiteSpace(c[0]) && streamReader.Peek() >= 0);
                return c[0];
            }
        }

        /// <summary>
        /// Vloží do databáze jeden záznam o mýtném s mýtnou branou
        /// Nejdříve je potřeba otevřít připojení a pak zase zavřít.
        /// </summary>
        /// <param name="when">kdy bylo mýto zaznamenáno</param>
        /// <param name="price">cena mýtného</param>
        /// <param name="roadNumber">idetifikační číslo silnice</param>
        /// <param name="roadType">typ silnice (1. třídy, 2. třídy, dálnice)</param>
        /// <param name="carType">typ auta (menší 3.5 t, větší 3.5 t)</param>
        /// <param name="SPZ">SPZ auta, u kterého bylo mýto zaznamenáno</param>
        /// <param name="gateId">id mýtné brány</param>
        /// <param name="gateType">typ mýtné brány (small, medium, large)</param>
        /// <returns>true pokud je záznam validní</returns>
        private void insertTollWithTollGate(int number, DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, string gateId, string gateType)
        {
            if (count > 5000)
            {
                showNumberOfProcessedTolls(number);
                insertGateBundle();
                count = 0;
            }
            else
            {
                count++;
                roads.Add("('" + roadNumber + "', '" + roadType + "')");
                cars.Add("('" + SPZ + "', '" + carType + "')");
                gates.Add("('" + gateId + "', '" + gateType + "', '" + roadNumber + "')");
                tolls.Add("('" + when + "', '" + price.ToString(CultureInfo.CreateSpecificCulture("en-us")) + "', '" + SPZ + "', '" + gateId + "')");
            }
        }

        /// <summary>
        /// Vloží do databáze jeden záznam o mýtném s GPS souřadnicema
        /// </summary>
        /// <param name="when">kdy bylo mýto zaznamenáno</param>
        /// <param name="price">cena mýtného</param>
        /// <param name="roadNumber">idetifikační číslo silnice</param>
        /// <param name="roadType">typ silnice (1. třídy, 2. třídy, dálnice)</param>
        /// <param name="carType">typ auta (menší 3.5 t, větší 3.5 t)</param>
        /// <param name="SPZ">SPZ auta, u kterého bylo mýto zaznamenáno</param>
        /// <param name="GPSLongitude">zeměpisná délka GPS souřadnic, kde bylo mýto zaznamenáno</param>
        /// <param name="GPSLatitude">zeměpisná šířka GPS souřadnic, kde bylo mýto zaznamenáno</param>
        /// <param name="GPSAccuracy">přesnost GPS souřadnic, kde bylo mýto zaznamenáno</param>
        private void insertTollWithGPS(int number, DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, double GPSLongitude, double GPSLatitude, int GPSAccuracy)
        {
            if (count > 5000)
            {
                showNumberOfProcessedTolls(number);
                insertGateBundle();
                count = 0;
            }
            else
            {
                count++;
                roads.Add("('" + roadNumber + "', '" + roadType + "')");
                cars.Add("('" + SPZ + "', '" + carType + "')");
                int GPSGateId = insertGPSGate(GPSLongitude, GPSLatitude, GPSAccuracy, roadNumber);
                if (GPSGateId == -1) {
                    oneTollErrorCallback(number, errMes);
                    return;
                }
                tolls.Add("('" + when + "', '" + price.ToString(CultureInfo.CreateSpecificCulture("en-us")) + "', '" + SPZ + "', '" + GPSGateId + "')");
            }
        }

        /// <summary>
        /// Vloží do databáze aktuálně ozparsovaný pakl všech záznamů. (s branou)
        /// </summary>
        private void insertGateBundle()
        {
            insertRoads(roads);
            insertCars(cars);
            insertTollGates(gates);
            insertGateTolls(tolls);
        }

        /// <summary>
        /// Vloží do databáze aktuálně ozparsovaný pakl všech záznamů. (s GPS)
        /// </summary>
        private void insertGPSBundle()
        {
            insertRoads(roads);
            insertCars(cars);
            insertGPSTolls(tolls);
        }

        protected override void postgresNotification(object sender, NpgsqlNotificationEventArgs e)
        {
            oneTollErrorCallback(0, e.AdditionalInformation);
        }

        /// <summary>
        /// Vloží do databáze silnice s danými parametry
        /// </summary>
        /// <param name="roads">list stringů obsahujících připravené hodnoty jednotlivých silnic</param>
        private void insertRoads(List<string> roads)
        {
            string queryInsertRoads = "INSERT INTO road (number, type) VALUES";
            queryInsertRoads += string.Join(",", roads);
            roads.Clear();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(queryInsertRoads, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errorCallback(ex.Message);
            }
        }

        /// <summary>
        /// Vloží do databáze auta s danými parametry
        /// </summary>
        /// <param name="cars">list stringů obsahujících připravené hodnoty jednotlivých aut</param>
        private void insertCars(List<string> cars)
        {
            string queryInsertCars = "INSERT INTO car (SPZ, type) VALUES";
            queryInsertCars += string.Join(",", cars);
            cars.Clear();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(queryInsertCars, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errorCallback(ex.Message);
            }
        }

        /// <summary>
        /// Vloží do databáze mýtné brány s danými parametry
        /// </summary>
        /// <param name="gates">list stringů obsahujících připravené hodnoty jednotlivých bran</param>
        private void insertTollGates(List<string> gates)
        {
            if (gates.Count == 0)
            {
                return;
            }
            string queryInsertGates = "INSERT INTO toll_gate (id, type, road_number) VALUES";
            queryInsertGates += string.Join(",", gates);
            gates.Clear();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(queryInsertGates, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errorCallback(ex.Message);
            }
        }

        /// <summary>
        /// Vloží do databáze GPS souřadnice mýta danými parametry
        /// </summary>
        /// <param name="longitude">zeměpisná délka GPS souřadnic</param>
        /// <param name="latitude">zeměpisná šířka GPS souřadnic</param>
        /// <param name="accuracy">přesnost GPS souřadnic</param>
        /// <param name="road_number">číslo silnice, na které byly GPS souřadnice zachyceny</param>
        /// <returns>pokud se povedlo, tak vrací id a jinak -1</returns>
        private int insertGPSGate(double longitude, double latitude, int accuracy, string road_number)
        {
            try
            {
                string query = "INSERT INTO gps_gate (longitude, latitude, accuracy, road_number) VALUES(" + longitude.ToString(CultureInfo.CreateSpecificCulture("en-us")) + ", " + latitude.ToString(CultureInfo.CreateSpecificCulture("en-us")) + ", " + accuracy + ", '" + road_number + "') RETURNING id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                return (int)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                errMes = ex.Message;
                return -1;
            }
        }

        /// <summary>
        /// Vloží do databáze mýta s danými parametry (s mýtnou branou)
        /// </summary>
        /// <param name="tolls">list stringů obsahujících připravené hodnoty jednotlivých mýt</param>
        private void insertGateTolls(List<string> tolls)
        {
            string queryInsertTolls = "INSERT INTO toll (whenn, price, car_spz, toll_gate_id) VALUES";
            queryInsertTolls += string.Join(",", tolls);
            tolls.Clear();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(queryInsertTolls, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errorCallback(ex.Message);
            }
        }

        /// <summary>
        /// Vloží do databáze mýta s danými parametry (s GPS branou)
        /// </summary>
        /// <param name="tolls">list stringů obsahujících připravené hodnoty jednotlivých mýt</param>
        private void insertGPSTolls(List<string> tolls)
        {
            string queryInsertTolls = "INSERT INTO toll (whenn, price, car_spz, gps_gate_id) VALUES";
            queryInsertTolls += string.Join(",", tolls);
            tolls.Clear();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(queryInsertTolls, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errorCallback(ex.Message);
            }
        }

        /// <summary>
        /// Získá z databáze výsledky pro zadaný dotaz
        /// </summary>
        /// <param name="query">SQL dotaz</param>
        /// <returns>List obsahující výsledky dotazu</returns>
        private List<List<String>> getSelectResults(String query)
        {
            if (openConnection())
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    NpgsqlDataReader dr = command.ExecuteReader();
                    List<List<String>> queryResult = new List<List<string>>();

                    for (int i = 0; i < dr.VisibleFieldCount; i++)
                    {
                        queryResult.Add(new List<string>());
                    }

                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.VisibleFieldCount; i++)
                        {
                            queryResult[i].Add(dr[i].ToString());
                        }

                    }
                    return queryResult;
                }
                catch (Exception ex)
                {
                    errMes = ex.Message;
                    return null;
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                return null;
            }
        }
    }
}