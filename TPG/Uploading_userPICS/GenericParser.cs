// this is new code for genericparser
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using Utility;
using static Parse_Pixit_Table.StringProcess;
namespace Parse_Pixit_Table
{

    class GenericParser
    {
        public Logging lg;
        string fn;
        _List<object> allMapping = new _List<object>();
        XmlDocument xmlDoc;
        _Dictionary<string, object> PICSoutput = new _Dictionary<string, object>();


        //_List<string> BandList = new _List<string>();
        _List<string> SCBandList = new _List<string>();
        _List<string> CABandList = new _List<string>();
        _List<string> CABandListTable1 = new _List<string>();
        _List<string> CABandListTable2 = new _List<string>();
        _List<string> CABandListTable3 = new _List<string>();
        _List<string> CABandListTable4 = new _List<string>();
        _List<string> CABandListTable5 = new _List<string>();
        _List<string> ULCAList = new _List<string>();
        _List<string> GSMList = new _List<string>();
        _List<string> UMTSList = new _List<string>();
        _List<string> TDSList = new _List<string>();

        _List<string> List453 = new _List<string>();
        _List<string> List454 = new _List<string>();
        _List<string> List455 = new _List<string>();
        _List<string> List456a = new _List<string>();


        public GenericParser(string fn)
        {
            this.fn = fn;
            xmlDoc = new XmlDocument();
            string curDir = System.Environment.CurrentDirectory;
            string fullPath = Path.Combine(curDir, "Config_PICS.xml");
            xmlDoc.Load(fullPath);
        }
        public _Dictionary<string, object> readwholepics()
        {
            XmlNodeList allp = xmlDoc.GetElementsByTagName("mappingspec");
            _List<int> app_mnem_col = new _List<int>();
            foreach (XmlNode pl in allp)
            {
                string specnoRaw = pl.ChildNodes[0].InnerText.Trim();
                string specno = "";
                //specno = "3GPP TS " + specno; // This is because the sheetName has "3GPP TS " as the prefix.
                if (specnoRaw == "102 230-1")
                    specno = "ETSI TS " + specnoRaw;
                else if (specnoRaw == "tty")
                    specno = "PTCRB Bearer Agnostic TTY Test";
                else if (specnoRaw == "PTCRB AT Command Test Spec. cov")
                    specno = "PTCRB AT Command Test Spec# cov";
                else if (specnoRaw == "37.901")
                    specno = "3GPP TR "+specnoRaw;
                else
                    specno = "3GPP TS " + specnoRaw;

                string id = pl.ChildNodes[1].InnerText.Trim();
                app_mnem_col.Add(Int16.Parse(pl.ChildNodes[3].InnerText.Trim()));
                app_mnem_col.Add(Int16.Parse(pl.ChildNodes[2].InnerText.Trim()));
                lgstr.deb("spec from config: "+specno+" \n\t\t" + app_mnem_col.ToString());
                try
                {
                    // this function will run over the whole pics file and update PICSoutput
                    getValue(specno, id, app_mnem_col);
                }
                catch(Exception ex)
                {
                    
                    lgstr.err("sheet missing in pics: " + specno);
                }
                app_mnem_col.Clear();
            }
            return PICSoutput;
        }

        DataSet dataTotal = new DataSet();
        DataTable ExcelDataSheetTemp;

        // idxcol must be greater or equal 2 and first one is key always
        //idxcol = [support column , mnemonic col]
        // PICSoutput has both support and pnemonic
        public void getValue(string sheetName, string id, _List<int> idxCol)
        {
            lg.inf("current PICS Sheet: " + sheetName);
            bool insidegroup = false;
            string firstval = "";
            ExcelDataSheetTemp = GetExcelData(sheetName);
            foreach (DataRow dr in ExcelDataSheetTemp.Rows)
            {
                _List<string> rowinfo = new _List<string>();
                firstval = dr[1].ToString().Trim(); // Considering 2nd column will be used as Item. Hard coding.
                foreach (int idx in idxCol)
                {
                    string val;
                    if (idx == -1)
                    {
                        val = "";
                    }
                    else
                    {
                        val = dr[idx - 1].ToString().Trim().Replace("\n","").Replace("\r","");
                    }

                    if (firstval.ToLower() == "item")
                    {
                        insidegroup = true;
                    }
                    else if (insidegroup && firstval.Trim() == "")
                    {
                        insidegroup = false;
                    }
                    if (insidegroup)
                    {
                        rowinfo.Add(val);
                    }
                }
                if (insidegroup && firstval.ToLower() != "item" && firstval != "")
                {
                    PICSoutput.Add(id + "#" + firstval, rowinfo);
                    string[] def = rowinfo.ToArray();
                }
            }
        }

        public DataTable GetExcelData(string ExcelSheetName = "")
        {
            string Condition = "";
            DataTable tbl = new DataTable();
            string excelConnString = @"provider=Microsoft.ACE.OLEDB.12.0;data source=" + fn + ";extended properties=" + "\"excel 12.0;hdr=yes;IMEX=1\"";



            //Create Connection to Excel work book 
            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                ///----------------------------
                string sql = "Select * " + " from [" + ExcelSheetName + "$] ";
                if (Condition.Trim().Length > 0)
                {
                    sql += Condition;
                }
                OleDbDataAdapter da = new OleDbDataAdapter(sql, excelConnection);
                da.Fill(tbl);

            }


