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
            Action<String> saveFileError = (String s) => Console.WriteLine("Error: file saving - ", s);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                String filename = saveFileDialog.FileName;
                String report = reportProperties.SelectedTab.Name;

                switch (report)
                {
                    case "vehicleTrackingPage":
                        model.exportVehicleTrackingReportToXML(filename, listView1.Items);
                        break;
                    case "vehicleTollPage":
                        model.exportVehicleTollReportToXML(filename, listView1.Items);
                        break;
                    case "tollsSummaryPage":
                        model.exportTollsSummaryReportToXML(filename, listView1.Items);
                        break;
                    case "gateReportPage":
                        model.exportGateReportToXML(filename, listView1.Items);
                        break;
                    default:
                        break;
                }
            }
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            clearFileError();
            List<List<String>> results = new List<List<String>>();
            String report = reportProperties.SelectedTab.Name;
            switch (report)
            {
                case "vehicleTrackingPage":
                    if (vehiclePicker.SelectedItem != null)
                    {
                        prepareVehicleTrackingReportListView();
                        results = model.getVehicleTrackingData(vehiclePicker.GetItemText(vehiclePicker.SelectedItem));
                    } else
                    {
                        printFileError("Zvolte spz vozu!");
                    }
                    break;
                case "vehicleTollPage":
                    if (vehiclePicker2.SelectedItem != null && datePicker.Value != null)
                    {
                        prepareVehicleTollReportListView();
                        results = model.getVehicleToll(vehiclePicker2.GetItemText(vehiclePicker2.SelectedItem), getStartDate(), getEndDate());
                    } else
                    {
                        printFileError("Zvolte spz vozu a datum!");
                    }
                    break;
                case "tollsSummaryPage":
                    prepareTollSummaryReportListView();
                    results = model.getTollsSummary();
                    break;
                case "gateReportPage":
                    if (gatePicker.SelectedItem != null)
                    {
                        prepareGateReportListView();
                        results = model.getGateReport(gatePicker.GetItemText(gatePicker.SelectedItem));     
                    } else
                            {
                        printFileError("Zvolte id brány!");
                    }
                    break;
                default:
                    results = new List<List<String>>();
                    prepareDefaultListView();
                    printFileError("Zvolte jeden z dotazů!");
                    break;
            }
            updateListView(results);
        }

        private void updateListView(List<List<string>> results)
        {
            if (results != null && results.Count > 0)
            {
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
            } else
            {
                printFileError("Chyba dat při pokusu o jejich vypsání");
            }
        }

        private void prepareDefaultListView()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("");
        }

        private void prepareGateReportListView()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("");
            listView1.Columns.Add("Spz");
            listView1.Columns.Add("Typ vozidla");
            listView1.Columns.Add("Čas");
        }

        private void prepareTollSummaryReportListView()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("");
            listView1.Columns.Add("Typ vozidel");
            listView1.Columns.Add("Mýtné");
        }

        private void prepareVehicleTollReportListView()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("");
            listView1.Columns.Add("Typ silnice");
            listView1.Columns.Add("Mýtné");
        }

        private void prepareVehicleTrackingReportListView()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("");
            listView1.Columns.Add("Čas");
            listView1.Columns.Add("Silnice - GPS");
            listView1.Columns.Add("Silnice - brána");
            listView1.Columns.Add("Mýtné");
        }

        private DateTime getStartDate()
        {
            if (dailyRadioButton.Checked)
            {
                return TimeUtils.getStartOfDay(datePicker.Value);
            }
            else if (weeklyRadioButton.Checked)
            {
                return TimeUtils.getStartOfWeek(datePicker.Value);
            }
            else if (monthlyRadioButton.Checked)
            {
                return TimeUtils.getStartOfMonth(datePicker.Value);
            }
            else return new DateTime().AddDays(-1);

        }

        private DateTime getEndDate()
        {
            if (dailyRadioButton.Checked)
            {
                return TimeUtils.getEndOfDay(datePicker.Value);
            }
            else if (weeklyRadioButton.Checked)
            {
                return TimeUtils.getEndOfWeek(datePicker.Value);
            }
            else if (monthlyRadioButton.Checked)
            {
                return TimeUtils.getEndOfMonth(datePicker.Value);
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

        public void clearFileError()
        {
            label1.Text = "";
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

        private void dailyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = TimeUtils.DAY_FORMAT;
        }

        private void weeklyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = TimeUtils.DAY_FORMAT;
        }

        private void monthlyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = TimeUtils.MONTH_FORMAT;
        }
    }
}
