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
            var loginForm = new LoginForm(Database);

            while (!Database.IsAuthenticated())
            {
                try
                {
                    var result = loginForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
