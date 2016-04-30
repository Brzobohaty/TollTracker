using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollTracker.model
{
    /// <summary>
    /// Obecný model s daty celé aplikace
    /// </summary>
    public class Model : DBConnector
    {
        private HashSet<string> gateTypeSet; //množina typů bran
        private HashSet<string> carTypeSet; //množina typů aut
        private HashSet<string> roadTypeSet; //množina typů silnic
        private HashSet<int> tollIdSet; //množina id již vložených mýt
        private enum ValidType { validNonPresent, nonValid, validPresent }; //výčtový typ pro návratové hodnoty metod pro kontrolu typů { validní typ pro nový prvek; nevalidní typ; validní typ pro prvek, který už je v databázi} 
        private Action<int, string> oneTollErrorCallback; //funkce, která bude zavolána v případě chyby při parsování jednoho záznamu mýta (jako parametry má funkce id mýta a chybovou hlášku)
        private String errMes; //poslední chybová hláška ke které došlo při práci s databází


        public Model()
        {
            if (openConnection())
            {
                deleteAll();
                inicializeDB();
                gateTypeSet = getTypeMap("gate_type");
                carTypeSet = getTypeMap("car_type");
                roadTypeSet = getTypeMap("road_type");
                tollIdSet = getTollIdMap();
                closeConnection();
            }
        }

        /// <summary>
        /// Přečte daný soubor a nahraje jeho obsah do databáze
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při čtení souboru a parsování souboru (jako parametr má funkce chybovou hlášku)</param>
        /// <param name="oneTollErrorCallback">funkce, která bude zavolána v případě chyby při parsování jednoho konkrétního mýta (jako parametr má funkce chybovou hlášku a id mýta)</param>
        /// <returns>false pokud nastala při čtení a parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        public bool readFile(string pathToFile, Action<string> errorCallback, Action<int, string> oneTollErrorCallback)
        {
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
                        if (!parseXML(pathToFile, errorCallback, oneTollErrorCallback))
                        {
                            return false;
                        }
                    }
                    else if (firstChar == '{')
                    {
                        if (!parseJSON(pathToFile, errorCallback, oneTollErrorCallback))
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
            gateTypeSet = getTypeMap("gate_type");
            if (gateTypeSet.Any())
            {
                return;
            }

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
            string query = "INSERT INTO " + tableName + " (name) VALUES('" + typeName + "')";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Rozparsování JSON dat a uložení do databáze
        /// </summary>
        /// <param name="content">JSON text</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <param name="oneTollErrorCallback">funkce, která bude zavolána v případě chyby při parsování jednoho konkrétního mýta (jako parametr má funkce chybovou hlášku a id mýta)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        private bool parseJSON(string pathToFile, Action<string> errorCallback, Action<int, string> oneTollErrorCallback)
        {
            this.oneTollErrorCallback = oneTollErrorCallback;
            JSONParser parser = new JSONParser();
            if (!parser.loadData(pathToFile, errorCallback, oneTollErrorCallback, insertTollWithGPS))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Rozparsování XML dat a uložení do databáze
        /// </summary>
        /// <param name="content">XML text</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <param name="oneTollErrorCallback">funkce, která bude zavolána v případě chyby při parsování jednoho konkrétního mýta (jako parametr má funkce chybovou hlášku a id mýta)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        private bool parseXML(string content, Action<string> errorCallback, Action<int, string> oneTollErrorCallback)
        {
            XMLParser parser = new XMLParser();
            if (!parser.loadData(content, errorCallback))
            {
                return false;
            }
            while (parser.next())
            {
                insertTollWithTollGate(0, parser.when, parser.price, parser.roadNumber, parser.roadType, parser.carType, parser.SPZ, parser.gateId, parser.gateType);
            }
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
        private bool insertTollWithTollGate(int id, DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, int gateId, string gateType)
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

            if (insertToll(id, when, price, SPZ, gateId, -1))
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
        /// <param name="id">id mýta</param>
        /// <param name="when">kdy bylo mýto zaznamenáno</param>
        /// <param name="price">cena mýtného</param>
        /// <param name="roadNumber">idetifikační číslo silnice</param>
        /// <param name="roadType">typ silnice (1. třídy, 2. třídy, dálnice)</param>
        /// <param name="carType">typ auta (menší 3.5 t, větší 3.5 t)</param>
        /// <param name="SPZ">SPZ auta, u kterého bylo mýto zaznamenáno</param>
        /// <param name="GPSLongitude">zeměpisná délka GPS souřadnic, kde bylo mýto zaznamenáno</param>
        /// <param name="GPSLatitude">zeměpisná šířka GPS souřadnic, kde bylo mýto zaznamenáno</param>
        /// <param name="GPSAccuracy">přesnost GPS souřadnic, kde bylo mýto zaznamenáno</param>
        private void insertTollWithGPS(int id, DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, double GPSLongitude, double GPSLatitude, int GPSAccuracy)
        {
            var insertRoadX = false;
            var insertCarX = false;

            switch (isRoadTypeValid(roadNumber, roadType))
            {
                case ValidType.nonValid:
                    oneTollErrorCallback(id, errMes);
                    return;
                case ValidType.validPresent:
                    break;
                case ValidType.validNonPresent:
                    insertRoadX = true;
                    break;
            }

            switch (isCarTypeValid(SPZ, carType))
            {
                case ValidType.nonValid:
                    oneTollErrorCallback(id, errMes);
                    return;
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
                    oneTollErrorCallback(id, " (vkládání silnice do databáze) " + errMes);
                    return;
                }
            }

            if (insertCarX)
            {
                if (!insertCar(SPZ, carType))
                {
                    oneTollErrorCallback(id, " (vkládání auta do databáze) " + errMes);
                    return;
                }
            }

            long GPSid = insertGPSGate(GPSLongitude, GPSLatitude, GPSAccuracy, roadNumber);

            if (GPSid == -1)
            {
                oneTollErrorCallback(id, " (vkládání GPS souřadnic do databáze) " + errMes);
                return;
            }

            if (!insertToll(id, when, price, SPZ, -1, GPSid))
            {
                oneTollErrorCallback(id, " (vkládání mýta do databáze) " + errMes);
                return;
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
                errMes = ex.Message;
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
                errMes = ex.Message;
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
                errMes = ex.Message;
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
        private int insertGPSGate(double longitude, double latitude, int accuracy, string road_number)
        {
            NpgsqlDataReader dr;
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
            return -1;
        }

        /// <summary>
        /// Vloží do databáze mýto s danými parametry
        /// </summary>
        /// <param name="id">id mýta</param>
        /// <param name="when">kdy bylo mýto zaznamenáno</param>
        /// <param name="price">cena mýta</param>
        /// <param name="SPZ">SPZ auta, ke ketrému mýto patří</param>
        /// <param name="tollGateId">id mýtné brány (-1, pokud se nejednalo o mýtnou bránu)</param>
        /// <param name="GPSGateId">id GPS souřadnic (-1, pokud se nejednalo o mýtné s GPS souřadnicema)</param>
        /// <returns>true pokud se povedlo vložit mýto</returns>
        private bool insertToll(int id, DateTime when, double price, string SPZ, int tollGateId, long GPSGateId)
        {
            string query;
            if (tollIdSet.Contains(id))
            {
                query = "UPDATE toll SET whenn = '" + when + "', price = '" + price.ToString(CultureInfo.CreateSpecificCulture("en-us")) + "', car_spz = '" + SPZ + "', gps_gate_id = '" + GPSGateId + "' WHERE id=" + id;
            }
            else if (tollGateId == -1)
            {
                query = "INSERT INTO toll (id, whenn, price, car_spz, gps_gate_id) VALUES('" + id + "', '" + when + "', '" + price.ToString(CultureInfo.CreateSpecificCulture("en-us")) + "', '" + SPZ + "', '" + GPSGateId + "')";
            }
            else {
                query = "INSERT INTO toll (id, whenn, price, car_spz, toll_gate_id) VALUES('" + id + "', '" + when + "', '" + price.ToString(CultureInfo.CreateSpecificCulture("en-us")) + "', '" + SPZ + "', '" + tollGateId + "')";
            }
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                errMes = ex.Message;
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
                errMes = "Neexistující typ brány: " + type;
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
                errMes = "Neexistující typ silnice: " + type;
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
                errMes = "Neexistující typ auta: " + type;
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
            NpgsqlDataReader dr;
            try
            {
                string query = "SELECT type FROM " + table + " WHERE " + idColumnName.ToLower() + " = '" + idColumnValue + "' LIMIT 1";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                dr = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                errMes = ex.Message;
                return ValidType.nonValid;
            }
            try
            {
                if (dr.Read())
                {
                    string typeFromDb = dr["type"] + "";
                    if (typeFromDb == type)
                    {
                        return ValidType.validPresent;
                    }
                    else {
                        errMes = "Již je v databázi prvek s idColumnName :" + idColumnValue + ", ale má přiřazený jiný typ (" + typeFromDb + " != " + type + ") silnice.";
                        return ValidType.nonValid;
                    }
                }
                return ValidType.validNonPresent;
            }
            finally
            {
                dr.Close();
            }
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

        /// <summary>
        /// Vytvoří množinu id mýt z databáze
        /// </summary>
        /// <returns>množina id</returns>
        private HashSet<int> getTollIdMap()
        {
            HashSet<int> map = new HashSet<int>();

            string query = "SELECT id FROM toll";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            NpgsqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                map.Add((int)dr["id"]);
            }
            dr.Close();
            return map;
        }
    }
}