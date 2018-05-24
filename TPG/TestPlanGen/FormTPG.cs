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
using System.Xml;
using Utility;
using Parse_Pixit_Table;
namespace TestPlanGen
{
    public partial class FormTPG : Form
    {
        private MySqlDb dbobj;
        private List<string> cols = new List<string> { "GCF/PTCRB/Operator Version", "PICS Version", "Spec", "SheetName", "Test Case Number", "Description", "Band", "Band Applicability", "Band Criteria", "Cert TP [V]", "Cert TP [E]", "Cert TP [D]", "TC Status", "PICS Status", "Env_Cond", "Band Support", "ICE Band Support", "Required Bands","wi_rft","PICSLogic","Band_old" };
        DataTable dtShowData;
        String FileLocationTemp = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        String FileLocation;
        String FileName;
        public Logging lg;
        private DataTable dtcommon = null; 
        public FormTPG()
        {
            InitializeComponent();
            dbobj = new MySqlDb();
            XmlDocument xmlDoc = new XmlDocument();
            string curDir = System.Environment.CurrentDirectory;
            string fullPath = Path.Combine(curDir, "Config_PICS.xml");
            xmlDoc.Load(fullPath);
            XmlNodeList logleveltag = xmlDoc.GetElementsByTagName("loglevel");
            XmlElement loglevelelement = (XmlElement)logleveltag[0];
            int lglvl = int.Parse(loglevelelement.InnerText);
            lg = new Logging("log.txt", lglvl);
            dbobj.lgsql = lg;
        }

        //show filterd data in showdata box based on version and pics
        private void button1_Click(object sender, EventArgs e)
        {
            
            string ver = "";
            string pic = "";
           
           
            
            if(comboBoxVer .SelectedIndex<0 || comboBoxPICSVer.SelectedIndex <0)
            {
                MessageBox.Show("Select version and PICS");
            }else
            {
                ver = comboBoxVer.SelectedItem.ToString();
                pic = comboBoxPICSVer.SelectedItem.ToString();
                string testcat = tc_cat_selected[tc_cat_selected.Count - 1];
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
                    dbobj.tablename = "v";

                    string sh = comboBoxSheet.SelectedItem.ToString();// +"$" ;
                    List<string> cond = new List<string>();
                    if (ver != "")
                    {
                        cond.Add(String.Format("`GCF/PTCRB/Operator Version` = \"{0}\"", ver));
                    }
                    if (pic != "")

                    {
                        cond.Add(String.Format("`PICS Version` = \"{0}\"", pic));
                    }
                    if (sh != "")
                    {
                        cond.Add(String.Format("SheetName = \"{0}\"", sh));
                    }
                    string condstr = String.Join(" and ", cond);
                    List<string> cols = this.cols;
                    //dtShowData.Clear();
                    //Debug.Print("condstr: " + condstr);
                    dtShowData = dbobj.getdatatble(cols, condstr);
                    dataGridView1.DataSource = dtShowData;
                    dtcommon = dtShowData;
                
                    label_showdata.Text = "Showing data for "+ testcat + "\t Category "+ sh.Replace("$", "") + "\t Version:"+ ver + "\t PICS: "+pic;
                    //Debug.Print(string.Format("VER:{0} PIC: {1} Sheet {2}", ver, pic, sh));
                    //Debug.Print(condstr);

                }
                else
                {
                    MessageBox.Show("Please Select a Test Group from TOP");
                }
            }
            


        }

        //dynamically update combobox for version, pics and subcategory, plus v1/v2 for delta
        private void updatecomboboxpicver(ComboBox cb, string ver)
        {
            List<string> commonpicsver = new List<string>();
            cb.Text = "";
            cb.Items.Clear();
            foreach(string tcg in tc_cat_selected)
            {
                dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                dbobj.changedb();
                dbobj.tablename = "user_picsver";
                List<string> piclist = dbobj.getuniqueitem("picsver", String.Format("gcfver = '{0}'", ver));
                List<string> commonpicsverclone = new List<string>(commonpicsver);
                if (commonpicsverclone.Count == 0)
                {
                    commonpicsver = piclist;
                }else
                {
                    commonpicsver.Clear();
                    foreach (string cp in commonpicsverclone)
                    {
                        if (piclist.Contains(cp))
                        {
                            commonpicsver.Add(cp);
                        }
                    }
                }
            }
    
            foreach (string pic in commonpicsver)
            {
                cb.Items.Add(pic);
            }
            if (commonpicsver.Count != 0)
            {
                cb.SelectedItem = commonpicsver[0].ToString();
            }
        }
        //check radio button change
     

        private void gcfptcrbcombinedtabclicked(object sender, EventArgs e)
        {
            if (dbobj.getconnectionstat())
            {
               // getversionlist();
            }
            else
            {
                MessageBox.Show("Check a radio Button to connect to database.");
            }

        }
        private void getversionlist(string testcat="RF")
        {
            string tabname = tabControl1.SelectedTab.Name;
            //string tab = tabControl1.SelectedTab.ToString();
            DataTable existingdt = (DataTable)dataGridViewgcfptcrb.DataSource;
            if (tabname == "tabPage3")
            {
                
                dbobj.tablename = "user_picsver";
                DataTable dt = dbobj.getdatatble();
                DataTable combineddt = dt.Copy();
                if (existingdt != null)
                {
                    combineddt.Merge(existingdt);
                }
                dataGridViewgcfptcrb.DataSource = combineddt;
                dtcommon = combineddt;
                //dgvcommon = dataGridViewgcfptcrb;
            }
        }

        //Find band information from 'common' database in pics table 
        private void updatebandsupport()
        {
            //dbobj.DatabaseName = "common";
            dbobj.tablename = "user_picsver";
            //dbobj.changedb();
            string picsver = comboBoxPICSVer.SelectedItem.ToString();
            List<string> bandsupport = dbobj.getuniqueitem("picssupportedbandlist", String.Format("picsver=\"{0}\"", picsver));
            List<string> icebandsupport = dbobj.getuniqueitem("icebands", String.Format("picsver=\"{0}\"", picsver));
            if (bandsupport.Count > 0)
            {
                textBox2.Text = bandsupport[0];
                textBox_icesb.Text = icebandsupport[0];
            }
            else
            {
                textBox2.Text = "";
                textBox_icesb.Text = "";
            }
            //dbobj.DatabaseName = dbname ;
            //dbobj.changedb();
        }