            return tbl;
        }


        public void clearVar()
        {
            //this.BandList.Clear();
            this.SCBandList.Clear();
            this.CABandList.Clear();
            this.CABandListTable1.Clear();
            this.CABandListTable2.Clear();
            this.CABandListTable3.Clear();
            this.CABandListTable4.Clear();
            this.CABandListTable5.Clear();
            this.ULCAList.Clear();
            this.GSMList.Clear();
            this.UMTSList.Clear();
            this.TDSList.Clear();
            this.List453.Clear();
            this.List454.Clear();
            this.List455.Clear();
            this.List456a.Clear();

            this.PICSoutput.Clear();
            // this.specData.Clear();

        }
        public _Dictionary<string, string> dict_logic = null;
        public _Dictionary<string, string> dict_logic_TF = null;
        public _Dictionary<string, string> dict_rel = null;
        public _Dictionary<string, string> dict_de = null;
        public _Dictionary<string, string> dict_pnemonic = null;
        public static _Dictionary<string, string> picssupport = null;
        public static _List<string> missingpicsitem = new _List<string>();
        public static bool fmnemonic;
        public void processlogic( string csvfile)
        {
            fmnemonic = false;
            getalldictfromTRLfile(csvfile);
            if (Path.GetFileName(csvfile).StartsWith("102"))
                fmnemonic = true;
            foreach (KeyValuePair<string, string> tcvslog in dict_logic)
            {
                string tc = tcvslog.Key;
                string logicstr = tcvslog.Value;
                lg.inf("#" + tc);
                if (!fmnemonic)
                {
                    logicstr = StringProcess.addIDtoLogicStr(tcvslog.Value);
                }
                dict_logic_TF[tc] = logicstrProcess(logicstr);
                

                // Evaluate combined true false from the pics. 

            }

        }

