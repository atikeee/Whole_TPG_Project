using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.OleDb;
using System.Data;
using System.IO;
using Utility;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;

namespace excelparse
{

    public class ParseExcel
    {
        private string _FilePath = "";
        private string _sheetname = "";
        private string _outputpath = "";
        public static string[] BIlist;
        public static string[] BIlistW;
        public static string[] BIlistM;
        private Dictionary<string, DataTable> allrfdata = new Dictionary<string, DataTable>();
        private Dictionary<string, DataTable> allrrmdata = new Dictionary<string, DataTable>();
        private Dictionary<string, DataTable> allctpsdata = new Dictionary<string, DataTable>();
        
        public Logging lgx;
        private Dictionary<string, StreamWriter> csvtowritedic = new Dictionary<string, StreamWriter>();
        public ParseExcel(string _FilePath, string _type)
        {
            this._FilePath = _FilePath;
            this._outputpath = Path.Combine(Directory.GetCurrentDirectory(), "output", _type);
            //cleanupfolder();
        }
        public ParseExcel(string _FilePath)
        {
            this._FilePath = _FilePath;
            //this._outputpath = Path.Combine(Directory.GetCurrentDirectory(), "output");
            //cleanupfolder();
        }
        public void cleanupfolder()
        {

            try
            {
                DirectoryInfo di = Directory.CreateDirectory(this._outputpath);
                FileInfo[] files = di.GetFiles("*.csv");

                foreach (FileInfo f in files)
                {
                    lgx.deb("removing file: " + f.ToString());
                    f.Delete();
                }


            }
            catch (Exception ex)
            {
                lgx.cri("Error while cleaning/preparing directory. " + ex.Message);
            }



        }

