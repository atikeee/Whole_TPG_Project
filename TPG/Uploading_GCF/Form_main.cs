using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WritingToDB;
using System.Threading;
using Utility;
namespace Uploading_db
{
    public partial class FormUpload : Form
    {
        //private List<string> srvconflist;
        private string dbname = "testplandb";
        private string ver;

        private int idv;
        private List<string> sheetlist;
        private string filepath;
        private Thread workerThread = null;
        private delegate void SetTextCallback(string text);
        public delegate void EnableNextCallback(bool enable);
        public delegate void AppendErrorLog(string text, int lvl);

        public Dictionary<string, string> returndictreco = new Dictionary<string, string>();
        public Dictionary<string, string> picsBandRecoStr = new Dictionary<string, string>();
        public MySqlDb dbobj;
        private Logging lg;
          
        public FormUpload()
        {
            //srvconflist = new List<string>() { "", "", "" };
            InitializeComponent();
            dbobj = new MySqlDb(dbname);
            lg = new Logging("log.txt", 0);
            dbobj.lgsql = lg;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xlsx";
            ofd.ShowDialog();
            textBoxPath.Text = ofd.FileName;
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormServerConfig formcfg = new FormServerConfig(dbobj);
            formcfg.Show();
        }
        public void apendlog(string text, int lvl = 4)
        {
            if (this.textBoxlog.InvokeRequired)
            {
                if (!this.textBoxlog.IsDisposed)
                {
                    AppendErrorLog d = new AppendErrorLog(apendlog);
                    this.Invoke(d, new object[] { text, lvl });
                }
            }
            else
            {
                string prefix = "";
                if (lvl == 1)
                {
                    prefix = "CRITICAL";
                }
                else if (lvl == 2)
                {
                    prefix = "ERROR";
                }
                else if (lvl == 3)
                {
                    prefix = "WARN";
                }
                else
                {
                    prefix = "INFO";
                }
                //this.textBoxlog.Text +=String.Format("{0,-20}: {1,20}",prefix,text+ "\r\n");
                string[] array = textBoxlog.Lines;
                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = String.Format("{0}\t{1}", prefix, text);
                textBoxlog.Lines = array;

            }
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {

            
            this.buttonabort.Enabled = true;
            
            this.workerThread = new Thread(new ThreadStart(this.buttonUploadHelper));
            this.workerThread.Start();
        }


        private void buttonUploadHelper()
        {
            if (textBoxVer.Text.Trim() == "")
            {
                MessageBox.Show("No Version information given !!!");
                return;
            }

            EnableButton(false);

            string fp = textBoxPath.Text;
            Console.WriteLine(fp);

            if (!File.Exists(fp))
            {
                MessageBox.Show("Please check the path.");
                apendlog("File path is wrong: " + fp.ToString());
            }
            else
            {
                ver = textBoxVer.Text;
                int tcgcount = 1; 
                foreach (string tcg in selected_testgroup)
                {
                    SetLabelstat(string.Format("UPLOADING.. [{0}] {1}/{2}",tcg, tcgcount,selected_testgroup.Count));
                    tcgcount += 1;
                    if (selected_testgroup.Count == 1)
                    {
                        filepath = fp;
                    }else
                    {
                        filepath = fp.ToLower().Replace(".xlsx", "_" + tcg + ".xlsx");
                    }
                    Debug.Print(filepath);
                    CheckBox chkbx = (CheckBox)this.Controls["chkBox" + tcg];
                    if ((chkbx.Checked)&&(File.Exists(filepath)))
                    {
                        sheetlist = MySqlDb.GetExcelsheetslist(filepath, "");
                        dbname = tcg.ToLower() + "testplandb";
                        dbobj.DatabaseName = dbname;
                        if (dbobj.getconnectionstat())
                        {
                            dbobj.changedb();
                            apendlog("db changed to: " + dbname);
                        }
                        else
                        {
                            dbobj.connectToDatabase();
                            apendlog("connected to: " + dbname);
                        }
                        if (dbobj.getconnectionstat())
                        {
                            dbobj.tablename = "gcfptcrbver";
                            idv = dbobj.getid("id", String.Format("ver_gcf_ptcrb_op = '{0}'", ver));
                            if (idv != -1)
                            {
                                apendlog("The version already exist");
                                MessageBox.Show("The version:({0})  already exist for re uploading delete existing version.", textBoxVer.Text);
                            }
                            else
                            {

                                dbobj.insertdata(new List<string>() { ver }, new List<string>() { "ver_gcf_ptcrb_op" });
                                idv = dbobj.getid("id", String.Format("ver_gcf_ptcrb_op = '{0}'", ver));
                                apendlog("Database Version ID: " + idv);
                                uploadDatabase();
                            }

                        }
                    }
                   
                }

                //Debug.Print("DatabaseName" + dbname);
                
                
                //MySqlDb newobj = new MySqlDb(dbname);
                
                //if (!dbobj.getconnectionstat())
                //    dbobj.connectToDatabase();

               
                //idv = -1;
                //lg.inf("versionid: " + idv);

                
            }

            EnableButton(true);
            SetLabelstat("UPLOADING DONE");
        }
        private void EnableButton(bool en)
        {
            if (this.buttonUpload.InvokeRequired)
            {
                Debug.Print("buttonUpload.InvokeRequired");
                if (!this.buttonUpload.IsDisposed)
                {
                    Debug.Print("buttonUpload.IsDisposed");
                    EnableNextCallback d = new EnableNextCallback(EnableButton);
                    this.Invoke(d, new object[] { en });
                }
            }
            else
            {
                this.buttonUpload.Enabled = en;
                this.menuStrip1.Enabled = en;
            }
        }


        private void SetTextBoxsh(string text)
        {
            //text = text.Remove(text.Length);
            if (this.textBoxsh.InvokeRequired)
            {
                if (!this.textBoxsh.IsDisposed)
                {
                    SetTextCallback d = new SetTextCallback(SetTextBoxsh);
                    this.Invoke(d, new object[] { text });
                }
            }
            else
            {
                this.textBoxsh.Text = text;
            }
        }

        private void SetLabelLn(string text)
        {
            if (this.labelln.InvokeRequired)
            {
                if (!this.labelln.IsDisposed)
                {
                    SetTextCallback d = new SetTextCallback(SetLabelLn);
                    this.Invoke(d, new object[] { text });
                }
            }
            else
            {
                this.labelln.Text = text;
            }
        }
        private void SetLabelstat(string text)
        {
            if (this.labelstat.InvokeRequired)
            {
                if (!this.labelstat.IsDisposed)
                {
                    SetTextCallback d = new SetTextCallback(SetLabelstat);
                    this.Invoke(d, new object[] { text });
                }
            }
            else
            {
                this.labelstat.Text = text;
            }
        }

        private void uploadDatabase()
        {
            lg.cri(string.Format("Start Time: {0:HH:mm:ss tt}", DateTime.Now));
            DataTable tbl;
            List<string> colname = new List<string>() { "VER", "PICS_Ver" };
            //dbobj.tablename = "versinfo";
            //dbobj.insertdata(colval,colname);
            //idv = dbobj.getid("ID", String.Format("VER = '{0}' and PICS_Ver = '{1}'", textBoxVer.Text, textBoxPICS.Text));
            //Debug.Print("ID Version table: ", idv);
            // this is for config error
            //var errcsv1 = new StringBuilder();
            // this is for data error
            //var errcsv2 = new StringBuilder();
            //string row7 = "";
            // this index is from the excel. band =3, icebandsupport 10 and tcsupport 6. 
            //List<int> indlisttctable = new List<int>() { 0, 1, 2, 8, 11 };
            //  0:TC Number,1:Description,2:Type of Test,3:Band Applicability,4:Cat,5:Standards,6:Environmental Condition,7:Band Criteria,8:Band,9:ICE Recommendation for Band,10:TC Status,11:Certified TP [V],12:Certified TP [E],13:Certified TP [D]
            //int[] excelcolidx = {0,1,2,3,4, };
            List<int> indlisttctable = new List<int>() { 5, 0, 1, 6, 7 };

            //List<int> indlistbandtable = new List<int>() { 3 };
            //List<int> indlistbanddtable = new List<int>() { 6, 10 };
            //List<int> indlistdatatable = new List<int>() { 4, 5 };
            List<int> indlistbandtable = new List<int>() { 8};
            List<int> indlistbanddtable = new List<int>() { 10, 9 };
            List<int> indlistdatatable = new List<int>() { 11, 12,13 };
            // this string is made of the database col names the size should be alligned with above intlist. 
            // col name should be matched with database. 

            string testcasecolstr = @"`spec`,`testcase`,`description`,`id#envcond`,`id#band_app_cri`,`sheetname`";
            //string bandcolstr = @"`band`,`tcstatus`,`icebandsupport`";
            string bandcolstr = @"`band`,`band2`";
            //string banddcolstr = @"`tcstatus`,`icebandsupport`";
            string datacolstr = @"`certified_tp_v`,`certified_tp_e`,`certified_tp_d`";
            string mappingcolstr = @"`id#tc`,`id#testbandconfig`,`id#tcdata`,`id#ver`,`tcstatus`,`wi_rft`";
            //string mappingcolstr = @"`id#tc`,`id#testbandconfig`,`id#tcdata`,`id#ver`,`id#bandd`";
            string mappingallstr = "";
            List<_List<string>> listoflisttc = new List<_List<string>>();
            List<_List<string>> listoflistdata = new List<_List<string>>();
            List<_List<string>> listoflistband = new List<_List<string>>();
            _List<string> curlisttc;
            _List<string> curlistband;
            _List<string> curlistdata;
            Dictionary<string, DataTable> alltbl = new Dictionary<string, DataTable>();
            foreach (string sh in sheetlist)
            {


                tbl = MySqlDb.GetExcelData(filepath, sh, "");
                string shnameclean = sh.Replace("$", "").Replace("#",".") ;
                alltbl.Add(shnameclean, tbl);
                Debug.Print(String.Format("Sheet info:{0}:", sh));
                apendlog(String.Format("Sheet:{0}", sh));
                SetTextBoxsh(shnameclean + " table: tcconfigtable");
                SetLabelLn("writing to csv for upload");

                foreach (DataRow row in tbl.Rows)
                {
                    // This portion of commented code will make sure no duplicates in the csv to upload

                    curlisttc = getlist(row, indlisttctable, new List<string>() { shnameclean });
                    curlistband = getlist(row, indlistbandtable);
                    string oldb = curlistband[0];
                    string newb = BandProcess.convertband(oldb);
                    curlistband.Add(newb);
                    curlistdata = getlist(row, indlistdatatable);
                    if (!curlisttc.chkmatch(listoflisttc, new List<int>() { 0, 1, 5 }))
                    {
                        //remove space from tc;
                        curlisttc[1] = curlisttc[1].Replace(" ", "");
                        listoflisttc.Add(curlisttc);
                        lg.inf(curlisttc.ToString());
                    }

                    if (!curlistband.chkmatch(listoflistband, new List<int>() { 0 }))
                    {
                        listoflistband.Add(curlistband);
                        lg.inf(curlistband.ToString());
                    }

                    if (!curlistdata.chkmatch(listoflistdata, new List<int>() { 0,1,2 }))
                    {
                        listoflistdata.Add(curlistdata);
                        lg.inf(curlistdata.ToString());
                    }


                }


                SetTextBoxsh(sh + " table: tcdatatable");

                SetLabelLn("");
            }

            SetLabelLn("uploading to database");
            writetocsv(@"c:/temp/_tc.txt", listoflisttc);
            writetocsv(@"c:/temp/_band.txt", listoflistband);

            writetocsv(@"c:/temp/_data.txt", listoflistdata);
            if (!dbobj.getconnectionstat())
                dbobj.connectToDatabase();
            apendlog("Uploading processed data to database");
            dbobj.tablename = "testcasetable";
            dbobj.insertfile(@"c:/temp/_tc.txt", testcasecolstr);
            dbobj.tablename = "testbandconfig";
            dbobj.insertfile(@"c:/temp/_band.txt", bandcolstr);
            //dbobj.tablename = "testbanddata";
            //dbobj.insertfile(@"c:/temp/_bandd.txt", banddcolstr);
            dbobj.tablename = "tcdata";
            dbobj.insertfile(@"c:/temp/_data.txt", datacolstr);
            SetLabelLn("");




            Dictionary<string, string> getidcond = new Dictionary<string, string>();
            string idtc, idband, iddata;// idbandd;
            // get tables from the database. and store to table. later need to grab ID and create the mapping table. 
            dbobj.tablename = "testbandconfig";
            DataTable dbtblband = dbobj.getdatatble();
            //dbobj.tablename = "testbanddata";
            //DataTable dbtblbandd = dbobj.getdatatble();
            dbobj.tablename = "tcdata";
            DataTable dbtbldata = dbobj.getdatatble();
            string csvmapping = @"c:/temp/_mapping.txt";
            //this table is from excel. so indexing should follow from excel. 
            foreach (KeyValuePair<string, DataTable> kvp in alltbl)
            {
                dbobj.tablename = "testcasetable";

                string shname = kvp.Key;

                DataTable dbtbltc = dbobj.getdatatble(String.Format("`sheetname` = '{0}'", shname));
                //foreach(DataRow dr in dbtbltc.Rows)
                //{
                //    string li;
                //    li = "tc # ";
                //    for (int dri = 0; dri < 6; dri++)
                //    {
                //        li += "\"" + dr[dri] + "\"" + "\t\t";
                //    }
                //    li += "\"" + shname + "\"" + "\t\t";
                //    lg.inf("tbl from db: " + li);
                //}
                //foreach (DataRow dr in kvp.Value.Rows)
                //{
                //    string li;
                //    li = "tc # ";
                //    for (int dri = 0; dri < 11; dri++)
                //    {
                //        li += "\"" + dr[dri] + "\"" + "\t\t";
                //    }
                //    li += "\"" + shname + "\"" + "\t\t";
                //    lg.inf("tbl from excel: " + li);
                //}
                //  0:TC Number,1:Description,2:Type of Test,3:Band Applicability,4:Cat,5:Standards,6:Environmental Condition,7:Band Criteria,8:Band,9:ICE Recommendation for Band,10:TC Status,11:Certified TP [V],12:Certified TP [E],13:Certified TP [D]
                foreach (DataRow dr in kvp.Value.Rows)
                {

                    idtc = "-1";
                    idband = "-1";
                    iddata = "-1";
                    getidcond.Add("spec", dr[5].ToString());
                    getidcond.Add("testcase", dr[0].ToString().Replace(" ",""));
                    getidcond.Add("sheetname", shname);
                    lg.war("spec"+dr[5].ToString()+" tc "+dr[0].ToString()+" sheetname "+shname);
                    idtc = dbobj.getid_str(dbtbltc, getidcond);
                    getidcond.Clear();


                    getidcond.Add("certified_tp_v", dr[11].ToString());
                    getidcond.Add("certified_tp_e", dr[12].ToString());
                    getidcond.Add("certified_tp_d", dr[13].ToString());
                    //string xxx = dr[11]+"->"+dr[12]+"->"+dr[13]+"->>";
                    //foreach(DataRow ddd in dbtbldata.Rows)
                    //{
                    //    lg.inf(xxx+ ddd[0].ToString()+"\t" + ddd[1].ToString() + "\t" + ddd[2].ToString() + "\t");
                    //}
                    iddata = dbobj.getid_str(dbtbldata, getidcond);
                    getidcond.Clear();
                    //
                    getidcond.Add("band", dr[8].ToString());
                    idband = dbobj.getid_str(dbtblband, getidcond);
                    getidcond.Clear();

                    //getidcond.Add("tcstatus", dr[10].ToString());
                    //getidcond.Add("icebandsupport", dr[9].ToString());
                    //idbandd = dbobj.getid_str(dbtblbandd, getidcond);
                    getidcond.Clear();
                    //lg.inf(String.Format("idtc: {0} idband: {1} iddata : {2}",idtc,idband,iddata));
                    string li = "";
                    if (idtc == "-1")
                    {
                        li += "tc # ";
                        foreach (int dri in indlisttctable)
                        {
                            li += "\"" + dr[dri] + "\"" + "\t\t";
                        }
                        li += "\"" + shname + "\"" + "\t\t";
                    }
                    else if (idband == "-1")
                    {
                        li += "bandinfo # ";
                        foreach (int dri in indlistbandtable)
                        {
                            li += "\"" + dr[dri] + "\"" + "\t\t";
                        }
                    }
                    else if (iddata == "-1")
                    {
                        li += "data # ";
                        foreach (int dri in indlistdatatable)
                        {
                            li += "\"" + dr[dri] + "\"" + "\t\t";
                        }
                    }

                    if (li != "")
                    {
                        li = "ID not match: " + li;
                        lg.cri(li);
                    }
                    /*
                    else
                    {
                        li = "tc # ";
                        foreach (int dri in indlisttctable)
                        {
                            li += "\"" + dr[dri] + "\"" + "\t\t";
                        }
                        li += "\"" + shname + "\"" + "\t\t";
                        lg.war("id match: " + idtc+ " : "+li);
                    }
                    */
                    //mappingallstr += (String.Format("{0}\t{1}\t{2}\t{3}\t\n", idtc, idband, iddata,idv));
                    //mappingallstr += (String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", idtc, idband, iddata, idv, idbandd,shname)); 
                    mappingallstr += (String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", idtc, idband, iddata, idv, dr[10].ToString(), dr[15].ToString()));
                }
            }

            // save file and upload to db
            File.WriteAllText(csvmapping, mappingallstr);
            dbobj.tablename = "mapping_tc_band_table";
            dbobj.insertfile(csvmapping, mappingcolstr);
            /*

            foreach (string sh in sheetlist)
            {
                //var csv1 = new StringBuilder();
                //var csv2 = new StringBuilder();
                //var csv3 = new StringBuilder();
                tbl = MySqlDb.GetExcelData(filepath, sh, "");
                //Debug.Print(String.Format("Sheet info:{0}:", sh));
              //  SetTextBoxsh(sh.Replace("$", "") + " table: tcconfigtable");
              //  SetLabelLn("writing to csv for upload");

                foreach (DataRow row in tbl.Rows)
                {
        
       

        
                }

            }


            */









            lg.cri(string.Format("End Time: {0:HH:mm:ss tt}", DateTime.Now));
        }

        private void writetocsv(string fn, List<_List<string>> lolstr)
        {
            StringBuilder csvstring = new StringBuilder();
            foreach (_List<string> oneline in lolstr)
            {
                csvstring.AppendLine(String.Join("\t", oneline) + "\t");
                File.WriteAllText(fn, csvstring.ToString(), Encoding.ASCII);
            }
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            string _filepath = this.textBoxPath.Text;
            string excelname = (System.IO.Path.GetFileNameWithoutExtension(_filepath));
            string pattern = @"(.+)_V_(.+)_P_(.+)";
            MatchCollection matches = Regex.Matches(excelname, pattern);
            this.textBoxFile.Text = excelname;
            foreach (Match match in matches)
            {
                this.textBoxVer.Text = match.Groups[2].Value;
                //comboBoxTCCat.SelectedItem = match.Groups[1].Value;
            }
        }




        private void buttondel_Click(object sender, EventArgs e)
        {
            if (!dbobj.getconnectionstat())
            {
                dbobj.connectToDatabase();
            }
            if (dbobj.getconnectionstat())
            {
                foreach(string tcgsel in selected_testgroup)
                {
                    dbobj.DatabaseName = tcgsel.ToLower() + "testplandb";
                    dbobj.changedb();
                    dbobj.tablename = "gcfptcrbver";
                    ver = textBoxVer.Text;
                    string idver = dbobj.getid_str("id", String.Format("ver_gcf_ptcrb_op = '{0}'", ver));
                    if (idver != "-1")
                    {
                        Form_Confirm fd = new Form_Confirm(String.Format("Please confirm if you want to delete all data of\n   version: '{0}' from database: '{1}'", ver, tcgsel));
                        if (fd.ShowDialog(this) == DialogResult.OK)
                        {
                            labelstat.Text = "Delete in progress";
                            //MySqlDb newobj = new MySqlDb(dbname);
                            labelstat.Text = "Deleting data.";
                            dbobj.deletedatafromalltable(idver);
                            MessageBox.Show("All data has been deleted from\nversion: " + ver +" from Database: "+tcgsel);
                            labelstat.Text = "Delete complete.";
                        }

                    }
                    else
                    {
                        MessageBox.Show("No matched version data found in database to delete for version: " + ver);
                    }
                }
                //if (this.comboBoxTCCat.SelectedItem.ToString() != "")
                //{
                //    dbname = comboBoxTCCat.SelectedItem.ToString().ToLower() + "testplandb";
                //    dbobj.DatabaseName = dbname;
                //    dbobj.tablename = "gcfptcrbver";
                //    dbobj.changedb();
                //    ver = textBoxVer.Text;
                //    //dbname ="testplandb";
                //    string idver = dbobj.getid_str("id", String.Format("ver_gcf_ptcrb_op = '{0}'", ver));
                //    if (idver != "-1")
                //    {
                //        Form_Confirm fd = new Form_Confirm(String.Format("Please confirm if you want to delete all data of\n   version: '{0}' from database: '{1}'", ver, dbname));
                //        if (fd.ShowDialog(this) == DialogResult.OK)
                //        {
                //            labelstat.Text = "Delete in progress";
                //            //MySqlDb newobj = new MySqlDb(dbname);
                //
                //
                //            labelstat.Text = "Deleting data.";
                //            dbobj.deletedatafromalltable(idver);
                //            MessageBox.Show("All data has been deleted from\nversion: " + ver);
                //            labelstat.Text = "Delete complete.";
                //        }
                //
                //    }
                //    else
                //    {
                //        MessageBox.Show("No matched version data found in database to delete for version: " + ver);
                //    }
                //}
                //else
                //{
                //
                //    MessageBox.Show("check the version field");
                //}
            }
            else
            {
                MessageBox.Show("no db connection");

            }

        }

        private bool validateform()
        {

            bool b = true;
            //if ((this.textBoxVer.Text == "") || (this.comboBoxTCCat.SelectedItem.ToString() == ""))
            //{
            //    b = false;
            //}
            return b;
        }

        private void buttonabort_Click(object sender, EventArgs e)
        {
            labelstat.Text = "Upload Aborted";
            workerThread.Abort();
            this.buttonUpload.Enabled = true;
            this.menuStrip1.Enabled = true;
            this.buttonabort.Enabled = false;
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Red;
        }
        private bool connectionind()
        {
            bool connind = dbobj.getconnectionstat();
            if (connind)
            {
                pictureBox1.BackColor = Color.Green;
            }
            else
            {
                pictureBox1.BackColor = Color.Red;
            }
            return connind;
        }



        private Dictionary<string, string> dbquery(string tablename, List<string> colnames, string conditiondict)
        {
            Dictionary<string, string> returnval = new Dictionary<string, string>();
            dbobj.tablename = tablename;
            DataTable retdt = dbobj.getdatatble(colnames, conditiondict);
            foreach (DataRow row in retdt.Rows)
            {
                //Debug.Print(row[0].ToString());
                returnval.Add(row[0].ToString(), row[1].ToString());
            }


            return returnval;

        }

        private void button_query_Click(object sender, EventArgs e)
        {
            dbobj.DatabaseName = "testplandb";
            dbobj.connectToDatabase();
            string tcno = "";
            string spec = "";
            List<string> colnames = new List<string>() { "Band", "ID" };
            string Conditionstring = String.Format("TCNumber = {0} and standards = {1}", tcno, spec);
            Dictionary<string, string> returnval;
            returnval = dbquery("tcconfigtable", colnames, Conditionstring);

            foreach (KeyValuePair<string, string> kvp in returnval)
            {
                Debug.Print("entered");
                Debug.Print("{0},{1}", kvp.Key, kvp.Value);
            }
        }



        //private string picsreco(string recostring)
        //{
        //    List<string> recoliststring = new List<string>();
        //    recoliststring.Add(recostring);
        //    string idval = "";
        //    int olddataind = 0;
        //    List<string> reconame = new List<string>() { "ID", "TCR_BandR_Rel" };
        //    List<string> recocol = new List<string>(){"TCR_BandR_Rel"}; 
        //    dbobj.tablename = "pics_recommended_table";
        //    string condrecostring = "";
        //     //returndictreco = dbquery(dbobj.tablename, reconame, condrecostring);


        //    foreach (KeyValuePair<string, string> kvp in returndictreco)
        //    {
        //        Debug.Print(kvp.Key + kvp.Value + "dictkvp");
        //        //Debug.Print(recostring + ":recostring" + kvp.Value + ":ID");
        //        if (recostring == kvp.Value)
        //        {
        //            idval = kvp.Key;
        //            olddataind = 1;
        //            Debug.Print("data already present");
        //            Debug.Print(idval + " :ID for 'TCR_BandR_Rel'");
        //        }
        //    }

        //    if (olddataind != 1)
        //        {
        //            Debug.Print("No corresponding data is present");
        //            dbobj.insertdata(recoliststring, recocol);
        //            condrecostring = String.Format("{0} = {1}", recocol[0],"\""+recostring+"\"");
        //            returndictreco = dbquery(dbobj.tablename, reconame, condrecostring);
        //            foreach (KeyValuePair<string, string> kvp1 in returndictreco)
        //            {
        //                idval = kvp1.Key;
        //                Debug.Print("new data inserted: ID = " + idval + ", val = " + kvp1.Value);
        //            }
        //            olddataind = 0;
        //        }
        //    return idval;
        //}



        public String stringfromrow(DataRow row, List<int> indexlist)
        {
            List<String> outstringlist = new List<string>();
            foreach (int ind in indexlist)
                outstringlist.Add(row[ind].ToString());
            string outstring = String.Join("\t", outstringlist);
            return outstring;
        }

        public _List<string> getlist(DataRow row, List<int> indexlist)
        {
            _List<String> outstringlist = new _List<string>();
            foreach (int ind in indexlist)
                outstringlist.Add(row[ind].ToString());
            //string outstring = String.Join("\t", outstringlist);
            return outstringlist;
        }
        public _List<string> getlist(DataRow row, List<int> indexlist, List<string> extracol)
        {
            _List<String> outstringlist = new _List<string>();
            foreach (int ind in indexlist)
                outstringlist.Add(row[ind].ToString().Replace("\n", ""));
            foreach (string extr in extracol)
                outstringlist.Add(extr);
            //string outstring = String.Join("\t", outstringlist);
            return outstringlist;
        }

        private void uploadBSInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPICSBandUL formcfg = new FormPICSBandUL(dbobj);
            formcfg.Show();
        }
        private string[] testgroups = {"RF","RRM","CTPS","ATT","VZW","CMCC" };
        
        private void FormUpload_Load(object sender, EventArgs e)
        {
            int n = 0;
            foreach(string testcat in testgroups)
            {
                CheckBox chkbx = new CheckBox();
                chkbx.AutoSize = true;
                //180 - 245- 
                
                chkbx.Location = new System.Drawing.Point(200 + 62 * n, 90);
                chkbx.Name = "chkBox" + testcat;
                chkbx.Size = new System.Drawing.Size(50, 20);
                //chkbx.TabIndex = 31;
                chkbx.Text = testcat;
                chkbx.UseVisualStyleBackColor = true;
                //allchkbox[n] = chkbx;
                chkbx.CheckedChanged += new System.EventHandler(this.chkchangeevent);
                n++;
                this.Controls.Add(chkbx);
            }
        }
        private List<string> selected_testgroup = new List<string>();
        private void chkchangeevent(object sender, EventArgs e)
        {
            selected_testgroup.Clear();
            foreach (string tcg in testgroups)
            {
                CheckBox chkbx = (CheckBox)this.Controls["chkBox" + tcg];
                if (chkbx.Checked) 
                {
                    selected_testgroup.Add(tcg);
                }
            }
        }



        //public String stringfromrow(DataRow row, List<int> indexlist)
        //{
        //    String outstring;
        //    foreach (int ind in indexlist)
        //        String.Join("\t",outstring(row[ind].ToString()));
        //    return outstring;
        //}


    }
}
