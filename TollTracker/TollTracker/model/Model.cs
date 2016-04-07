using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollTracker.model
{
    /// <summary>
    /// Obecný model s daty celé aplikace
    /// </summary>
    class Model : DBConnector
    {
        private HashSet<string> gateTypeSet; //množina typů bran
        private HashSet<string> carTypeSet; //množina typů aut
        private HashSet<string> roadTypeSet; //množina typů silnic
        private enum ValidType { validNonPresent, nonValid, validPresent }; //výčtový typ pro návratové hodnoty metod pro kontrolu typů { validní typ pro nový prvek; nevalidní typ; validní typ pro prvek, který už je v databázi} 

        public Model()
        {
            if (openConnection())
            {
                gateTypeSet = getTypeMap("gate_type");
                carTypeSet = getTypeMap("car_type");
                roadTypeSet = getTypeMap("road_type");
                closeConnection();
            }
        }

        /// <summary>
        /// Přečte daný soubor a nahraje jeho obsah do databáze
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při čtení souboru a parsování souboru (jako parametr má funkce chybovou hlášku)</param>
        /// <returns>false pokud nastala při čtení a parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        public bool readFile(string pathToFile, Action<string> errorCallback)
        {
            string fileContent;
            try
            {
                fileContent = readFile(pathToFile).Trim();
            }
            catch (Exception ex)
            {
                errorCallback("Došlo k chybě při čtení souboru: " + ex.Message);
                return false;
            }

            if (fileContent.Substring(0, 5) == "<?xml")
            {
                if (!parseXML(fileContent, errorCallback))
                {
                    return false;
                }
            }
            else if (fileContent.Substring(0, 1) == "{" || fileContent.Substring(0, 1) == "[")
            {
                if (!parseJSON(fileContent, errorCallback))
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

        /// <summary>
        /// Získá z databáze informace o zvolené bráně záznam o mýtném s mýtnou branou
        /// </summary>
        /// <param name="gateId">id mýtné brány</param>
        /// <returns>průjezdy aut zvolenou mýtnou branou</returns>
        internal bool getGateReport(int gateId)
        {

        }

        /// <summary>
        /// Získá z databáze informace o sumě vybraných peněz pro každý druh vozidla
        /// </summary>
        /// <returns>obnosy vybraných peněz</returns>
        internal bool getTollsSummary()
        {

        }

        /// <summary>
        /// Získá z databáze informace o penězích zaplacených daným vozidlem za zvolenou dobu
        /// pro každý druh silnice
        /// </summary>
        /// /// <param name="spz">spz vozidla</param>
        /// /// <param name="from">termín od kdy</param>
        /// /// <param name="to">termín do kdy</param>
        /// <returns>obnosy vybraných peněz</returns>
        internal bool getVehicleToll(String spz, DateTime from, DateTime to)
        {

        }

        /// <summary>
        /// Získá z databáze informace o pohybu vozidla
        /// </summary>
        /// <param name="spz">id mýtné brány</param>
        /// <returns>seznam pozic a časů, kde se vozidlo pohybovalo</returns>
        internal bool getVehicleTrackingData(String spz)
        {

        }

        /****************************************************************PRIVATE*******************************************************/

        /// <summary>
        /// Rozparsování JSON dat a uložení do databáze
        /// </summary>
        /// <param name="content">JSON text</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        private bool parseJSON(string content, Action<string> errorCallback)
        {
            JSONParser parser = new JSONParser();
            if (!parser.loadData(content, errorCallback))
            {
                return false;
            }
            while (parser.next())
            {
                insertTollWithGPS(parser.when, parser.price, parser.roadNumber, parser.roadType, parser.carType, parser.SPZ, parser.GPSLongitude, parser.GPSLatitude, parser.GPSAccuracy);
            }
            return true;
        }

        /// <summary>
        /// Rozparsování XML dat a uložení do databáze
        /// </summary>
        /// <param name="content">XML text</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        private bool parseXML(string content, Action<string> errorCallback)
        {
            XMLParser parser = new XMLParser();
            if (!parser.loadData(content, errorCallback)) {
                return false;
            }
            while(parser.next()){
                insertTollWithTollGate(parser.when, parser.price, parser.roadNumber, parser.roadType, parser.carType, parser.SPZ, parser.gateId, parser.gateType);
            }
            return true;
        }

        /// <summary>
        /// Prečte soubor vrátí jeho obsah
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <returns>obsah soboru</returns>
        private string readFile(string pathToFile)
        {
            string readContents;
            using (StreamReader streamReader = new StreamReader(pathToFile, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }
            return readContents;
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
        private bool insertTollWithTollGate(DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, int gateId, string gateType)
        {
            var insertGateX = false;
            var insertRoadX = false;
            var insertCarX = false;

            switch (isGateTypeValid(gateId, gateType))
            {
                case ValidType.nonValid:
                    return false;
                case ValidType.validPresent:
                    break;
                case ValidType.validNonPresent:
                    insertGateX = true;
                    break;
            }

            switch (isRoadTypeValid(roadNumber, roadType))
            {
                case ValidType.nonValid:
                    return false;
                case ValidType.validPresent:
                    break;
                case ValidType.validNonPresent:
                    insertRoadX = true;
                    break;
            }

            switch (isCarTypeValid(SPZ, carType))
            {
                case ValidType.nonValid:
                    return false;
                case ValidType.validPresent:
                    break;
                case ValidType.validNonPresent:
                    insertCarX = true;
                    break;
            }

            if (insertGateX)
            {
                if (!insertGate(gateId, gateType, roadNumber))
                {
                    return false;
                }
            }

            if (insertRoadX)
            {
                if (!insertRoad(roadNumber, roadType))
                {
                    return false;
                }
            }

            if (insertCarX)
            {
                if (!insertCar(SPZ, carType))
                {
                    return false;
                }
            }

            if (insertToll(when, price, SPZ, gateId, -1))
            {
                return true;
            }
            else {
                return false;
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
        /// <returns>true pokud je záznam validní</returns>
        private bool insertTollWithGPS(DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, double GPSLongitude, double GPSLatitude, int GPSAccuracy)
        {
            var insertRoadX = false;
            var insertCarX = false;

            switch (isRoadTypeValid(roadNumber, roadType))
            {
                case ValidType.nonValid:
                    return false;
                case ValidType.validPresent:
                    break;
                case ValidType.validNonPresent:
                    insertRoadX = true;
                    break;
            }

            switch (isCarTypeValid(SPZ, carType))
            {
                case ValidType.nonValid:
                    return false;
                case ValidType.validPresent:
                    break;
                case ValidType.validNonPresent:
                    insertCarX = true;
                    break;
            }

            if (insertRoadX)
            {
                if (!insertRoad(roadNumber, roadType))
                {
                    return false;
                }
            }

            if (insertCarX)
            {
                if (!insertCar(SPZ, carType))
                {
                    return false;
                }
            }

            long GPSid = insertGPSGate(GPSLongitude, GPSLatitude, GPSAccuracy, roadNumber);

            if (GPSid == -1)
            {
                return false;
            }

            if (insertToll(when, price, SPZ, -1, GPSid))
            {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Vloží do databáze silnici s danými parametry
        /// </summary>
        /// <param name="roadNumber">číslo silnice</param>
        /// <param name="type">typ silnice</param>
        /// <returns>true pokud se povedlo vložit silnici</returns>
        private bool insertRoad(string roadNumber, string type)
        {
            try
            {
                string query = "INSERT INTO road (number, type) VALUES('" + roadNumber + "', '" + type + "')";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Vloží do databáze auto s danými parametry
        /// </summary>
        /// <param name="SPZ">SPZ auta</param>
        /// <param name="type">typ auta</param>
        /// <returns>true pokud se povedlo vložit auto</returns>
        private bool insertCar(string SPZ, string type)
        {
            try
            {
                string query = "INSERT INTO car (SPZ, type) VALUES('" + SPZ + "', '" + type + "')";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Vloží do databáze mýtnou bránu s danými parametry
        /// </summary>
        /// <param name="gateId">id brány</param>
        /// <param name="type">typ brány</param>
        /// <param name="road_number">číslo silnice, na které se brána nachází</param>
        /// <returns>true pokud se povedlo vložit bránu</returns>
        private bool insertGate(int gateId, string type, string road_number)
        {
            try
            {
                string query = "INSERT INTO toll_gate (id, type, road_number) VALUES('" + gateId + "', '" + type + "', '" + road_number + "')";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
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
        private long insertGPSGate(double longitude, double latitude, int accuracy, string road_number)
        {
            try
            {
                string query = "INSERT INTO GPS_gate (longitude, latitude, accuracy, road_number) VALUES('" + longitude + "', '" + latitude + "', '" + accuracy + "', '" + road_number + "') RETURNING id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Vloží do databáze mýto s danými parametry
        /// </summary>
        /// <param name="when">kdy bylo mýto zaznamenáno</param>
        /// <param name="price">cena mýta</param>
        /// <param name="SPZ">SPZ auta, ke ketrému mýto patří</param>
        /// <param name="tollGateId">id mýtné brány (-1, pokud se nejednalo o mýtnou bránu)</param>
        /// <param name="GPSGateId">id GPS souřadnic (-1, pokud se nejednalo o mýtné s GPS souřadnicema)</param>
        /// <returns>true pokud se povedlo vložit mýto</returns>
        private bool insertToll(DateTime when, double price, string SPZ, int tollGateId, long GPSGateId)
        {
            string gatesValue;
            if (tollGateId == -1)
            {
                gatesValue = "NULL', '" + GPSGateId;
            }
            else {
                gatesValue = tollGateId + "', 'NULL";
            }
            try
            {
                string query = "INSERT INTO toll (when, price, car_SPZ, toll_gate_id, GPS_gate_id) VALUES('" + when + "', '" + price + "', '" + SPZ + "', '" + gatesValue + "')";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Zjistí, zda je v databázi již taková brána a pokud ano, tak jestli má stejný typ
        /// </summary>
        /// <param name="tollGateId">id brány</param>
        /// <param name="type">typ brány</param>
        /// <returns>true pokud taková brána neexistuje nebo pokud existuje a má stejný typ (a pokud existuje takový typ)</returns>
        private ValidType isGateTypeValid(int tollGateId, string type)
        {
            if (!gateTypeSet.Contains(type))
            {
                return ValidType.nonValid;
            }
            return isTypeValid("toll_gate", "id", tollGateId.ToString(), type);
        }

        /// <summary>
        /// Zjistí, zda je v databázi již taková silnice a pokud ano, tak jestli má stejný typ
        /// </summary>
        /// <param name="roadNumber">číslo silnice</param>
        /// <param name="type">typ silnice</param>
        /// <returns>true pokud taková silnice neexistuje nebo pokud existuje a má stejný typ (a pokud existuje takový typ)</returns>
        private ValidType isRoadTypeValid(string roadNumber, string type)
        {
            if (!roadTypeSet.Contains(type))
            {
                return ValidType.nonValid;
            }
            return isTypeValid("road", "number", roadNumber, type);
        }

        /// <summary>
        /// Zjistí, zda je v databázi již takové auto a pokud ano, tak jestli má stejný typ
        /// </summary>
        /// <param name="SPZ">SPZ auta</param>
        /// <param name="type">typ brány</param>
        /// <returns>ValidType</returns>
        private ValidType isCarTypeValid(string SPZ, string type)
        {
            if (!carTypeSet.Contains(type))
            {
                return ValidType.nonValid;
            }
            return isTypeValid("car", "SPZ", SPZ, type);
        }

        /// <summary>
        /// Zjistí, zda je typ nějakého prvku v souladu s již vloženými prvky
        /// Respektive pokud už je prvek v databázi, tak ho najde a zeptá se, zda má daný prvek stejný typ jako chceme právě zadat. 
        /// </summary>
        /// <param name="table">tabulka prvku</param>
        /// <param name="idColumnName">název primárního klíče v tabulce prvku</param>
        /// <param name="idColumnValue">hodnota primárního klíče v tabulce prvku</param>
        /// <param name="type">jméno typu</param>
        /// <returns>ValidType</returns>
        private ValidType isTypeValid(string table, string idColumnName, string idColumnValue, string type)
        {
            string query = "SELECT type FROM " + table + " WHERE " + idColumnName + " = " + idColumnValue + " LIMIT 1";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            NpgsqlDataReader dr = command.ExecuteReader();

            if (dr.Read())
            {
                string typeFromDb = dr["type"] + "";
                dr.Close();
                if (typeFromDb == type)
                {
                    return ValidType.validPresent;
                }
                else {
                    return ValidType.nonValid;
                }
            }
            return ValidType.validNonPresent;
        }

        /// <summary>
        /// Vytvoří množinu z tabulky typu
        /// </summary>
        /// <param name="tableName">jméno tabulky typu</param>
        /// <returns>množina názvů typů</returns>
        private HashSet<string> getTypeMap(string tableName)
        {
            HashSet<string> typeMap = new HashSet<string>();

            string query = "SELECT name FROM " + tableName;

            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            NpgsqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                typeMap.Add(dr["name"] + "");
            }
            dr.Close();
            return typeMap;
        }
    }
}