        public List<string> GetExcelsheetslist()
        {

            string excelConnString = @"provider=Microsoft.ACE.OLEDB.12.0;data source=" + this._FilePath + ";extended properties=" + "\"excel 12.0;hdr=yes;IMEX=1\"";

            List<string> sheetnameexcel = new List<string>();
            //Create Connection to Excel work book 
            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                excelConnection.Open();
                DataTable Sheets = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow dr in Sheets.Rows)
                {
                    sheetnameexcel.Add(dr[2].ToString().Replace("'", ""));
                    string excelSheets = dr["TABLE_NAME"].ToString();
                }
            }
            return sheetnameexcel;
        }


        public DataTable GetExcelData(string ExcelSheetName, string Condition = "")
        {
            //DataTable tbl = new DataTable();
            this._sheetname = ExcelSheetName.Replace("$", "");
            string excelConnString = @"provider=Microsoft.ACE.OLEDB.12.0;data source=" + this._FilePath + ";extended properties=" + "\"excel 12.0;hdr=yes;IMEX=1\"";
            List<DataTable> tbllist = new List<DataTable>();
            Debug.Print(this._FilePath);
            DataTable allcombined = new DataTable();
            string[] excelcols = { "A:IU", "IV:SP", "SQ:ACK", "ACL:AMF", "AMG:AWA", "AWB:BFV", "BFW:BZL", "BZM:CJG", "CJH:CTB", "CTC:DCW", "DCX:DMR", "DMS:DWM", "DWN:EGH", "EGI:EQC", "EQD:FZX", "FZY:FJS", "FJT:FTN", "FTO:GDI", "GDJ:GNE" };

            //Create Connection to Excel work book 
            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                int tableidx = 0;
                int abscolidx = 0;
                foreach (string excelcol in excelcols)
                {
                    abscolidx = tableidx * 255;
                    DataTable tbl = new DataTable();
                    Debug.Print("READING Table : " + tableidx);
                    tableidx++;
                    string sql = "Select * " + " from [" + ExcelSheetName + excelcol + "]";
                    Debug.Print(sql);
                    //string sql = String.Format("select * from [{0}A1:AZZ3000]", ExcelSheetName);

                    OleDbDataAdapter da = new OleDbDataAdapter(sql, excelConnection);
                    //DataSet ds = new DataSet();
                    da.Fill(tbl);
                    int colcount = tbl.Columns.Count;
                    foreach (DataColumn dc in tbl.Columns)
                    {
                        abscolidx++;
                        dc.ColumnName = abscolidx.ToString();
                        allcombined.Columns.Add(abscolidx.ToString());
                        //Debug.Print(dc.ToString());
                    }
                    tbllist.Add(tbl);
                    if (colcount < 250)
                    {
                        break;
                    }

                }
                mergeTables(allcombined, tbllist);
            }
            return allcombined;
        }
        private void mergeTables(DataTable allcombined, List<DataTable> dataTables)
        {
            List<int> oList = new List<int>();
            //DataTable allcombined = new DataTable();
            DataTable curtable = dataTables[0];
            int tblidx = 0;
            int colidx = 0;
            int rowcount = curtable.Rows.Count;
            int colcount = allcombined.Columns.Count;
            //lgx.inf("colcount: " + colcount.ToString() + " tablecount " + dataTables.Count.ToString());
            for (int rn = 0; rn < rowcount; rn++)
            {
                DataRow newRow = allcombined.NewRow();
                for (int cn = 0; cn < colcount; cn++)
                {
                    if (cn == 0)
                    {
                        tblidx = 0;
                    }
                    if (cn % 255 == 0)
                    {
                        curtable = dataTables[tblidx];
                        tblidx++;
                        colidx = 0;
                    }
                    newRow[cn] = curtable.Rows[rn][colidx];
                    colidx++;
                }
                allcombined.Rows.Add(newRow);
            }



        }



        public void writetocsv(DataTable dt, string csvfilename, string sep = ",")
        {
            StreamWriter w = new StreamWriter(csvfilename);
            foreach (DataRow dr in dt.Rows)
            {
                string oneline = dr.ItemArray.Count().ToString() + " # ";
                oneline += String.Join(sep, dr.ItemArray);
                oneline = oneline.Replace("\n", "").Replace("\r", "");
                w.WriteLine(oneline);
            }

            w.Close();
        }

        private Dictionary<string, string> specinout = new Dictionary<string, string>()
        {
            { "102 230","102 230-1"},
            { "31.121","31.121-1"},
            { "31.124","31.124-1"},

        };
        private string convertspec(string specinput, string[] patterns)
        {
            string specop = specinput;
            //string[] patterns = { @"(RCS) \d\.\d", @"(C\.S00)", @"(GCF) TS\.\d+", @"(GSMA) TS\.\d+", @"(SUPL)-V", @"(SYSDET)-", @"(VZW)", @"(\d+\.\d+(?:-\d)?)", @"(\d+\s+\d+(:?-\d)?)" };
            //string[] patterns = { @"GPP T[RS] (\d+\.\d+(?:-\d)?)", @"(CITA).+", @"ETSI TS (\d+\s+\d+(:?-\d)?)", @"(GSMA) PRD TS.+", @"(OMA).+", @"(AT-Command)" , @"(TTY)" };
            Debug.Print("spec in: " + specinput);
            foreach (string pat in patterns)
            {
                Match m = Regex.Match(specinput, pat, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    specop = m.Groups[1].ToString();
                    break;
                }
            }
            //Debug.Print("spec output1: " + specop);

            foreach (KeyValuePair<string, string> kvp in specinout)
            {
                if (specop == kvp.Key)
                {
                    //Debug.Print("xxxx");
                    specop = kvp.Value;
                }
            }
            //Debug.Print("spec output2: " + specop);

            //lgx.deb("spec conversion: " +specinput+"->"+specop);
            return specop;

        }
        public void writeoutput()
        {
            string prefix = _FilePath.Replace(".xlsx", "_");
            MISC.writetoexcel(allrfdata, prefix + "RF.xlsx");
            MISC.writetoexcel(allrrmdata, prefix + "RRM.xlsx");
            MISC.writetoexcel(allctpsdata, prefix + "CTPS.xlsx");
        }
        public void processgcffile(DataTable dt, Dictionary<string, List<string>> tc_env, string sep = "\t")
        {
            //reading config files. 
            Dictionary<string, List<string>> iratsm = new Dictionary<string, List<string>>();
            Dictionary<string,List<string>> iratbi = new Dictionary<string, List<string>>();
            MISC.readconfig_BI("conf\\GCF_IRAT_SM_TC.conf", iratsm);
            MISC.readconfig_BI("conf\\GCF_IRAT_BI_TC.conf", iratbi);
            Dictionary<string, List<string>> bandlistdic = MISC.readconfiglist("conf\\GCF_IRAT_band.conf", '\t');
            MISC.allrfdata = allrfdata;
            MISC.allrrmdata = allrrmdata;
            MISC.allctpsdata = allctpsdata;
            MISC.coltitle = new string[] { "TC Number", "Description", "Type of Test", "Band Applicability", "sheetname", "Standards", "Environmental Condition", "Band Criteria", "Band", "ICE Recommendation for Band", "TC Status", "Certified TP [V]", "Certified TP [E]", "Certified TP [D]", "BandWidth", "WI" };
            string wi = "";
            string spec = "";
            string oldspec = "";
            string conspec = "";
            //int[] indextopick = { 1, 2, 5, 6 }; // 3.65 1= tcno 2 = desc 5 = type of test 6 = band applicability
            int[] indextopick = { 1, 2, 6, 7 }; // 3.66 1= tcno 2 = desc 6 = type of test 7 = band applicability
            string[] rowdata = { "", "", "", "", this._sheetname, "", "", "" };
            bool hdr = true;
            bool validline = false;
            int colcount = 0;
            int bandcolidx = 0;
            int colidxforvalidation = 0;
            string[] combostringlist = new string[1];

            lgx.deb("Datatable Row Count: " + dt.Rows.Count.ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                Dictionary<string, string[]> bandinfodic = new Dictionary<string, string[]>();
                //Debug.Print(dr[0].ToString()+dr[1].ToString());
                if (!hdr)
                {
                    if (dr[1].ToString().StartsWith("3GPP TS") || dr[1].ToString().StartsWith("ETSI TS") || (dr[1].ToString() == ""))
                    {
                        //Debug.Print("not a valid testconfig line"+ dr[0].ToString()+dr[1].ToString());
                        validline = false;
                    }
                    else
                    {
                        spec = dr[0].ToString();
                        rowdata[5] = spec;
                        

                        //rowdata[6] = 
                        validline = true;
                        //Debug.Print("tc "+dr[1].ToString());
                        for (int ii = 0; ii < indextopick.Count(); ii++)
                        {
                            int indpick = indextopick[ii];
                            rowdata[ii] = dr[indpick].ToString();
                        }
                        wi = dr[5].ToString();
                        Regex rgxwi = new Regex(@"^(WI-\d+)");
                        Match mwi = rgxwi.Match(wi);
                        if (mwi.Success)
                        {
                            wi = mwi.Groups[1].ToString();
                        }
                        rowdata[6] = "1";

                        if (tc_env.ContainsKey(spec))
                        {
                            List<string> tcl = tc_env[spec];
                            //lgx.err(String.Format("spec: {0} tcl: {1} rowdata0: {2}", spec, string.Join(",",tcl),rowdata[0]));
                            if (tcl.Contains(rowdata[0]))
                            {
                                rowdata[6] = "2";
                            }
                        }
                        switch (rowdata[3].ToLower())
                        {
                            case "single":
                                rowdata[7] = "1";
                                break;
                            case "all":
                                rowdata[7] = "2";
                                break;
                            case "i-rat single":
                            case "irat-single":
                                rowdata[7] = "3";
                                break;
                            case "irat-all":
                                rowdata[7] = "4";
                                break;
                            case "rat-all":
                                rowdata[7] = "5";
                                break;
                            default:
                                rowdata[7] = "6";
                                break;
                        }
                        //rowdata[7] = rowdata[3].ToLower().Contains("single") ? "C1" : "C2";
                        for (int j = bandcolidx; j < colcount; j++)
                        {
                            string band = "";
                            if (dr[j].ToString() != "-")
                            {
                                if (j < colidxforvalidation)
                                {
                                    band = combostringlist[j];
                                    string[] bandinfolist = { "", "", "", "", "", "" };
                                    //Debug.Print("band1: " + band);
                                    bandinfodic[band] = bandinfolist;
                                    bandinfodic[band][2] = dr[j].ToString();

                                }
                                else
                                {
                                    string[] band_validtp = combostringlist[j].Split('#');
                                    band = band_validtp[0];
                                    string tptype = band_validtp[1];
                                    //Debug.Print("band2: " + band);


                                    if (!bandinfodic.ContainsKey(band) && dr[j].ToString() != "")
                                    {
                                        string[] bandinfolist = { "", "", "", "", "", "" };
                                        bandinfodic[band] = bandinfolist;
                                    }
                                    if (dr[j].ToString() != "")
                                    {
                                        if (tptype.ToUpper() == "V")
                                        {
                                            bandinfodic[band][3] = dr[j].ToString();
                                        }
                                        else if (tptype.ToUpper() == "E")
                                        {
                                            bandinfodic[band][4] = dr[j].ToString();
                                        }
                                        else if (tptype.ToUpper() == "D")
                                        {
                                            bandinfodic[band][5] = dr[j].ToString();
                                        }
                                        else
                                        {
                                            lgx.cri("Unknown Validation Type: " + tptype);
                                        }
                                    }

                                }

                            }

                        }

                    }
                }
                else
                {
                    if (dr[0].ToString().StartsWith("Conformance"))
                    {
                        colcount = dr.ItemArray.Count();

                        //Debug.Print("colcount"+colcount.ToString());
                        for (int j = 0; j < colcount; j++)
                        {
                            if (dr[j].ToString().StartsWith("Cat"))
                            {
                                bandcolidx = j;
                                break;
                            }

                        }
                    }
                    else if (dr[0].ToString().StartsWith("Specification"))
                    {
                        i++;
                        DataRow drnext = dt.Rows[i];
                        combostringlist = new string[colcount];
                        hdr = false;
                        string band = "";
                        string comb = "";
                        string valstatus = "";

                        // initial value of j should set according to the first column index of the band from the excel file. 
                        Debug.Print("Colcount: " + colcount.ToString());
                        for (int j = bandcolidx; j < colcount; j++)
                        {
                            if (dr[j].ToString() != "")
                            {
                                band = dr[j].ToString();
                            }
                            valstatus = drnext[j].ToString();
                            if (valstatus == "")
                            {
                                comb = band;
                            }
                            else
                            {
                                if (colidxforvalidation == 0)
                                {
                                    colidxforvalidation = j;
                                }
                                comb = band + "#" + valstatus;
                            }
                            //Debug.Print("header read:(comb) " + comb);

                            combostringlist[j] = comb;
                        }



                    }
                }


                if ((spec != "") && (oldspec != spec))
                {
                    string[] patterns = { @"(RCS) \d\.\d", @"(C\.S00)", @"(GCF) TS\.\d+", @"(GSMA) TS\.\d+", @"(SUPL)-V", @"(SYSDET)-", @"(VZW)", @"(\d+\.\d+(?:-\d)?)", @"(\d+\s+\d+(:?-\d)?)" };
                    conspec = convertspec(spec, patterns);
                    conspec = conspec.Replace(@"/", "_");

                    lgx.deb("Spec Convert: " + spec + " => " + conspec);
                    if (!csvtowritedic.ContainsKey(conspec))
                    {
                        string csvpath = Path.Combine(this._outputpath, conspec + ".csv");
                        StreamWriter w = new StreamWriter(csvpath);
                        csvtowritedic[conspec] = w;
                    }


                }

                if (validline)
                {
                    string commondata = String.Join(sep, rowdata).Replace("\n", "").Replace("\r", "");
                    string oneline = "";

                    if (bandinfodic.Count > 0)
                    {
                        foreach (KeyValuePair<string, string[]> kvp in bandinfodic)
                        {
                            int rowsz = rowdata.Length + kvp.Value.Length;
                            string[] allrow = new string[rowsz + 3];
                            allrow[rowsz + 2] = wi;
                            allrow[0] = rowdata[5];
                            string band = processband(kvp.Key);
                            //string bandsupport = MISC.bandSupportHelper(band) ? "S" : "NS";
                            kvp.Value[0] = band;
                            //kvp.Value[1] = bandsupport;





                            //Array.Copy(rowdata, allrow, rowdata.Length);
                            Array.Copy(rowdata, 0, allrow, 1, rowdata.Length);
                            Array.Copy(kvp.Value, 0, allrow, rowdata.Length + 1, kvp.Value.Length);
                            //oneline = commondata + sep + band + sep + bandsupport + sep + String.Join(sep, kvp.Value.ToArray());
                            allrow[6] = conspec;
                            if (conspec.Trim() == "36.521-1")
                            {
                                //Debug.Print("TC: "+rowdata[0]+ "band: "+ band);
                                allrow[15] = MISC.getbw(rowdata[0], band);
                                //Debug.Print(allrow.Count().ToString());
                            }
                            oneline = String.Join(sep, allrow);
                            csvtowritedic[conspec].WriteLine(oneline);
                            //lgx.war("allrow1: " + string.Join("|",allrow));
                            // address BI issue here. 
                            int cat = 0;
                            if (cat == 0)
                            {
                                MISC.getRFdata(allrow);
                                if (allrow[0] != "undef")
                                    cat = 1;
                            }
                            if (cat == 0)
                            {
                                MISC.getRRMdata(allrow);
                                if (allrow[0] != "undef")
                                    cat = 2;
                            }
                            if (cat == 0)
                            {
                                MISC.getCTPSdata(allrow);
                                if (allrow[0] != "undef")
                                    cat = 3;
                            }
                            if (cat == 0)
                            {
                                //lgx.war("GCF!RF!RRM!PS: \t" + string.Join("\t", allrow));
                            }
                            MISC.removetpduplicate(allrow);
                            //allrow[9] = allrow[9].Replace("_RX4", "");
                            string spectc = allrow[6]+":"+ allrow[1];
                            List<string> allband = new List<string>();
                            bool iratconfig = false;
                            bool mband = false;
                            if (allrow[9].Contains("M"))
                            {
                                mband = true;
                            }
                            if (iratsm.ContainsKey(spectc))
                            {
                                iratconfig = true;
                                foreach (string iratbandcat in iratsm[spectc])
                                    if (bandlistdic.ContainsKey(iratbandcat))
                                        foreach (string b in bandlistdic[iratbandcat])
                                        {
                                            allband.Add(b);
                                        }
                            }
                                
                            else if (iratbi.ContainsKey(spectc))
                            {
                                iratconfig = true;
                                foreach (string iratbandcat in iratbi[spectc])
                                    if (bandlistdic.ContainsKey(iratbandcat))
                                        foreach (string b in bandlistdic[iratbandcat])
                                        {
                                            allband.Add(b);
                                        }
                            }
                                
                            if (iratconfig)
                            {
                                string[] allrow2 = new string[allrow.Length];
                                Array.Copy(allrow, 0, allrow2, 0, allrow.Length);
                                Regex r = new Regex("^([IVX]+)$", RegexOptions.IgnoreCase);
                                Match m = r.Match(allrow[9]);
                                foreach (string b in allband)
                                {
                                    if (!((b.Contains("M")) ^ mband))
                                    {
                                        if (b.Split('-')[0].Trim('E').TrimStart('0') == allrow[9])
                                        {
                                            allrow2[9] = allrow[9] + ">" + b;
                                            MISC.adddata(cat, allrow2);
                                        }

                                        else if (m.Success)
                                        {
                                            if (b.Split('-')[0].Trim('U').TrimStart('0') == MISC.RtoI[allrow[9]])
                                            {
                                                allrow2[9] = allrow[9] + ">" + b;
                                                MISC.adddata(cat, allrow2);
                                            }
                                        }
                                        else
                                        {
                                            allrow2[9] = allrow[9] + ">" + b;
                                            MISC.adddata(cat, allrow2);
                                        }
                                    }
                                    
                                }
                            }
                            //else if (iratbi.ContainsKey(spectc))
                            //{
                            //    string[] allrow2 = new string[allrow.Length];
                            //    Array.Copy(allrow, 0, allrow2, 0, allrow.Length);
                            //    foreach (string iratbandcat in iratsm[spectc])
                            //        foreach (string b in bandlistdic[iratbandcat])
                            //        {
                            //            allrow2[9] = allrow[9] + ">" + b;
                            //            MISC.adddata(cat, allrow2);
                            //        }
                            //}
                            else if (allrow[9] == "BI-M")
                            {
                                foreach (string b in BIlistM)
                                {
                                    string[] allrow2 = new string[allrow.Length];
                                    Array.Copy(allrow, 0, allrow2, 0, allrow.Length);
                                    allrow2[9] = allrow[9] + ":" + b + "M";

                                    MISC.adddata(cat, allrow2);
                                }
                            }
                            else if (allrow[9] == "BI")
                            {
                                foreach (string b in BIlist)
                                {
                                    string[] allrow2 = new string[allrow.Length];
                                    Array.Copy(allrow, 0, allrow2, 0, allrow.Length);
                                    allrow2[9] = allrow[9] + ":" + b;

                                    MISC.adddata(cat, allrow2);
                                }
                            }
                            else
                            {
                                MISC.adddata(cat, allrow);

                            }
                        }
                    }
                    else
                    {
                        lgx.cri("else no bandinfo");
                        string[] t = { "", "", "", "", "", "" };
                        string[] allrow = new string[rowdata.Length + t.Length + 1];
                        Array.Copy(rowdata, allrow, rowdata.Length);

                        oneline = String.Join(sep, allrow);
                        csvtowritedic[conspec].WriteLine(oneline);
                    }
                    csvtowritedic[conspec].Flush();

                }
                oldspec = spec;
            }


        }
        private string processband(string bin)
        {
            string bout = bin.Replace("FDD", "").Replace("TDD", "").Trim();
            //lgx.err(");
            bout = bout.TrimStart('0');
            bout = bout.Replace("GSM", "").Trim();
            return bout;
        }
        public void processptcrbfile(DataTable dt, Dictionary<string, List<string>> tc_env, string[] ptcrbspec, string sep = "\t")
        {
            Dictionary<string, List<string>> iratsm = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> iratbi = new Dictionary<string, List<string>>();
            MISC.readconfig_BI("conf\\PTCRB_IRAT_SM_TC.conf", iratsm);
            MISC.readconfig_BI("conf\\PTCRB_IRAT_BI_TC.conf", iratbi);
            Dictionary<string, List<string>> bandlistdic = MISC.readconfiglist("conf\\PTCRB_IRAT_band.conf", '\t');
            MISC.allrfdata = allrfdata;
            MISC.allrrmdata = allrrmdata;
            MISC.allctpsdata = allctpsdata;
            MISC.coltitle = new string[] { "TC Number", "Description", "Type of Test", "Band Applicability", "Cat", "Standards", "Environmental Condition", "Band Criteria", "Band", "ICE Recommendation for Band", "TC Status", "Certified TP [V]", "Certified TP [E]", "Certified TP [D]", "BandWidth", "RFT" };
            string spec = "";
            string rft = "";
            string oldspec = "";
            string conspec = "";
            int[] indextopick = { 1, 2, 6, 8, 7 };
            string[] rowdata = { "", "", "", "", "", "", "", "" };
            bool hdr = true;
            //bool validline = false;
            int colcount = 0;
            int bandcolidx = 0;
            //int colidxforvalidation = 0;
            string[] combostringlist = new string[1];
            lgx.deb("Datatable Row Count: " + dt.Rows.Count.ToString());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string tcno = "";
                string band_with_tcno = "";
                Dictionary<string, string[]> bandinfodic = new Dictionary<string, string[]>();
                if (!hdr)
                {
                    spec = dr[0].ToString();
                    if (ptcrbspec.Contains(spec))
                    {
                        for (int ii = 0; ii < indextopick.Count(); ii++)
                        {
                            int indpick = indextopick[ii];
                            rowdata[ii] = dr[indpick].ToString().Replace("\n", "").Replace("\r", "");
                        }
                        rft = dr[3].ToString();
                        Regex rgxrft = new Regex(@"^(\d+):");
                        Match mrft = rgxrft.Match(rft);
                        if (mrft.Success)
                        {
                            rft = mrft.Groups[1].ToString();
                        }
                        tcno = rowdata[0];
                        tcno = tcno.Replace("FDD", "").Replace("TDD", "").Trim();

                        Regex regex = new Regex(@"(\S+)\s*\[r(?:el)?\d+\]", RegexOptions.IgnoreCase);
                        Match mtcno = regex.Match(tcno);
                        if (mtcno.Success)
                        {
                            tcno = mtcno.Groups[1].ToString();
                        }

                        regex = new Regex(@"(\S+)\s*\[(.+)[–-](.+)\]");
                        mtcno = regex.Match(tcno);
                        if (mtcno.Success)
                        {
                            tcno = mtcno.Groups[1].ToString();
                            band_with_tcno = MISC.bandformat(mtcno.Groups[2].ToString()) + "-" + MISC.bandformat(mtcno.Groups[3].ToString());
                        }
                        rowdata[0] = tcno;
                        string _t = rowdata[3].ToLower();
                        switch (rowdata[3].ToLower())
                        {
                            case "single":
                                rowdata[7] = "1";
                                break;
                            case "all":
                                rowdata[7] = "2";
                                break;
                            case "irat-single":
                            case "i-rat single":
                                rowdata[7] = "3";
                                break;
                            case "irat-all":
                                rowdata[7] = "4";
                                break;
                            case "rat-all":
                                rowdata[7] = "5";
                                break;
                            default:
                                rowdata[7] = "6";
                                break;
                        }
                        //rowdata[7] = _t.Contains("single") ? "C1" :"C2";
                        for (int j = bandcolidx; j < colcount; j++)
                        {
                            string band = "";
                            if (dr[j].ToString() != "")
                            {

                                string[] band_validtp = combostringlist[j].Split('#');
                                band = band_validtp[0];
                                string tptype = band_validtp[1];
                                //Debug.Print("band2: " + band);


                                if (!bandinfodic.ContainsKey(band) && dr[j].ToString() != "")
                                {
                                    //List<string> bandinfolist = new List<string>() { "", "", "", "" };
                                    // 0 and 1 reserve for band and bandsupport
                                    string[] bandinfolist = { "", "", "", "", "", "" };
                                    bandinfodic[band] = bandinfolist;
                                }
                                if (dr[j].ToString() != "")
                                {
                                    string dd = dr[j].ToString().Replace("(", "").Replace(")", "");
                                    if (tptype.ToUpper() == "V")
                                    {
                                        bandinfodic[band][3] = dd;
                                    }
                                    else if (tptype.ToUpper() == "E")
                                    {
                                        bandinfodic[band][4] = dd;
                                    }
                                    else if (tptype.ToUpper() == "D")
                                    {
                                        bandinfodic[band][5] = dd;
                                    }
                                    else if (tptype.ToUpper() == "CATEGORY")
                                    {
                                        bandinfodic[band][2] = dd;
                                    }
                                    else
                                    {
                                        lgx.cri("Unknown Validation Type: " + tptype);
                                    }
                                }



                            }

                        }

                        if ((spec != "") && (oldspec != spec))
                        {
                            string[] patterns = { @"GPP T[RS] (\d+\.\d+(?:-\d)?)", @"(CTIA).+", @"ETSI TS (\d+\s+\d+(:?-\d)?)", @"(GSMA) PRD TS.+", @"(OMA).+", @"(AT-Command)", @"(TTY)" };
                            conspec = convertspec(spec, patterns);
                            conspec = conspec.Replace("//", "_");
                   

                            lgx.deb("Current spec: " + spec + " => " + conspec);
                            if (!csvtowritedic.ContainsKey(conspec))
                            {
                                string csvpath = Path.Combine(this._outputpath, conspec + ".csv");
                                StreamWriter w = new StreamWriter(csvpath);
                                csvtowritedic[conspec] = w;
                            }
                        }


                        rowdata[5] = conspec;
                        rowdata[6] = "1";
                        if (tc_env.ContainsKey(conspec))
                        {
                            List<string> tcl = tc_env[conspec];
                            if (tcl.Contains(rowdata[0]))
                            {
                                rowdata[6] = "2";
                            }
                        }
                        string commondata = String.Join(sep, rowdata);
                        string oneline = "";
                        if (bandinfodic.Count > 0)
                        {
                            foreach (KeyValuePair<string, string[]> kvp in bandinfodic)
                            {
                                int rowsz = rowdata.Length + kvp.Value.Length;
                                string[] allrow = new string[rowsz + 3];
                                //allrow[rowsz + 1] = bw;
                                allrow[rowsz + 2] = rft;
                                string band = processband(kvp.Key);
                                kvp.Value[0] = band;
                                //string bandsupport = MISC.bandSupportHelper(band) ? "S" : "NS";
                                //kvp.Value[1] = bandsupport;
                                Array.Copy(rowdata, 0, allrow, 1, rowdata.Length);
                                Array.Copy(kvp.Value, 0, allrow, rowdata.Length + 1, kvp.Value.Length);
                                // this is to replace the band information for IRAT. 
                                if (band_with_tcno != "")
                                {
                                    allrow[9] = band_with_tcno;
                                }
                                oneline = String.Join(sep, allrow);
                                //oneline = commondata + sep + kvp.Key + sep + String.Join(sep, kvp.Value.ToArray());
                                csvtowritedic[conspec].WriteLine(oneline);
                                //lgx.war("allrow1: " + string.Join("|", allrow));
                                int cat = 0;
                                if (cat == 0)
                                {
                                    MISC.getRFdata(allrow);
                                    if (allrow[0] != "undef")
                                        cat = 1;
                                }
                                if (cat == 0)
                                {
                                    MISC.getRRMdata(allrow);
                                    if (allrow[0] != "undef")
                                        cat = 2;
                                }
                                if (cat == 0)
                                {
                                    MISC.getCTPSdata(allrow);
                                    if (allrow[0] != "undef")
                                        cat = 3;
                                }
                                if (cat == 0)
                                {
                                    lgx.war("PTCRB!RF!RRM!PS: \t" + string.Join("\t", allrow));
                                }
                                // checking for BI and replace BI with configuration BI list in the config.
                                MISC.removetpduplicate(allrow);
                                //allrow[9] = allrow[9].Replace("_RX4", "");
                                // find out which BIlist to pick up 
                                string[] bil=BIlist; 

                                List<string> allspeclistforM = new List<string>() { "34.229-1", "36.521-3", "36.523-1" };
                                bool mband = false;
                                if ((rft == "132") && (allspeclistforM.Contains(allrow[6].ToString())))
                                {
                                    mband = true;
                                }


                                List<string> allband = new List<string>();
                                bool iratconfig = false;
                                
                                string spectc = allrow[6] + ":" + allrow[1];
                                if (iratsm.ContainsKey(spectc))
                                {
                                    iratconfig = true;
                                    foreach (string iratbandcat in iratsm[spectc])
                                        if (bandlistdic.ContainsKey(iratbandcat))
                                            foreach (string b in bandlistdic[iratbandcat])
                                            {
                                                allband.Add(b);
                                            }
                                }

                                else if (iratbi.ContainsKey(spectc))
                                {
                                    iratconfig = true;
                                    foreach (string iratbandcat in iratbi[spectc])
                                        if (bandlistdic.ContainsKey(iratbandcat))
                                            foreach (string b in bandlistdic[iratbandcat])
                                            {
                                                allband.Add(b);
                                            }
                                }

                                if (iratconfig)
                                {
                                    string[] allrow2 = new string[allrow.Length];
                                    Array.Copy(allrow, 0, allrow2, 0, allrow.Length);
                                    Regex r = new Regex("^([IVX]+)$", RegexOptions.IgnoreCase);
                                    Match m = r.Match(allrow[9]);
                                    foreach (string b in allband)
                                    {
                                        //if (!((b.Contains("M")) ^ mband))
                                        {
                                            if (b.Split('-')[0].Trim('E').TrimStart('0') == allrow[9])
                                            {
                                                allrow2[9] = allrow[9] + ">" + b;
                                                MISC.adddata(cat, allrow2);
                                            }

                                            else if (m.Success)
                                            {
                                                if (b.Split('-')[0].Trim('U').TrimStart('0') == MISC.RtoI[allrow[9]])
                                                {
                                                    allrow2[9] = allrow[9] + ">" + b;
                                                    MISC.adddata(cat, allrow2);
                                                }
                                            }
                                            else
                                            {
                                                allrow2[9] = allrow[9] + ">" + b;
                                                MISC.adddata(cat, allrow2);
                                            }
                                        }

                                    }
                                }


                                else if (conspec == "AT-Command")
                                {
                                    string[] allrow2 = new string[allrow.Length];
                                    Array.Copy(allrow, 0, allrow2, 0, allrow.Length);
                                    MISC.adddata(3, allrow2);
                                }

                                else if (allrow[9] == "BA") 
                                {
                                    if (conspec == "31.124-1")
                                        bil = BIlistW;

                                    foreach (string b in bil)
                                    {
                                        string[] allrow2 = new string[allrow.Length];
                                        Array.Copy(allrow, 0, allrow2, 0, allrow.Length);
                                        if (mband)
                                        {
                                            allrow2[9] += ":" + b + "M";
                                        }else
                                        {
                                            allrow2[9] += ":" + b ;
                                        }
                                        
                                        MISC.adddata(cat,allrow2);
                                    }
                                }
                                else if (allrow[9] == "BI")
                                {

                                    foreach (string b in bil)
                                    {
                                        string[] allrow2 = new string[allrow.Length];
                                        Array.Copy(allrow, 0, allrow2, 0, allrow.Length);

                                        if (mband)
                                        {
                                            allrow2[9] += ":" + b + "M";
                                        }
                                        else
                                        {
                                            allrow2[9] += ":" + b;
                                        }

                                        MISC.adddata(cat, allrow2);
                                    }
                                }
                                else
                                {
                                    Regex rgxband = new Regex(@"^(\d+)");
                                    Match mb = rgxband.Match(allrow[9]);
                                    if (mb.Success)
                                    {
                                        if (mband)
                                            allrow[9] += ">" + allrow[9] + "M";
                                    }
                                    MISC.adddata(cat, allrow);
                                }
                            }
                        }
                        else
                        {
                            string[] t = { "", "", "", "", "", "" };
                            string[] allrow = new string[rowdata.Length + t.Length];
                            Array.Copy(rowdata, allrow, rowdata.Length);
                            lgx.cri("else no bandinfo " + allrow.Count());
                            oneline = String.Join(sep, allrow);
                            csvtowritedic[conspec].WriteLine(oneline);


                            // this should match with bandinfolist
                            //List<string> t = new List<string>() {"", "","","", "", "", "" };
                            //oneline = commondata + sep + String.Join(sep, t);
                            //csvtowritedic[conspec].WriteLine(oneline);
                        }

                        csvtowritedic[conspec].Flush();
                    }
                    //validline = true;
                }
                else // this code will run once for first line. 
                {

                    hdr = false;
                    lgx.deb("reading header: once only..");
                    colcount = dr.ItemArray.Count();
                    Debug.Print("column count: " + colcount.ToString());
                    for (int j = 0; j < colcount; j++)
                    {
                        Debug.Print(dr[j].ToString());
                        if (dr[j].ToString() != "")
                        {
                            bandcolidx = j;
                            break;
                        }

                    }
                    lgx.deb("start of band col idx: " + bandcolidx);
                    Debug.Print("start of band col idx: " + bandcolidx);
                    //foreach (DataColumn dc in dt.Columns)
                    //{
                    //    lgx.deb(String.Format("columns {0}->{1}",dc,dc.ColumnName ) );
                    //
                    //}
                    i++;
                    DataRow drnext = dt.Rows[i];
                    combostringlist = new string[colcount];
                    //this order might be depending on version. 
                    string[] spcband = { "BA", "BI", "NI" };

                    int spcbandidx = 0;
                    string band = "";
                    string comb = "";
                    string valstatus = "";

                    // initial value of j should set according to the first column index of the band from the excel file. 

                    for (int j = bandcolidx; j < colcount; j++)
                    {
                        string tband = dt.Columns[j].ColumnName.Trim().ToLower();
                        if (dr[j].ToString() != "")
                        {

                            if ((dr[j].ToString().ToLower() == "yes") || (dr[j].ToString().ToLower() == "no"))
                            {
                                Debug.Print("tband: " + tband + " " + dr[j].ToString().ToLower());
                                if (spcbandidx < 3)
                                {
                                    band = spcband[spcbandidx];
                                    spcbandidx++;
                                }

                               
                            }
                            else
                            {
                                band = dr[j].ToString();
                            }
                            //lgx.deb("band: dbg "+dr[j]+"band: "+band);

                        }

                        valstatus = drnext[j].ToString();
                        if (valstatus == "")
                        {
                            comb = band;
                            lgx.cri("null valid tp list title or categroy");
                        }
                        else
                        {

                            comb = band + "#" + valstatus;
                        }

                        combostringlist[j] = comb;
                        //Debug.Print(j.ToString() + " : " + dr[j].ToString() + " : " + drnext[j].ToString() + " : " + comb);
                    }
                    Debug.Print(String.Format("combinedstringlist {0}", String.Join(" - ", comb)));




                }

                oldspec = spec;
            }
        }

        public void processenv(DataTable dt, Dictionary<string, List<string>> spec_tc_ec, string printlevel = "0")
        {
            string spec = this._FilePath.Split('_')[0];
            lgx.deb("Datatable Row Count: " + dt.Rows.Count.ToString());
            string[] lines = File.ReadAllLines(@"conf\envcond.conf");
            List<string> envk = new List<string>();
            List<string> envv = new List<string>();
            foreach (string line in lines)
            {
                string[] kvp = line.Split('=');
                if (kvp.Count() > 1)
                {
                    envk.Add(kvp[0].Trim());
                    envv.Add(kvp[1].Trim());

                }
                // Use a tab to indent each line of the file.
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string tcno = dr[0].ToString().Trim();
                if (tcno == "")
                    continue;
                string envc = dr[3].ToString().Replace("\n", "").Replace("\t", "");
                envc = envc.Replace(" ", "").ToLower();
                if (printlevel == "1")
                {
                    lgx.inf(tcno + " -- " + envc);
                }

                //order is important here. 
                for (int j = 0; j < envk.Count; j++)
                {

                    if (printlevel == "2")
                    {
                        lgx.inf("\t" + envk[j]);
                    }
                    envc = envc.TrimEnd('t');
                    envc = envc.Replace(envk[j], envv[j]);
                    if (printlevel == "2")
                    {
                        lgx.inf("\t" + envc);
                    }

                }
                if (printlevel == "1")
                {
                    lgx.inf(tcno + " ++ " + envc + "[consider before space only]");
                }
                envc = envc.Split(' ')[0];

                if (spec_tc_ec.ContainsKey(spec))
                {
                    List<string> tclistextreme = spec_tc_ec[spec];
                    if (tclistextreme.Contains(tcno))
                    {
                        lgx.cri("spec: " + spec + "  Duplicate tc: " + tcno);
                    }
                    else
                    {
                        if (spec == "36.521-1" && (tcno.StartsWith("8") || tcno.StartsWith("9")))
                        {
                            lgx.war("spec 36521-1 and ch 8/9 ec extreme for " + tcno);
                        }
                        else
                        {
                            tclistextreme.Add(tcno);
                        }
                    }
                }
                else
                {
                    lgx.cri("spec data for env not found" + tcno + "  : " + envc);
                }
            }
        }
    }
}
