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
            List<List<String>> results;
            String report = reportProperties.SelectedTab.Name;
            switch (report)
            {
                case "vehicleTrackingPage":
                    listView1.Columns.Clear();
                    listView1.Columns.Add("");
                    listView1.Columns.Add("Čas");
                    listView1.Columns.Add("Silnice - GPS");
                    listView1.Columns.Add("Silnice - brána");
                    listView1.Columns.Add("Mýtné");
                    results = model.getVehicleTrackingData(vehiclePicker.GetItemText(vehiclePicker.SelectedItem));
                    break;
                case "vehicleTollPage":
                    listView1.Columns.Clear();
                    listView1.Columns.Add("");
                    listView1.Columns.Add("Typ silnice");
                    listView1.Columns.Add("Mýtné");
                    results = model.getVehicleToll(vehiclePicker2.GetItemText(vehiclePicker2.SelectedItem), getStartDate(), getEndDate());
                    break;
                case "tollsSummaryPage":
                    listView1.Columns.Clear();
                    listView1.Columns.Add("");
                    listView1.Columns.Add("Typ vozidel");
                    listView1.Columns.Add("Mýtné");
                    results = model.getTollsSummary();
                    break;
                case "gateReportPage":
                    listView1.Columns.Clear();
                    listView1.Columns.Add("");
                    listView1.Columns.Add("Spz");
                    listView1.Columns.Add("Typ vozidla");
                    listView1.Columns.Add("Čas");
                    results = model.getGateReport(gatePicker.GetItemText(gatePicker.SelectedItem));
                    break;
                default:
                    results = new List<List<String>>();
                    listView1.Columns.Clear();
                    listView1.Columns.Add("");
                    listView1.Columns.Add("Data");
                    listView1.Items.Add("Prosím zvolte jeden z dotazů");
                    break;
            }

            listView1.BeginUpdate();

            for (int i = 0; i < results[0].Count; i++)
            {
                ListViewItem item = new ListViewItem(listView1.Items.Count.ToString());
                String[] row = new String[results.Count];
                for (int j = 0; j < results.Count; j++)
                {
                    row[j] = results[j][i];
                }
                item.SubItems.AddRange(row.ToArray());
                listView1.Items.Add(item);
            }
            listView1.EndUpdate();
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
            foreach (String vehicle in model.getAllVehicles()[0]) 
            {
                vehiclePicker.Items.Add(vehicle);
                vehiclePicker2.Items.Add(vehicle);
            }
            foreach (String gate in model.getAllGates()[0])
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
