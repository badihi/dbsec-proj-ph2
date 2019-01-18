using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBSecProject
{
    public partial class Form1 : Form
    {
        private Database Database { get; set; }
        public Form1()
        {
            InitializeComponent();
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppDatabaseConnectionString"].ConnectionString;
            Database = new Database(connectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Database.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the database: " + ex.Message);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void DoLogin()
        {
            var loginForm = new LoginForm(Database);

            do
            {
                try
                {
                    var result = loginForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } while (!Database.IsAuthenticated());

            lblLoggedInAs.Text = "Logged in as: " + Database.Subject.Username + " (" + (SecureDB.Type == SecureDBType.EncryptedDB ? "Encrypted DB mode" : "Separated DB mode") + ")";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnChangeUser_Click(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var result = Database.ExecuteQuery(txQuery.Text);
            if (result.Error != null)
            {
                txResults.Text = "An error has occured:\n" + result.Error;
                tab.SelectedIndex = 1;
            }
            else
                switch (result.Type)
                {
                    case QueryResultType.Data:
                        grid.DataSource = result.DataTable;
                        tab.SelectedIndex = 0;
                        break;
                    case QueryResultType.Text:
                        txResults.Text = result.Status;
                        tab.SelectedIndex = 1;
                        break;
                }
        }
    }
}
