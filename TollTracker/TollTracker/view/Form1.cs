using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TollTracker.model;

namespace TollTracker
{
    public partial class Form1 : Form
    {
        private Model model;

        public Form1(Model model)
        {
            this.model = model;
            InitializeComponent();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            Action<String> openFileError = (String s) => Console.WriteLine("Error: file opening - ", s);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                model.readFile(openFileDialog.FileName, printFileError, printOneTollError, showNumberOfProcessedTolls);
                initializeReportProperties(sender, e);
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //TODO je třeba dodělat funkci pro export dat
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            //TODO zde se budou spouštět dotazy a nastavovat data do resultsGridView
            model.Model m = new model.Model();
            switch (reportProperties.SelectedTab.Name)
            {
                case "Vehicle Tracking":
                    //m.getVehicleTrackingData(vehicle);
                    break;
                case "Vehicle Toll":
                    //m.getVehicleToll(vehicle,from,to);
                    break;
                case "Tolls Summary":
                    //m.getTollsSummary();
                    break;
                case "Gate Report":
                    //m.getGateReport(gate);
                    break;
            }   
        }

        private void initializeReportProperties(object sender, EventArgs e)
        {
            foreach (String vehicle in model.getAllVehicles()) 
            {
                vehiclePicker.Items.Add(vehicle);
                vehiclePicker2.Items.Add(vehicle);
            }
            foreach (String gate in model.getAllGates())
            {
                gatePicker.Items.Add(gate);
            }
        }

        public void printFileError(string text) {
            label1.Text = text;
        }

        public void printOneTollError(int tollOrderNumber, string text) {
            if (tollOrderNumber != 0)
            {
                listView1.Items.Add("Na " + tollOrderNumber + ". mýtu v souboru došlo k chybě: " + text);
            }
            else {
                listView1.Items.Add(text);
            }    
        }

        public void showNumberOfProcessedTolls(int number) {
            label2.Text = number.ToString();
            label2.Refresh();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }
    }
}
