using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TollTracker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void vehicleChooser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void importButton_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog(); - je to třeba?
            Action<String> openFileError = (String s) => Console.WriteLine("Error: file opening - ", s);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                model.Model m = new model.Model();
                if (!m.readFile(openFileDialog.FileName, openFileError))
                {
                    MessageBox.Show("There was some FOE!", "File Open Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    initialize_reportProperties(sender, e);
                }
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
                    m.getVehicleTrackingData(vehicle);
                    break;
                case "Vehicle Toll":
                    m.getVehicleToll(vehicle,from,to);
                    break;
                case "Tolls Summary":
                    m.getTollsSummary();
                    break;
                case "Gate Report":
                    m.getGateReport(gate);
                    break;
            }   
        }

        private void initialize_reportProperties(object sender, EventArgs e)
        {
            //TODO inicializovat vehiclePicker, vehiclePicker2, gatePicker
           
        }

    }
}
