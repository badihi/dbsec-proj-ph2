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
    public partial class LoginForm : Form
    {
        private Database database;
        public LoginForm(Database database)
        {
            InitializeComponent();
            this.database = database;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            database.Login(txUsername.Text, txPassword.Text);
            Hide();
        }
    }
}
