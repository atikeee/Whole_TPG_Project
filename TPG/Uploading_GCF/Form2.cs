using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using WritingToDB;
namespace Uploading_db
{

    public partial class FormServerConfig : Form
    {
        private MySqlDb sqldb; 
        //private List<string> srvconflist;
        public FormServerConfig(MySqlDb sqldb)
        {
            InitializeComponent();
          //  this.srvconflist = srvconflist;
            this.sqldb = sqldb; 
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var inputServerIP = Convert.ToString(textBoxServerIP.Text);
            inputServerIP = inputServerIP.Trim();
            var inputUsername = Convert.ToString(textBoxUsername.Text);
            inputUsername = inputUsername.Trim();
            var inputPassword = Convert.ToString(textBoxPassword.Text);
            inputPassword = inputPassword.Trim();
            //Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            //config.AppSettings.Settings.Add("IP", inputServerIP);
            //config.Save(ConfigurationSaveMode.Minimal);
            sqldb .ServerName= inputServerIP;
            sqldb.UserName = inputUsername;
            sqldb.PassWord = inputPassword;
            sqldb.connectToDatabase();
            this.Close();
            if (sqldb.getconnectionstat())
            {
                MessageBox.Show("Connected to Database");

            }
            else
            {
                MessageBox.Show("Not Connected to Database");

            }
        }

        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormServerConfig_Load(object sender, EventArgs e)
        {
            textBoxServerIP.Text = sqldb.ServerName;
            textBoxUsername.Text = sqldb.UserName;
            textBoxPassword.Text = sqldb.PassWord;
        }


    }
}