        public void getalldictfromTRLfile(string CSVName, bool remspacefrmkey = false)
        {
            int key_id = 0;
            int logic_id = 4;
            int rel_id = 2;
            int de_id = 3;
            int pnemonic_id = 5;
            lg.deb("TRL FILE: " + CSVName);

            using (var fs = File.OpenRead(CSVName))
            {
                using (var reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        _List<string> valTot = new _List<string>();
                        var line = reader.ReadLine();
                        if (line != "")
                        {
                            var values = line.Split('\t');
                            int colidx = 0;
                            foreach (string val in values)
                            {
                                string val2 = val.Replace("\"", "");
                                if ((colidx == key_id) && (remspacefrmkey))
                                    val2 = val2.Replace(" ", "");
                                valTot.Add(val2);
                                colidx++;
                            }
                            string dctkey = valTot[key_id];

                            dctkey = dctkey.ToLower();
                            if (dctkey.Trim() == "")
                                continue;
                            if (values.Count() > 5)
                            {
                                try
                                {
                                    if (dict_logic.ContainsKey(dctkey))
                                    {
                                        //lg.war("\t{gp:getdictionary} duplicate entry in  " + CSVName + " " + valTot[key_id] + " " + valTot[logic_id]);

                                        dict_logic[dctkey] = valTot[logic_id];
                                        dict_de[dctkey] = valTot[de_id];
                                        dict_rel[dctkey] = valTot[rel_id];
                                        dict_pnemonic[dctkey] = valTot[pnemonic_id];
                                    }
                                    else
                                    {
                                        dict_logic.Add(dctkey, valTot[logic_id]);
                                        dict_de.Add(dctkey, valTot[de_id]);
                                        dict_rel.Add(dctkey, valTot[rel_id]);
                                        dict_pnemonic .Add(dctkey, valTot[pnemonic_id]);
                                    }
                                }
                                catch
                                {
                                    lg.err("\t{gp:getdictionary}  " + valTot[key_id] + " \n\t\t\t" + valTot[logic_id]);
                                }
                            }
                            else
                            {
                                lg.war("{gp:getdictionary} line has few elements " + line);

                                //if (values.Count() > key_id)
                                //{
                                //    dict_logic.Add(dctkey, @"N/A");
                                //}
                            }
                        }

                    }
                }
            }
            //List<_Dictionary<string, string>> alldict = new List<_Dictionary<string, string>>();
            //alldict.Add(dict_logic);
            //alldict.Add(dict_rel);
            //alldict.Add(dict_pnemonic);
            //alldict.Add(dict_de);
        }


        /// <summary>
        /// this will take a pics item as input and generate true false statement. 
        /// 
        /// </summary>
        /// <param name="logicforpicsitem"></param>
        /// <param name="picssupport">
        /// (NOT(7#A.4.3-4a/1) AND 7#A.4.1-1/1 AND 7#A.4.3-3a/1) 
        /// </param>
        /// <returns>
        /// (NOT(False) AND True AND True) 
        /// </returns>
        public static string logicstrProcess(string logicforpicsitem)
        {
            if (logicforpicsitem== "R")
            {
                return "True";
            }
            
            string dbg = " In: " + logicforpicsitem;
            string boolstr = logicforpicsitem;
            boolstr = convertToBoolExpr(logicforpicsitem);
            dbg += "\tBoolExpr: " + boolstr;
            string boolstrverify = boolstr.Replace("AND", "").Replace("OR", "").Replace("NOT", "").Replace("True", "").Replace("False", "");
            Match m = Regex.Match(boolstrverify, @"\w+");
            if (boolstr.Trim() == "")
            {
                lgstr.war("{cond_process}: " + dbg);
            }
            else if (!(m.Success))
            {
                boolstr = evalBooleanExpr(boolstr).ToString();
                dbg += "\tEvalRes: " + boolstr;
                lgstr.deb("{cond_process} " + dbg);
            }
            else
            {
                lgstr.war("{cond_process} InvPart: " + boolstrverify + dbg);

            }
            return boolstr;
        }
       
        public static bool evalBooleanExpr(string strinput)
        {

            int count1 = strinput.Count(f => f == '(');
            int count2 = strinput.Count(f => f == ')');
            if (count1 == count2 + 1)
            {
                strinput = strinput + ')';
            }
            else if (count1 == count2 - 1)
            {
                strinput = '(' + strinput;

            }
            else if (count1 != count2)
            {
                lgstr.cri("{evalBooleanExpr}" + String.Format("Bracket mismatch more than 1 {0} ", strinput));
            }
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("", typeof(Boolean));
            table.Columns[0].Expression = strinput;
            System.Data.DataRow r = table.NewRow();
            table.Rows.Add(r);
            Boolean b = (Boolean)r[0];
            lgstr.inf("{evalBooleanExpr}" + String.Format("In: {0} Out: {1} ", strinput, b.ToString()));
            return b;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="picssupport">
        /// _Dictionary that contains 
        /// itemno/pnemonic vs true false. 
        /// </param>
        /// <param name="strinput"></param>
        /// <returns></returns>
        public static string convertToBoolExpr(string strinput)
        {
            string strop = "";
            
            
            string pattern = "";
            //string pattern = @"\b\PICSoutputd+#[AEG]\.\d[\d\-\/\.a-z]+\b";
            if (fmnemonic)
            {
                strinput = strinput.Replace("O_COMP_121.111", "O_COMP_121_111");
                pattern = @"O_[A-Z_\d]+";
            }
            else
            {
                strinput = strinput.Replace("O.1", "True");
                strinput = strinput.Replace("O.2", "True");
                strinput = strinput.Replace("O.3", "True");
                strinput = strinput.Replace("O.4", "True");
                pattern = @"\d+#\b[AEDGX]\.\d[\d\-\/\.a-z]+\b";
            }
            Regex itemRegex = new Regex(pattern, RegexOptions.IgnoreCase);
            strop = itemRegex.Replace(strinput, m => convertBoolHelper(m.ToString()));
            lgstr.inf("{convertToBoolExpr} " + strinput + " => " + strop);
            return strop;
        }
       

        /// <summary>
        /// This function will be used to get value from a dictionary. no key found treated as False. 
        /// </summary>
        /// <param name="a">
        /// this is the string will be searched in the dictionary
        /// </param>
        /// <param name="b">
        /// The dictionary
        /// </param>
        /// <returns></returns>
        public static string convertBoolHelper(string a)
        {
            string c;
            if (!fmnemonic && picssupport.ContainsKey(a))
            {
                c = picssupport[a];
            }
            else if (fmnemonic && PICSmnemonicDic.ContainsKey(a))
            {
                c = PICSmnemonicDic[a];
            }
            else
            {
                c = "False";
                lgstr.war("{convertBoolHelper} KeyNotFound " + "-> " + a);
            }

            return c;
        }

        public static string convertBoolHelpermnemonic(string a)
        {
            string c;
            if (PICSmnemonicDic.ContainsKey(a))
            {
                c = PICSmnemonicDic[a];
            }
            else
            {
                c = "False";
                lgstr.war("{convertBoolHelpermnemonic} KeyNotFound  " + "-> " + a);
            }

            return c;
        }



        // it returns a dictionary from a csv file with key and value column index passed. 
        public _Dictionary<string, string> getdictionaryRowCol(int val_id, int key_id = 0, string CSVName = "",bool remspacefrmkey=false)
        {
            lg.deb("FILE: "+CSVName);
            _Dictionary<string, string> dictcsv = new _Dictionary<string, string>();
            using (var fs = File.OpenRead(CSVName))
            {
                using (var reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        _List<string> valTot = new _List<string>();
                        var line = reader.ReadLine();
                        if (line != "")
                        {
                            var values = line.Split('\t');
                            int colidx = 0;
                            foreach (string val in values)
                            {
                                string val2 = val.Replace("\"", "");
                                if ((colidx == key_id) && (remspacefrmkey))
                                    val2=val2.Replace(" ", "");
                                valTot.Add(val2);
                                colidx++;
                            }
                            string dctkey = valTot[key_id];
                            string fn = Path.GetFileName(CSVName);
                            if (fn.Contains("TVA"))
                                dctkey = dctkey.ToLower();

                            if ((values.Count() > val_id)&& (values.Count() > key_id))
                            {
                                try
                                {
                                    if (dictcsv.ContainsKey(dctkey))
                                    {
                                        lg.war("\t{gp:getdictionary} duplicate entry in  "+ CSVName+" " + valTot[key_id] + " " + valTot[val_id]);

                                    }else
                                    {
                                        dictcsv.Add(dctkey, valTot[val_id]);
                                    }
                                }
                                catch
                                {
                                    lg.err("\t{gp:getdictionary}  " + valTot[key_id] + " \n\t\t\t" + valTot[val_id]);
                                }
                            }
                            else
                            {
                                //lg.war("{gp:getdictionary}  "+ line);
                                if(values.Count() > key_id)
                                {
                                    dictcsv.Add(dctkey, @"N/A");
                                }
                            }
                        }

                    }
                }
            }
            return dictcsv;
        }



        public _Dictionary<string, string> PICSFormatChangeSupport(Dictionary<string, object> dataPICS)
        {
            // datapics 0 element is support and 1 for pnemonic. 
            _Dictionary<string, string> PICSSupportFormatted = new _Dictionary<string, string>();
            foreach (KeyValuePair<string, object> tempPICS in dataPICS)
            {
                _List<string> abc = (_List<string>)tempPICS.Value;
                //string[] def = abc.ToArray();
                if (abc[0].ToLower().Trim() == "y")
                {
                    PICSSupportFormatted.Add(tempPICS.Key, "True");
                }
                else
                {
                    PICSSupportFormatted.Add(tempPICS.Key, "False");
                }

            }
            return PICSSupportFormatted;

        }

        public _Dictionary<string, string> PICSFormatMnemonicsChangeSupport(Dictionary<string, object> dataPICS)
        {
            _Dictionary<string, string> PICSSupportFormattedA = new _Dictionary<string, string>();
            foreach (KeyValuePair<string, object> tempPICS in dataPICS)
            {
                string patt = @"(12)\#(.+)";
                Match mc = Regex.Match(tempPICS.Key, patt);
                if (mc.Success)
                {
                    _List<string> abc = (_List<string>)tempPICS.Value;
                    if (abc[1] != "")
                    {
                        if (abc[0] == "Y")
                        {
                            PICSSupportFormattedA.Add(abc[1], "True");
                        }
                        else
                        {
                            PICSSupportFormattedA.Add(abc[1], "False");
                        }
                    }
                }
            }
            return PICSSupportFormattedA;
        }

        public string specIDForBandFun()
        {
            string specIdForBand = "";
            string specno = "";
            XmlNodeList allp = xmlDoc.GetElementsByTagName("mappingspec");
            _List<int> mappingInfo = new _List<int>();
            foreach (XmlNode pl in allp)
            {
                specno = pl.ChildNodes[0].InnerText.Trim();
                if (specno == "36.521-2")
                {
                    specIdForBand = pl.ChildNodes[1].InnerText.Trim();
                }
            }
            return specIdForBand;
        }
        public _List<string> PICSSupportedBands(Dictionary<string, string> PICSItemSupportFormatted, Dictionary<string, object> PICSFull)
        {
            // Fetching ID for spec: 36.521-2: specIdForBand
            _List<string> BandList = new _List<string>();
            string specIdForBand = specIDForBandFun();
            string patternSCBand = specIdForBand + @"\#A\.4\.3\-3\/(\d+)";
            string patternCABandTable1 = specIdForBand + @"\#\bA\.4\.6\.1\-3\/(\d+\w)\b";
            string patternCABandTable2 = specIdForBand + @"\#\bA\.4\.6\.2\-3\/(\d+\w\-\d+\w)\b";
            string patternCABandTable3 = specIdForBand + @"\#\bA\.4\.6\.3\-3\/(\d+\w\-\d+\w\-?(?:\d+)?\w?\-?(?:\d+)?\w?)\b";
            string patternCABandTable4 = specIdForBand + @"\#\bA\.4\.6\.3\-4\/(\d+\w\-\d+\w\-?(?:\d+)?\w?\-?(?:\d+)?\w?)_DL\b";
            string patternCABandTable5 = specIdForBand + @"\#\bA\.4\.6\.3\-5\/(\d+\w\-?(?:\d+)?\w?\-?(?:\d+)?\w?\-?(?:\d+)?\w?)\b";
            string patternULCASupport = @"CA_(.+)";

            // Combined:
            foreach (KeyValuePair<string, string> temp in PICSItemSupportFormatted)
            {
                //Debug.Print("Inside the PICS Support Band For Loop:::::");
                var mcSC = Regex.Match(temp.Key, patternSCBand);
                var mcCA1 = Regex.Match(temp.Key, patternCABandTable1);
                var mcCA2 = Regex.Match(temp.Key, patternCABandTable2);
                var mcCA3 = Regex.Match(temp.Key, patternCABandTable3);
                var mcCA4 = Regex.Match(temp.Key, patternCABandTable4);
                var mcCA5 = Regex.Match(temp.Key, patternCABandTable5);

                string resSC = mcSC.Groups[1].ToString();
                string resCA1 = mcCA1.Groups[1].ToString();
                string resCA2 = mcCA2.Groups[1].ToString();
                string resCA3 = mcCA3.Groups[1].ToString();
                string resCA4 = mcCA4.Groups[1].ToString();
                string resCA5 = mcCA5.Groups[1].ToString();

                if ((resSC != "") && (temp.Value == "True"))
                {
                    SCBandList.Add(resSC);
                }

                if ((resCA1 != "") && (temp.Value == "True"))
                {
                    CABandListTable1.Add(resCA1);
                }

                if ((resCA2 != "") && (temp.Value == "True"))
                {
                    CABandListTable2.Add(resCA2);
                }

                if ((resCA3 != "") && (temp.Value == "True"))
                {
                    CABandListTable3.Add(resCA3);
                }

                if ((resCA4 != "") && (temp.Value == "True"))
                {
                    CABandListTable4.Add(resCA4);
                }
                if ((resCA5 != "") && (temp.Value == "True"))
                {
                    CABandListTable5.Add(resCA5);
                }

            }
            foreach (KeyValuePair<string, object> temp in PICSFull)
            {
                _List<string> abc = (_List<string>)temp.Value;
                var mcULCA = Regex.Match(abc[0], patternULCASupport);
                string resULCA = mcULCA.Groups[1].ToString();

                string patternWI = specIdForBand + @"\#.*";
                var mcpatternWI = Regex.Match(temp.Key, patternWI);
                string respatternWI = mcpatternWI.Value.ToString();
                if (respatternWI != "")
                {
                    if (resULCA != "")
                    {
                        ULCAList.Add("ULCA_" + resULCA);
                    }
                }
            }

            XmlNodeList allPICSBands = xmlDoc.GetElementsByTagName("PICSBandSupport");
            //_List<int> mappingInfo = new _List<int>();
            foreach (XmlNode pl in allPICSBands)
            {
                string band2G = pl.ChildNodes[0].InnerText.Trim();
                string band3G = pl.ChildNodes[1].InnerText.Trim();
                string bandTDS = pl.ChildNodes[3].InnerText.Trim();

                string[] band2GPro1 = band2G.Split(',');
                string[] band3GPro1 = band3G.Split(',');
                string[] bandTDSPro1 = bandTDS.Split(',');

                foreach (string temp in band2GPro1)
                {
                    GSMList.Add(temp.Trim());
                }
                foreach (string temp in band3GPro1)
                {
                    UMTSList.Add(temp.Trim());
                }
                foreach (string temp in bandTDSPro1)
                {
                    TDSList.Add(temp.Trim());
                }

            }
            CABandList.AddRange(CABandListTable1);
            CABandList.AddRange(CABandListTable2);
            CABandList.AddRange(CABandListTable3);
            CABandList.AddRange(CABandListTable4);
            CABandList.AddRange(CABandListTable5);
            BandList.AddRange(SCBandList);
            BandList.AddRange(CABandList);
            BandList.AddRange(ULCAList);
            BandList.AddRange(GSMList);
            BandList.AddRange(UMTSList);
            BandList.AddRange(TDSList);

            return BandList;
        }


        // New Function:
        public Dictionary<string, object> RFBandTableD(Dictionary<string, string> PICSItemSupportFormatted)
        {
            Dictionary<string, object> DList = new Dictionary<string, object>();
            Dictionary<string, object> DTableCodeSelection = new Dictionary<string, object>();

            string specIdForBand = specIDForBandFun();
            XmlNodeList all3GPPLTEBands = xmlDoc.GetElementsByTagName("Bands");
            _List<string> FDDBands = new _List<string>();
            _List<string> TDDBands = new _List<string>();
            _List<string> Bands1441 = new _List<string>();
            string FDDBandsRaw1 = "";
            string TDDBandsRaw1 = "";
            string Bands1441Raw1 = "";

            foreach (XmlNode pl in all3GPPLTEBands)
            {
                FDDBandsRaw1 = pl.ChildNodes[0].InnerText.Trim();
                TDDBandsRaw1 = pl.ChildNodes[1].InnerText.Trim();
                Bands1441Raw1 = pl.ChildNodes[2].InnerText.Trim();
            }
            string[] FDDBandsRaw2 = FDDBandsRaw1.Split(',');
            string[] TDDBandsRaw2 = TDDBandsRaw1.Split(',');
            string[] Bands1441Raw2 = Bands1441Raw1.Split(',');

            foreach (string temp in FDDBandsRaw2)
            {
                FDDBands.Add(temp);
            }
            foreach (string temp in TDDBandsRaw2)
            {
                TDDBands.Add(temp);
            }
            foreach (string temp in Bands1441Raw2)
            {
                Bands1441.Add(temp);
            }
            XmlNodeList DTableCodes = xmlDoc.GetElementsByTagName("DTable");
            foreach (XmlNode pl in DTableCodes)
            {
                DTableCodeSelection.Add(pl.ChildNodes[0].InnerText.Trim(), pl.ChildNodes[1].InnerText.Trim());
            }

            var D01Selection = SCBandList.ToList();
            var D02Selection = SCBandList.Intersect(FDDBands).ToList();
            var D03Selection = SCBandList.Intersect(TDDBands).ToList();
            var D04Selection = SCBandList.Intersect(Bands1441).ToList();

            string pattern453Band = specIdForBand + @"\#A\.4\.5\-3\/(\d+)";
            string pattern454Band = specIdForBand + @"\#A\.4\.5\-4\/(\d+)";
            string pattern455Band = specIdForBand + @"\#A\.4\.5\-5\/(\d+)";
            string pattern456aBand = specIdForBand + @"\#A\.4\.5\-6a\/(\d+)";

            foreach (KeyValuePair<string, string> temp in PICSItemSupportFormatted)
            {
                //Debug.Print("Inside the PICS Support Band For Loop:::::");
                var mc453 = Regex.Match(temp.Key, pattern453Band);
                var mc454 = Regex.Match(temp.Key, pattern454Band);
                var mc455 = Regex.Match(temp.Key, pattern455Band);
                var mc456a = Regex.Match(temp.Key, pattern456aBand);

                string res453 = mc453.Groups[1].ToString();
                string res454 = mc454.Groups[1].ToString();
                string res455 = mc455.Groups[1].ToString();
                string res456a = mc456a.Groups[1].ToString();

                if ((res453 != "") && (temp.Value == "True"))
                {
                    List453.Add(res453);
                }
                if ((res454 != "") && (temp.Value == "True"))
                {
                    List454.Add(res454);
                }
                if ((res455 != "") && (temp.Value == "True"))
                {
                    List455.Add(res455);
                }
                if ((res456a != "") && (temp.Value == "True"))
                {
                    List456a.Add(res456a);
                }
            }
            var D05Selection = SCBandList.Intersect(List453).ToList();
            var D06Selection = SCBandList.Except(List453).ToList();
            var D07Selection = SCBandList.Intersect(List454).ToList();
            var D09Selection = SCBandList.Intersect(List455).ToList();
            var D10Selection = SCBandList.Intersect(List456a).ToList();

            _List<string> FBA4613 = new _List<string>();
            foreach (string temp in CABandListTable1)
            {
                FBA4613.Add(fallbackBand(temp));
            }
            var D08Selection = SCBandList.Except(FBA4613).ToList();

            DList.Add("D01", D01Selection);
            DList.Add("D02", D02Selection);
            DList.Add("D03", D03Selection);
            DList.Add("D04", D04Selection);
            DList.Add("D05", D05Selection);
            DList.Add("D06", D06Selection);
            DList.Add("D07", D07Selection);
            DList.Add("D08", D08Selection);
            DList.Add("D09", D09Selection);
            DList.Add("D10", D10Selection);

            return DList;

        }


        // RF CA Bands Table E:
        public Dictionary<string, object> RFBandTableE(Dictionary<string, string> PICSItemSupportFormatted, Dictionary<string, object> PICSFull)
        {
            string specIdForBand = specIDForBandFun();

            string pattern4613UL = specIdForBand + @"\#\bA\.4\.6\.1\-3\/(\d+\w)_UL\b";
            string pattern4613ULValue = @"CA.+";

            string pattern4623UL = specIdForBand + @"\#\bA\.4\.6\.2\-3\/(\d+\w\-\d+\w)_UL\b";
            string pattern4623ULValue = @"CA.+";

            string pattern4633UL = specIdForBand + @"\#\bA\.4\.6\.3\-3\/(\d+\w\-\d+\w\-?(?:\d+)?\w?\-?(?:\d+)?\w?)_UL\b";
            string pattern4633ULValue = @"CA.+";

            string pattern4634UL = specIdForBand + @"\#\bA\.4\.6\.3\-4\/(\d+\w\-\d+\w\-?(?:\d+)?\w?\-?(?:\d+)?\w?)_UL\b";
            string pattern4634ULValue = @"CA.+";

            string pattern4635UL = specIdForBand + @"\#\bA\.4\.6\.3\-5\/(\d+\w\-?(?:\d+)?\w?\-?(?:\d+)?\w?\-?(?:\d+)?\w?)_UL\b";
            string pattern4635ULValue = @"CA.+";

            _List<string> BandList4613UL = new _List<string>();
            _List<string> BandList4623UL = new _List<string>();
            _List<string> BandList4633UL = new _List<string>();
            _List<string> BandList4634UL = new _List<string>();
            _List<string> BandList4635UL = new _List<string>();

            Dictionary<string, object> EList = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> temp in PICSFull)
            {
                var mc4613UL = Regex.Match(temp.Key, pattern4613UL);
                var mc4623UL = Regex.Match(temp.Key, pattern4623UL);
                var mc4633UL = Regex.Match(temp.Key, pattern4633UL);
                var mc4634UL = Regex.Match(temp.Key, pattern4634UL);
                var mc4635UL = Regex.Match(temp.Key, pattern4635UL);

                string res4613UL = mc4613UL.Groups[1].ToString();
                string res4623UL = mc4623UL.Groups[1].ToString();
                string res4633UL = mc4633UL.Groups[1].ToString();
                string res4634UL = mc4634UL.Groups[1].ToString();
                string res4635UL = mc4635UL.Groups[1].ToString();

                _List<string> abc = (_List<string>)temp.Value;

                var mc4613ULValue = Regex.Match(abc[0], pattern4613ULValue);
                var mc4623ULValue = Regex.Match(abc[0], pattern4623ULValue);
                var mc4633ULValue = Regex.Match(abc[0], pattern4633ULValue);
                var mc4634ULValue = Regex.Match(abc[0], pattern4634ULValue);
                var mc4635ULValue = Regex.Match(abc[0], pattern4635ULValue);

                string res4613ULValue = mc4613ULValue.Value.ToString();
                string res4623ULValue = mc4623ULValue.Value.ToString();
                string res4633ULValue = mc4633ULValue.Value.ToString();
                string res4634ULValue = mc4634ULValue.Value.ToString();
                string res4635ULValue = mc4635ULValue.Value.ToString();

                if ((res4613UL != "") && (res4613ULValue != ""))
                {
                    BandList4613UL.Add(res4613UL);
                }
                if ((res4623UL != "") && (res4623ULValue != ""))
                {
                    BandList4623UL.Add(res4623UL);
                }
                if ((res4633UL != "") && (res4633ULValue != ""))
                {
                    BandList4633UL.Add(res4633UL);
                }
                if ((res4634UL != "") && (res4634ULValue != ""))
                {
                    BandList4634UL.Add(res4634UL);
                }
                if ((res4635UL != "") && (res4635ULValue != ""))
                {
                    BandList4635UL.Add(res4635UL);
                }

            }

            // Fallbacks:
            _List<string> resA4613FB = new _List<string>();
            _List<string> resA4623FB = new _List<string>();
            _List<string> resA4633FB = new _List<string>();
            _List<string> resA4634FB = new _List<string>();

            foreach (string temp in CABandListTable1)
            {
                _List<string> A4613FB = new _List<string>();
                A4613FB = fallbackCA(temp);
                resA4613FB.Concat(A4613FB).ToList();
            }
            foreach (string temp in CABandListTable2)
            {
                _List<string> A4623FB = new _List<string>();
                A4623FB = fallbackCA(temp);
                resA4623FB.Concat(A4623FB).ToList();
            }
            foreach (string temp in CABandListTable3)
            {
                _List<string> A4633FB = new _List<string>();
                A4633FB = fallbackCA(temp);
                resA4633FB.Concat(A4633FB).ToList();
            }
            foreach (string temp in CABandListTable4)
            {
                _List<string> A4634FB = new _List<string>();
                A4634FB = fallbackCA(temp);
                resA4634FB.Concat(A4634FB).ToList();
            }
            var DLFallbackAB = resA4613FB.Union(resA4623FB).ToList();
            var DLFallbackABC = DLFallbackAB.Union(resA4633FB).ToList();
            var DLFallbackABCD = DLFallbackABC.Union(resA4634FB).ToList();


            // E01, E02, E03:

            _List<string> E01Selection = carrierNo2(BandList4613UL);
            _List<string> E02Selection = carrierNo2(BandList4623UL);
            _List<string> E03Selection = carrierNo2(BandList4633UL);

            // E04:
            _List<string> E04SelectionPartA = carrierNo2(CABandListTable1);
            _List<string> E04Selection = new _List<string>(E04SelectionPartA.Except(BandList4613UL).ToList());

            //E05:
            _List<string> E05Selection = carrierNo2(CABandListTable2);

            //E06:
            _List<string> E06Selection = carrierNo2(CABandListTable3);

            //E07:
            _List<string> E07SelectionPartA = new _List<string>(CABandListTable1.Except(BandList4613UL).ToList());
            _List<string> E07SelectionPartB = new _List<string>(CABandListTable2.Except(BandList4623UL).ToList());
            _List<string> E07SelectionPartC = new _List<string>(CABandListTable3.Except(BandList4633UL).ToList());
            _List<string> E07SelectionPartD = new _List<string>(CABandListTable4.Except(BandList4634UL).ToList());
            var E07SelectionPartAB = new _List<string>(E07SelectionPartA.Union(E07SelectionPartB).ToList());
            var E07SelectionPartABC = new _List<string>(E07SelectionPartAB.Union(E07SelectionPartC).ToList());
            var E07SelectionPartABCD = new _List<string>(E07SelectionPartABC.Union(E07SelectionPartD).ToList());
            _List<string> E07Selection = carrierNo3(E07SelectionPartABCD);

            //E08:
            var E08Selection = E04Selection.Except(DLFallbackABCD).ToList();
            //E09:
            var E09Selection = E05Selection.Except(DLFallbackABCD).ToList();
            //E10:
            var E10Selection = E06Selection.Except(DLFallbackABCD).ToList();

            //E11:
            var E11SecPart = resA4623FB.Union(resA4633FB).Union(resA4634FB).ToList();
            var E11Selection = E04Selection.Except(E11SecPart).ToList();

            //E12:
            var E12SecPart = resA4623FB.Union(resA4634FB).ToList();
            var E12Selection = E06Selection.Except(E12SecPart).ToList();

            //E13:
            _List<string> E13Selection = DLOnlyBand(E06Selection);

            EList.Add("E01", E01Selection);
            EList.Add("E02", E02Selection);
            EList.Add("E03", E03Selection);
            EList.Add("E04", E04Selection);
            EList.Add("E05", E05Selection);
            EList.Add("E06", E06Selection);
            EList.Add("E07", E07Selection);
            EList.Add("E08", E08Selection);
            EList.Add("E09", E09Selection);
            EList.Add("E10", E10Selection);
            EList.Add("E11", E11Selection);
            EList.Add("E12", E12Selection);
            EList.Add("E13", E13Selection);

            return EList;

        }

        public string fallbackBand(string CABand)
        {
            string patt = @"(\d+)[A-E]";
            var mcBand = Regex.Match(CABand, patt);
            string resBand = mcBand.Groups[1].ToString();
            return resBand;
        }

        public _List<string> fallbackCA(string inputBand)
        {
            _List<string> fallbackCAOutput = new _List<string>();
            string patt1 = @"(\d+)D";
            string patt2 = @"(\d+)A-(\d+)A-(\d+)A";
            string patt3 = @"(\d+)C-(\d+)A";
            string patt4 = @"(\d+)B-(\d+)A";
            string patt5 = @"(\d+)A-(\d+)C";
            string patt6 = @"(\d+)A-(\d+)B";

            var mc1 = Regex.Match(inputBand, patt1);
            var mc2 = Regex.Match(inputBand, patt2);
            var mc3 = Regex.Match(inputBand, patt3);
            var mc4 = Regex.Match(inputBand, patt4);
            var mc5 = Regex.Match(inputBand, patt5);
            var mc6 = Regex.Match(inputBand, patt6);

            if (mc1.Success)
            {
                string res1 = mc1.Groups[1].ToString();
                fallbackCAOutput.Add(res1 + "C");
            }
            if (mc2.Success)
            {
                string res2A = mc2.Groups[1].ToString();
                string res2B = mc2.Groups[2].ToString();
                string res2C = mc2.Groups[3].ToString();
                fallbackCAOutput.Add(res2A + "A" + "-" + res2B + "A");
                fallbackCAOutput.Add(res2A + "A" + "-" + res2C + "A");
                fallbackCAOutput.Add(res2B + "A" + "-" + res2C + "A");
            }
            if (mc3.Success)
            {
                string res3A = mc3.Groups[1].ToString();
                string res3B = mc3.Groups[2].ToString();
                fallbackCAOutput.Add(res3A + "C");
                fallbackCAOutput.Add(res3A + "A" + "-" + res3B + "A");
            }
            if (mc4.Success)
            {
                string res4A = mc4.Groups[1].ToString();
                string res4B = mc4.Groups[2].ToString();
                fallbackCAOutput.Add(res4A + "B");
                fallbackCAOutput.Add(res4A + "A" + "-" + res4B + "A");
            }
            if (mc5.Success)
            {
                string res5A = mc5.Groups[1].ToString();
                string res5B = mc5.Groups[2].ToString();
                fallbackCAOutput.Add(res5B + "C");
                fallbackCAOutput.Add(res5A + "A" + "-" + res5B + "A");
            }
            if (mc6.Success)
            {
                string res6A = mc6.Groups[1].ToString();
                string res6B = mc6.Groups[2].ToString();
                fallbackCAOutput.Add(res6B + "B");
                fallbackCAOutput.Add(res6A + "A" + "-" + res6B + "A");
            }

            return fallbackCAOutput;

        }


        public _List<string> DLOnlyBand(_List<string> inputBand)
        {
            string patternDLOnlyBand = @"29|32|67";
            _List<string> DLOnlyBandOutput = new _List<string>();
            foreach (string temp in inputBand)
            {
                var mcBand = Regex.Match(temp, patternDLOnlyBand).ToString();
                if (mcBand != "")
                {
                    DLOnlyBandOutput.Add(temp);
                }
            }
            return DLOnlyBandOutput;
        }

        public _List<string> carrierNo2(_List<string> inputBand)
        {
            _List<string> carrierNo2Output = new _List<string>();
            string patt1 = @"^\d+A-\d+A$";
            string patt2 = @"^\d+[B-C]$";

            foreach (string temp in inputBand)
            {
                var mc1 = Regex.Match(temp, patt1).ToString();
                var mc2 = Regex.Match(temp, patt2).ToString();
                if ((mc1 != "") || (mc2 != ""))
                {
                    carrierNo2Output.Add(temp);
                }
            }
            return carrierNo2Output;
        }

        public _List<string> carrierNo3(_List<string> inputBand)
        {
            _List<string> carrierNo3Output = new _List<string>();
            string patt1 = @"^\d+A-\d+A-\d+A$";
            string patt2 = @"^\d+A-\d+B$";
            string patt3 = @"^\d+B-\d+A$";
            string patt4 = @"^\d+A-\d+C$";
            string patt5 = @"^\d+C-\d+A$";
            string patt6 = @"^\d+D$";
            foreach (string temp in inputBand)
            {
                var mc1 = Regex.Match(temp, patt1).ToString();
                var mc2 = Regex.Match(temp, patt2).ToString();
                var mc3 = Regex.Match(temp, patt3).ToString();
                var mc4 = Regex.Match(temp, patt4).ToString();
                var mc5 = Regex.Match(temp, patt5).ToString();
                var mc6 = Regex.Match(temp, patt6).ToString();

                if ((mc1 != "") || (mc2 != "") || (mc3 != "") || (mc4 != "") || (mc5 != "") || (mc6 != ""))
                {
                    carrierNo3Output.Add(temp);
                }
            }
            return carrierNo3Output;
        }

        // Functions for band processing:


        public string TCvsAppRefined(string verdictIn)
        {
            string verdictOut = "";
            if (verdictIn.ToLower() == "true"||verdictIn.ToLower() == "a")
            {
                verdictOut = "R";
            }
            else if (verdictIn.ToLower() == "false")
            {
                verdictOut = "NR";
            }
            else
            {
                verdictOut = verdictIn;
            }
            return verdictOut;
        }
        public static List<string> filereset = new List<string>();
        public static void writetocsv(string csvfilename, string line)
        {
            if (!filereset.Contains(csvfilename))
            {
                using (FileStream fs = new FileStream(csvfilename, FileMode.Create, FileAccess.Write))
                using (StreamWriter outputFile = new StreamWriter(fs))
                {
                    outputFile.WriteLine(line);
                }
                filereset.Add(csvfilename);
            }
            else
            {
                using (FileStream fs = new FileStream(csvfilename, FileMode.Append, FileAccess.Write))
                using (StreamWriter outputFile = new StreamWriter(fs))
                {
                    outputFile.WriteLine(line);
                }
            }

        }

    }
}
