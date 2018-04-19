using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
//using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using static Parse_Pixit_Table.StringProcess;
using Utility;
using WritingToDB;
using System.Data;

namespace Parse_Pixit_Table
{

    public class PICSParseProcessing
    {
        public static string uerelease = "";
        private static double progress = 0;
        private static int progressint = 0;
        //       private static int oldprogressint = 0;


        private static _List<string> picsSupBandList;
        private static _List<string> getListSupportedBand(_List<string> bandlist)
        {
            _List<string> supportedBandList = new _List<string>();
            foreach (string b in bandlist)
            {
                bool sup = StringProcess.bandSupportHelper(b, picsSupBandList);
                if (sup)
                {
                    supportedBandList.Add(b);
                }
            }
            return supportedBandList;

        }
        /// <summary>
        /// This will be called in every test cases
        /// </summary>
        /// <returns> dictionary that contains band as key and value as Y/N </returns>
        //
        private static string[] bandOther;
        private static string[] bandHO;
        private static _Dictionary<string, string> processRequiredBand(_Dictionary<string, string> band_status_in, string bandapplicability)
        {
            _Dictionary<string, string> band_rb = new _Dictionary<string, string>();
            _Dictionary<string, string> band_status = new _Dictionary<string, string>();
            foreach (KeyValuePair<string, string> keyval in band_status_in)
            {
                string b = keyval.Key;
                string bpatt = @"[FT]DD\s*(\w+)";
                Match bc = Regex.Match(keyval.Key, bpatt);
                if (bc.Success)
                {
                    b = bc.Groups[1].ToString();
                }
                band_status[b] = keyval.Value;
            }
            //this might be used for required band.
            //_List<string> bandOther = new _List<string> { "CA_4A-12A", "CA_2A-12A", "CA_4A-7A",
            //          "CA_4A-29A", "CA_2A-29A", "CA_4A-5A", "CA_4A-4A", "CA_2A-4A", "CA_2A-5A",
            //          "CA_2A-30A", "CA_5A-30A", "CA_4A-30A",
            //          "CA_30A-29A", "CA_12A-30A", "CA_4A-17A", "CA_2A-17A",
            //          "CA_5A-29A", "CA_7A-7A", "CA_66A-66A", "CA_66B", "CA_66C", "CA_66A-29A","4", "17", "14", "25", "2", "5", "7", "12", "13", "30", "II", "V","IV", "1900", "850"};
            //_List<string> bandHO = new _List<string> { "E04-UII", "E04-UV", "E04-UIV", "E17-UII", "E17-UV", "E17-UIV", "E14-UII", "E14-UV", "E14-UIV", "E25-UII", "E25-UV", "E25-UIV", "E02-UII", "E02-UV", "E02-UIV", "E05-UII", "E05-UV", "E05-UIV", "E07-UII", "E07-UV", "E07-UIV", "E12-UII", "E12-UV", "E12-UIV", "E13-UII", "E13-UV", "E13-UIV", "E30-UII", "E30-UV", "E30-UIV", "UII-E04", "UII-E17", "UII-E14", "UII-E25", "UII-E02", "UII-E05", "UII-E07", "UII-E12", "UII-E13", "UII-E30", "UV-E04", "UV-E17", "UV-E14", "UV-E25", "UV-E02", "UV-E05", "UV-E07", "UV-E12", "UV-E13", "UV-E30", "UIV-E04", "UIV-E17", "UIV-E14", "UIV-E25", "UIV-E02", "UIV-E05", "UIV-E07", "UIV-E12", "UIV-E13", "UIV-E30" };


            _List<string> intersectprioritybandlist = new _List<string>();
            _List<string> statuslist = new _List<string> { "A", "B", "P" };
            _List<string> bandlist = new _List<string>(band_status.Keys.ToList<string>());
            lgstr.deb("bandlist" + bandlist.ToString());
            _List<string> finalbandlist = getListSupportedBand(bandlist);
            foreach (string bl in bandlist)
            {
                //string b = bl;
                ////_Dictionary<string, string> bb = new _Dictionary<string, string>();
                //string bpatt = @"FDD\s*(\w+)";
                //Match bc = Regex.Match(bl, bpatt);
                //if (bc.Success)
                //{
                //    b = bc.Groups[1].ToString();
                //}
                band_rb[bl] = "N";
            }

            if (bandapplicability.ToLower() == "all")
            {
                foreach (string d in finalbandlist)
                {
                    band_rb[d] = "Y";
                }
            }
            else
            {
                // filtered with testcase status. generate list for ABP
                foreach (string d in finalbandlist)
                {
                    var val = statuslist.Find(x => x == band_status[d]);
                    if (val != null)
                    {
                        intersectprioritybandlist.Add(d);                   //list of bands which are common within priority bands as per PTCRB
                    }
                }
                if (bandapplicability.ToLower() == "single")
                {
                    foreach (string g in bandOther)
                    {
                        var gval = intersectprioritybandlist.Find(y => y == g.Trim());
                        if (gval != null)
                        {
                            band_rb[gval] = "Y";
                            break;
                        }
                    }
                }
                else if (bandapplicability.ToLower() == "irat-single")
                {
                    foreach (string g in bandHO)
                    {
                        
                        var gval = intersectprioritybandlist.Find(y => y == g.Trim());
                        if (gval != null)
                        {
                            band_rb[gval] = "Y";
                            break;
                        }
                    }
                }
            }
            lgstr.inf("band_rb: " + band_rb.ToString());
            return band_rb;
        }
        public static string servDir = "";
        //public static Logging lg;
        //public static void mainprocess(string filePICSFullPath, _List<string> specTVAFiles, _List<string> specCVLFiles, Logging lg, MySqlDb dbobj, FormUpload frm)
        public static void mainprocess(string filePICSFullPath, _List<string> specTRLFiles, Logging lg, MySqlDb dbobj, FormUpload frm)
        {
            frm.SetProgress(0);
            frm.apendlog("Started PICS process",4);
            lg.deb(" ++++ PICS Process started ++++ ");
            lg.deb("PICS User File : " + filePICSFullPath);
            lg.deb("List of CSV files on server spec data: \n\t\t" + specTRLFiles.ToString());
            
            progressint = Convert.ToInt32(progress);
            //lg.inf("progress: " + progress.ToString() + " t : " + DateTime.Now.ToString("h:mm:ss tt"));
            // START
            string curDir = System.Environment.CurrentDirectory;
            string outputfilename = Path.Combine(curDir, "combinedOutput.csv");
            string file_picsmapping = Path.Combine(curDir, "_picsmapping.csv");
            string file_picsstatus = Path.Combine(curDir, "_picsstatus.csv");
            string icebandall="";
            string icebandulca="";
            string ice4x4MIMO = "";

            GenericParser gp = new GenericParser(@filePICSFullPath);
            List<string> excelsheetlist = MySqlDb.GetExcelsheetslist(@filePICSFullPath);
            Debug.Print("all sheet name: "+string.Join("   ,    ",excelsheetlist));
            gp.lg = lg;
            List<int> x = new List<int> { 1, 6 };
            //itemno vs [support, pnemonic]
            Dictionary<string, object> PICSoutput = gp.readwholepics(); 
            //
            //lg.inf("PICS file Data: itemno vs [support,pnemonic]");
            //lg.inf(PICSoutput.ToString());
            TwoKeyDictionary<string, string, string> pics_reco_dic = new TwoKeyDictionary<string, string, string>();
            _Dictionary<string, string> pics_reco_table;
            _Dictionary<string, string> PICSFormattedSupportOutput = gp.PICSFormatChangeSupport(PICSoutput);
            //lg.deb("PICS file Data. calculated to True/False with #");
            //lg.deb(PICSFormattedSupportOutput.ToString());
            _Dictionary<string, string> PICSmnemonicDic = gp.PICSFormatMnemonicsChangeSupport(PICSoutput);
            StringProcess.PICSmnemonicDic = PICSmnemonicDic;
            lg.inf("PICSmnemonicDic only for spec 12 = 102 230 :\n"+ PICSmnemonicDic.ToString());
            picsSupBandList = gp.PICSSupportedBands(PICSFormattedSupportOutput, PICSoutput);

            lg.inf("picsSupBandList :\n" + picsSupBandList.ToString());
            progress = 2;
            //lg.inf("progress: " + progress.ToString() + "t : " + DateTime.Now.ToString("h:mm:ss tt"));
            progressint = Convert.ToInt32(progress);
            frm.SetProgress(progressint);
            Dictionary<string, object> RFBandListD = gp.RFBandTableD(PICSFormattedSupportOutput);
            Dictionary<string, object> RFBandListE = gp.RFBandTableE(PICSFormattedSupportOutput, PICSoutput);
            TwoKeyDictionary<string, string, string> user_bs_rb_pics = new TwoKeyDictionary<string, string, string>();
            dbobj.tablename = "user_bs_rb_pics";
            DataTable dt = dbobj.getdatatble();
            Debug.Print("Reading Data Table");
            foreach (DataRow dr in dt.Rows)
            {
                user_bs_rb_pics[dr["bandsupport"].ToString(), dr["requiredband"].ToString()] = dr["id"].ToString();
            }
            //lg.inf("user_bs_rb_pics" + user_bs_rb_pics.ToString());

            // user_picsver |  picsver -> picssupportedbandlist -> id#gcfver
            // write user_picsver and get id. this will return picsversionid too. 
            string idpicsver = frm.insertpicsver(picsSupBandList.toCommaList());
            DataTable icebanddt = dbobj.getDtFromSqlSt(String.Format("Select icebands, icebands_ulca, 4x4_mimo from user_picsver where id = {0}", idpicsver));
            foreach (DataRow row in icebanddt.Rows)
            {
                //returnval[row[0].ToString(),row[1].ToString()] = row[2].ToString();
                //Debug.Print(row[0].ToString());
                icebandall= row[0].ToString();
                icebandulca = row[1].ToString();
                ice4x4MIMO = row[2].ToString();
                break;
            }
            lg.inf("customer band from db: (all) "+icebandall);
            lg.inf("customer band from db:(ulca) "+icebandulca);

            frm.getbandicesupport(icebandall);
            //string servDir = @"\\sd-ct-opiot.sn.intel.com\Dropbox\tools\TPG\sampleinput\Specs";
            //New Code End
            progress = 3;
            progressint = Convert.ToInt32(progress);
            frm.SetProgress(progressint);
            int specfilescount = specTRLFiles.Count();
            double progressinc = 97 / specfilescount;
            for (int i = 0; i < specfilescount; i++)
            {
                //lg.inf("progress: " + progress.ToString() + " t : " + DateTime.Now.ToString("h:mm:ss tt"));
                string status = "Processing : " + specTRLFiles[i];
                frm.SetLabelstat(status);
                //string fileNameTVA = specTVAFiles[i];
                //string fileNameCVL = specCVLFiles[i];
                string filenameTRL = specTRLFiles[i];
                string fileTRLPath = Path.Combine(servDir, filenameTRL);
                gp.clearVar();

                // Getting a portion of string:
                string patt = @"(.+?)_\w+";
                var mc = Regex.Match(filenameTRL, patt);
                string spec = mc.Groups[1].ToString();

                //string fileCVLPath = Path.Combine(servDir, fileNameCVL);
                //string fileTVAPath = Path.Combine(servDir, fileNameTVA);
                lg.deb("{MainProcess} Processing -> spec: " + spec + "\tfiles: " + filenameTRL.ToString() );
                frm.apendlog("Processing spec = "+spec, 4);
                //_Dictionary<string, string> condVsLogicDic = gp.getdictionaryRowCol(1, 0, fileCVLPath);
                // this function will change  condDict make adding hash. (not sure) 
                // itemProcess is not needed for new algo
                //itemProcess(condVsLogicDic, spec);
                //lg.inf("{MainProcess} Condition vs. logical exp from server csv for Spec: " + spec);
                //lg.inf(condVsLogicDic.ToString());
                //this will convert item to true false as per PICS
                // Here, we are changing the conDict according to spec. It is different only for Spec:102.230.
                //combinedCond(condVsLogicDic, PICSFormattedSupportOutput);

                //lg.deb(@"{MainProcess} Condition vs T/F after using PICS for Spec: " + spec);
                //lg.deb(condVsLogicDic.ToString());
                
                // filter spec string to address spec confusion for 51.010
                //string spec = spec;

                //if (spec.StartsWith("51.010"))
                //{
                //    spec = "51.010";
                //}
                //lg.inf("tempdebug: spec: " + spec +" new spec: "+ spec);
                frm.SetLabelstat(status + " db query to get test configuration");
                // Database query to get all the info for the spec. from tcconfig table 
                frm.gettable_tcno_spec_id_band(spec);
                //TwoKeyDictionary<string, string, string> tc_band_sheet = frm.tc_band_sheet;
                _Dictionary<string, string> tc_des = frm.tc_des;
                TwoKeyDictionary<string, string, string> tc_band_id = frm.tc_band_id;
                TwoKeyDictionary<string, string, string> tc_band_status = frm.tc_band_status;
                _Dictionary<string, string> tc_bandapplicability = frm.tc_bandapplicability;
                //lg.deb("sheetname from db"+tc_band_sheet.ToString());
                //lg.deb("testconfig from DB");
                //lg.deb(tc_band_id.ToString());
                //lg.deb("tc_bandapplicability from DB");
                //lg.deb(tc_bandapplicability.ToString());
                //lg.deb("tc_band_status from DB");
                //lg.deb(tc_band_status.ToString());
                _Dictionary<string, string> TCvsApp = new _Dictionary<string, string>();
                _Dictionary<string, string> TCvsAppFiltered = new _Dictionary<string, string>();
                _Dictionary<string, string> csvTCvsRel = new _Dictionary<string, string>();
                _Dictionary<string, object> TCvsBand = new _Dictionary<string, object>();
                TwoKeyDictionary<string, string, string> bsrbidxdic = new TwoKeyDictionary<string, string, string>();
                if (File.Exists(fileTRLPath))
                {

                    _Dictionary<string, string> dict_logic = new _Dictionary<string, string>();
                    _Dictionary<string, string> dict_logic_TF = new _Dictionary<string, string>();
                    _Dictionary<string, string> dict_rel = new _Dictionary<string, string>();
                    _Dictionary<string, string> dict_de = new _Dictionary<string, string>();
                    _Dictionary<string, string> dict_pnemonic = new _Dictionary<string, string>();
                    StringProcess.specDash1 = spec;
                    StringProcess.readconfigpics();
                    //StringProcess.
                    gp.dict_de = dict_de;
                    gp.dict_logic = dict_logic;
                    gp.dict_logic_TF = dict_logic_TF;
                    gp.dict_pnemonic = dict_pnemonic;
                    gp.dict_rel = dict_rel;
                    GenericParser.picssupport = PICSFormattedSupportOutput;
                    //this will generate logic to TF
                    gp.processlogic(fileTRLPath);
                    if (GenericParser.missingpicsitem.Count > 0)
                    {
                        lg.inf("{MainProcess} Missing Item in pics file:");
                        lg.war(GenericParser.missingpicsitem.ToString());
                    }
                    
                    lg.inf("{MainProcess} TC vs True False TRL + pics file:");
                    lg.inf(dict_logic_TF.ToString());
                    //_Dictionary<string, string> csvTCvsCond;
                    //_Dictionary<string, string> dict_de;
                    //frm.SetLabelstat(status + " process tc vs cond");
                    //csvTCvsCond = new _Dictionary<string, string>(gp.getdictionaryRowCol(1, 0, fileTVAPath,true));
                    //frm.SetLabelstat(status + " process tc vs rel");
                    //csvTCvsRel = new _Dictionary<string, string>(gp.getdictionaryRowCol(2, 0, fileTVAPath,true));
                    //frm.SetLabelstat(status + " process tc vs Band cond");
                    //dict_de = new _Dictionary<string, string>(gp.getdictionaryRowCol(3, 0, fileTVAPath,true));
                    //lg.inf("{MainProcess} TC vs Condion from server csv file:");
                    //lg.inf(csvTCvsCond.ToString());
                    // This loop iterates over the configurations obtained from the database
                    progress += progressinc * 0.1;
                    progressint = Convert.ToInt32(progress);
                    double progressinc2 = progressinc * 0.8;
                    int tccount = tc_band_id.getCount();
                    double progressinc3 = progressinc2 / tccount;
                    //lg.inf(String.Format("progress: {0} ,progressinc: {1}, progressinc2: {2},progressinc3: {3} tccount: {4}", progress, progressinc, progressinc2, progressinc3, tccount));
                    StringProcess.mismatchedbandbandsupporthelper.Clear();
                    //read new csv file here 

                    foreach (KeyValuePair<string, Dictionary<string, string>> temp in tc_band_id)
                    {// reading database test case - band vs id start
                        string tc = temp.Key.Replace(" ","").ToLower();
                        string oldTC = tc;
                        string picstmpstr = "";
                        string picsBand = "";
                        string picsRel = "Rel: NF";
                        string picsTCReco = "";
                        _Dictionary<string, string> band_id = new _Dictionary<string, string>(temp.Value);
                        _List<string> bandsPerTC = new _List<string>(band_id.Keys.ToList());
                        lg.inf("{MainProcess} List of bands from db for TestCase: " + tc);
                        lg.inf(bandsPerTC.ToString());

                        // Start: For Remving V1 or V2 from TC
                        string TCvsAppFilteredTempSingle = "";
                        string pattTC = @"(.*)\s\(v\d+\)";
                        Match mcTC = Regex.Match(tc, pattTC);
                        if (mcTC.Success)
                        {
                            tc = mcTC.Groups[1].ToString().Trim();
                            lg.war("TC Converted from old " + oldTC + " to " + tc);
                            frm.apendlog("TC Converted from old " + oldTC + " to " + tc);
                        }
                        // End: For Remving V1 or V2 from TC
                        // for spec 37.901 to match test case after the dash everything should be removed
                        if(spec == "37.901")
                        {
                            tc = tc.Split('-')[0];
                        }
                        //var tctemplist = dict_logic_TF.Keys.Where(t => t.Trim().Replace(" ", "").ToLower() == tc.Trim().Replace(" ", "").ToLower());
                        string tcapplicability = "";
                        foreach (string tctemp in dict_logic_TF.Keys)
                        {
                            if (tctemp.Trim().Replace(" ", "").ToLower() == tc.Trim().Replace(" ", "").ToLower())
                            {
                                tcapplicability = dict_logic_TF[tctemp];
                                if (dict_rel.ContainsKey(tctemp))
                                    picsRel = dict_rel[tctemp];
                                break;
                            }
                        }
                        // checking if csvTCvsCond has the tc(after removing v1/v2)
                        if (tcapplicability!="")
                        {
                            //string tcapplicability = dict_logic_TF[tc].Trim();
                            
                            if (tcapplicability == "R")
                            {
                                TCvsApp.Add(tc, "R");
                                picsTCReco = "TC: R";

                            }
                            else if (tcapplicability== "m")
                            {
                                TCvsApp.Add(tc, "R");
                                picsTCReco = "TC: R";

                            }
                            else if (tcapplicability != "")
                            {
                                string dbgexception = " TC = " + tc;
                                //string condCurTC = "";
                                string resCurTC = tcapplicability;
                                try
                                {
                                    // Pass it through a function: addressing special case 51.010-4...
                                    //condCurTC = csvTCvsCond[tc];
                                    //dbgexception += "\tcondCurTC = " + condCurTC;
                                    // Pass it through a function:
                                    //resCurTC = tcapplicability;//dict_logic_TF[tc];
                                    //resCurTC = StringProcess.TCVsTF(condCurTC, condVsLogicDic);
                                    dbgexception += "\tresCurTC = " + resCurTC;
                                    //lg.deb("Result from condition map:  " + resCurTC + "for TestCase: " + tc);
                                    if (!TCvsApp.ContainsKey(tc))
                                        TCvsApp.Add(tc, resCurTC);
                                    TCvsAppFilteredTempSingle = gp.TCvsAppRefined(resCurTC);
                                    if (!TCvsAppFiltered.ContainsKey(tc))
                                        TCvsAppFiltered.Add(tc, TCvsAppFilteredTempSingle);
                                    dbgexception += "\tresCurTCFiltered = " + TCvsAppFilteredTempSingle;
                                    picsTCReco = "TC: " + TCvsAppFilteredTempSingle;
                                    

                                }
                                catch (Exception ex)
                                {
                                    lg.err("{MainProcess} Exception: " + ex.Message+"\n\t\t\t"+ dbgexception);
                                    picsTCReco = "TC: X";
                                    frm.apendlog(ex.Message, 2);
                                }
                            }
                        }
                        else
                        { // tc is not in the dictionary. csvTCvsCond.ContainsKey(tc)
                            if (spec == "tty")
                            {
                                picsTCReco = "TC: R";
                            }
                            else
                            {
                                picsTCReco = "TC: -";
                                lg.war("{MainProcess} CSVOutputTCApp KeyNotFound = " + tc);
                                frm.apendlog("TC missing :  " + tc, 3);
                            }
                                                       
                            
                        }
                        _List<string> bandListDE = new _List<string>();
                        //_List<string> keyInTCvsRel = new _List<string>(csvTCvsRel.Keys.ToList());
                        //if (csvTCvsRel.ContainsKey(tc))
                        //    picsRel = csvTCvsRel[tc];

                        if (spec == "36.521-1")
                        {
                            if (dict_de.ContainsKey(tc))
                            {
                                patt = @"([DE])\d+";
                                var mc1 = Regex.Match(dict_de[tc], patt);
                                string res = mc1.Groups[1].ToString();
                                if (res == "D")
                                {
                                    List<string> bandListD = new List<string>();
                                    if (RFBandListD.ContainsKey(dict_de[tc]))
                                        bandListD = (List<string>)RFBandListD[dict_de[tc]];

                                    TCvsBand.Add(tc, bandListD);
                                    foreach (string bandVal in bandListD)
                                    {
                                        bandListDE.Add(bandVal);
                                    }
                                }
                                else if (res == "E")
                                {
                                    List<string> bandListE = new List<string>();
                                    if (RFBandListE.ContainsKey(dict_de[tc]))
                                        bandListE = (List<string>)RFBandListE[dict_de[tc]];
                                    TCvsBand.Add(tc, bandListE);
                                    foreach (string bandVal in bandListE)
                                    {
                                        bandListDE.Add(bandVal);
                                    }
                                }
                            }
                            else
                            {
                                lg.war("{MainProcess} CSVOutputTCBand KeyNotFound = " + tc);
                                frm.apendlog("TC missing :: " + tc, 3);
                            }
                            lg.inf("DE Check: " + tc + " band list: " + bandListDE.ToString());
                            //frm.apendlog("bandlist error " + tc, 1);

                        }
                        _Dictionary<string, string> band_status = new _Dictionary<string, string>(frm.tc_band_status[oldTC]);
                        lg.inf("    " + band_status.ToString());
                        bandHO = frm.rb_order_ho_str.Split(',');
                        bandOther = frm.rb_order_other_str.Split(',');
                        _Dictionary<string, string> band_rb = processRequiredBand(band_status, tc_bandapplicability[oldTC]);
                        foreach (KeyValuePair<string, string> kvp in band_id)
                        {
                            string band_id_key = kvp.Key;
                            string band_id_val = kvp.Value;
                            string band_rb_val = "E";
                            _List<string> modifiedband = StringProcess.getValIntiligent(band_id_key, band_rb);
                            band_rb_val = modifiedband[2];
                            if (modifiedband[0] != "1")
                            {
                                lg.inf("bandchange"+modifiedband.ToString());

                            }

                            string pics_reco_str = picstmpstr;
                            picsBand = "";
                            bool isSuppBand = false;
                            if((frm.testcat=="rf") || (frm.testcat =="att") || (frm.testcat == "vzw") || (frm.testcat == "cmcc"))
                            {
                                band_rb_val = "";
                            }

                            if (spec == "36.521-1")
                            {
                                //string PICSRecoBand = "";
                                bool PICSRecoBand = StringProcess.bandSupportHelper(band_id_key, bandListDE);
                                //lg.cri("After call: " + tc + " band list: " + bandListDE.ToString());
                                if (PICSRecoBand)
                                    //picsBand = "Band: R";
                                    picsBand = "";
                                else
                                    picsBand = "Band: NR";
                                if (tc.StartsWith("8") | tc.StartsWith("9"))
                                    picsBand = "";

                                //if ((tc_band_sheet[tc, band_id_key] == "LTE_UL_CA") || ((tc_band_sheet[tc, band_id_key] == "64QAM")))
                                string desc = tc_des[tc].ToLower();
                                if ( desc.Contains("64qam") || desc.Contains("dl ca and ul ca") ) 
                                {
                                    string pattULCA = @"CA_(\d+[A-E])";
                                    Match mcULCA = Regex.Match(band_id_key, pattULCA);
                                    if (mcULCA.Success)
                                    {
                                        string band_id_key_mod = "UL" + band_id_key;
                                        if (picsSupBandList.Contains(band_id_key_mod))
                                            isSuppBand = true;
                                    }
                                    else
                                    {
                                        isSuppBand = StringProcess.bandSupportHelper(band_id_key, picsSupBandList);
                                    }
                                }
                                else
                                {
                                    isSuppBand = StringProcess.bandSupportHelper(band_id_key, picsSupBandList);
                                }
                                

                            }
                            else
                            {
                                isSuppBand = StringProcess.bandSupportHelper(band_id_key, picsSupBandList);
                            }
                            picsRel = picsRel.Replace(","," ");
                            if (picsTCReco != "")
                                if (pics_reco_str == "")
                                    pics_reco_str += picsTCReco;
                                else
                                    pics_reco_str += ", " + picsTCReco;
                            if (picsRel != "")
                                if (pics_reco_str == "")
                                    pics_reco_str += picsRel;
                                else
                                    pics_reco_str += ", " + picsRel;
                            if (picsBand != "")
                                if (pics_reco_str == "")
                                    pics_reco_str += picsBand;
                                else
                                    pics_reco_str += ", " + picsBand;
                            //if (picsRel.ToLower().Contains("only"))
                              //  pics_reco_str += ",Rel:NR";
                            //r(?:el)?(?:ease)?[-\s]?\d+\s*(?:to|and)\s*r(?:el)?(?:ease)?[-\s]?(\d+)\s*only
                            //\s*r(?:el)?(?:ease)?[-\s]?(\d+)\s*only
                            // rel 2 to rel 6
                            string relpatt = @"(?:r(?:el)?(?:ease)?[-\s]?\d+\s*(?:to|and)\s*)?r(?:el)?(?:ease)?[-\s]?(\d+)\s*only";
                            Match relm = Regex.Match(picsRel, relpatt,RegexOptions.IgnoreCase);
                            if (relm.Success)
                            {
                                int rel = int.Parse(relm.Groups[1].ToString());
                                if ((rel < 90) && (rel < int.Parse(uerelease)))
                                {
                                    pics_reco_str += ", Rel:NR";
                                }

                            }


                            string PICSSuppBand = "NS";
                            if (isSuppBand)
                                PICSSuppBand = "S";
                            string idstr = "-1";
                            if (user_bs_rb_pics.ContainsKey(PICSSuppBand, band_rb_val))
                            {
                                idstr = user_bs_rb_pics[PICSSuppBand, band_rb_val];
                            }
                            else
                            {
                                lg.cri("no key found: picssupband" + PICSSuppBand + " requiredband " + band_rb_val+"\n"+user_bs_rb_pics.ToString() );
                                frm.apendlog("pics support band and requiredband process failed for band:" + band_id_key, 1);

                            }
                            if (!bsrbidxdic.ContainsKey(oldTC, band_id_key))
                                bsrbidxdic[oldTC, band_id_key] = idstr;

                            // user_picsstatus | picsstatus
                            // 

                            //string idstr = FormUpload.getid_Reco(pics_reco_table, PICSOutputStr);
                            if (!pics_reco_dic.ContainsKey(oldTC, band_id_val))
                                pics_reco_dic[oldTC, band_id_val] = pics_reco_str;
                            GenericParser.writetocsv(file_picsstatus, pics_reco_str + "\t");
                            pics_reco_str = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", band_id_key, oldTC, band_id_val, pics_reco_str, PICSSuppBand, idstr);
                            GenericParser.writetocsv(outputfilename, pics_reco_str);
                            lg.deb("FinalOutput: " + tc + " Result: " + pics_reco_str);

                        }
                        progress += progressinc3;
                        if (progressint < Convert.ToInt32(progress))
                        {
                            progressint = Convert.ToInt32(progress);
                            frm.SetProgress(progressint);
                        }
                        progressint = Convert.ToInt32(progress);

                    }// reading database test case - band vs id end

                    
                    lg.war("Missing Bands from 'picsSupBandList':\n\t\t " + string.Join("   ",StringProcess.mismatchedbandbandsupporthelper));
                    
                    // GET user_picsstatus table here. as a dictionary. 
                    dbobj.tablename = "user_picsstatus";
                    file_picsstatus = file_picsstatus.Replace(@"\", @"/");
                    dbobj.insertfile(file_picsstatus, "picsstatus");
                    pics_reco_table = frm.getfulltable("user_picsstatus", "picsstatus");
                    //lg.deb("PICS Reco String table from DB");
                    //lg.deb(pics_reco_table.ToString());
                    //lg.deb("rb_sb");
                    //lg.deb(bsrbidxdic.ToString());
                    //lg.deb("tcr: " + pics_reco_table["TC: R, REL-8"]);
                    foreach (KeyValuePair<string, Dictionary<string, string>> temp in tc_band_id)
                    {

                        string tc = temp.Key.Replace(" ", "");
                        _Dictionary<string, string> band_id = new _Dictionary<string, string>(temp.Value);
                        foreach (KeyValuePair<string, string> kvp in band_id)
                        {
                            string picsrecoid = "-1";
                            string bsrbidx = "-1";
                            string band_id_key = kvp.Key;
                            string band_id_val = kvp.Value;
                            string pics_reco_str = pics_reco_dic[tc, band_id_val];

                            //------converting REL to Rel - start---------
                            foreach (KeyValuePair<string, string> kvp1 in pics_reco_table)
                            {
                                string picsstringdb = kvp1.Key;
                                
                                if (picsstringdb.ToLower().Trim()==pics_reco_str.ToLower().Trim())
                                {
                                    picsrecoid = kvp1.Value;
                                }

                            }

                            //if (pics_reco_table.ContainsKey(pics_reco_str))
                            //{
                            //    picsrecoid = pics_reco_table[pics_reco_str];
                            //}
                            //else
                            if(picsrecoid=="-1")
                            {
                                
                                lg.cri("No Key for " + pics_reco_str);
                                lg.deb(pics_reco_table.ToString());
                            }
                            if (bsrbidxdic.ContainsKey(tc, band_id_key))
                            {
                                bsrbidx = bsrbidxdic[tc, band_id_key];
                            }
                            else
                            {
                                lg.cri(String.Format("Missing ID for BS: {0} and RB : {1}", tc, band_id_key));
                                lg.deb(bsrbidxdic.ToString());
                            }
                            string picsbs = frm.band_vs_icesupport[band_id_key];
                            if(spec == "36.521-1")
                            {
                                //if ((tc_band_sheet[tc, band_id_key] == "LTE_ UL_CA") || ((tc_band_sheet[tc, band_id_key] == "64QAM")))
                                string desc = tc_des[tc].ToLower();
                                if (desc.Contains("64qam") || desc.Contains("dl ca and ul ca"))
                                {
                                    string band_id_key_mod =  band_id_key.ToUpper().Replace("CA_","");
                                    if(BandProcess.containstheword(icebandulca,band_id_key_mod))
                                    {
                                        picsbs = "S";
                                    }else
                                    {
                                        picsbs = "NS";
                                    }
                                }

                                if (desc.Contains("x4") || desc.Contains("4 rx"))
                                {
                                    string band_id_key_mod = band_id_key.ToUpper().Replace("CA_", "");
                                    if (BandProcess.containstheword(ice4x4MIMO, band_id_key_mod))
                                    {
                                        picsbs = "S";
                                    }
                                    else
                                    {
                                        picsbs = "NS";
                                    }
                                }


                            }
                            
                            string map_str = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t", idpicsver, bsrbidx, picsrecoid, band_id_val, picsbs);
                            GenericParser.writetocsv(file_picsmapping, map_str);

                        }
                    }
                    progress += progressinc * 0.1;
                    progressint = Convert.ToInt32(progress);
                    //lg.inf("progress: " + progress.ToString() + "t : " + DateTime.Now.ToString("h:mm:ss tt"));
                    frm.SetProgress(progressint);
                    dbobj.tablename = "user_picsmappingtable";
                    file_picsmapping = file_picsmapping.Replace(@"\", @"/");
                    frm.SetLabelstat(status + " uploading pics information to db");
                    dbobj.insertfile(file_picsmapping, String.Format("`{0}`,`{1}`,`{2}`,`{3}`,`{4}`", "id#pics", "id#bsrb", "id#picsstat", "id#v_comb_serv_info","icebs"));
                    frm.SetLabelstat(status + " Uploading pics Complete");
                    

                }
                else
                {
                    lg.cri("{MainProcess} File Does not exist !  =>" + filenameTRL);
                    Debug.Print("File " + filenameTRL + " NOT found!");
                    frm.apendlog("File " + filenameTRL + " NOT found!",1);
                }

                //////lg.inf("{MainProcess} TC (from db) vs App: RAW");
                //////lg.inf("======================");
                //////lg.inf(TCvsApp.ToString());
                //////lg.deb("{MainProcess} TC (from db) vs App: FILTERED");
                //////lg.deb("======================");
                //////lg.deb(TCvsAppFiltered.ToString());
                //////lg.cri("Band Support: Verification:");
                //////lg.cri(TCBandSupport.ToString());
                // End of the Loop:
            }
            progress = 100;
            progressint = Convert.ToInt32(progress);
            frm.SetProgress(progressint);
            
            frm.SetLabelstat("Idle");
            // END


        }



    }
}
