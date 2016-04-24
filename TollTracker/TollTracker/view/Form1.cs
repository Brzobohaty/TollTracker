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
        public Form1(Model model)
        {
            InitializeComponent();
            model.readFile("test.txt.json", printFileError, printOneTollError);
        }

        public void printFileError(string text) {
            label1.Text = text;
        }

        public void printOneTollError(int tollId, string text) {
            if (tollId != 0)
            {
                listView1.Items.Add("Na mýtu s ID " + tollId + " došlo k chybě: " + text);
            }
            else {
                listView1.Items.Add(text);
            }    
        }
    }
}
