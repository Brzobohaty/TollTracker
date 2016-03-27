using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
        private Dictionary<string, int> gateTypeMap; //mapa typů bran, kde klíč je název typu a hodnota je id typu
        private Dictionary<string, int> carTypeMap; //mapa typů aut, kde klíč je název typu a hodnota je id typu
        private Dictionary<string, int> roadTypeMap; //mapa typů silnic, kde klíč je název typu a hodnota je id typu
        private enum ValidType { validNonPresent, nonValid, validPresent }; //výčtový typ pro návratové hodnoty metod pro kontrolu typů { validní typ pro nový prvek; nevalidní typ; validní typ pro prvek, který už je v databázi} 

        public Model() : base()
        {
            if (openConnection())
            {
                gateTypeMap = getTypeMap("gate_type");
                carTypeMap = getTypeMap("car_type");
                roadTypeMap = getTypeMap("road_type");
                closeConnection();
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
        public bool insertTollWithTollGate(DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, int gateId, string gateType)
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

            if (insertGateX) {
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
        public bool insertTollWithGPS(DateTime when, double price, string roadNumber, string roadType, string carType, string SPZ, double GPSLongitude, double GPSLatitude, int GPSAccuracy)
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

        /****************************************************************PRIVATE*******************************************************/

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
                string query = "INSERT INTO road (number, type_id) VALUES('" + roadNumber + "', '" + roadTypeMap[type] + "')";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
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
                string query = "INSERT INTO car (SPZ, type_id) VALUES('" + SPZ + "', '" + carTypeMap[type] + "')";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
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
                string query = "INSERT INTO toll_gate (id, type_id, road_number) VALUES('" + gateId + "', '" + gateTypeMap[type] + "', '" + road_number + "')";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
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
                string query = "INSERT INTO GPS_gate (longitude, latitude, accuracy, road_number) VALUES('" + longitude + "', '" + latitude + "', '" + accuracy + "', '" + road_number + "')";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                return cmd.LastInsertedId;
            }
            catch (MySqlException ex)
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
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
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
            int typeId;
            try
            {
                typeId = gateTypeMap[type];
            }
            catch (KeyNotFoundException)
            {
                return ValidType.nonValid;
            }
            return isTypeValid("toll_gate", "id", tollGateId.ToString(), typeId);
        }

        /// <summary>
        /// Zjistí, zda je v databázi již taková silnice a pokud ano, tak jestli má stejný typ
        /// </summary>
        /// <param name="roadNumber">číslo silnice</param>
        /// <param name="type">typ silnice</param>
        /// <returns>true pokud taková silnice neexistuje nebo pokud existuje a má stejný typ (a pokud existuje takový typ)</returns>
        private ValidType isRoadTypeValid(string roadNumber, string type)
        {
            int typeId;
            try
            {
                typeId = roadTypeMap[type];
            }
            catch (KeyNotFoundException)
            {
                return ValidType.nonValid;
            }
            return isTypeValid("road", "number", roadNumber, typeId);
        }

        /// <summary>
        /// Zjistí, zda je v databázi již takové auto a pokud ano, tak jestli má stejný typ
        /// </summary>
        /// <param name="SPZ">SPZ auta</param>
        /// <param name="type">typ brány</param>
        /// <returns>ValidType</returns>
        private ValidType isCarTypeValid(string SPZ, string type)
        {
            int typeId;
            try
            {
                typeId = carTypeMap[type];
            }
            catch (KeyNotFoundException)
            {
                return ValidType.nonValid;
            }
            return isTypeValid("car", "SPZ", SPZ, typeId);
        }

        /// <summary>
        /// Zjistí, zda je typ nějakého prvku v souladu s již vloženými prvky
        /// Respektive pokud už je prvek v databázi, tak ho najde a zeptá se, zda má daný prvek stejný typ jako chceme právě zadat. 
        /// </summary>
        /// <param name="table">tabulka prvku</param>
        /// <param name="idColumnName">název primárního klíče v tabulce prvku</param>
        /// <param name="idColumnValue">hodnota primárního klíče v tabulce prvku</param>
        /// <param name="typeId">id typu</param>
        /// <returns>ValidType</returns>
        private ValidType isTypeValid(string table, string idColumnName, string idColumnValue, int typeId)
        {
            string query = "SELECT type_id FROM " + table + " WHERE " + idColumnName + " = " + idColumnValue + " LIMIT 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (dataReader.Read())
            {
                int type_id = Convert.ToInt32(dataReader["type_id"]);
                if (typeId == type_id)
                {
                    dataReader.Close();
                    return ValidType.validPresent;
                }
                else {
                    dataReader.Close();
                    return ValidType.nonValid;
                }
            }
            dataReader.Close();
            return ValidType.validNonPresent;
        }

        /// <summary>
        /// Vytvoří mapu z tabulky typu
        /// </summary>
        /// <param name="tableName">jméno tabulky typu</param>
        /// <returns>mapu, kde klíč je název typu a hodnota je id typu</returns>
        private Dictionary<string, int> getTypeMap(string tableName)
        {
            Dictionary<string, int> typeMap = new Dictionary<string, int>();

            string query = "SELECT id, name FROM " + tableName;

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                int id = Convert.ToInt32(dataReader["id"]);
                typeMap.Add(dataReader["name"] + "", id);
            }
            dataReader.Close();

            return typeMap;
        }
    }
}