        //get selected/specific TC information given in text box
        private void button2_Click(object sender, EventArgs e)
        {
            string ver = "";
            string pic = "";
            try
            {
                ver = comboBoxVer.SelectedItem.ToString();
                pic = comboBoxPICSVer.SelectedItem.ToString();
            }
            catch (Exception ex)
            {

            }

            if (ver == "" || pic == "")
            {
                MessageBox.Show("Select version and PICS");
            }
            else
            {
                string testcat = tc_cat_selected[tc_cat_selected.Count - 1];
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

                    var iChoiceTCTemp = Convert.ToString(textBox1.Text);
                    var iChoiceTC = iChoiceTCTemp.Trim();
                    dbobj.tablename = "v";
                    
                    string tcno = iChoiceTC;
                    List<string> cond = new List<string>();
                    if (ver != "")
                    {
                        cond.Add(String.Format("`GCF/PTCRB/Operator Version` = \"{0}\"", ver));
                    }
                    if (pic != "")
                    {
                        cond.Add(String.Format("`PICS Version` = \"{0}\"", pic));
                    }
                    if (tcno != "")
                    {
                        cond.Add(String.Format("`Test Case Number` = \"{0}\"", tcno));
                    }
                    string condstr = String.Join(" and ", cond);
                    List<string> cols = this.cols;
                    //dtShowData.Clear();
                    dtShowData = dbobj.getdatatble(cols, condstr);
                    dataGridView1.DataSource = dtShowData;
                    dtcommon = dtShowData;
                    //dgvcommon = dataGridView1;
                    label_showdata.Text = "Data Searched from:  " + testcat + " Database only..";

                }
                else
                {
                    MessageBox.Show("Please Select a Test Group from TOP");
                }
            }

        }


        private void button3_Click(object sender, EventArgs e)
        {
            string txttoshow = "";
            var iChoiceBandTemp = Convert.ToString(textBox3.Text);
            var iChoiceBand = iChoiceBandTemp.Trim().ToUpper();

            //string[] SC_7560 = new string[] { "3A-8A", "7A-12A", "39A-41C", "41C", "28", "V", "4A-13A", "4A-7A", "4A-12A", "1A-3A-26A", "40", "1A-7A-20A", "2C-29A", "III", "2A-4A-4A", "25", "26", "27", "20", "39A-41A", "4A-4A-7A", "29", "3A-7A-20A", "3A-20A", "2", "4", "1A-7A", "2A-4A-12A", "41A-41A", "8", "1A-3A-19A", "2A-4A", "XI", "40C", "7A-28A", "3", "3A-5A", "1A-20A", "1A-3A-8A", "2A-2A-13A", "38", "12A-30A", "2C", "7A-7A", "2A-2A", "29A-30A", "13", "12", "17", "19", "18", "4A-5A-30A", "3A-19A", "5A-7A", "2A-30A", "VIII", "4A-29A-30A", "4A-17A", "1A-8A", "4A-29A", "2A-17A", "4A-4A-5A", "IX", "1A-3A-20A", "2A-12A-30A", "1A-19A", "5A-30A", "4A-5A", "2A-5A-30A", "3C", "5", "1A-3A-5A", "38C", "7C", "2A-2A-5A", "1A-5A", "4A-4A-12A", "2A-29A", "7A-20A", "41", "1", "4A-30A", "1A-28A", "7A-8A", "7", "2A-12A", "3A-28A", "1A-19A-21A", "3C-7A", "4A-12A-30A", "I", "VI", "4A-4A", "3A-7A", "1A-3A", "1A-18A", "3A-3A", "2A-4A-13A", "2A-5A", "II", "1A-5A-7A", "3A-26A", "1A-26A", "IV", "39C", "2A-29A-30A", "39", "12B", "4A-4A-13A", "32", "30", "25A-25A", "2A-13A" };
            string[] picsband = textBox2.Text.Split(',', ' ');
            string[] iceband = textBox_icesb.Text.Split(' ', ' ');
            Debug.Print(String.Format("VALUE:" + picsband[0]));

            int pos = Array.IndexOf(picsband, iChoiceBand);
            int pos2 = Array.IndexOf(iceband, iChoiceBand);
            if (iChoiceBand != "")
            {
                if (pos > -1)
                {
                    txttoshow = "PICS = Yes ";
                }
                else
                {
                    txttoshow = "PICS = No ";
                }
                if (pos2 > -1)
                {
                    txttoshow += "ICEDoc = Yes.";
                }
                else
                {
                    txttoshow += "ICEDoc = No.";
                }
            }
            else
            {
                txttoshow = "Enter the band information!";
            }
            textBox4.Text= txttoshow;
        }




        private void Form1_Resize(object sender, EventArgs e)
        {
            /*
            int h = Form1.ActiveForm.Height;
            int w = Form1.ActiveForm.Width;

            tabControl1.Height = h - 260;
            tabControl1.Width = w - 40;
            dataGridView1.Height = h - 411;
            dataGridView1.Width = w - 100;
            */

            //int hTab = tabControl1.Height;
            //int wTab = tabControl1.Width;
            /*
            int xForm = Form1.ActiveForm.Location.X+10;
            int yForm = Form1.ActiveForm.Location.Y;
            int gridw = (w - 100) / 3; 

            dataGridViewConfig.Height = h - 39;
            dataGridViewV1.Height = h - 39;
            dataGridViewV2.Height = h - 39;
            
            //dataGridViewConfig.Width = w - 1200;
            dataGridViewConfig.Location = new System.Drawing.Point(xForm, yForm + 100);

            dataGridViewV1.Location = new System.Drawing.Point(xForm + gridw, yForm + 100);


            dataGridViewV2.Location = new System.Drawing.Point(xForm + gridw*2, yForm + 100);


            /*
            dataGridViewConfig.Height = h - 924;
            dataGridViewConfig.Width = w - 450;

            dataGridViewV1.Height = h - 924;
            dataGridViewV1.Width = w - 413;

            dataGridViewV2.Height = h - 924;
            dataGridViewV2.Width = w - 413;
            */
            //tabControl1.Refresh();


            //Debug.Print(String.Format("H: {0}, W: {1}", h.ToString(), w.ToString()));

        }

        private void autoSC(object sender, EventArgs e)
        {
            int h = FormTPG.ActiveForm.Height;
            dataGridViewConfig.Height = h;
        }

        private void comboBoxPICSVer_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatebandsupport();
            labelcommon.Text = comboBoxPICSVer.SelectedItem.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dbobj.DatabaseName = tc_cat_selected[tc_cat_selected.Count - 1].ToLower()+"testplandb";
            dbobj.changedb();
            //PleaseWaitForm pleaseWait = new PleaseWaitForm();
            //pleaseWait.Show();
            //Application.DoEvents();
            if (dbobj.getconnectionstat())
            {
                string v1 = comboBoxV1.SelectedItem.ToString();
                string v2 = comboBoxV2.SelectedItem.ToString();
                string p1 = comboBoxPICS1.SelectedItem.ToString();
                string p2 = comboBoxPICS2.SelectedItem.ToString();
                dbobj.tablename = "user_picsver";
                string verid1 = dbobj.getuniqueitem("id", String.Format("gcfver = '{0}' and picsver = '{1}'", v1, p1))[0];
                string verid2 = dbobj.getuniqueitem("id", String.Format("gcfver = '{0}' and picsver = '{1}'", v2, p2))[0];
                string sqlcmd1 = String.Format(@"call getdeltaofver('{0}','{1}')", verid1, verid2);
                //dtShowData.Clear();
                Debug.Print("sqlcmd1: " + sqlcmd1);
                dtShowData = dbobj.getDtFromSqlSt(sqlcmd1);
                dataGridViewConfig.DataSource = dtShowData;
                dtcommon = dtShowData;
                //dgvcommon = dataGridViewConfig;
                int rowind = 0;
                //List<string> row_1 = new List<string>() { "","",""};
                //List<string> row_2 = new List<string>() {"","","" };
                DataRow dr_1;
                //DataRow dr_2;
                foreach(DataRow dr in dtShowData.Rows)
                {
                //    if (dr_1==dr)




                  //  dr_2 = dr_1;
                    dr_1 = dr;
                }
                string tc = "", band = "",spec="";
                int tcidx = 4, bandidx = 5, specidx = 2,picsveridx = 1,gcfveridx = 0;
                //int datagridrowid = 0; 
                foreach (DataGridViewRow row in dataGridViewConfig.Rows)
                {
                   // Debug.Print(String.Format("info: {0} {1} {2} {3} {4} {5} ", tc ,row.Cells[tcidx].Value.ToString() ,band,row.Cells[bandidx].Value.ToString(),spec,row.Cells[specidx].Value.ToString()));
                    if ((tc == row.Cells[tcidx].Value.ToString()) && (band == row.Cells[bandidx].Value.ToString()) && (spec == row.Cells[specidx].Value.ToString()))
                    {
                     //   Debug.Print("match");
                        for (int i = 0; i < 6; i++)
                        {
                            dataGridViewConfig.Rows[rowind].Cells[i].Style.BackColor = Color.Aqua;
                            dataGridViewConfig.Rows[rowind - 1].Cells[i].Style.BackColor = Color.Aquamarine;
                            

                        }

                        for (int i = 6; i < 15; i++)
                        {
                            if (dataGridViewConfig.Rows[rowind].Cells[i].Value.ToString() != dataGridViewConfig.Rows[rowind - 1].Cells[i].Value.ToString())
                            {
                                //lg.deb("Current Cell: " + dataGridViewConfig.Rows[rowind].Cells[i].Value + ". Previous Cell: "+ dataGridViewConfig.Rows[rowind-1].Cells[i].Value + ". Row ind "+rowind);
                                dataGridViewConfig.Rows[rowind].Cells[i].Style.ForeColor = Color.Red;
                                dataGridViewConfig.Rows[rowind - 1].Cells[i].Style.ForeColor = Color.Red;
                                dataGridViewConfig.Rows[rowind].Cells[i].Style.BackColor = Color.Aqua;
                                dataGridViewConfig.Rows[rowind - 1].Cells[i].Style.BackColor = Color.Aquamarine;
                            }


                        }


                    }
                    else if ( (comboBoxV1.SelectedItem.ToString() == row.Cells[gcfveridx].Value.ToString())&&((comboBoxPICS1.SelectedItem.ToString() == row.Cells[picsveridx].Value.ToString())))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            dataGridViewConfig.Rows[rowind].Cells[i].Style.BackColor = Color.Coral;

                        }
                    }
                    else if ((comboBoxV2.SelectedItem.ToString() == row.Cells[gcfveridx].Value.ToString()) && ((comboBoxPICS2.SelectedItem.ToString() == row.Cells[picsveridx].Value.ToString())))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            dataGridViewConfig.Rows[rowind].Cells[i].Style.BackColor = Color.LightCoral;
                        }
                    }


                    tc   = row.Cells[tcidx].Value.ToString();
                    band = row.Cells[bandidx].Value.ToString();
                    spec = row.Cells[specidx].Value.ToString();
                    //for (int i = 4; i < dataGridViewConfig.ColumnCount; i += 2)
                    //{
                    //
                    //    if ((row.Cells[i].Value.ToString()) != (row.Cells[i + 1].Value.ToString()))
                    //    {
                    //        dataGridViewConfig.Rows[rowind].Cells[i].Style.BackColor = Color.Aquamarine;
                    //        dataGridViewConfig.Rows[rowind].Cells[i + 1].Style.BackColor = Color.Aquamarine;
                    //    }
                    //}
                    rowind += 1;
                }
                
            }
            else
            {
                MessageBox.Show("Please Select a Test Group from TOP");
            }
        }


        //Start_CheckBox:
        private CheckBox[] allchkbox;
        private string[] testcategory = { "RF", "RRM", "CTPS", "ATT", "VZW", "CMCC" };
        private void FormTPG_Load(object sender, EventArgs e)
        {
            allchkbox = new CheckBox[testcategory.Count()];
            int n = 0;
            foreach (string testcat in testcategory)
            {
                CheckBox chkbx = new CheckBox();
                chkbx.AutoSize = true;
                //180 - 245- 
                chkbx.Location = new System.Drawing.Point(30 + 65 * n, 15);
                chkbx.Name = "chkBox" + testcat;
                chkbx.Size = new System.Drawing.Size(60, 20);
                //chkbx.TabIndex = 31;
                chkbx.Text = testcat;
                chkbx.UseVisualStyleBackColor = true;
                allchkbox[n] = chkbx;
                chkbx.CheckedChanged += new System.EventHandler(this.chkchangeevent);
                n++;
                //this.Controls.Add(chkbx);
                groupBox1.Controls.Add(chkbx);

            }

        }
        private List<string> tc_cat_selected = new List<string>();
        private void chkchangeevent(object sender, EventArgs e)
        {
            
            comboBoxVer.Text = "";
            comboBoxPICSVer.Text = "";
            comboBoxV1.Text = "";
            comboBoxV2.Text = "";
            comboBoxgcfver.Text = "";
            comboBoxPICS.Text = "";
            comboBoxptcrbver.Text = "";
            comboBoxVer.Items.Clear();
            comboBoxPICSVer.Items.Clear();
            comboBoxV1.Items.Clear();
            comboBoxV2.Items.Clear();
            comboBoxgcfver.Items.Clear();
            comboBoxPICS.Items.Clear();
            comboBoxPICS1.Items.Clear();
            comboBoxPICS2.Items.Clear();
            comboBoxptcrbver.Items.Clear();
            dataGridViewgcfptcrb.DataSource = null;
            List<string> comListGP = new List<string>();
            List<string> comListPICS = new List<string>();
            List<string> comListSheets = new List<string>();
            CheckBox cur_chkbx = (CheckBox)sender;
            Debug.Print("Current chkbox" + cur_chkbx.Name);
            List<string> gcfver = new List<string>();
            tc_cat_selected.Clear();
            foreach (string testcat in testcategory)
            {
                CheckBox chkbx = (CheckBox)groupBox1.Controls["chkBox" + testcat];
                if (chkbx.Checked)
                {
                    
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
                        getversionlist(testcat);
                        dbobj.tablename = "gcfptcrbver";
                        List<string> verlist = dbobj.getuniqueitem("ver_gcf_ptcrb_op", "");
                        lg.deb("Test Group: " + testcat + " version list: " + string.Join(",",verlist));
                        List<string> commonListgcfclone = new List<string>(comListGP);
                        if (commonListgcfclone.Count == 0)
                        {
                            comListGP = verlist;
                        }else
                        {
                            comListGP.Clear();
                            foreach (string cg in commonListgcfclone)
                            {
                                if(verlist.Contains(cg)){
                                    comListGP.Add(cg);
                                }
                            }
                        }
                        dbobj.tablename = "user_picsver";
                        List<string> piclist = dbobj.getuniqueitem("picsver", "");
                        List<string> comListPICSclone = new List<string>(comListPICS);
                        if (comListPICSclone.Count == 0)
                        {
                            comListPICS = piclist;
                        }
                        else
                        {
                            comListPICS.Clear();
                            foreach (string cp in comListPICSclone)
                            {
                                if (piclist.Contains(cp))
                                {
                                    comListPICS.Add(cp);
                                }
                            }
                        }
                        dbobj.tablename = "testcasetable";
                        List<string> sheetlist = dbobj.getuniqueitem("sheetname", "");
                        comboBoxSheet.Items.Clear();
                        foreach (string sh in sheetlist)
                        {
                            comboBoxSheet.Items.Add(sh.Replace("$", ""));
                        }
                        if (sheetlist.Count != 0)
                        {
                            comboBoxSheet.SelectedItem = sheetlist[0].ToString().Replace("$", "");
                        }
                        /*
                        dbobj.tablename = "user_picsver";
                        List<string> piclist = dbobj.getuniqueitem("picsver", "");
                        foreach (string plist in piclist)
                        {
                            comboBoxPICS.Items.Add(plist);
                        }
                        
                        if (verlist.Count != 0)
                        {
                            comboBoxVer.SelectedItem = verlist[0].ToString();
                            updatecomboboxpicver(comboBoxPICSVer, verlist[0].ToString());
                        }
                        
                        
                        */
                        // versiontable = getfulltable("gcfptcrbver", "ver_gcf_ptcrb_op");
                        //
                        //
                        // if (commonList.Count == 0)
                        // {
                        //     commonList = versiontable.Keys.ToList();
                        // }
                        // else
                        // {
                        //     List<string> commonlistclone = new List<string>(commonList);
                        //     commonList.Clear();
                        //     foreach (string gcfv in versiontable.Keys)
                        //     {
                        //         if (commonlistclone.Contains(gcfv))
                        //         {
                        //             commonList.Add(gcfv);
                        //         }
                        //         comboBoxGcfver.Items.Add(gcfv);
                        //     }
                        // }
                    }
                    //else
                    //{
                    //    MessageBox.Show("no db connection");
                    //}
                }
            }//end of for loop for test cat.
            foreach(string com_ver in comListGP)
            {
                comboBoxVer.Items.Add(com_ver);
                comboBoxV1.Items.Add(com_ver);
                comboBoxV2.Items.Add(com_ver);
                //if (com_ver.ToLower().StartsWith("g"))
                //{
                //    comboBoxgcfver.Items.Add(com_ver);
                //}
                //else if (com_ver.ToLower().StartsWith("p"))
                //{
                //    comboBoxptcrbver.Items.Add(com_ver);
                //}
            }
            foreach(string com_pics in comListPICS)
            {
                comboBoxPICS.Items.Add(com_pics);
            }
            

            //comboBoxGcfver.Items.Clear();
            //comboBoxGcfver.Text = "";
            //foreach (string gcfv in commonList)
            //{
            //    comboBoxGcfver.Items.Add(gcfv);
            //}
            //lg.deb("Common Versions: " + String.Join(",", commonList));
        }

        //End_CheckBok

        // Create Excel Function: Start:



        private void buttonDownload_Click(object sender, EventArgs e)
        {
            //List < string > l= new List<string>() { "a1","a2","a3"};
            //List < string > l1= new List<string>() { "1","2","3"};
            //List < string > l2= new List<string>() { "3","2","3"};
            //List < string > l3= new List<string>() { "1","4","3"};
            //List<List<string>> alllist = new List<List<string>>() { l1,l2,l3};
            //ExcelProcess exl = new ExcelProcess(@"C:\Dropbox\Scripts\C#\TPG\Beta_V1.0_20170422\TestPlanGen\bin\Debug\test.xlsx");
            //exl.writeToExcelList(alllist,l,"h");
            //exl.writeToExcelList(alllist,l,"i");
            //exl.writeToExcelList(alllist,l, "j");
            //exl.savefile();
            
            
            if(dataGridView1.RowCount != 0)
            //try
            {
                FileName = String.Format("{0}_{1}_{2}.xlsx",comboBoxSheet.SelectedItem.ToString(),comboBoxVer.SelectedItem.ToString(),comboBoxPICSVer.SelectedItem.ToString());
                if (dtShowData != null)
                {
                    FileLocation = FileLocationTemp + '\\';
                    try
                    {
                        ExcelProcess exp = new ExcelProcess(FileLocation+FileName);
                        exp.converttoexcelmultisheet(dtShowData,3);
                        exp.savefile();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    MessageBox.Show(FileName + "\nSuccessfully created to \n"+FileLocation+FileName);

                }
                else
                {
                    MessageBox.Show("Not successful!");
                }
            }
            //catch(Exception ex)
            else
            {
                MessageBox.Show("Check if the grid is empty");
            }
            

        }



        private void buttondldelta_Click(object sender, EventArgs e)
        {
            if (dataGridViewConfig.RowCount != 0)

            {
                FileName = String.Format("Delta_{0}_{1}.xlsx", comboBoxV1.SelectedItem.ToString(), comboBoxV2.SelectedItem.ToString());
                if (dtShowData != null)
                {
                    FileLocation = FileLocationTemp + '\\';
                    try
                    {
                        ExcelProcess exp = new ExcelProcess(FileLocation + FileName);
                       // exp.CreateExcelFileFromDataTable(dtShowData);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    MessageBox.Show(FileName + "\nSuccessfully created to desktop");

                }
                else
                {

                    MessageBox.Show("FILE not created!");
                }
            }
            //catch(Exception ex)
            else
            {
                MessageBox.Show("Check if the grid is empty");
            }


        }

        private void comboBoxVer_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            updatecomboboxpicver(comboBoxPICSVer, comboBoxVer.SelectedItem.ToString());
            labelcommon.Text = comboBoxVer.SelectedItem.ToString();
        }

        private void comboBoxV1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatecomboboxpicver(comboBoxPICS1, comboBoxV1.SelectedItem.ToString());
            labelcommon.Text = comboBoxV1.SelectedItem.ToString();
        }


        private void comboBoxV2_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatecomboboxpicver(comboBoxPICS2, comboBoxV2.SelectedItem.ToString());
            labelcommon.Text = comboBoxV2.SelectedItem.ToString();
        }

        private void pICSUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUpload frm = new FormUpload(lg);
            frm.Show();
        }

        private void button_alldld_Click(object sender, EventArgs e)
        {
            string ver = "";
            string pic = "";
            

            if (comboBoxVer.SelectedIndex<0|| comboBoxPICSVer.SelectedIndex < 0)
            {
                MessageBox.Show("Select version and PICS");
            }
            else
            {
                ver = comboBoxVer.SelectedItem.ToString();
                pic = comboBoxPICSVer.SelectedItem.ToString();
                DialogResult result = folderBrowserDialogall.ShowDialog();
                if (result == DialogResult.OK)
                {
                    foreach (string tcg in tc_cat_selected)
                    {
                        string fn = String.Format("combined_{0}_{1}_{2}.xlsx", ver, pic, tcg);
                        fn = Path.Combine(folderBrowserDialogall.SelectedPath, fn);
                        lg.inf("Test Group: " + tcg + " File output: " + fn );
                        if (!dbobj.getconnectionstat())
                        {
                            dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                            dbobj.connectToDatabase();

                        }
                        else
                        {
                            dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                            dbobj.changedb();
                        }


                        if (dbobj.getconnectionstat())
                        {
                            lg.inf("Connected to db - "+dbobj.DatabaseName);
                            //FileLocation = FileLocationTemp + '\\';
                            var iChoiceTCTemp = Convert.ToString(textBox1.Text);
                            var iChoiceTC = iChoiceTCTemp.Trim();
                            dbobj.tablename = "v";
                            string tcno = iChoiceTC;
                            List<string> cond = new List<string>();
                            if ((ver != "") && (pic != ""))
                            {

                                cond.Add(String.Format("`GCF/PTCRB/Operator Version` = \"{0}\"", ver));
                                cond.Add(String.Format("`PICS Version` = \"{0}\"", pic));
                                //FileName = String.Format("combined_{0}_{1}.xlsx", ver, pic);
                            }

                            string condstr = String.Join(" and ", cond);
                            List<string> cols = this.cols;
                            //dtShowData.Clear();
                            DataTable dt = dbobj.getdatatble(cols, condstr);
                            //Debug.Print("full Path: " + fn + "_" + tcg + ".xlsx");
                            ExcelProcess exp = new ExcelProcess(fn);
                            exp.converttoexcelmultisheet(dt, 3);
                            exp.savefile();

                        }
                        else
                        {
                            MessageBox.Show("Please Select Test Group from TOP");
                            lg.war("no db connection made");
                        }
                    }
                }
                    
            }
            
            
            
            /*
            if (dbobj.getconnectionstat())
            {
                FileLocation = FileLocationTemp + '\\';
                var iChoiceTCTemp = Convert.ToString(textBox1.Text);
                var iChoiceTC = iChoiceTCTemp.Trim();
                dbobj.tablename = "v";
                string ver = comboBoxVer.SelectedItem.ToString();
                string pic = comboBoxPICSVer.SelectedItem.ToString();
                string tcno = iChoiceTC;
                List<string> cond = new List<string>();
                if ((ver != "") && (pic != ""))
                {

                    cond.Add(String.Format("`GCF/PTCRB/Operator Version` = \"{0}\"", ver));
                    cond.Add(String.Format("`PICS Version` = \"{0}\"", pic));
                    FileName = String.Format("combined_{0}_{1}.xlsx", ver,pic);
                }
                
                string condstr = String.Join(" and ", cond);
                List<string> cols = this.cols;
                //dtShowData.Clear();
                DataTable dt = dbobj.getdatatble(cols, condstr);
                ExcelProcess exp = new ExcelProcess(FileLocation +FileName);
                exp.converttoexcelmultisheet(dt, 3);
                exp.savefile();

            }
            else
            {
                MessageBox.Show("Please Select Test Group from TOP");
            }
            */
        }

        private void dataGridViewConfig_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxPICS_SelectedIndexChanged(object sender, EventArgs e)
        {


            List<string> commongplist = new List<string>();
            string picscond = String.Format("picsver = '{0}'", comboBoxPICS.SelectedItem.ToString());
            foreach (string tcg in tc_cat_selected)
            {
                dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                dbobj.tablename = "user_picsver";
                List<string> gpverlist = dbobj.getuniqueitem("gcfver", picscond);
                List<string> commongplistclone = new List<string>(commongplist);
                if (commongplistclone.Count == 0)
                {
                    commongplist = gpverlist;
                }
                else
                {
                    commongplist.Clear();
                    foreach (string cg in commongplistclone)
                    {
                        if (gpverlist.Contains(cg))
                        {
                            commongplist.Add(cg);
                        }
                    }
                }
            }
            comboBoxgcfver.Items.Clear();
            comboBoxptcrbver.Items.Clear();
            foreach (string ver in commongplist)
            {
                if (ver.ToUpper().StartsWith("G"))
                {
                    comboBoxgcfver.Items.Add(ver);
                }
                else if (ver.ToUpper().StartsWith("P"))
                {
                    comboBoxptcrbver.Items.Add(ver);
                }
            }
            labelcommon.Text = comboBoxPICS.SelectedItem.ToString();


        }

        private void button5_Click(object sender, EventArgs e)
        {
            string verg = "";
            string verp = "";
            string pic = "";
            
            if((comboBoxPICS.SelectedIndex>-1)&& (comboBoxgcfver.SelectedIndex > -1)&& (comboBoxptcrbver.SelectedIndex > -1))
            {
                pic = comboBoxPICS.SelectedItem.ToString();
                verg = comboBoxgcfver.SelectedItem.ToString();
                verp = comboBoxptcrbver.SelectedItem.ToString();
                DialogResult result = folderBrowserDialogall.ShowDialog();
                if (result == DialogResult.OK)
                {

                    foreach (string tcg in tc_cat_selected)
                    {
                        string fn = String.Format("combined_{0}_{1}_{2}_{3}.xlsx", verg, verp, pic, tcg);
                        fn = Path.Combine(folderBrowserDialogall.SelectedPath, fn);
                        if (!dbobj.getconnectionstat())
                        {
                            dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                            dbobj.connectToDatabase();

                        }
                        else
                        {
                            dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                            dbobj.changedb();
                        }
                        try
                        {

                            if (dbobj.getconnectionstat())
                            {

                                //int gcfverid = dbobj.getid("id", String.Format("picsver = '{0}' and gcfver = '{1}'", comboBoxPICS.SelectedItem.ToString(), comboBoxgcfver.SelectedItem.ToString()));
                                //int ptcrbverid = dbobj.getid("id", String.Format("picsver = '{0}' and gcfver = '{1}'", comboBoxPICS.SelectedItem.ToString(), comboBoxptcrbver.SelectedItem.ToString()));
                                //MessageBox.Show("id = " + gcfverid.ToString() + " " + ptcrbverid.ToString());
                                //string sqlcmd1 = String.Format(@"call getcombinedtestplan('{0}','{1}')", gcfverid, ptcrbverid);
                                dbobj.tablename = "v";
                                List<string> cols2 = new List<string>(cols);
                                //cols2.Add("id#picsmapping");


                                string condg = String.Format("`PICS Version` = '{0}' and `GCF/PTCRB/Operator Version` = '{1}'", comboBoxPICS.SelectedItem.ToString(), comboBoxgcfver.SelectedItem.ToString());
                                string condp = String.Format("`PICS Version` = '{0}' and `GCF/PTCRB/Operator Version` = '{1}'", comboBoxPICS.SelectedItem.ToString(), comboBoxptcrbver.SelectedItem.ToString());



                                List<object> gcfobj = (List<object>)dbobj.gettblaslist(cols2, condg);
                                Dictionary<string, List<string>> gcftcvsid = (Dictionary<string, List<string>>)gcfobj[0];
                                Dictionary<string, List<string>> gcfidvsrow = (Dictionary<string, List<string>>)gcfobj[1];
                                List<object> ptcrbobj = (List<object>)dbobj.gettblaslist(cols2, condp);
                                Dictionary<string, List<string>> ptcrbtcvsid = (Dictionary<string, List<string>>)ptcrbobj[0];
                                Dictionary<string, List<string>> ptcrbidvsrow = (Dictionary<string, List<string>>)ptcrbobj[1];

                                DataTable dtcomb = new DataTable();
                                List<string> combcollist = new List<string>() { "Spec", "SheetName", "Test Case Number", "Description", "Band", "EnvCondition", "TC Status[G]", "TC Status[P]", "TP V[G]", "TP V[P]", "TP E[G]", "TP E[P]", "TP D[G]", "TP D[P]", "PICS Status [G]", "PICS Status [P]", "Band Support", "ICE Band Support" ,"WI","RFT","Band Applicability[G]", "Band Applicability[P]", "Band Criteria[G]", "Band Criteria[P]","PICS Logic[G]", "PICS Logic[P]","Band raw[G]","Band raw[P]" };
                                foreach (string combcol in combcollist)
                                {
                                    dtcomb.Columns.Add(combcol, typeof(System.String));
                                }
                                foreach (string gtc in gcftcvsid.Keys)
                                {
                                    
                                    List<string> gids = gcftcvsid[gtc];
                                    List<string> gidsclone = new List<string>(gids);
                                    

                                    if (ptcrbtcvsid.ContainsKey(gtc))
                                    {
                                        List<string> pids = ptcrbtcvsid[gtc];
                                       
                                        List<string> pidsclone = new List<string>(pids);
                                        foreach (string gid in gidsclone)
                                        {
                                            List<string> gcfrow = gcfidvsrow[gid];

                                            foreach (string pid in pidsclone)
                                            {
                                                List<string> ptcrbrow = ptcrbidvsrow[pid];
                                                if ((gcfrow[2] == ptcrbrow[2]) && (gcfrow[6] == ptcrbrow[6]))
                                                {
                                                    DataRow dr = dtcomb.NewRow();
                                                    dr["Spec"] = gcfrow[2];
                                                    dr["SheetName"] = gcfrow[3];
                                                    dr["Test Case Number"] = gcfrow[4];
                                                    dr["Description"] = gcfrow[5];
                                                    dr["Band"] = gcfrow[6];
                                                    dr["EnvCondition"] = gcfrow[14];
                                                    dr["TC Status[G]"] = gcfrow[12];
                                                    dr["TC Status[P]"] = ptcrbrow[12];
                                                    dr["TP V[G]"] = gcfrow[9];
                                                    dr["TP V[P]"] = ptcrbrow[9];
                                                    dr["TP E[G]"] = gcfrow[10];
                                                    dr["TP E[P]"] = ptcrbrow[10];
                                                    dr["TP D[G]"] = gcfrow[11];
                                                    dr["TP D[P]"] = ptcrbrow[11];
                                                    dr["PICS Status [G]"] = gcfrow[13];
                                                    dr["PICS Status [P]"] = ptcrbrow[13];
                                                    dr["Band Support"] = gcfrow[15];
                                                    dr["ICE Band Support"] = gcfrow[16];
                                                    dr["WI"] = gcfrow[18];
                                                    dr["RFT"] = ptcrbrow[18];
                                                    dr["Band Applicability[G]"] = gcfrow[7];
                                                    dr["Band Applicability[P]"] = ptcrbrow[7];
                                                    dr["Band Criteria[G]"] = gcfrow[8];
                                                    dr["Band Criteria[P]"] = ptcrbrow[8];
                                                    dr["PICS Logic[G]"] = gcfrow[19];
                                                    dr["PICS Logic[P]"] = ptcrbrow[19];
                                                    dr["Band raw[G]"] = gcfrow[20];
                                                    dr["Band raw[P]"] = ptcrbrow[20];

                                                    
                                                    dtcomb.Rows.Add(dr);
                                                    pids.Remove(pid);
                                                    gids.Remove(gid);
                                                    break;

                                                }
                                            }


                                        }
                                        foreach (string gid in gids)
                                        {
                                            
                                            //0{ "GCF/PTCRB/Operator Version", "PICS Version", "Spec", "SheetName", "Test Case Number", "Description",
                                            //6  "Band", "Band Applicability", "Band Criteria", "Cert TP [V]", "Cert TP [E]", 
                                            //11 "Cert TP [D]", "TC Status", "PICS Status", "Env_Cond", "Band Support", 
                                            //16 "ICE Band Support", "Required Bands" }
                                            List<string> gcfrow = gcfidvsrow[gid];
                                            DataRow dr = dtcomb.NewRow();
                                            dr["Spec"] = gcfrow[2];
                                            dr["SheetName"] = gcfrow[3];
                                            dr["Test Case Number"] = gcfrow[4];
                                            dr["Description"] = gcfrow[5];
                                            dr["Band"] = gcfrow[6];
                                            dr["EnvCondition"] = gcfrow[14];
                                            dr["TP V[G]"] = gcfrow[9];
                                            dr["TP E[G]"] = gcfrow[10];
                                            dr["TP D[G]"] = gcfrow[11];
                                            dr["TC Status[G]"] = gcfrow[12];
                                            dr["PICS Status [G]"] = gcfrow[13];
                                            dr["Band Support"] = gcfrow[15];
                                            dr["ICE Band Support"] = gcfrow[16];
                                            dr["WI"] = gcfrow[18];
                                            dr["Band Applicability[G]"] = gcfrow[7];
                                            
                                            dr["Band Criteria[G]"] = gcfrow[8];
                                            dr["PICS Logic[G]"] = gcfrow[19];
                                            dr["Band raw[G]"] = gcfrow[20];

                                            dtcomb.Rows.Add(dr);

                                        }



                                    }
                                    else
                                    {
                                        foreach (string gid in gids)
                                        {
                                           
                                            //0{ "GCF/PTCRB/Operator Version", "PICS Version", "Spec", "SheetName", "Test Case Number", "Description",
                                            //6  "Band", "Band Applicability", "Band Criteria", "Cert TP [V]", "Cert TP [E]", 
                                            //11 "Cert TP [D]", "TC Status", "PICS Status", "Env_Cond", "Band Support", 
                                            //16 "ICE Band Support", "Required Bands" }
                                            List<string> gcfrow = gcfidvsrow[gid];
                                            DataRow dr = dtcomb.NewRow();
                                            dr["Spec"] = gcfrow[2];
                                            dr["SheetName"] = gcfrow[3];
                                            dr["Test Case Number"] = gcfrow[4];
                                            dr["Description"] = gcfrow[5];
                                            dr["Band"] = gcfrow[6];
                                            dr["EnvCondition"] = gcfrow[14];
                                            dr["TP V[G]"] = gcfrow[9];
                                            dr["TP E[G]"] = gcfrow[10];
                                            dr["TP D[G]"] = gcfrow[11];
                                            dr["TC Status[G]"] = gcfrow[12];
                                            dr["PICS Status [G]"] = gcfrow[13];
                                            dr["Band Support"] = gcfrow[15];
                                            dr["ICE Band Support"] = gcfrow[16];
                                            dr["WI"] = gcfrow[18];
                                            dr["Band Applicability[G]"] = gcfrow[7];
                                            
                                            dr["Band Criteria[G]"] = gcfrow[8];
                                            dr["PICS Logic[G]"] = gcfrow[19];
                                            dr["Band raw[G]"] = gcfrow[20];

                                            dtcomb.Rows.Add(dr);

                                        }

                                    }
                                }
                                foreach (KeyValuePair<string, List<string>> kvpp in ptcrbtcvsid)
                                {
                                    List<string> pids_remaining = kvpp.Value;
                                    string ptc = kvpp.Key;
                                    
                                    foreach (string pid in pids_remaining)
                                    {
                                        
    
                                        List <string> ptcrbrow = ptcrbidvsrow[pid];
                                        DataRow dr = dtcomb.NewRow();
                                        dr["Spec"] = ptcrbrow[2];
                                        dr["SheetName"] = ptcrbrow[3];
                                        dr["Test Case Number"] = ptcrbrow[4];
                                        dr["Description"] = ptcrbrow[5];
                                        dr["Band"] = ptcrbrow[6];
                                        dr["EnvCondition"] = ptcrbrow[14];
                                        dr["TP V[P]"] = ptcrbrow[9];
                                        dr["TP E[P]"] = ptcrbrow[10];
                                        dr["TP D[P]"] = ptcrbrow[11];
                                        dr["TC Status[P]"] = ptcrbrow[12];
                                        dr["PICS Status [P]"] = ptcrbrow[13];
                                        dr["Band Support"] = ptcrbrow[15];
                                        dr["ICE Band Support"] = ptcrbrow[16];
                                        dr["RFT"] = ptcrbrow[18];
                                        
                                        dr["Band Applicability[P]"] = ptcrbrow[7];
                                        
                                        dr["Band Criteria[P]"] = ptcrbrow[8];
                                        dr["PICS Logic[P]"] = ptcrbrow[19];
                                        dr["Band raw[P]"]   = ptcrbrow[20];


                                    }

                                }

                                dataGridViewgcfptcrb.DataSource = dtcomb;
                                //FileLocation = FileLocationTemp + '\\';
                                //FileName = String.Format("combined_{0}_{1}_{2}.xlsx", comboBoxPICS.SelectedItem.ToString(), comboBoxgcfver.SelectedItem.ToString(), comboBoxptcrbver.SelectedItem.ToString());
                                ExcelProcess exp = new ExcelProcess(fn);
                                exp.converttoexcelmultisheet(dtcomb, 1);
                                exp.savefile();

                            }
                            else
                            {
                                MessageBox.Show("!! No DB connection !!");
                            }

                        }
                        catch (Exception x)
                        {
                            MessageBox.Show("Exception: " + x.Message + "\n Please check the version selected properly");
                        }
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Select version and PICS");
            }
          
            
           
        }

        private void comboBoxgcfver_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelcommon.Text = comboBoxgcfver.SelectedItem.ToString();
        }

        private void comboBoxptcrbver_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelcommon.Text = comboBoxptcrbver.SelectedItem.ToString();
        }

        private void comboBoxPICS2_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelcommon.Text = comboBoxPICS2.SelectedItem.ToString();
        }

        private void comboBoxPICS1_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelcommon.Text = comboBoxPICS1.SelectedItem.ToString();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string verg = "";
            string verp = "";
            string pic = "";


            if ((comboBoxPICS.SelectedIndex > -1) &&( (comboBoxgcfver.SelectedIndex > -1) || (comboBoxptcrbver.SelectedIndex > -1)))
            {
                DataTable allcombined = new DataTable();
                bool cpflag = true;
                foreach (string tcg in tc_cat_selected)
                {
                    string fn = String.Format("combined_{0}_{1}_{2}_{3}.xlsx", verg, verp, pic, tcg);
                    fn = Path.Combine(folderBrowserDialogall.SelectedPath, fn);
                    if (!dbobj.getconnectionstat())
                    {
                        dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                        dbobj.connectToDatabase();

                    }
                    else
                    {
                        dbobj.DatabaseName = tcg.ToLower() + "testplandb";
                        dbobj.changedb();
                    }
                    try
                    {

                        if (dbobj.getconnectionstat())
                        {
                            dbobj.tablename = "v";
                            pic = comboBoxPICS.SelectedItem.ToString();
                            if (comboBoxgcfver.SelectedIndex > -1)
                                verg = comboBoxgcfver.SelectedItem.ToString();
                            if (comboBoxptcrbver.SelectedIndex > -1)
                                verp = comboBoxptcrbver.SelectedItem.ToString();
                            string cond = "";
                            string cond1 = "";
                            string cond2 = "";
                            string cond3 = "";
                            if (pic != "")
                            {
                                cond1 = String.Format("`PICS Version` = \"{0}\"", pic);
                            }

                            
                            if (verp != "")
                            {
                                if (verg != "")
                                {
                                    cond2 = String.Format("`GCF/PTCRB/Operator Version` = \"{0}\" or `GCF/PTCRB/Operator Version` = \"{1}\"", verg,verp);
                                }
                                else 
                                {
                                    cond2 = String.Format("`GCF/PTCRB/Operator Version` = \"{0}\"", verp);
                                }
                            }
                            else//verp==""
                            {
                                if (verg != "")
                                {
                                    cond2 = String.Format("`GCF/PTCRB/Operator Version` = \"{0}\"", verg);
                                }
                            }
                            string txtsrch = textBoxSearch.Text;
                            string[] txtarraysrch = txtsrch.Split(',');
                            if (txtsrch.StartsWith("re"))
                            {
                                string regexsrch = txtsrch.Replace("re", "");
                                //Debug.Print(regexsrch);
                                cond3 = String.Format("`Test Case Number` REGEXP '{0}'", regexsrch.Trim());
                            }
                            else if (txtarraysrch.Count() > 1)
                            {
                                List<string> txtarraylst = new List<string>();
                                foreach (string tsrch in txtarraysrch)
                                {
                                    txtarraylst.Add(String.Format("`Test Case Number` = '{0}'",tsrch.Trim()));
                                    //tsrch = tsrch.Trim();
                                    //dbobj

                                }
                                cond3 = string.Join("or",txtarraylst);
                                cond3 = "(" + cond3 + ")";

                            }else if(txtsrch.Trim()!= "")
                            {
                                cond3 = String.Format("`Test Case Number` = '{0}'", txtsrch.Trim());

                            }
                            cond = string.Format("({0}) and ({1}) and ({2})",cond1,cond2,cond3);
                            Debug.Print(cond);
                            Debug.Print(string.Join(",",cols));
                            dtShowData = dbobj.getdatatble(cols, cond);
                            if (cpflag)
                            {
                                allcombined = dtShowData.Copy();
                                cpflag = false;
                            }
                            else
                            {
                                allcombined.Merge(dtShowData);
                            }
                            //dataGridView1.DataSource = dtShowData;
                        }
                    }catch
                    {
                        
                    }
                    



                }
                dataGridViewgcfptcrb.DataSource = allcombined;
                dtcommon = allcombined;
            }
            else
            {
                MessageBox.Show("Check The versions. PICS must be selected. \n And GCF or PTCRB version should be selected. ");
            }
        }

        

        private void saveAsExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //DataTable dtcommon = (DataTable)dgvcommon.DataSource;
            if (dtcommon != null)
            {
                foreach(DataRow dr in dtcommon.Rows)
                {
                    string x = "";
                    foreach (DataColumn dc in dtcommon.Columns)
                    {
                        x+="-" +(dr[dc.ToString()].ToString());
                    }
                    Debug.Print(x);
                }
                saveFileDialog1.Filter = "Excel Files|*.xlsx";
                DialogResult result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string name = saveFileDialog1.FileName;
                    Debug.Print(name);
                    try
                    {
                        ExcelProcess exp = new ExcelProcess(name);
                        // doesnt need save. 
                        exp.CreateExcelFileFromDataTableOriginal(dtcommon);
                        //exp.savefile();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }


            }
            else
            {
                MessageBox.Show("grid is empty");
            }


        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtcommon=null;
            //dgvcommon = null;
            //Debug.Print("event");
            TabControl tc = (TabControl)sender;
            int ind = tc.SelectedIndex;
            if(ind == 0)
            {
                dtcommon = (DataTable)dataGridView1.DataSource;
            }else if (ind == 1)
            {
                dtcommon = (DataTable)dataGridViewConfig.DataSource;
            }else if (ind == 2)
            {
                dtcommon = (DataTable)dataGridViewgcfptcrb.DataSource;
            }
            //Debug.Print(tc.SelectedIndex.ToString());
        }
    }
}
