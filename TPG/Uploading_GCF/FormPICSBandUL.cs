using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.OleDb;
using System.IO;
using Microsoft;
using WritingToDB;

namespace Uploading_db
{
    public partial class FormPICSBandUL : Form
    {
        private MySqlDb sqldb;
        public FormPICSBandUL(MySqlDb sqldb)
        {
            InitializeComponent();
            this.sqldb = sqldb;
        }


       

      

        private void button_submit_Click(object sender, EventArgs e)
        {
            sqldb.DatabaseName = "rftestplandb";
            sqldb.connectToDatabase();

            //   button_submit.Enabled = false;
            //string[] dbnmaearr = { "rftestplandb", "rrmtestplandb", "ctpstestplandb", "vzwtestplandb", "atttestplandb", "cmcctestplandb" };
            string[] dbnmaearr = {  "ctpstestplandb", "vzwtestplandb"};
            foreach (string dbname in dbnmaearr)
            {
                sqldb.DatabaseName = dbname;
                sqldb.changedb();
                sqldb.tablename = "iceband";
                //List<string> colname = new List<string>() { "PICS_Ver", "BandSupported" };
                //List<string> colvalue = new List<string>() { comboBoxpicsver.SelectedItem.ToString(), textBoxpicsband.Text };
                try
                {
                    sqldb.insertorupdateicebandinfo(textBoxProj.Text, textBoxband.Text, textBoxBandULCA.Text);
                    
                }
                catch (Exception E)
                {
                    MessageBox.Show("Exception: " + E.ToString());
                }
            }
            //MessageBox.Show("New data inserted .. ");

        }

      
    }
}
