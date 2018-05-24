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
using Utility;
using System.Threading;
using static Parse_Pixit_Table.StringProcess;
using System.Xml;

namespace Parse_Pixit_Table
{
    public partial class FormUpload : Form
    {
        private string[] testcategory = {"RF","RRM","CTPS","ATT","VZW","CMCC" };
        private List<string> tc_cat_selected = new List<string>();
        public string testcat; 
        public string dbnameprefix = "rf";
        public string picsver;
        public string gcfver = "";
        public _Dictionary<string, string> versiontable;
        private int idv;
        private string filepath;
        private delegate void SetTextCallback(string text);
        private delegate void SetProgressCallback(int val);
        private delegate void AppendErrorLog(string val, int lvl);
        public delegate void EnableNextCallback(bool enable);
        public _Dictionary<string, string> returndictreco = new _Dictionary<string, string>();
        public _Dictionary<string, string> picsBandRecoStr = new _Dictionary<string, string>();
        public static MySqlDb dbobj;
        public  Logging lg;
        public TwoKeyDictionary<string, string, string> tc_band_id;
        //public TwoKeyDictionary<string, string, string> tc_band_sheet;
        public _Dictionary<string, string> tc_des;
        public TwoKeyDictionary<string, string, string> tc_band_status;
        public _Dictionary<string, string> tc_bandapplicability;
        private Thread workerThread;
        public string icebandall;
        public string icebandulca;
        //private CheckBox chkbx  ;
        private Button btn ;
        public FormUpload()
        {
            MessageBox.Show("FORMUPLOAD");
            int n = 0;
            InitializeComponent();
            dbobj = new MySqlDb(dbnameprefix + "testplandb");
            string curDir = System.Environment.CurrentDirectory;
            string _logfilepath = Path.Combine(curDir, "log.txt");
            this.lg = new Logging(_logfilepath, 21);
            dbobj.lgsql = lg;
        }
        public FormUpload(Logging lg)
        {
            InitializeComponent();
            dbobj = new MySqlDb(dbnameprefix + "testplandb");
            string curDir = System.Environment.CurrentDirectory;
            string _logfilepath = Path.Combine(curDir, "log.txt");
            this.lg = lg; 
            dbobj.lgsql = lg;
        }
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm;*.xlsb";
            ofd.ShowDialog();
            textBoxPath.Text = ofd.FileName;
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormServerConfig formcfg = new FormServerConfig(dbobj);
            formcfg.Show();
        }


        private void buttonUpload_Click(object sender, EventArgs e)
        {
            SetLabelstat2("Task Started. ");
            SetLabelstat("Task Initiated. ");
            lgstr = lg;
            
            picsver = textBoxPICS.Text;
            if (validateform())
            {
                allver = checkBoxComboBox1.Text;
                this.workerThread = new Thread(new ThreadStart(this.uploadDatabase));
                this.workerThread.Start();
            }

            


        }

