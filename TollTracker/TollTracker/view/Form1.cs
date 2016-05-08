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
            listView1.Items.Clear();
            List<String> results = new List<String>();
            String report = reportProperties.SelectedTab.Name;
            switch (report)
            {
                case "vehicleTrackingPage":
                    results = model.getVehicleTrackingData(vehiclePicker.GetItemText(vehiclePicker.SelectedItem));
                    break;
                case "vehicleTollPage":
                    results = model.getVehicleToll(vehiclePicker2.GetItemText(vehiclePicker2.SelectedItem), getStartDate(), getEndDate());
                    break;
                case "tollsSummaryPage":
                    results = model.getTollsSummary();
                    break;
                case "gateReportPage":
                    results = model.getGateReport(gatePicker.GetItemText(gatePicker.SelectedItem));
                    break;
                default:
                    listView1.Items.Add("Prosím zvolte jeden z dotazů");
                break;
            }
            foreach (String result in results)
            {
                listView1.Items.Add(result);
            }
        }

        private DateTime getStartDate()
        {
            if (dailyRadioButton.Checked)
            {
                return datePicker.Value.Date;
            }
            else if (weeklyRadioButton.Checked)
            {
                return datePicker.Value.Date.AddDays(-6);
            }
            else if (monthlyRadioButton.Checked)
            {
                return new DateTime(datePicker.Value.Year, datePicker.Value.Month, 1);
            }
            else return new DateTime().AddDays(-1);

        }

        private DateTime getEndDate()
        {
            if (dailyRadioButton.Checked)
            {
                return datePicker.Value.Date.AddDays(1);
            }
            else if (weeklyRadioButton.Checked)
            {
                return datePicker.Value.Date.AddDays(1);
            }
            else if (monthlyRadioButton.Checked)
            {
                return new DateTime(datePicker.Value.Year, datePicker.Value.Month, 1).AddMonths(1).AddDays(-1);
            }
            else return new DateTime();
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

        private void dailyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = "d. MMMM yyyy";
        }

        private void weeklyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = "d. MMMM yyyy";
        }

        private void monthlyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = "MMMM yyyy";
        }
    }
}
