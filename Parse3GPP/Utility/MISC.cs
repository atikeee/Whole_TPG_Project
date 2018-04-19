using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
//using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using xl = Microsoft.Office.Interop.Excel;
//using DocumentFormat.OpenXml;
using ClosedXML.Excel;
using System.Diagnostics;

namespace Utility
{
    public static class MISC
    {
        public static string getbw(string tc, string band)
        {
            string allbw = "";
            ReadConfig rc1 = new ReadConfig("1.txt", 1);
            ReadConfig rc2 = new ReadConfig("2.txt", 2);
            /*
            foreach (KeyValuePair<string, string> kvp in rc1.getdict())
            {
                Debug.Print(kvp.Key + " " + kvp.Value);
            }
            Debug.Print("next");
            foreach (KeyValuePair<string, string> kvp in rc2.getdict())
            {
                Debug.Print(kvp.Key + " " + kvp.Value);
            }
            */
            Dictionary<string, string> tc_bw = rc1.getdict();
            Dictionary<string, string> band_bw = rc2.getdict();
            string[] bwstrtc = { };
            string[] bwstrband = { };

            if (tc_bw.ContainsKey(tc) && band_bw.ContainsKey(band))
            {
                bwstrtc = tc_bw[tc].Split(',');
                bwstrband = band_bw[band].Split(',');
                //Debug.Print("BW tc: " + string.Join(",", bwstrtc));
                //Debug.Print("BW band: " + string.Join(",", bwstrband));
                for (int i = 0; i < bwstrtc.Count(); i++)
                {

                    if (bwstrtc[i] == "Lowest")
                    {
                        bwstrtc[i] = bwstrband[0];
                    }
                    else if (bwstrtc[i] == "Highest")
                    {
                        bwstrtc[i] = bwstrband[bwstrband.Count() - 1];
                    }
                }
                //Debug.Print("BW tc2: " + string.Join(",", bwstrtc));
                allbw = string.Join(",", bwstrband.Intersect(bwstrtc));
                //Debug.Print(string.Join(",",bwstrband.Intersect(bwstrtc)));
            }
            return allbw;

        }
        public static _List<string> PICSBandSupportList;
        public static Logging lgstr;
        public static _Dictionary<string, string> readconfig(string fn)
        {
            _Dictionary<string, string> retdic = new _Dictionary<string, string>();
            string[] lines = File.ReadAllLines(fn);
            foreach (string line in lines)
            {
                if (!line.StartsWith("#"))
                {
                    string[] kvp = line.Split('=');
                    if (kvp.Count() > 1)
                    {
                        retdic.Add(kvp[0].Trim(), kvp[1].Trim());

                    }
                }

                // Use a tab to indent each line of the file.
            }
            return retdic;
        }
        public static string[] coltitle;
        public static void adddata(Dictionary<string,DataTable> datadic, string[] indat)
        {

            if (indat.Count() > 1)
            {
                string key = indat[0];
                string[] onedat = indat.Skip(1).ToArray();
                if (key != "")
                {
                    DataTable dt; 
                    if (datadic.ContainsKey(key))
                    {
                        dt = datadic[key];
                        
                    }
                    else
                    {
                        dt = new DataTable();
                        for (int i = 0; i < onedat.Count(); i++)
                        {
                            dt.Columns.Add(coltitle[i]);
                        }
                        //alist.Add(onedat);
                        datadic[key] = dt;
                    }
                    DataRow row = dt.NewRow();
                    row.ItemArray = onedat;
                    dt.Rows.Add(row);
                }

            }

        }
        public static void writetoexcel(Dictionary<string, DataTable> datadic,string excelfilename)
        {
            if (datadic.Count > 0)
            {
                var wb = new XLWorkbook();
                foreach (KeyValuePair<string, DataTable> kvp in datadic)
                {
                    wb.Worksheets.Add(kvp.Value, kvp.Key);
                }
                wb.SaveAs(excelfilename);
            }
            
         
        }
        
