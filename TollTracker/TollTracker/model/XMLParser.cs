using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TollTracker.model
{
    /// <summary>
    /// Parser XML dat
    /// http://stackoverflow.com/questions/642293/how-do-i-read-and-parse-an-xml-file-in-c
    /// </summary>
    class XMLParser
    {
        private XmlDocument doc; //XML dokument
        private int tollCount = 0; //počet záznamů v dokumentu
        private int index = 0; //index současného XML záznamu

        //údaje o jednom právě teď načteném mýtu
        public DateTime when { private set; get; } //kdy bylo mýto zaznamenáno
        public double price { private set; get; } //cena mýtného
        public string roadNumber { private set; get; } //idetifikační číslo silnice
        public string roadType { private set; get; } //typ silnice (1. třídy, 2. třídy, dálnice)
        public string carType { private set; get; } //typ auta (menší 3.5 t, větší 3.5 t)
        public string SPZ { private set; get; } //SPZ auta, u kterého bylo mýto zaznamenáno
        public int gateId { private set; get; } //id mýtné brány
        public string gateType { private set; get; } //typ mýtné brány (small, medium, large)

        public XMLParser() {
            XmlDocument doc = new XmlDocument();
        }

        /// <summary>
        /// Načtení dat do parseru
        /// </summary>
        /// <param name="content">XML text</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        public bool loadData(string content, Action<string> errorCallback)
        {
            try
            {
                doc.LoadXml(content);
                tollCount = doc.DocumentElement.ChildNodes.Count;
            }
            catch (XmlException ex)
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
        public bool next(){
            if (index < tollCount)
            {
                XmlNode node = doc.DocumentElement.ChildNodes[index];

                //TODO rozparsovat jednotlivé části
                string text = node.InnerText; //or loop through its children as well

                return true;
            }
            else {
                return false;
            }
        }
    }
}
