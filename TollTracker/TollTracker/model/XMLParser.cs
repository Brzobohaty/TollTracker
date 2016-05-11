using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private int number = 0; //index současného XML záznamu

        //údaje o jednom právě teď načteném mýtu
        private Nullable<DateTime> when; //kdy bylo mýto zaznamenáno
        private double price; //cena mýtného
        private string roadNumber; //idetifikační číslo silnice
        private string roadType; //typ silnice (1. třídy, 2. třídy, dálnice)
        private string carType; //typ auta (menší 3.5 t, větší 3.5 t)
        private string SPZ; //SPZ auta, u kterého bylo mýto zaznamenáno
        private string gateId; //id mýtné brány
        private string gateType; //typ mýtné brány (small, medium, large)
        private string errMes; //chybová hláška v případě, že došlo k chybě při čtení jednoho mýta

        public XMLParser() {
            doc = new XmlDocument();
        }

        /// <summary>
        /// Načtení dat do parseru
        /// </summary>
        /// <param name="filePath">cesta k XML souboru</param>
        /// <param name="errorCallback">funkce, která bude zavolána v případě chyby při parsování (jako parametr má funkce chybovou hlášku)</param>
        /// <param name="errorOnOneTollCallback">funkce, která bude zavolána v případě chyby při parsování jednoho záznamu mýta (jako parametry má funkce id mýta a chybovou hlášku)</param>
        /// <param name="oneTollReadedCallback">funkce, která bude zavolána při každém dokončení parsování jednoho mýta (jako parametry má funkce všechny parametry mýta)</param>
        /// <returns>false pokud nastala při parsování fatalní chyba, která zamezila načtení všech záznamů</returns>
        public bool loadData(string filePath, Action<string> errorCallback, Action<int, string> errorOnOneTollCallback, Action<int, DateTime, double, string, string, string, string, string, string> oneTollReadedCallback)
        {
            try
            {
                using (XmlReader myReader = XmlReader.Create(filePath))
                {
                    while (myReader.Read())
                    {
                        switch (myReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (myReader.Name == "passage") {
                                    XmlNode toll = doc.ReadNode(myReader);
                                    if (parseOneToll(toll))
                                    {
                                        oneTollReadedCallback(number, (DateTime)when, price, roadNumber, roadType, carType, SPZ, gateId, gateType);
                                    }
                                    else {
                                        errorOnOneTollCallback(number, errMes);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                errorCallback("Nastala chyba při čtení souboru: " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Exportuje výsledky vehicleTrackingReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportVehicleTrackingReport(string pathToFile, ListView.ListViewItemCollection data)
        {
            XmlTextWriter writer = new XmlTextWriter(pathToFile, System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            writer.WriteStartElement("VehicleTrackingReport");
            for (int i = 0; i < data.Count; i++)
            {
                writer.WriteStartElement("TrackingPoint");
                writer.WriteElementString("Time", data[i].SubItems[1].Text);
                writer.WriteElementString("GpsGateId", data[i].SubItems[2].Text);
                writer.WriteElementString("TollGateId", data[i].SubItems[3].Text);
                writer.WriteElementString("Price", data[i].SubItems[4].Text);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        /// <summary>
        /// Exportuje výsledky vehicleTollReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportVehicleTollReport(string pathToFile, ListView.ListViewItemCollection data)
        {
            XmlTextWriter writer = new XmlTextWriter(pathToFile, System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("VehicleTollReport");
            for (int i = 0; i < data.Count; i++)
            {
                writer.WriteStartElement("TollClass");
                writer.WriteElementString("RoadType", data[i].SubItems[1].Text);
                writer.WriteElementString("Price", data[i].SubItems[2].Text);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        /// <summary>
        /// Exportuje výsledky tollsSummaryReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportTollsSummaryReport(string pathToFile, ListView.ListViewItemCollection data)
        {
            XmlTextWriter writer = new XmlTextWriter(pathToFile, System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("TollSummaryReport");
            for (int i = 0; i < data.Count; i++)
            {
                writer.WriteStartElement("Toll");
                writer.WriteElementString("VehicleType", data[i].SubItems[1].Text);
                writer.WriteElementString("Price", data[i].SubItems[2].Text);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        /// <summary>
        /// Exportuje výsledky gateReportu do XML
        /// </summary>
        /// <param name="pathToFile">cesta k souboru</param>
        /// <param name="data">data určená k exportu</param>
        public void exportGateReport(string pathToFile, ListView.ListViewItemCollection data)
        {
            XmlTextWriter writer = new XmlTextWriter(pathToFile, System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("GateReport");
            for (int i = 0; i < data.Count; i++)
            {
                writer.WriteStartElement("Vehicle");
                writer.WriteElementString("SPZ", data[i].SubItems[1].Text);
                writer.WriteElementString("VehicleType", data[i].SubItems[2].Text);
                writer.WriteElementString("Time", data[i].SubItems[3].Text);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        /*************************************************************** private *****************************************************************/

        ///// <summary>
        ///// Rozparsuje jeden záznam mýta
        ///// </summary>
        ///// <param name="toll">objekt jednoho mýta přečtený ze souboru</param>
        ///// <returns>true pokud nedošlo k chybě</returns>
        private bool parseOneToll(XmlNode toll)
        {

            when = null;
            price = -1;
            roadNumber = "";
            roadType = "";
            carType = "";
            SPZ = "";
            gateId = "";
            gateType = "";

            errMes = "";
            number++;
            string value1 = "";
            string value2 = "";
            IEnumerator enumerator = toll.GetEnumerator();
            while (enumerator.MoveNext()) {
                XmlNode node = (XmlNode)enumerator.Current;
                switch (node.Name)
                {
                    case "ts":
                        if (!parseWhen(node.InnerText))
                        {
                            return false;
                        }
                        break;
                    case "value":
                        if (value2 != "") {
                            if (!parsePrice(node.InnerText, value2))
                            {
                                return false;
                            }
                        }
                        value1 = node.InnerText;
                        break;
                    case "value2":
                        if (value1 != "")
                        {
                            if (!parsePrice(value1, node.InnerText))
                            {
                                return false;
                            }
                        }
                        value2 = node.InnerText;
                        break;
                    case "road_type":
                        if (!parseRoadType(node.InnerText))
                        {
                            return false;
                        }
                        break;
                    case "road_no":
                        if (!parseRoadNumber(node.InnerText))
                        {
                            return false;
                        }
                        break;
                    case "vehicle_reg":
                        if (!parseCarSPZ(node.InnerText))
                        {
                            return false;
                        }
                        break;
                    case "vehicle_type":
                        if (!parseCarType(node.InnerText))
                        {
                            return false;
                        }
                        break;
                    case "gate":
                        if (!parseGate(node.InnerText))
                        {
                            return false;
                        }
                        break;
                    case "gate_type":
                        if (!parseGateType(node.InnerText))
                        {
                            return false;
                        }
                        break;
                }
            }
            if (when == null) {
                errMes = "čas " + number + ". mýta chybí";
                return false;
            }
            if (price < 0)
            {
                errMes = "cena " + number + ". mýta chybí";
                return false;
            }
            if (roadNumber == "")
            {
                errMes = "silnice " + number + ". mýta chybí";
                return false;
            }
            if (roadType == "")
            {
                errMes = "druh silnice " + number + ". mýta chybí";
                return false;
            }
            if (carType == "")
            {
                errMes = "typ auta " + number + ". mýta chybí";
                return false;
            }
            if (SPZ == "")
            {
                errMes = "SPZ auta " + number + ". mýta chybí";
                return false;
            }
            if (gateId == "")
            {
                errMes = "ID mýtné brány " + number + ". mýta chybí";
                return false;
            }
            if (gateType == "")
            {
                errMes = "Typ mýtné brány " + number + ". mýta chybí";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje čas mýta
        /// </summary>
        /// <param name="timestamp">časový údaj</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseWhen(string timestamp)
        {
            when = null;
            try
            {
                when = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                when = ((DateTime)when).AddSeconds(Convert.ToInt64(timestamp)).ToLocalTime();
                if (when == null)
                {
                    errMes = "čas " + number + ". mýta chybí";
                    return false;
                }
            }
            catch (FormatException ex)
            {
                errMes = "čas " + number + ". mýta není platný timestamp";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje cenu mýta
        /// </summary>
        /// <param name="price">cena mýta vynásobená 100</param>
        /// <param name="price2">cena mýta</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parsePrice(string price, string price2)
        {
            int priceC;
            double price2C;
            try
            {
                priceC = Convert.ToInt32(price);
            }
            catch (FormatException ex)
            {
                errMes = "Parametr value nemá správný formát čísla.";
                return false;
            }
            try
            {
                price2C = double.Parse(price2, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                errMes = "Parametr value2 nemá správný formát desetiného čísla.";
                return false;
            }
            if (((int)Math.Round(price2C * 100)) != priceC)
            {
                errMes = "Parametry value2 a value mají rozdílné hodnoty.";
                return false;
            }
            else {
                this.price = price2C;
            }
            return true;
        }

        /// <summary>
        /// Vyparsuje číslo silnice
        /// </summary>
        /// <param name="roadNumber">číslo silnice</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseRoadNumber(string roadNumber)
        {
            this.roadNumber = roadNumber;
            return true;
        }

        /// <summary>
        /// Vyparsuje typ silnice
        /// </summary>
        /// <param name="roadType">typ silnice</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseRoadType(string roadType)
        {
            this.roadType = roadType;
            return true;
        }

        /// <summary>
        /// Vyparsuje typ auta
        /// </summary>
        /// <param name="carType">typ auta</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseCarType(string carType)
        {
            this.carType = carType;
            return true;
        }

        /// <summary>
        /// Vyparsuje spz auta
        /// </summary>
        /// <param name="spz">SPZ auta</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseCarSPZ(string spz)
        {
            this.SPZ = spz;
            return true;
        }

        /// <summary>
        /// Vyparsuje id mýtné brány
        /// </summary>
        /// <param name="gate">id brány</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseGate(string gate)
        {
            this.gateId = gate;
            return true;
        }

        /// <summary>
        /// Vyparsuje typ mýtné brány
        /// </summary>
        /// <param name="gateType">typ brány</param>
        /// <returns>true pokud nenastala žádná chyba</returns>
        private bool parseGateType(string gateType)
        {
            this.gateType = gateType;
            return true;
        }
    }
}
