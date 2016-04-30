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
            model.readFile("testBig.txt.json", printFileError, printOneTollError, showNumberOfProcessedTolls);
        }
    }
}