        public static void getRFdata(string[] indat)
        {
            // input length is predefined.. 
            string sh = "undef";
            //string[] dat = new string[indat.Count()] ;
            string tcno = indat[1].ToLower() ;
            string desc = indat[2].ToLower() ;
            string spec = indat[6].ToLower();
            string band = indat[9].ToLower();
            string[] lbandF =  { "1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26","27","28","29","30","31","32","33","34" ,"66"};
            string[] lbandT = { "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47" };

         
            string[] rf2g = { "12", "13", "14", "16", "18", "21", "22" };
            if (spec.Contains("51.010"))
            {
                foreach(string r in rf2g)
                {
                    if (tcno.StartsWith(r))
                    {
                        //sh = "CT_RF_2G";
                        sh = "51.010";

                        if (tcno.StartsWith("27.22"))
                        {
                            indat[6] = "51.010-4";
                        }
                        else
                        {
                            indat[6] = "51.010-2";
                        }
                        break;
                    }
                }
            }
            else if (spec == "34.121-1")
            {
                if (!tcno.StartsWith("8"))
                {
                    //sh = "CT_RF_3G";
                    sh = "34.121-1";
                }
            }
            else if(spec == "36.521-1")
            {
                sh = spec;
            }
            

            indat[0] = sh;

            //return dat;
        }
        public static void getRRMdata(string[] indat)
        {   
            /*
            -If spec = ’34.121 - 1’ and TC# starts with ‘8’, then ‘W-CDMA’

            -If spec = ’36.521 - 1’ and band contains ‘CA_’, then ‘LTE(CA)’
            
            -If spec = ’36.521 - 1’ and band format is ‘\d +’, then ‘LTE(Single)’
            
            -If spec = ’36.521 - 1’ and band format is ‘[a-zA-Z]+\d+-\w+’, then ‘LTE(Handover)’
            */

            string sh = "undef";
            string tcno = indat[1].ToLower();
            string desc = indat[2].ToLower();
            string spec = indat[6].ToLower();
            string band = indat[9].ToLower();

            
            if (spec == "36.521-3")
            {
                Regex regex = new Regex(@"^\d+$");
                Match m1 = regex.Match(band);
                regex = new Regex(@"[a-zA-Z]+\d+-\w+");
                Match m2 = regex.Match(band);
                if (band.Contains("ca_"))
                {
                    //sh = "LTE(CA)";
                    sh = spec;
                }
                else if (m1.Success)
                {
                    //sh = "LTE(Single)";
                    sh = spec;

                }
                else if (m2.Success)
                {
                    //sh = "LTE(Handover)";
                    sh = spec;

                }
                else
                {
                    lgstr.war("RRM: 36.521-3 -> ungrouped " + tcno + " band: " + band + " Desc: " + desc);
                }
            }

            else if (spec == "34.121-1")
            {
                if (tcno.StartsWith("8"))
                {
                    //sh = "WCDMA";
                    sh = spec;
                }
            }
            else if (spec == "34.122")
            {
                sh = spec; 
            }
           
            // No pics CVA/TVL file for 34.171
            
            indat[0] = sh;
        }
        public static void getCTPSdata(string[] indat)
        {
            string sh = "undef";
            string tcno = indat[1].ToLower();
            string desc = indat[2].ToLower();
            string spec = indat[6].ToLower();
            string band = indat[9].ToLower();
            /*
            - If spec = ’34.123-1’, then ‘3G_CTPS’

            - If spec = ’36.523-1’, then ‘LTE_CTPS’
            
            - If spec = ’51.010’ and TC# does not start with any of the element of the list {12, 13, 14, 16, 18, 21, 22}, then ‘2G_CTPS’
            
            */
            // this will contains converted spec only. and the spec is matched with pics only. 
            string[] otherspec = { "31.121-1", "31.124-1", "34.123-1", "34.229-1", "34.171",  "36.523-1", "37.571-1", "37.571-2", "37.901", "102 230-1" ,"tty","at-command"};
            string[] rf2g = { "12", "13", "14", "16", "18", "21", "22" };
            //sh = spec;
            //lgstr.deb("Spec333: " + spec);
            foreach(string othsp in otherspec)
            {
                if (spec.Contains(othsp))
                {
                    sh = othsp;
                    // to make standards unified. 
                    indat[6] = sh;
                    break;
                }
            }
            //if (otherspec.Contains(spec))
            //{
            //    sh = spec;
            //}
            //else if (spec == "36.523-1")
            //{
            //    sh = "36.521-1";
            //}
            //else if (spec == "34.123-1")
            //{
            //    sh = "34.123-1";
            //}
            //else
             if (spec.Contains("51.010"))
            {
                //System.Diagnostics.Debug.Print("spec: " + spec + " tcno: " + tcno);
                bool rrm2g = true;
                foreach (string r in rf2g)
                {
                    if (tcno.StartsWith(r))
                    {
                        rrm2g = false;
                        break;
                    }
                }
                if (rrm2g)
                {
                    sh = "51.010";

                    if (indat[1].ToString().StartsWith("27.22"))
                    {
                        indat[6] = "51.010-4";
                    }
                    else
                    {
                        indat[6] = "51.010-2";
                    }

                    //indat[6] = sh;
                }
            }
 
            indat[0] = sh;
        }
        public static Dictionary<string, string[]> prjsupbl(_Dictionary<string,string> bandlistdic, string project)
        {
            Dictionary<string, string[]> retdic = new Dictionary<string, string[]>();
            foreach(KeyValuePair<string,string> kvp in bandlistdic)
            {
                if (kvp.Key.EndsWith(project))
                {
                    string k = kvp.Key.Split('_')[0];
                    string[] bl = kvp.Value.Split(' ');
                    retdic.Add(k,bl);
                }
            }
            return retdic;
        }
        public static bool bandSupportHelper(string inputBand)
        {
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
                if (PICSBandSupportList.Contains(outputBand))
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
                if (PICSBandSupportList.Contains(outputBand))
                {
                    finaloutputBand = true;
                }
                else if (PICSBandSupportList.Contains(outputBandHOA1) && PICSBandSupportList.Contains(outputBandHOA2))
                {
                    finaloutputBand = true;
                }
                else if (PICSBandSupportList.Contains(outputBandHOB1) && PICSBandSupportList.Contains(outputBandHOB2) && PICSBandSupportList.Contains(outputBandHOB3) && PICSBandSupportList.Contains(outputBandHOB4))
                {
                    finaloutputBand = true;
                }
                else if (PICSBandSupportList.Contains(outputBandHO1) && PICSBandSupportList.Contains(outputBandHO2))
                {
                    finaloutputBand = true;
                }
                else if (PICSBandSupportList.Contains(outputBandHO1_3) && PICSBandSupportList.Contains(outputBandHO2_3) && PICSBandSupportList.Contains(outputBandHO3_3))
                {
                    finaloutputBand = true;
                }
                else
                {
                    lgstr.err("{bandSupportHelper} Band Not Match: " + inputBand);
                }
            }

            //lgstr.cri("Input band: " + inputBand + ", Output band: " + outputBand + ", Result: " + finaloutputBand);
            return finaloutputBand;
        }


        public static string bandformat(string bin)
        {
            string bout=bin;
            Regex r = new Regex(@"^(\d{1,2})$");
            Match m = r.Match(bin) ;
            if (m.Success)
            {
                bout = "E"+m.Groups[1].ToString();
            }
            r = new Regex("([IVX])",RegexOptions.IgnoreCase);
            m = r.Match(bin);
            if (m.Success)
            {
                bout = "U" + m.Groups[1].ToString();
            }
            return bout; 
        }

    }
}
