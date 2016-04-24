using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollTracker.model
{
    /// <summary>
    /// Parser dat ve formátu JSON
    /// http://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp
    /// </summary>
    class JSONParser
    {
        private int number = 0; //pořadí mýta v souboru

        //údaje o jednom právě teď načteném mýtu
        private int id; //id mýta
        private DateTime when; //kdy bylo mýto zaznamenáno
        private double price; //cena mýtného
        private string roadNumber; //idetifikační číslo silnice
        private string roadType; //typ silnice (1. třídy, 2. třídy, dálnice)
        private string carType; //typ auta (menší 3.5 t, větší 3.5 t)
        private string SPZ; //SPZ auta, u kterého bylo mýto zaznamenáno
        private double GPSLongitude; //zeměpisná délka GPS souřadnic, kde bylo mýto zaznamenáno
        private double GPSLatitude; //zeměpisná šířka GPS souřadnic, kde bylo mýto zaznamenáno
        private int GPSAccuracy; //přesnost GPS souřadnic, kde bylo mýto zaznamenáno
        private string errMes; //chybová hláška v případě, že došlo k chybě při čtení jednoho mýta

        public JSONParser()
        {
        }

        /// <summary>
        /// Načtení dat do parseru
        /// </summary>
        /// <param name="filePath">cesta k JSON souboru</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <param name="errorOnOneTollCallback">funkce, která bude zavolána v případě chyby při parsování jednoho záznamu mýta (jako parametry má funkce id mýta a chybovou hlášku)</param>
        /// <param name="oneTollReadedCallback">funkce, která bude zavolána při každém dokončení parsování jednoho mýta (jako parametry má funkce všechny parametry mýta)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        public bool loadData(string filePath, Action<string> errorCallback, Action<int, string> errorOnOneTollCallback, Action<int, DateTime, double, string, string, string, string, double, double, int> oneTollReadedCallback)
        {
            try {
                using (FileStream sr = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(sr))
                    {
                        if (!readStartOfRootObject(reader))
                        {
                            errorCallback("Obsah JSON souboru musí začínat {\"data\":");
                            return false;
                        }
                        using (JsonReader jsReader = new JsonTextReader(reader))
                        {
                            try
                            {
                                while (jsReader.Read())
                                {
                                    if (jsReader.TokenType == JsonToken.StartObject)
                                    {
                                        JObject toll = JObject.Load(jsReader);
                                        if (parseOneToll(toll))
                                        {
                                            oneTollReadedCallback(id, when, price, roadNumber, roadType, carType, SPZ, GPSLongitude, GPSLatitude, GPSAccuracy);
                                        }
                                        else {
                                            errorOnOneTollCallback(id, errMes);
                                        }
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                if (ex.Message.StartsWith("Additional text encountered after finished reading JSON content: }."))
                                {
                                    return true;
                                    
                                }
                                else {
                                    errorCallback("Obsah souboru není validní JSON: " + ex.Message);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (IOException ex) {
                errorCallback("Nastala chyba při čtení souboru: " + ex.Message);
                return false;
            }
            return false;
        }



        /*************************************************************** private *****************************************************************/

        /// <summary>
        /// Přečte začátek souboru zkontroluje a odstraní {"data":
        /// </summary>
        /// <param name="reader">reader ze souboru</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool readStartOfRootObject(StreamReader reader)
        {
            string startString = "{\"data\":";
            for (int i = 0; i < startString.Length; i++)
            {
                if (readNextNotWhiteSpaceChar(reader) != startString[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Přečte všechny bílé znaky a vrátí až nebílý znak
        /// </summary>
        /// <param name="reader">reader ze souboru</param>
        /// <returns>další nebílý znak</returns>
        private char readNextNotWhiteSpaceChar(StreamReader reader)
        {
            char[] c = new char[1];
            do
            {
                reader.Read(c, 0, 1);
            } while (Char.IsWhiteSpace(c[0]) && reader.Peek() >= 0);
            return c[0];
        }

        /// <summary>
        /// Rozparsuje jeden záznam mýta
        /// </summary>
        /// <param name="toll">objekt jednoho mýta přečtený ze souboru</param>
        /// <returns>true pokud nedošlo k chybě</returns>
        private bool parseOneToll(JObject toll)
        {
            errMes = "";
            number++;
            if (!parseId(toll))
            {
                return false;
            }
            if (!parsePrice(toll))
            {
                return false;
            }
            if (!parseRoadType(toll))
            {
                return false;
            }
            if (!parseRoadNumber(toll))
            {
                return false;
            }
            if (!parseCarSPZ(toll))
            {
                return false;
            }
            if (!parseCarType(toll))
            {
                return false;
            }
            if (!parseGPSLongitude(toll))
            {
                return false;
            }
            if (!parseGPSLatitude(toll))
            {
                return false;
            }
            if (!parseGPSAccuracy(toll))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje id mýta
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseId(JToken toll)
        {
            id = 0;
            try
            {
                id = toll.Value<int>("ts");
                if (id == 0)
                {
                    errMes = "id " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "id " + number + ". mýta není číslo";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje cenu mýta
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parsePrice(JToken toll)
        {
            int price2 = -1;
            price = -1;
            try
            {
                price = toll.Value<double>("value2");
            }
            catch (FormatException ex)
            {
                errMes = "Parametr value2 nemá správný formát desetiného čísla.";
                return false;
            }
            try
            {
                price2 = toll.Value<int>("value");
            }
            catch (FormatException ex)
            {
                errMes = "Parametr value nemá správný formát čísla.";
                return false;
            }
            if (price2 == -1)
            {
                errMes = "Chybí parametr value";
                return false;
            }
            if (price == -1)
            {
                errMes = "Chybí parametr value2";
                return false;
            }
            if (((int)Math.Round(price * 100)) != price2)
            {
                errMes = "Parametry value2 a value mají rozdílné hodnoty.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje číslo silnice
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseRoadNumber(JToken toll)
        {
            roadNumber = "";
            try
            {
                roadNumber = toll.Value<string>("road_no");
                if (roadNumber == "")
                {
                    errMes = "road_no " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "road_no " + number + ". mýta není textový řetězec";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje typ silnice
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseRoadType(JToken toll)
        {
            roadType = "";
            try
            {
                roadType = toll.Value<string>("road_type");
                if (roadType == "")
                {
                    errMes = "road_type " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "road_type " + number + ". mýta není textový řetězec";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje typ auta
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseCarType(JToken toll)
        {
            carType = "";
            try
            {
                carType = toll.Value<string>("vehicle_type");
                if (carType == "")
                {
                    errMes = "vehicle_type " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "vehicle_type " + number + ". mýta není textový řetězec";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje spz auta
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseCarSPZ(JToken toll)
        {
            SPZ = "";
            try
            {
                SPZ = toll.Value<string>("vehicle_reg");
                if (SPZ == "")
                {
                    errMes = "vehicle_reg " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "vehicle_reg " + number + ". mýta není textový řetězec";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje GPS zeměpisnou délku
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseGPSLongitude(JToken toll)
        {
            GPSLongitude = 9999999;
            try
            {
                GPSLongitude = toll.Value<double>("gps_longitude");
                if (GPSLongitude == 9999999)
                {
                    errMes = "gps_longitude " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "gps_longitude " + number + ". mýta není validní desetinné číslo";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje GPS zeměpisnou šířku
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseGPSLatitude(JToken toll)
        {
            GPSLatitude = 9999999;
            try
            {
                GPSLatitude = toll.Value<double>("gps_latitude");
                if (GPSLatitude == 9999999)
                {
                    errMes = "gps_latitude " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "gps_latitude " + number + ". mýta není validní desetinné číslo";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje GPS přesnost
        /// </summary>
        /// <param name="toll">mýto</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseGPSAccuracy(JToken toll)
        {
            GPSAccuracy = 9999999;
            try
            {
                GPSAccuracy = toll.Value<int>("gps_accuracy");
                if (GPSAccuracy == 9999999)
                {
                    errMes = "gps_accuracy " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "gps_accuracy " + number + ". mýta není validní číslo";
                return false;
            }
            return true;
        }
    }
}
