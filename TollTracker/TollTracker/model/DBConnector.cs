using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

//https://codeabout.wordpress.com/2012/03/08/working-with-postgres-database-and-c-by-using-npgsql-and-reflection/
namespace TollTracker.model
{
    /// <summary>
    /// Abstarktní třída pro modely, která zajišťuje hlavní práci s DB
    /// </summary>
    public abstract class DBConnector
    {
        private const string server = "localhost";
        private const string port = "5432";
        private const string database = "TollTracker";
        private const string uid = "postgres";
        private const string password = "postgres";
        protected NpgsqlConnection connection = new NpgsqlConnection("Server=" + server + ";Port=" + port + ";User Id=" + uid + ";Password=" + password + ";Database=" + database + ";");

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
