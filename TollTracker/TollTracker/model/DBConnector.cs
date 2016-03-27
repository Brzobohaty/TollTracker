using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace TollTracker.model
{
    /// <summary>
    /// Abstarktní třída pro modely, která zajišťuje hlavní práci s DB
    /// </summary>
    abstract class DBConnector
    {
        protected MySqlConnection connection;
        private string server = "localhost";
        private string database = "TollTracker";
        private string uid = "root";
        private string password = "poklop";

        public DBConnector() {
            initialize();
        }

        /// <summary>
        /// Inicializuje připojení k databázi
        /// </summary>
        private void initialize()
        {
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Otevře připojení k databázi
        /// </summary>
        /// <returns>true pokud se povedlo</returns>
        public bool openConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// Ukončí připojení k databázi
        /// </summary>
        /// <returns>true pokud se povedlo</returns>
        public bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