        public void SetProgress(int val)
        {
            if (this.progressBar1.InvokeRequired)
            {
                if (!this.progressBar1.IsDisposed)
                {
                    SetProgressCallback d = new SetProgressCallback(SetProgress);
                    this.Invoke(d, new object[] { val });

                }
            }
            else
            {
                this.progressBar1.Value = val;

            }
        }
        private void EnableButton(bool en)
        {
            if (this.buttonUpload.InvokeRequired)
            {
                if (!this.buttonUpload.IsDisposed)
                {
                    EnableNextCallback d = new EnableNextCallback(EnableButton);
                    this.Invoke(d, new object[] { en });
                }
            }
            else
            {
                this.buttonUpload.Enabled = en;
                this.menuStrip1.Enabled = en;
                this.buttonBrowse.Enabled = en;
                this.button1.Enabled = en;
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
        public void SetLabelstat(string text)
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
        public void SetLabelstat2(string text)
        {
            if (this.labelstat2.InvokeRequired)
            {
                if (!this.labelstat2.IsDisposed)
                {
                    SetTextCallback d = new SetTextCallback(SetLabelstat2);
                    this.Invoke(d, new object[] { text });
                }
            }
            else
            {
                this.labelstat2.Text = text;
            }
        }
        public  void apendlog(string text,int lvl=4)
        {
            if (this.textBoxlog.InvokeRequired)
            {
                if (!this.textBoxlog.IsDisposed)
                {
                    AppendErrorLog d = new AppendErrorLog(apendlog);
                    this.Invoke(d, new object[] { text ,lvl});
                }
            }
            else
            {
                string prefix = "";
                if(lvl == 1)
                {
                    prefix = "CRITICAL";
                }else if(lvl == 2)
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
                textBoxlog.ScrollToCaret();

            }
        }
        private string allver;
        private void uploadDatabase()
        {

            EnableButton(false);
            int testcatcount = 0;
            //string PICSFileName = "XMM7480_PICS_V1.4.xlsx";
            _List<string> fileSpecDataList = new _List<string>();

            // File name prefix: Example: 34.121-1_CVL.csv, 34.121-1_TVA_RF.csv, etc.
            fileSpecDataList.Add("34.123-1");      //fileindex 0
            fileSpecDataList.Add("36.523-1");      //fileindex 1
            fileSpecDataList.Add("36.521-1");      //fileindex 2
            fileSpecDataList.Add("36.521-3");      //fileindex 3
            fileSpecDataList.Add("34.121-1");      //fileindex 4
            fileSpecDataList.Add("51.010-2");      //fileindex 5

            // Chad's specs:
            fileSpecDataList.Add("31.121-1");      //fileindex 6
            fileSpecDataList.Add("31.124-1");      //fileindex 7
            fileSpecDataList.Add("34.229-1");      //fileindex 8
            fileSpecDataList.Add("51.010-4");      //fileindex 9   used in new code
            fileSpecDataList.Add("102 230-1");     //fileindex 10
            fileSpecDataList.Add("37.571-1");     //fileindex 11
            fileSpecDataList.Add("37.571-2");     //fileindex 12
            fileSpecDataList.Add("34.171");     //fileindex 13
            fileSpecDataList.Add("37.901");     //fileindex 14
            fileSpecDataList.Add("tty");     //fileindex 15
            fileSpecDataList.Add("at-command");     //fileindex 16
            fileSpecDataList.Add("34.122");     //fileindex 17
            string[] onever = allver.Split(',');
            Debug.Print("Database Name: ");
            Debug.Print(dbobj.DatabaseName);
            foreach(string ver in onever)
            {
                gcfver = ver.Trim();
                foreach (string testcat in tc_cat_selected)
                {
                    testcatcount++;

                    SetLabelstat2(string.Format("{0}  {1}  {2} [{3}/{4}]", picsver,gcfver,testcat, testcatcount, (tc_cat_selected.Count*onever.Count())));
                    dbnameprefix = testcat.ToString().ToLower();
                    GenericParser.filereset.Clear();
                    if (!dbobj.getconnectionstat())
                    {
                        dbobj.DatabaseName = dbnameprefix + "testplandb";
                        dbobj.connectToDatabase();

                    }
                    else
                    {
                        dbobj.DatabaseName = dbnameprefix + "testplandb";
                        dbobj.changedb();
                    }
                    if (dbobj.getconnectionstat())
                    {
                        apendlog("Connected to : " + dbnameprefix + "testplandb");
                        versiontable = getfulltable("gcfptcrbver", "ver_gcf_ptcrb_op");
                    }
                    else
                    {
                        MessageBox.Show("no db connection");
                    }

                    _List<string> specTRLFiles = new _List<string>();
                    //_List<string> specTVAFiles = new _List<string>();
                    //_List<string> specCVLFiles = new _List<string>();
                    lg.inf("Select :(database) " + dbnameprefix);
                    lg.inf("all spec files" + fileSpecDataList.ToString());

                    switch (dbnameprefix)
                    {

                        case "rf":
                            List<int> rffiles = new List<int>() {2,4,5 };
                            foreach (int psf in rffiles)
                            {
                                if ((psf == 4)|| (psf == 5))
                                {
                                    specTRLFiles.Add(fileSpecDataList[psf] + "_TRL_RF.txt");
                                }
                                else
                                {
                                    specTRLFiles.Add(fileSpecDataList[psf] + "_TRL.txt");
                                }
                            }
                            //specTVAFiles.Add(fileSpecDataList[2] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[2] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[4] + "_TVA_RF.csv");
                            //specCVLFiles.Add(fileSpecDataList[4] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[5] + "_TVA_RF.csv");
                            //specCVLFiles.Add(fileSpecDataList[5] + "_CVL.csv");
                            break;
                        case "rrm":
                            List<int> rrmfiles = new List<int>() {3,4, 17 };
                            foreach (int psf in rrmfiles)
                            {
                                if (psf == 4)
                                {
                                    specTRLFiles.Add(fileSpecDataList[psf] + "_TRL_RRM.txt");  // only 2 has no RF in file
                                }
                                else
                                {
                                    specTRLFiles.Add(fileSpecDataList[psf] + "_TRL.txt");
                                }
                            }
                            //specTVAFiles.Add(fileSpecDataList[3] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[3] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[4] + "_TVA_RRM.csv");
                            //specCVLFiles.Add(fileSpecDataList[4] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[17] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[17] + "_CVL.csv");
                            break;
                        case "ctps":
                            List<int> psfiles = new List<int>() { 0, 1,5,6,7,8,9,10,11,12,13,14,15,16 };
                            //List<int> psfiles = new List<int>() { 7};
                            foreach (int psf in psfiles)
                            {
                                if(psf == 5)
                                {
                                    specTRLFiles.Add(fileSpecDataList[psf]+"_TRL_CTPS.txt");
                                }
                                else
                                {
                                    specTRLFiles.Add(fileSpecDataList[psf]+"_TRL.txt");
                                }
                            }
                            //specTVAFiles.Add(fileSpecDataList[1] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[1] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[0] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[0] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[5] + "_TVA_CTPS.csv");
                            //specCVLFiles.Add(fileSpecDataList[5] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[6] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[6] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[7] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[7] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[8] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[8] + "_CVL.csv");
                            //
                            ////specTVAFiles.Add(fileSpecDataList[9] + "_TVA.csv");
                            ////specCVLFiles.Add(fileSpecDataList[9] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[10] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[10] + "_CVL.csv");
                            //specTVAFiles.Add(fileSpecDataList[11] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[11] + "_CVL.csv");
                            //specTVAFiles.Add(fileSpecDataList[12] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[12] + "_CVL.csv");
                            //specTVAFiles.Add(fileSpecDataList[13] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[13] + "_CVL.csv");
                            //specTVAFiles.Add(fileSpecDataList[14] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[14] + "_CVL.csv");
                            //specTVAFiles.Add(fileSpecDataList[15] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[15] + "_CVL.csv");
                            //specTVAFiles.Add(fileSpecDataList[16] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[16] + "_CVL.csv");


                            break;

                        case "att":
                            //specTVAFiles.Add(fileSpecDataList[2] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[2] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[4] + "_TVA_RF.csv");
                            //specCVLFiles.Add(fileSpecDataList[4] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[5] + "_TVA_RF.csv");
                            //specCVLFiles.Add(fileSpecDataList[5] + "_CVL.csv");
                            break;
                        default:
                            break;
                        case "vzw":
                            //specTVAFiles.Add(fileSpecDataList[1] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[1] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[0] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[0] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[5] + "_TVA_CTPS.csv");
                            //specCVLFiles.Add(fileSpecDataList[5] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[6] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[6] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[7] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[7] + "_CVL.csv");
                            //
                            //specTVAFiles.Add(fileSpecDataList[8] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[8] + "_CVL.csv");

                            //specTVAFiles.Add(fileSpecDataList[9] + "_TVA.csv");
                            //specCVLFiles.Add(fileSpecDataList[9] + "_CVL.csv");

                            // specTVAFiles.Add(fileSpecDataList[10] + "_TVA.csv");
                            // specCVLFiles.Add(fileSpecDataList[10] + "_CVL.csv");
                            // specTVAFiles.Add(fileSpecDataList[11] + "_TVA.csv");
                            // specCVLFiles.Add(fileSpecDataList[11] + "_CVL.csv");
                            // specTVAFiles.Add(fileSpecDataList[12] + "_TVA.csv");
                            // specCVLFiles.Add(fileSpecDataList[12] + "_CVL.csv");
                            // specTVAFiles.Add(fileSpecDataList[13] + "_TVA.csv");
                            // specCVLFiles.Add(fileSpecDataList[13] + "_CVL.csv");
                            // specTVAFiles.Add(fileSpecDataList[14] + "_TVA.csv");
                            // specCVLFiles.Add(fileSpecDataList[14] + "_CVL.csv");
                            // specTVAFiles.Add(fileSpecDataList[15] + "_TVA.csv");
                            // specCVLFiles.Add(fileSpecDataList[15] + "_CVL.csv");
                            // specTVAFiles.Add(fileSpecDataList[16] + "_TVA.csv");
                            // specCVLFiles.Add(fileSpecDataList[16] + "_CVL.csv");


                            break;

                    }

                    //PICSParseProcessing.mainprocess(this.textBoxPath.Text, specTVAFiles, specCVLFiles, lg, dbobj, this);
                    PICSParseProcessing.mainprocess(this.textBoxPath.Text, specTRLFiles,  lg, dbobj, this);
                }
            }
            /*
            
            */
            apendlog("Processing complete");

            EnableButton(true);
            SetLabelstat("Idle");
            SetLabelstat2("Task Completed. ");
        }





        private void update_label_version()
        {

            label_version.Text = String.Format("PICS Version: {0}\n", textBoxPICS.Text);

        }
       

        private bool validateform()
        {
            string specver = checkBoxComboBox1.Text;
            string[] allver = specver.Split(',');
            string errstr = "";
            bool b = true;
            filepath = textBoxPath.Text;
            if ((this.textBoxPICS.Text == "") || tc_cat_selected.Count == 0)
            {
                b = false;
                MessageBox.Show("Check all the selection !");
            }
            else if (!File.Exists(filepath))
            {
                b = false;
                MessageBox.Show("Please check the pics file path.");
            }
            else if(specver=="")
            {
                MessageBox.Show("Please check at least one spec version");
            }
            else
            {
                foreach (string tccat in tc_cat_selected)
                {
                    if (!dbobj.getconnectionstat())
                    {
                        dbobj.DatabaseName = tccat.ToLower() + "testplandb";
                        dbobj.connectToDatabase();

                    }
                    else
                    {
                        dbobj.DatabaseName = tccat.ToLower() + "testplandb";
                        dbobj.changedb();
                    }
                    foreach (string onever in allver)
                    {
                       
                        gcfver = onever.Trim();
                        picsver = textBoxPICS.Text;

                        dbobj.tablename = "user_picsver";
                        idv = dbobj.getid("id", String.Format(" picsver = '{0}' and gcfver = '{1}' ", picsver, gcfver));
                        if (idv != -1)
                        {
                            errstr += String.Format("{2}  Existing version:({0}) & PICS_ver:({1}).\n", gcfver, picsver,tccat.ToUpper());
                            b = false;
                        }
                    }
                }
                if (errstr != "")
                {
                    MessageBox.Show(errstr);
                }
                

            }
            
            return b;
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

        private void textBoxPICS_TextChanged(object sender, EventArgs e)
        {
            update_label_version();
        }

        private static _Dictionary<string, string> dbquery(string tablename, List<string> colnames, string conditiondict)
        {
            _Dictionary<string, string> returnval = new _Dictionary<string, string>();
            dbobj.tablename = tablename;
            DataTable retdt = dbobj.getdatatble(colnames, conditiondict);
            foreach (DataRow row in retdt.Rows)
            {
                //returnval[row[0].ToString(),row[1].ToString()] = row[2].ToString();
                //Debug.Print(row[0].ToString());
                returnval.Add(row[0].ToString(), row[1].ToString());
            }

            return returnval;
        }
        // colname is the key ID is value. 
        public  _Dictionary<string, string> getfulltable(string tablename, string colname, string valID = "ID")
        {
            _Dictionary<string, string> alldata_table = new _Dictionary<string, string>();
            try
            {
                dbobj.tablename = tablename;
                List<string> colnames = new List<string>() { colname, valID };
                DataTable retdt = dbobj.getdatatble(colnames, "");
                foreach (DataRow row in retdt.Rows)
                {
                    alldata_table[row[0].ToString()] = row[1].ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Exception from gettable : " + ex.ToString());
            }
            return alldata_table;

        }
        public void gettable_tcno_spec_id_band(string spec)
        {
            
            //tc_band_sheet = new TwoKeyDictionary<string, string, string>();
            tc_band_id = new TwoKeyDictionary<string, string, string>();
            tc_band_status = new TwoKeyDictionary<string, string, string>();
            tc_bandapplicability = new _Dictionary<string, string>();
            tc_des = new _Dictionary<string, string>();

            try
            {
                dbobj.tablename = "v_comb_serv_info";
                string Conditionstring = String.Format("(spec = \"{0}\" and ver_gcf_ptcrb_op = \"{1}\")", spec,gcfver);
                List<string> colnames = new List<string>() { "testcase", "band", "id" ,"bandapplicability" , "tcstatus","sheetname", "Description" };
                DataTable retdt = dbobj.getdatatble(colnames, Conditionstring);
                foreach (DataRow row in retdt.Rows)
                {
                    string tcno = row[0].ToString().Replace(" ","").ToLower();
                    if (tc_band_id.ContainsKey(tcno, row[1].ToString()))
                    {
                        //lgstr.cri(String.Format("{GenericParser:gettable_tcno_spec_id_band} duplicate of {0} and {1} found in spec {2} ", row[0].ToString(), row[1].ToString(), spec));
                        lgstr.cri("{ FORMUpload:gettable_tcno_spec_id_band} duplicate tc/band combination. TC: "+ tcno+" Band: "+ row[1].ToString());
                    }
                    if (!tc_bandapplicability.ContainsKey(tcno))
                    {
                        tc_bandapplicability[tcno] = row[3].ToString();
                    }
                    if (!tc_des.ContainsKey(tcno))
                    {
                        tc_des[tcno] = row[6].ToString();
                    }

                    tc_band_id[tcno, row[1].ToString()] = row[2].ToString();
                    tc_band_status[tcno, row[1].ToString()] = row[4].ToString();
                    //tc_band_sheet[tcno, row[1].ToString()] = row[5].ToString();
                }
                //lgstr.inf("tc_bandapplicability");
                //lgstr.inf(tc_bandapplicability.ToString());
                //lgstr.inf("tcbandstatus");
                //lgstr.inf(tc_band_status.ToString());

            }
            catch (Exception ex)
            {
                Debug.Print("Exception from gettable : " + ex.ToString());
                lg.war("{FORMUpload:gettable_tcno_spec_id_band} Exception: " + ex.Message.ToString());
            }
            //return tc_band_id;
        }
        //public static _Dictionary<string, string> getBandListTc(string tcno, string spec)
        //{
        //    //dbobj.DatabaseName = "testplandb";
        //    //dbobj.connectToDatabase();
        //    _Dictionary<string, string> returnval = new _Dictionary<string, string>();
        //    try
        //    {
        //        List<string> colnames = new List<string>() { "Band", "ID" };
        //        string Conditionstring = String.Format("TCNumber = {0} and standards = {1}", tcno, spec);
        //
        //        returnval = dbquery("tcconfigtable", colnames, Conditionstring);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print("Exception: " + ex.ToString());
        //    }
        //    return returnval;
        //}

        private void picsreco(string recostring)
        {
            string idk;
            if (picsBandRecoStr.ContainsKey(recostring))
            {
                idk = picsBandRecoStr[recostring];
            }
            else
            {
                dbobj.insertdata(new List<string>() { recostring }, new List<string>() { "TCR_BandR_Rel" });
                string condrecostring = String.Format(" TCR_BandR_Rel = \"{0}\"", recostring);
                idk = dbobj.getid("ID", condrecostring).ToString();
                //returndictreco = dbquery(dbobj.tablename, reconame, condrecostring);
                picsBandRecoStr.Add(recostring, idk.ToString());
            }
        }

        //private void getpicsrecotable()
        //{
        //    //dbobj.DatabaseName = "testplandb";
        //    dbobj.tablename = "pics_recommended_table";
        //    List<string> reconame = new List<string>() { "TCR_BandR_Rel", "ID" };
        //    picsBandRecoStr = dbquery(dbobj.tablename, reconame, "");
        //}
        //
        //
        //
        //private void button_picsreco_Click(object sender, EventArgs e)
        //{
        //    string TCR = "TC: NR, Band: R: Rel-14";
        //    getpicsrecotable();
        //    picsreco(TCR);
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //
        //
        //}
        public string insertpicsver(string picsSupportedBandList)
        {
            string id = "-1";
            dbobj.tablename = "user_picsver";
            if(!dbobj.getconnectionstat())
                dbobj.connectToDatabase();
            List<string> recocol = new List<string>() { "picsver", "picssupportedbandlist","id#gcfver","gcfver" };
            List<string> recoval = new List<string>() { picsver, picsSupportedBandList, versiontable[gcfver],gcfver };
            dbobj.insertdata(recoval, recocol);
            
            id = dbobj.getid_str("id", String.Format("(picsver = '{0}' and `id#gcfver` = '{1}')", picsver, versiontable[gcfver]));
          
            return id;
        }
      

        

        private void button1_Click(object sender, EventArgs e)
        {
            int res;
            button1.Enabled = false;
            picsver = textBoxPICS.Text.Trim();
            allver = checkBoxComboBox1.Text;
            string[] onever = allver.Split(',');
            foreach (string ver in onever)
            {
                gcfver = ver.Trim();
                foreach (string tcg in tc_cat_selected)
                {
                    dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                    if (!dbobj.getconnectionstat())
                    {
                        dbobj.connectToDatabase();
                    }
                    if (dbobj.getconnectionstat())
                    {
                        dbobj.changedb();
                        res=dbobj.deletepicsdata(gcfver, picsver);
                        if (res > 0)
                        {
                            lg.deb(string.Format("PICS Information Deleted for {0} specver: {1} picsver {2}", dbobj.DatabaseName, gcfver, picsver));
                            apendlog(string.Format("PICS Information Deleted for {0} specver: {1} picsver {2}", dbobj.DatabaseName, gcfver, picsver));
                        }
                        else
                        {
                            lg.war(string.Format("PICS Information Fails to Delete for {0} specver: {1} picsver {2}", dbobj.DatabaseName, gcfver, picsver));
                            apendlog(string.Format("PICS Information Fails to Delete for {0} specver: {1} picsver {2}", dbobj.DatabaseName, gcfver, picsver),3);

                        }
                    }
                }
            }
            
            button1.Enabled = true;
            //MessageBox.Show(picsver+":"+ gcfver);

        }
        public _Dictionary<string, string> band_vs_icesupport = new _Dictionary<string, string>();
        public void  getbandicesupport(string icebandsupportall)
        {
 

            dbobj.tablename = "testbandconfig";
            DataTable dt = dbobj.getdatatble();
            band_vs_icesupport.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string ba = row["band2"].ToString();
                //string icesupport = bandSupportHelper(ba, icebandsupportall) ?"S":"NS";
                _List<string> icebandalllst = new _List<string>(icebandsupportall.Split(' ').ToList());
                string icesupport = StringProcess.bandSupportHelper(ba, icebandalllst) ? "S" : "NS";
                if (!band_vs_icesupport.ContainsKey(ba))
                    band_vs_icesupport.Add(ba,icesupport);
            }
            lg.deb("band vs customer band support: \n\t\t\t" + band_vs_icesupport.ToString());
            //Debug.Print(band_vs_icesupport.ToString());
 //           return band_vs_icesupport;
        }

        private string bandModCheckM2M(string inB)
        {
            string outB = inB;
            string pattB = @"(\d+)M";
            Match mcB = Regex.Match(inB, pattB);
            string tempB = mcB.Groups[1].ToString();
            
            if (mcB.Success)
            {
                if (tempB.Length == 1)
                {
                    outB = "E0" + tempB;
                }
                else if (tempB.Length > 1)
                {
                    outB = "E" + tempB;
                }
            }
            return outB;
        }
        //delete this function

        private bool bandSupportHelper(string inputBand,string PICSBandSupportList)
        {

            inputBand = inputBand.Replace("U01", "UI").Replace("U02", "UII").Replace("U03", "UIII").Replace("U04", "UIV").Replace("U05", "UV").Replace("U06", "UVI").Replace("U07", "UVII").Replace("U08", "UVIII");
            //string PICSBandSupportList=icebandall;
            //string PICSBandSupportList= "1 2 3 4 5 7 8 12 13 17 18 19 20 25 26 28 29 30 34 38 39 40 41 66 7C 38C 40C 41C 2C 3C 7B 7C 12B 38C 39C 40C 41C 41D 66B 66C 2A-2A 3A-3A 4A-4A 7A-7A 25A-25A 41A-41A 41A-41C 41C-41A 66A-66A 1A-3A 1A-3C 1A-5A 1A-7A 1A-8A 1A-18A 1A-19A 1A-20A 1A-26A 1A-28A 2A-2A-5A 2A-2A-12A 2A-2A-13A 2A-2A-29A 2A-2A-30A 2A-4A 2A-5A 2A-12A 2C-12A 2A-12B 2A-13A 2A-17A 2A-29A 2A-30A 2C-30A 2A-66A 2A-66B 2A-66C 3A-3A-7A 3A-3A-7A-7A 3A-3A-8A 3A-5A 3C-5A 3A-7A 3A-7A-7A 3A-7B 3A-7C 3C-7A 3A-8A 3A-19A 3A-20A 3A-26A 3A-28A 4A-4A-5A 4A-4A-7A 4A-4A-12A 4A-4A-13A 4A-4A-29A 4A-4A-30A 4A-5A 4A-7A 4A-12A 4A-12B 4A-13A 4A-17A 4A-29A 4A-30A 5A-7A 5A-25A 5A-30A 5A-40A 5A-40C 7A-8A 7A-12A 7A-20A 7A-28A 7B-28A 7C-28A 12A-30A 12A-66A 12A-66A-66A 12A-66B 13A-66A 20A-38A 25A-26A 29A-30A 39A-41A 39C-41A 39A-41C 1A-3A-5A 1A-3A-7A 1A-3A-8A 1A-3A-19A 1A-3A-20A 1A-3A-26A 1A-3A-28A 1A-5A-7A 1A-7A-8A 1A-7A-20A 1A-7A-28A 2A-2A-5A-30A 2A-2A-12A-30A 2A-2A-29A-30A 2C-29A-30A 2A-4A-5A 2A-4A-12A 2A-4A-13A 2A-4A-29A 2A-4A-30A 2A-5A-30A 2C-5A-30A 2A-12A-30A 2C-12A-30A 2A-12A-66A 2A-29A-30A 3A-7A-8A 3A-7A-20A 3A-7A-28A 3A-7C-28A 4A-4A-5A-30A 4A-4A-12A-30A 4A-4A-29A-30A 4A-5A-30A 4A-7A-12A 4A-12A-30A 4A-29A-30A 1A-3A-7A-8A 1A-3A-7A-28A 2A-4A-5A-30A 2A-4A-12A-30A 2A-4A-29A-30A I II IV V VI VIII A E F 850 900 1800 1900";
            string outputBand = inputBand.Trim();
            string outputBandHOA1 = "";
            string outputBandHOA2 = "";
            string outputBandHOB1 = "";
            string outputBandHOB2 = "";
            string outputBandHOB3 = "";
            string outputBandHOB4 = "";
            string outputBandHO1 = "";
            string outputBandHO2 = "";
            string outputBandHO1_3 = "";
            string outputBandHO2_3 = "";
            string outputBandHO3_3 = "";

            bool finaloutputBand = false;

            string pattSC = @"^\d+$";
            Match mcSC = Regex.Match(inputBand, pattSC);
            if (mcSC.Success)
            {
                if (BandProcess.containstheword(PICSBandSupportList,outputBand))
                {
                    finaloutputBand = true;
                }
            }
            else
            {
                // Basic CA band matching:
                string pattBasicCA = @"CA_(.+)";
                var mcBasicCA = Regex.Match(inputBand, pattBasicCA);

                string pattULCA = @"CA_(.+)\+(.+)";
                var mcULCA = Regex.Match(inputBand, pattULCA);

                string pattHOA = @"[SD]B_(.+)\-(.+)";
                var mcHOA = Regex.Match(inputBand, pattHOA);

                string pattHOB = @"(.+)\-(.+)\-(.+)\-(.+)";
                var mcHOB = Regex.Match(inputBand, pattHOB);

                string pattHOC = @"FDD(.+)(\D+)";
                var mcHOC = Regex.Match(inputBand, pattHOC);

                string pattHO = @"([EUCG])?(\w+)?\-([EUCG])?(\w+)?";
                var mcHO = Regex.Match(inputBand, pattHO);

                string pattHO_3 = @"([EUCG])?(\w+)?\-([EUCG])?(\w+)?\-([EUCG])?(\w+)?";
                var mcHO_3 = Regex.Match(inputBand, pattHO_3);

                //
                if (mcBasicCA.Success)
                {
                    outputBand = mcBasicCA.Groups[1].ToString();
                }
                else if (mcHO_3.Success)
                {
                    string outputBandHO1_3Temp = mcHO_3.Groups[2].ToString();
                    outputBandHO1_3 = outputBandHO1_3Temp.TrimStart('0');
                    string outputBandHO2_3Temp = mcHO_3.Groups[4].ToString();
                    outputBandHO2_3 = outputBandHO2_3Temp.TrimStart('0');
                    string outputBandHO3_3Temp = mcHO_3.Groups[6].ToString();
                    outputBandHO3_3 = outputBandHO3_3Temp.TrimStart('0');
                }
                else if (mcHO.Success)
                {
                    string outputBandHO1Temp = mcHO.Groups[2].ToString();
                    outputBandHO1 = outputBandHO1Temp.TrimStart('0');
                    string outputBandHO2Temp = mcHO.Groups[4].ToString();
                    outputBandHO2 = outputBandHO2Temp.TrimStart('0');
                }
                //
                else if (mcULCA.Success)
                {
                    outputBand = "ULCA_" + mcULCA.Groups[1].ToString();
                }

                else if (mcHOA.Success)
                {
                    outputBandHOA1 = mcHOA.Groups[1].ToString();
                    outputBandHOA2 = mcHOA.Groups[2].ToString();
                }
                else if (mcHOB.Success)
                {
                    outputBandHOB1 = mcHOB.Groups[1].ToString();
                    outputBandHOB2 = mcHOB.Groups[2].ToString();
                    outputBandHOB3 = mcHOB.Groups[3].ToString();
                    outputBandHOB4 = mcHOB.Groups[4].ToString();
                }
                else if (mcHOC.Success)
                {
                    outputBand = mcHOC.Groups[1].ToString();
                }



                // Band Checking and Producing Boolean Decision:
                //lgstr.cri("List of Bands from Function:");
                //lgstr.cri(PICSBandSupportList.ToString());
                // BandProcess.containstheword(PICSBandSupportList, outputBand)
                //if (PICSBandSupportList.Contains(outputBand))
                if (BandProcess.containstheword(PICSBandSupportList, outputBand))
                    {
                    finaloutputBand = true;
                }
                //else if (PICSBandSupportList.Contains(outputBandHOA1) && PICSBandSupportList.Contains(outputBandHOA2))
                else if (BandProcess.containstheword(PICSBandSupportList, outputBandHOA1) && BandProcess.containstheword(PICSBandSupportList, outputBandHOA2))
                {
                    finaloutputBand = true;
                }
                //else if (PICSBandSupportList.Contains(outputBandHOB1) && PICSBandSupportList.Contains(outputBandHOB2) && PICSBandSupportList.Contains(outputBandHOB3) && PICSBandSupportList.Contains(outputBandHOB4))
                else if (BandProcess.containstheword(PICSBandSupportList, outputBandHOB1) && BandProcess.containstheword(PICSBandSupportList, outputBandHOB2) && BandProcess.containstheword(PICSBandSupportList, outputBandHOB3) && BandProcess.containstheword(PICSBandSupportList, outputBandHOB4))
                {
                    finaloutputBand = true;
                }
                //else if (PICSBandSupportList.Contains(outputBandHO1) && PICSBandSupportList.Contains(outputBandHO2))
                else if (BandProcess.containstheword(PICSBandSupportList, outputBandHO1) && BandProcess.containstheword(PICSBandSupportList, outputBandHO2))
                {
                    finaloutputBand = true;
                }
                //else if (PICSBandSupportList.Contains(outputBandHO1_3) && PICSBandSupportList.Contains(outputBandHO2_3) && PICSBandSupportList.Contains(outputBandHO3_3))
                else if (BandProcess.containstheword(PICSBandSupportList, outputBandHO1_3) && BandProcess.containstheword(PICSBandSupportList, outputBandHO2_3) && BandProcess.containstheword(PICSBandSupportList, outputBandHO3_3))
                {
                    finaloutputBand = true;
                }
                else
                {
                    lgstr.err("{bandSupportHelper-formupload} Band Not Match: " + inputBand);
                }
            }

            //lgstr.cri("Input band: " + inputBand + ", Output band: " + outputBand + ", Result: " + finaloutputBand);
            return finaloutputBand;
        }
        public _Dictionary<string, string> colidxmapingdic;
        //public int logginglvl;
        //private List<CheckBox> allchkbox = new List<CheckBox>();
        private CheckBox[] allchkbox;
        public string rb_order_ho_str;
        public string rb_order_other_str;
        private void FormUpload_Load(object sender, EventArgs e)
        {

            XmlDocument xmlDoc = new XmlDocument();
            string curDir = System.Environment.CurrentDirectory;
            string fullPath = Path.Combine(curDir, "Config_PICS.xml");
            xmlDoc.Load(fullPath);
            XmlNodeList requiredbandtag = xmlDoc.GetElementsByTagName("requiredband");
            XmlElement allrequiredband = (XmlElement)requiredbandtag[0];
            XmlNodeList rb_order_ho = allrequiredband.GetElementsByTagName("handover");
            XmlNodeList rb_order_other = allrequiredband.GetElementsByTagName("others");
            rb_order_ho_str = rb_order_ho[0].InnerText.Trim();
            rb_order_other_str = rb_order_other[0].InnerText.Trim();
            XmlNodeList logleveltag = xmlDoc.GetElementsByTagName("colindexpicsfile");
            XmlElement colidx = (XmlElement)logleveltag[0];
            XmlNodeList uereleaseinf = xmlDoc.GetElementsByTagName("UERelease");
            string uerelease = uereleaseinf[0].InnerText.Trim();
            PICSParseProcessing.uerelease = uerelease;
            XmlNodeList cvltvaloctag = xmlDoc.GetElementsByTagName("cvltvafileslocation");
            string cvltvalocation = cvltvaloctag[0].InnerText.Trim();
            PICSParseProcessing.servDir = cvltvalocation;
            lg.inf("CVL TVA location: " + cvltvalocation);
            //Debug.Print("location:"+cvltvalocation);
            XmlNodeList smallElementDash1 = colidx.GetElementsByTagName("cols");
            XmlNodeList smallElementDash2 = colidx.GetElementsByTagName("idx");
            colidxmapingdic = new _Dictionary<string, string>();
            for (int i = 0; i < smallElementDash1.Count; i++)
            {
                string K = smallElementDash1[i].InnerText.Trim();
                string V = smallElementDash2[i].InnerText.Trim();
                colidxmapingdic[K] = V;
            }
            allchkbox = new CheckBox[testcategory.Count()];
            int n = 0;
            foreach (string testcat in testcategory)
            {
                CheckBox chkbx = new CheckBox();
                chkbx.AutoSize = true;
                //180 - 245- 
                if (n < 3)
                {
                    chkbx.Location = new System.Drawing.Point(140 + 62 * n, 75);
                }
                else
                {
                    chkbx.Location = new System.Drawing.Point(140 + 62 * (n-3),100);

                }
                chkbx.Name =  "chkBox"+testcat;
                chkbx.Size = new System.Drawing.Size(50, 20);
                //chkbx.TabIndex = 31;
                chkbx.Text = testcat;
                chkbx.UseVisualStyleBackColor = true;
                allchkbox[n] = chkbx;
                chkbx.CheckedChanged += new System.EventHandler(this.chkchangeevent);
                n++;
                this.Controls.Add(chkbx);

            }

        }
        public List<string> commonverlistselected = new List<string>();
        private void chkchangeevent(object sender, EventArgs e)
        {
            List<string> commonverlist = new List<string>();
            List<string> curList = new List<string>();
            CheckBox cur_chkbx = (CheckBox)sender;
            Debug.Print("Current chkbox"+ cur_chkbx.Name);
            tc_cat_selected.Clear();
            commonverlist.Clear();
            bool first = true;
            foreach (string testcat in testcategory)
            {
                CheckBox chkbx = (CheckBox)this.Controls["chkBox"+testcat];
                if (chkbx.Checked)
                {
                    Debug.Print("check cat: "+testcat);
                    tc_cat_selected.Add(testcat);
                    if (!dbobj.getconnectionstat())
                    {
                        dbobj.DatabaseName = testcat.ToLower() + "testplandb";
                        dbobj.connectToDatabase();

                    }
                    else
                    {
                        dbobj.DatabaseName = testcat.ToLower() + "testplandb";
                        dbobj.changedb();
                    }

                    if (dbobj.getconnectionstat())
                    {
                        versiontable = getfulltable("gcfptcrbver", "ver_gcf_ptcrb_op");

                        curList = versiontable.Keys.ToList();
                        //if (curList.Count == 0)
                        //{
                        //    Debug.Print("curlist empty");
                        //    commonList.Clear();
                        //}
                        //else
                        if(curList.Count!=0)
                        {
                            if (first)
                            {
                                commonverlist = curList;
                                first = false;
                            }else
                            {
                                List<string> commonlistclone = new List<string>(commonverlist);
                                commonverlist.Clear();
                                Debug.Print("Commonlistclone: " + string.Join(",", commonlistclone));
                                foreach (string specver in curList)
                                {
                                    Debug.Print("specver" + specver);
                                    if (commonlistclone.Contains(specver))
                                    {
                                        commonverlist.Add(specver);
                                    }
                                    //comboBoxGcfver.Items.Add(gcfv);
                                    //checkBoxComboBox1.Items.Add(gcfv);
                                }
                            }

                            
                        }
                    }
                }
                
                
                
                
                

            }
            //comboBoxGcfver.Items.Clear();
            checkBoxComboBox1.Items.Clear();
            //comboBoxGcfver.Text = "";

            foreach (string specver in commonverlist)
            {
                //comboBoxGcfver.Items.Add(specver);
                checkBoxComboBox1.Items.Add(specver);
            }
            //lg.deb("Common Versions: " + String.Join(",",commonList));
        }

        private void checkBoxComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = checkBoxComboBox1.SelectedIndex;
            Debug.Print(x.ToString());
        }
    }
}
