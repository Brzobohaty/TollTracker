using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
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
        IEnumerator<JToken> tollsEnumerator; //čítač jednotlivých mýt

        //údaje o jednom právě teď načteném mýtu
        public DateTime when { private set; get; } //kdy bylo mýto zaznamenáno
        public double price { private set; get; } //cena mýtného
        public string roadNumber { private set; get; } //idetifikační číslo silnice
        public string roadType { private set; get; } //typ silnice (1. třídy, 2. třídy, dálnice)
        public string carType { private set; get; } //typ auta (menší 3.5 t, větší 3.5 t)
        public string SPZ { private set; get; } //SPZ auta, u kterého bylo mýto zaznamenáno
        public double GPSLongitude { private set; get; } //zeměpisná délka GPS souřadnic, kde bylo mýto zaznamenáno
        public double GPSLatitude { private set; get; } //zeměpisná šířka GPS souřadnic, kde bylo mýto zaznamenáno
        public int GPSAccuracy { private set; get; } //přesnost GPS souřadnic, kde bylo mýto zaznamenáno

        public JSONParser()
        {
        }

        /// <summary>
        /// Načtení dat do parseru
        /// </summary>
        /// <param name="content">JSON text</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        public bool loadData(string content, Action<string> errorCallback)
        {
            try
            {
                JArray tolls = JArray.Parse(content);
                tollsEnumerator = tolls.GetEnumerator();
            }
            catch (JsonException ex)
            {
                errorCallback("Obsah souboru není validní XML: " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Přečte následující záznam mýta
        /// </summary>
        /// <returns>true pokud je zde další záznam</returns>
        public bool next()
        {
            if (tollsEnumerator.MoveNext())
            {
                JToken toll = tollsEnumerator.Current;

                //TODO rozparsovat jednotlivé části
                
                return true;
            }
            else {
                return false;
            }
        }
    }
}
