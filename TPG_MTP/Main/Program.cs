using System;
using XLReadWrite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Main
{
    class Program
    {
        static string getkey(string[,] res, int rownum , int[] colnum)
        {
            string k = "";
            foreach(int i in colnum)
            {
                //Debug.Print("getkey " + res[rownum - 1, i - 1]);
                k += "#" + res[rownum-1,i-1];
            }
            //Debug.Print("getk:" + k);
            return k.TrimStart('#');
        }
        static string getband(string[,] res, int rownum, int[] colnum)
        {
            string k = "";
            for (int i=0;i<5;i++)
            {
                if (res[rownum - 1, colnum[i] - 1] == "")
                    break;
                else
                    k += "~" + res[rownum - 1, colnum[i] - 1];
            }
            if(k!="")
                k = k.Substring(1);
            k += "|";
            for (int i = 5; i < 10; i++)
            {
                if (res[rownum - 1, colnum[i] - 1] == "")
                    break;
                else
                {
                    if (i != 5)
                    {
                        k += "~";
                    }
                    k +=  res[rownum - 1, colnum[i] - 1];
                }
                    
            }
            return k;
        }
        static string configctps = "";
        //static void addres(string[,] res, List<string> tpgval, int rownum)
        static void addres(List<string[]> res, string[] ln ,  List<string> tpgval, int rownum)
        {
            //Debug.Print("Results of tpgVal.");
            //foreach (string valTemp in tpgval)
            //{
            //    Debug.Print(valTemp);
            //}

            rownum--;
            // Config file for mapping Spec, Ch#, ChName, RAT, Subdomain/ Complied with MTPToQCConfig now!
            StreamReader srMTPCTPSO1 = new StreamReader(configctps);

            // Config file to define CS and PS for Spec: 34.123-1:
            StreamReader srDomain = new StreamReader("DomainConfig34123-1.txt");

            // Limited Applicability Parsing:
            StreamReader srLAFile1 = new StreamReader("TCtoR.csv");
            StreamReader srLAFile2 = new StreamReader("LA_Rx.csv");


            // Dictionaries:
            // specChNVsChT[spec#ChN] = ChT / From: MTP_CTPS_O1_Modified_09262017
            // specChNVsCat[spec#ChN] = Cat / From: MTP_CTPS_O1_Modified_09262017
            // specChNVsRAT[spec#ChN] = RAT / From: MTP_CTPS_O1_Modified_09262017
            // TCVsDomain[TC] = Domain (CS or PS) / From: DomainConfig34123-1
            Dictionary<string, string> specChNVsChT = new Dictionary<string, string>();
            Dictionary<string, string> specChNVsCat = new Dictionary<string, string>();
            Dictionary<string, string> specChNVsRAT = new Dictionary<string, string>();
            Dictionary<string, string> TCVsDomain = new Dictionary<string, string>();

            Dictionary<string, string> LATCVsLA = new Dictionary<string, string>();
            Dictionary<string, string> LARVsApp = new Dictionary<string, string>();

            string line = "";
            while ((line = srMTPCTPSO1.ReadLine()) != null)
            {
                string[] lineSplit = line.Split('\t');
                string TCTemp = lineSplit[1];
                string chN = "";
                //Debug.Print(String.Format("Debug Line MTP_CTPS_O1: TC: {0}", TC));
                if (chN != null)
                {
                    //chN = TC[0].ToString();
                    chN = TCTemp;
                }
                string key = lineSplit[0] + "#" + chN;
                specChNVsChT[key] = lineSplit[2];
                specChNVsCat[key] = lineSplit[3];
                specChNVsRAT[key] = lineSplit[4];
            }
            //
            string lineTemp2 = "";
            while ((lineTemp2 = srDomain.ReadLine()) != null)
            {
                string[] lineSplit2 = lineTemp2.Split('\t');
                TCVsDomain[lineSplit2[0]] = lineSplit2[1];
            }

            // Limited Applicability Configurations::
            string lineTemp3 = "";
            while ((lineTemp3 = srLAFile1.ReadLine()) != null)
            {
                string[] lineSplit3 = lineTemp3.Split('\t');
                LATCVsLA[lineSplit3[0]] = lineSplit3[1];
            }

            string lineTemp4 = "";
            while ((lineTemp4 = srLAFile1.ReadLine()) != null)
            {
                string[] lineSplit4 = lineTemp4.Split('\t');
                LARVsApp[lineSplit4[0]] = lineSplit4[1];
            }

            foreach (KeyValuePair<string, string> kvp in LATCVsLA)
            {
                //string[] krem = kvp2.Key.Split('#');
                //string[] valsfromtpg = kvp2.Value;
                if (LARVsApp.ContainsKey(kvp.Value))
                {
                    LATCVsLA[kvp.Key] = LARVsApp[kvp.Value];
                }
            }

            //CASplit(string inputband, string TC): will return 5 DL bands, 5 UL bands, and 5 BWs.

            // bandBWAll: BandSplit(BandCombo, TC#): In old MTP, BandCombo was in 168th position. In new MTP, BandCombo is now in 29th position.
            List<string> bandBWAll = BandSplit(ln[27], ln[ 3]);

            //Debug.Print(String.Join(",", bandBWAll));
            string bandCombo = "";
            int numDLBand = 5;
            for (int i = 0; i < numDLBand; i++)
            {
                bandCombo += "#" + bandBWAll[i];
            }
            bandCombo = bandCombo.Substring(1);
            //Debug.Print(bandCombo);

            // TC is in 3rd position. Same position in old and new versions.
            string TC = ln[3];
            string[] TCOctate = TC.Split('.');

            string TCSub = TCOctate[0];

            // Area in 0th position | OldPosition = NewPosition | specChNVsRAT says RAT: RF/RRM/PS for a given test configuration.
            string keyForArea = ln[1] + "#" + TCSub;
            //if (specChNVsRAT.ContainsKey(keyForArea))
            //    ln[ 0] = specChNVsRAT[keyForArea];

            ln[ 0] = Area(ln[ 1], TC);
            //Mode is in 2nd position | Mode(string spec, string bandCombo, Dictionary<string, string> configSpecRAT)
            ln[ 2] = Mode(ln[ 1], bandCombo, specChNVsRAT, ln[ 3], ln[ 0]);

            //FDD/TDD is in 4th position | FDDorTDD(string inB)
            ln[ 4] = FDDorTDD(bandBWAll[0]);

            //CS or PS info is in 8th position | Domain info: For 34.123-1. [To do:] Include 51.010 and 31.121. Get help from Stefan.
            if (ln[ 1].Contains("34.123"))
            {
                if (TCVsDomain.ContainsKey(ln[ 3]))
                {
                    ln[ 8] = TCVsDomain[ln[ 3]];
                }
            }

            // Band1, Band2, Band3, Band4, Band5 are in positions of: 9, 10, 11, 12, 13, respectively.
            for (int i = 0; i < numDLBand; i++)
            {
                ln[ 9 + i] = bandBWAll[i];
            }

            // BW1, BW2, BW3, BW4 are in positions of: 14, 15, 16, 17, respectively. For now, BW is of NO USE. No need to improve.
            for (int i = 0; i < numDLBand - 1; i++)
            {
                ln[ 14 + i] = bandBWAll[10 + i];
            }

            // RF/PROT = Test subcategory: RF/RRM/CTPS in 19th column
            if (specChNVsCat.ContainsKey(keyForArea))
                ln[ 18] = specChNVsCat[keyForArea];

            // Test Name = Chapter Name in 24th position| Included in New MTP 
            Debug.Print("keyofarea" + keyForArea);
            if (specChNVsChT.ContainsKey(keyForArea))
                ln[ 23] = specChNVsChT[keyForArea];

            // Description in 25th position | Included in New MTP
            Debug.Print("Adding TC: "+tpgval[0]+" band " + tpgval[2]);
            ln[ 24] = tpgval[1];
            // WI GCF
            ln[ 33] = tpgval[16];
            // RFT PTCRB
            if(tpgval[17]!=""&&tpgval[17]!=null)
                ln[ 35] = "RFT "+ tpgval[17];
            // GCF/PTCRB criteria
            string gcfcrit = tpgval[18];
            string ptcrbcrit = tpgval[19];
            // gcf wil be high priority. 
            if ((gcfcrit == "")||(gcfcrit==null))
            {
                //destination column = GCF/PTCRB Crit.
                ln[ 37] = ptcrbcrit;
            }else
            {
                ln[ 37] = gcfcrit;
            }

            // Release information:
            string PICS_G = tpgval[12];
            string PICS_P = tpgval[13];
            //string relG = releaseInfo(PICS_G);
            //string relP = releaseInfo(PICS_P);

            if (PICS_G != null && PICS_G != "")
            {
                ln[ 39] = releaseInfo(PICS_G);
            }
            else if (PICS_P != null && PICS_P != "")
            {
                ln[ 39] = releaseInfo(PICS_P);
            }
            else
            {
                ln[ 39] = "Not Found";
            }

            // GCF CAT in 42th position.
            ln[ 42] = tpgval[4];

            // GCF TP in 43rd position.
            ln[ 43] = tpgval[6];

            // PTCRB CAT in 44th position.
            ln[ 44] = tpgval[5];

            // PTCRB TP in 45th position.
            ln[ 45] = tpgval[7];


            // Limited Applicability Conditions:
            if (ln[ 1].Contains("51.010"))
            {
                if(LATCVsLA.ContainsKey(TC))
                    ln[ 64] = LATCVsLA[TC];
            }

            // Applicability in 65th position | New MTP:
            if (PICS_G != null && PICS_G != "")
            {
                ln[ 65] = appCondMod(PICS_G);
            }
            else if (PICS_P != null && PICS_P != "")
            {
                ln[ 65] = appCondMod(PICS_P);
            }
            else
            {
                ln[ 65] = "NA";
            }

            // Band Support in 66th position | New MTP:
            ln[ 66] = tpgval[14];

            // ICE Band Requirement in 67th position | New MTP:
            string iceBandTemp = tpgval[15];
            ln[ 67] = ICEBandResMod(iceBandTemp);
            //ln[ 67] = tpgval[15];

            // Environmental Condition in 69th position | New MTP | This column has been added
            ln[ 68] = tpgval[3];

            // Required band calculation: Insert Atiq bhai's function here to calculate the required band
            //ln[ 69] = "";
            // added before calling this function. 
            // update the spec for 51.010 only . 
            // split the spec into two different spec
            if((ln[1]== "3GPP TS 51.010-1")&&(ln[3].StartsWith("27.22")))
            {
                ln[1] = "3GPP TS 51.010-4";
            }

            // Final applicability:
            ln[ 70] = finalApp(ln[ 65], ln[ 66], ln[ 69]);
            //Debug.Print("After adding all columns");
            if (ln[68] == @"NC,TL/VL,TL/VH,TH/VL,TH/VH")
            {
                ln[68] = "NC";
                res.Add(ln);
                string[] ln2 = (string[]) ln.Clone();
                ln2[68] = "EXTR";
                ln2[3] += "_EXTR";
                res.Add(ln2);
               
            }
            else
            {
                res.Add(ln);
            }
        }


        public static string releaseInfo(string inStr)
        {
            string outStr = "";

            string[] outStrFull = inStr.Split(',');
            if (outStrFull.Count() > 1)
            {
                outStr = outStrFull[1];
            }else
            {
                if (inStr.Trim().ToLower().StartsWith("tc"))
                    outStr = inStr;
            }
            return outStr;
        }


        static void updateres(string[,] res, List<string> tpgval, int rownum)
        {
            string alteredidx = "";
            //tpgval string [] 
            rownum--;
            //8-
            // tpgval[0] = col 7
            //Debug.Print(string.Join("$",tpgval));
            //tcstatus-g
            if (res[rownum, 42] != tpgval[4])
                alteredidx += ",43";
            res[rownum, 42] = tpgval[4];
            //tcstatus-p
            if(res[rownum, 44] != tpgval[5])
                alteredidx += ",45";

            res[rownum, 44] = tpgval[5];
            //TP-G
            if (res[rownum, 43] != tpgval[6])
                alteredidx += ",44";
            res[rownum, 43] = tpgval[6];
            //TP-P
            if (res[rownum, 45] != tpgval[7])
                alteredidx += ",46";
            res[rownum, 45] = tpgval[7];
            //Applicability / picsstatus . need to show Y or N
            string picsstatus = "";
            string picsstatusg = tpgval[12];
            string picsstatusp = tpgval[13];
            picsstatus = ((picsstatusg == "") || (picsstatusg == null)) ? picsstatusp : picsstatusg;

            //Debug.Print("g "+picsstatusg + " P " + picsstatusp + " f " + picsstatus);
            //if ((picsstatus == "") || (picsstatus == null))
            //    picsstatus = "NA";
            //else
            picsstatus = ((picsstatus == "") || (picsstatus == null))?"NA":((picsstatus.Contains("TC: R")) ? "Y" : (picsstatus.Contains("TC: NR") ? "N" : "NA"));
            //Debug.Print(picsstatus);
            
            if (res[rownum, 65] != picsstatus)
                alteredidx += ",66";
            res[rownum, 65] = picsstatus;
            // add 66= band support,67= iceband support,68=env,69=requiredband
            // band suport from tpg 17 , iceband suport 18. env 6. 
            // add result should have required band. 
            // Band Support in 66th position | New MTP:
            res[rownum, 66] = tpgval[14];

            // ICE Band Requirement in 67th position | New MTP:
            string iceBandTemp = tpgval[15];
            res[rownum, 67] = ICEBandResMod(iceBandTemp);
            //res[rownum, 67] = tpgval[15];

            // Environmental Condition in 69th position | New MTP | This column has been added
            res[rownum, 68] = tpgval[3];

            // Required band calculation: Insert Atiq bhai's function here to calculate the required band
            //updated before already
            //res[rownum, 69] = "";

            // Final applicability:
            res[rownum, 70] = finalApp(res[rownum, 65], res[rownum, 66], res[rownum, 69]);
            //Debug.Print("After adding all columns");
            if (alteredidx != "")
            {
                alteredidx = alteredidx.Substring(1);
                res[rownum, 410] = "U";
                res[rownum, 411] = alteredidx;
            }


        }
  private static Dictionary<string, bool> rbdic = new Dictionary<string, bool>();
  private static Dictionary<string, bool> rbdicclone = new Dictionary<string, bool>();
  private static void reqBandRefine(Dictionary<string, List<string>> TCVsSuppStatBand, Dictionary<string, string> TCVsBA)
        {

            //ReadConfig rc_rb = new ReadConfig("rb.conf");
            //
            //List<string> rb_nho = rc_rb.getList("band4g");
            //List<string> rb_ho = rc_rb.getList("bandho");

            int counter = 0;
            string line;
            string[] hoband = null;
            Dictionary<string, string> rb_ho = new Dictionary<string, string>();
            Dictionary<string, string> rb_nho = new Dictionary<string, string>();
            Dictionary<string, string> rb_tdd = new Dictionary<string, string>();
            //string[] tlist;
            // Read the file and display it line by line.  
            StreamReader file = new StreamReader(@"rbpriority.csv");
            while ((line = file.ReadLine()) != null)
            {
                string k = "";
                string[] tlist = line.Split(',');
                //Debug.Print(tlist[2]);
                //Debug.Print(tlist[3]);
                if (counter == 1)
                {
                    hoband = tlist;
                    //Debug.Print(string.Join(",",tlist));
                }
                if (counter > 2)
                {
                    //Debug.Print(string.Join(",", hoband));

                    if ((tlist[2] != "") && (tlist[3] != ""))
                    {
                        k = string.Format("{0,0:0000}{1,0:0000}", int.Parse(tlist[2]), int.Parse(tlist[3]));
                        // k is zero padded number generated from 2nd and 3rd element. 
                        if (hoband.Contains(tlist[2]))
                        {
                            if (tlist[3] != "1000")
                            {
                                if (!rb_ho.ContainsKey(k))
                                {
                                    rb_ho.Add(k, tlist[0]);
                                    //Debug.Print(k + tlist[0]);
                                }
                            }
                        }
                        else
                        {
                            if (tlist[3] != "1000")
                            {
                                if (!rb_nho.ContainsKey(k))
                                {
                                    rb_nho.Add(k, tlist[0]);
                                    Debug.Print(k + tlist[0]);
                                }
                                if (tlist[2] == "15")
                                {
                                    if (!rb_tdd.ContainsKey(k))
                                        rb_tdd.Add(k, tlist[0]);
                                }
                            }
                        }

                    }

                }

                counter++;
            }
            file.Close();
            List<string> rb_hosort = new List<string>();
            List<string> rb_hok = rb_ho.Keys.ToList();
            rb_hok.Sort();
            foreach (string bandhoKey in rb_hok)
            {
                rb_hosort.Add(rb_ho[bandhoKey]);
            }
            List<string> rb_tddsort = new List<string>();
            List<string> rb_tddk = rb_tdd.Keys.ToList();
            rb_tddk.Sort();
            foreach (string bandhoKey in rb_tddk)
            {
                rb_tddsort.Add(rb_tdd[bandhoKey]);
            }
            List<string> rb_nhosort = new List<string>();
            List<string> rb_nhok = rb_nho.Keys.ToList();
            rb_nhok.Sort();
            foreach (string bandnhoKey in rb_nhok)
            {
                rb_nhosort.Add(rb_nho[bandnhoKey]);
            }

            Debug.Print("NON-HO Case Start:");
            foreach (string val1 in rb_nhosort)
            {
                Debug.Print(val1);
            }

            Debug.Print("HO Case Start:");
            foreach (string val2 in rb_hosort)
            {
                Debug.Print(val2);
            }



            //List<string> Band4G = new List<string> { "CA_4A-12A", "CA_2A-12A", "CA_4A-7A",
            //          "CA_4A-29A", "CA_2A-29A", "CA_4A-5A", "CA_4A-4A", "CA_2A-4A", "CA_2A-5A",
            //          "CA_2A-30A", "CA_5A-30A", "CA_4A-30A",
            //          "CA_30A-29A", "CA_12A-30A", "CA_4A-17A", "CA_2A-17A",
            //          "CA_5A-29A", "CA_7A-7A", "CA_66A-66A", "CA_66B", "CA_66C", "CA_66A-29A","E04", "E17", "E14", "E25", "E02", "E05", "E07", "E12", "E13", "E30", "U02", "U05","U04", "G1900", "G850"};
            //List<string> BandHO = new List<string> { "E04-U02", "E04-U05", "E04-U04", "E17-U02", "E17-U05", "E17-U04", "E14-U02", "E14-U05", "E14-U04", "E25-U02", "E25-U05", "E25-U04", "E02-U02", "E02-U05", "E02-U04", "E05-U02", "E05-U05", "E05-U04", "E07-U02", "E07-U05", "E07-U04", "E12-U02", "E12-U05", "E12-U04", "E13-U02", "E13-U05", "E13-U04", "E30-U02", "E30-U05", "E30-U04", "U02-E04", "U02-E17", "U02-E14", "U02-E25", "U02-E02", "U02-E05", "U02-E07", "U02-E12", "U02-E13", "U02-E30", "U05-E04", "U05-E17", "U05-E14", "U05-E25", "U05-E02", "U05-E05", "U05-E07", "U05-E12", "U05-E13", "U05-E30", "U04-E04", "U04-E17", "U04-E14", "U04-E25", "U04-E02", "U04-E05", "U04-E07", "U04-E12", "U04-E13", "U04-E30" };
            //List<string> BandHO = new List<string> { "E04-UII", "E04-UV", "E04-UIV", "E17-UII", "E17-UV", "E17-UIV", "E14-UII", "E14-UV", "E14-UIV", "E25-UII", "E25-UV", "E25-UIV", "E02-UII", "E02-UV", "E02-UIV", "E05-UII", "E05-UV", "E05-UIV", "E07-UII", "E07-UV", "E07-UIV", "E12-UII", "E12-UV", "E12-UIV", "E13-UII", "E13-UV", "E13-UIV", "E30-UII", "E30-UV", "E30-UIV", "UII-E04", "UII-E17", "UII-E14", "UII-E25", "UII-E02", "UII-E05", "UII-E07", "UII-E12", "UII-E13", "UII-E30", "UV-E04", "UV-E17", "UV-E14", "UV-E25", "UV-E02", "UV-E05", "UV-E07", "UV-E12", "UV-E13", "UV-E30", "UIV-E04", "UIV-E17", "UIV-E14", "UIV-E25", "UIV-E02", "UIV-E05", "UIV-E07", "UIV-E12", "UIV-E13", "UIV-E30" };

            foreach (string TCCur in TCVsSuppStatBand.Keys)
            {
                string TCBACur = "";
                if (TCVsBA.Keys.Contains(TCCur))
                {
                    TCBACur = TCVsBA[TCCur];
                    if ((TCBACur != null) && (TCBACur != ""))
                    {
                        if (TCBACur.ToLower() == "all")
                        {
                            foreach (string bandTemp in TCVsSuppStatBand[TCCur])
                            {
                                rbdic[TCCur + "#" + bandTemp] = true;
                            }
                        }
                        else if (TCBACur.ToLower() == "single")
                        {
                            foreach (string bandTemp in TCVsSuppStatBand[TCCur])
                            {
                                rbdic[TCCur + "#" + bandTemp] = false;
                            }
                            foreach (string bandToCheck in rb_nhosort)
                            {
                                if (TCVsSuppStatBand[TCCur].Contains(bandToCheck))
                                {
                                    rbdic[TCCur + "#" + bandToCheck] = true;
                                    break;
                                }
                            }
                            // This is to add TDD on band 41 test cases seperately. 
                            foreach (string bandToCheck in rb_tddsort)
                            {
                                if (TCVsSuppStatBand[TCCur].Contains(bandToCheck))
                                {
                                    rbdic[TCCur + "#" + bandToCheck] = true;
                                    break;
                                }
                            }
                        }
                        else if (TCBACur.ToLower() == "irat-single")
                        {
                            foreach (string bandTemp in TCVsSuppStatBand[TCCur])
                            {
                                rbdic[TCCur + "#" + bandTemp] = false;
                            }
                            foreach (string bandToCheck in rb_hosort)
                            {
                                if (TCVsSuppStatBand[TCCur].Contains(bandToCheck))
                                {
                                    rbdic[TCCur + "#" + bandToCheck] = true;
                                    break;
                                }
                            }
                        }
                    }
            
                }
                
            }
            //return outDict;
        }

        static void Main(string[] args)
        {

            //dummy


            //dummy

            string curdir = Directory.GetCurrentDirectory();
            string fullPathSource = "";
            string inputfilename = "";
            if (args.Length < 1)
            {
                Console.WriteLine("You have entered less number of argument");
                return;
            }else 
            {
                inputfilename = args[0];
            }
                //fullPathSource = Path.Combine(curdir, "combined_g3.67_t1_p5.33_t1_XMM7660V1.3_t1_CTPS.xlsx");
                fullPathSource = Path.Combine(curdir, inputfilename);
            if (!File.Exists(fullPathSource))
            {
                Console.WriteLine("the file doesnt exist."+fullPathSource);
                return;
            }
            Logging lg = new Logging("debug.txt",0);
            string targetfile= "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string curtime = DateTime.Now.ToString("yyyyMMddHHmmss"); 
                targetfile = appSettings["targetfile"];
                string patt = @"(.+)\.(.+)$";
                Match mc = Regex.Match(targetfile, patt);
                string ext = "";
                if (mc.Success)
                {
                    ext = mc.Groups[2].ToString();
                }

                if (File.Exists(targetfile))
                {
                    File.Copy(targetfile, curtime+"_MTP."+ext,true);
                }
                targetfile = curtime + "_MTP." + ext;
                configctps = appSettings["configctps"];
            }
            catch (Exception)
            {

                lg.err("Error while reading appconfig file.");
            }
            StreamWriter sw = new StreamWriter(@"data.log", false);
            
            ReadConfig rc = new ReadConfig("specmap.conf");
            //string s = rc.getvalue("34.122");
            //Debug.Print(s);
            //string fullPathtarget = Path.Combine(curdir, "CT_PS_Test_Plan_ShortDebug.xlsx");
            //string fullPathtarget = Path.Combine(curdir, "MTP_Phase2.xlsb");
            string fullPathtarget = Path.Combine(curdir,targetfile);
            Dictionary<string, Dictionary<string, List<string>>> alldatafromtpg = new Dictionary<string, Dictionary<string, List<string>>>();
            Dictionary<string, Dictionary<string, string>> alldatafromtpgba = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, List<string>>> alldatafromtpgrb = new Dictionary<string, Dictionary<string, List<string>>>();
            Dictionary<string, Dictionary<string, bool>> rbboolval = new Dictionary<string, Dictionary<string, bool>>();

                        
            //lg.inf("TPG File: " + fullPathSource);

            XLRdWr xlrw = new XLRdWr(fullPathSource);
            xlrw.openfile();
            List<string> allsheet = xlrw.getsheets();
            //string sheetname = "tty";
            //Dictionary<FourVal, string[]> alldatafromtpg = new Dictionary<FourVal, string[]>();
            lg.inf("Reading TPG"+ fullPathSource);
            foreach (string sheetname in rc.getkeys())
            {
                if (!allsheet.Contains(sheetname))
                {
                    continue;
                }
                xlrw.setsheet(sheetname);
                Dictionary<string, List<string>> datafromtpg = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> datarb = new Dictionary<string, List<string>>();

                Dictionary<string, string> databa = new Dictionary<string, string>();
                string spec = rc.getvalue(sheetname);

                alldatafromtpg.Add(spec, datafromtpg);
                alldatafromtpgba.Add(spec, databa);
                alldatafromtpgrb.Add(spec, datarb);
                Debug.Print("spec: "+spec + " sheetName: "+sheetname);
                int lr = xlrw.getlastrow();
                int lc = xlrw.getlastcol();
                //xlrw.setrangeval("C1", res);
                string[,] res = xlrw.getrange("C1", lr, lc);
                int r = res.GetLength(0);
                int c = res.GetLength(1);
                Debug.Print("spec: "+spec+ " R: " + r + " C: " + c);
                lg.inf("Reading TPG: Spec = " +spec);
                sw.WriteLine("Reading TPG: Spec = " + spec);
                for (int i = 2; i <= r; i++)
                {
                    //Debug.Print(i.ToString());
                    // col c = 0  and continue
                    string tc = res[i - 1, 0];
                    string band = res[i - 1, 2];
                    string env = res[i - 1, 3];
                    
                    
                    string tcsg = res[i - 1, 4];
                    string tcsp = res[i - 1, 5];
                    // correction needed
                    //bool tcstf = ((tcsg == "A")||(tcsg == "P")|| (tcsg == "B")|| (tcsp == "A") || (tcsp == "P") || (tcsp == "B")) ? true : false;
                    bool tcstf = ((tcsg == "A") || (tcsg == "B") || (tcsp == "A") || (tcsp == "E") || (tcsp == "B")) ? true : false;
                    string bs = res[i - 1, 14];
                    string bag = res[i - 1, 18];
                    string bap = res[i - 1, 19];
                    string ba = ((bag == "")||(bag==null)) ? bap : bag;
                    List<string> tcdata = null; 
                    if (datarb.ContainsKey(tc))
                    {
                        tcdata = datarb[tc];
                    }
                    else
                    {
                        tcdata = new List<string>();
                        datarb.Add(tc,tcdata);
                        databa.Add(tc,ba);
                    }
                    if (tcstf && bs.ToLower() == "s")
                    {
                        //List<string> data = new List<string>() { band,ba};
                        tcdata.Add(band);
                    }
                    List<string> var = new List<string>(new string[c - 2]);
                    for (int l = 0; l < c-2;l++)
                    {
                        var[l] = res[i - 1,  l];
                    }
                    //Description:
                    //var[12] = res[i - 1, 1];
                    ////Environment:
                    //var[13] = res[i - 1, 3];
                    ////wi
                    //var[13] = res[i - 1, 3];
                    //
                    ////rft
                    //var[13] = res[i - 1, 3];
                    ////
                    //var[13] = res[i - 1, 3];
                    ////
                    //var[13] = res[i - 1, 3];
                    //
                    //string band = res[i - 1, 2];
                    //FourVal k = new FourVal(spec, tc, band, env);
                    string k = tc + "#"+  band;
                    if (!datafromtpg.ContainsKey(k))
                    {
                        datafromtpg.Add(k, var);
                    }
                    else
                    {
                        sw.WriteLine("\t" + i.ToString()+"DUP key: "+k);
                    }
            
            
                }
                reqBandRefine(datarb, databa); // this function will fill rbdic dictionary.
                rbdicclone = new Dictionary<string, bool>(rbdic);

                //rbdicclone = rbdic
                
                rbboolval.Add(spec, rbdicclone);
                rbdic.Clear();
            }

            //foreach (KeyValuePair<string, Dictionary<string, string[]>> kvp in alldatafromtpg)
            //{
            //    string spec = kvp.Key;
            //    Debug.Print("Spec: " + spec);
            //    foreach (KeyValuePair<string, string[]> kvp2 in kvp.Value)
            //    {
            //        Debug.Print("key: " + kvp2.Key.ToString() + " val: " + string.Join("|", kvp2.Value));
            //    }
            //}

            lg.inf("Reading MTP File: " + fullPathtarget);
            Debug.Print(fullPathtarget);
            XLRdWr xlrw2 = new XLRdWr(fullPathtarget);
            xlrw2.openfile();
            xlrw2.setsheet(1);
            int lr2 = xlrw2.getlastrow();
            int lc2 = xlrw2.getlastcol();

            // create loop and read chunk by chunk 
            int sz = 1000;  
            int firstrow = 3;
            int lr2_current = sz + firstrow - 1;
            int loopvar = 1;
            string[,] res2=null;
            //int i = 3; 

            do
            {
                if (sz + firstrow > lr2)
                {
                    sz = lr2 - firstrow+1;
                }
               
                res2 = xlrw2.getrange("A" + firstrow, sz, lc2);
                int r2 = res2.GetLength(0);
                int c2 = res2.GetLength(1);
                Debug.Print("R: " + r2 + "C: " + c2);
                for (int i2 = 1; i2 <= r2; i2++)
                //for (int i = 19400; i <= 19500; i++)
                {
                    //Debug.Print("Benchmark_1");
                    int[] bandbwidx = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 6, 28 };
                    Util.bandCombine(res2, i2, bandbwidx);
                    //Debug.Print("Benchmark_2");
                    //Debug.Print(res2[i-1,168]);
                    //int[] bandindex = { 10, 11, 12, 13, 14,15,16,17,18,19 };
                    //string ba = getband(res2,i,bandindex);
                    int[] keyindex = { 4, 28 };
                    string spec = res2[i2 - 1, 1];
                    string key = getkey(res2, i2, keyindex);
                    //Debug.Print("Line: " + i + " Key: " + key+"spec: "+ res2[i - 1, 1] + "band: " + res2[i - 1, 9]);
                    if ((spec!=null)&&(alldatafromtpg.ContainsKey(spec)))
                    {
                        Dictionary<string, string> databa = alldatafromtpgba[spec];
                        Dictionary<string, List<string>> datarb = alldatafromtpgrb[spec];

                        //Debug.Print("Benchmark_3");
                        res2[i2 - 1, 410] = "D";
                        Dictionary<string, List<string>> tpgdata = alldatafromtpg[spec];
                        string kf;
                        if (tpgdata.ContainsKey(key))
                        {
                            kf = "y";
                            List<string> tpgvals = (List<string>)(tpgdata[key]);
                            string  rb = "N";

                            try
                            {
                                rb = (rbboolval[spec][key]) ?"Y":"N";
                            }
                            catch (Exception)
                            {
                                lg.err("NKF rbbool. spec: "+spec+" key: "+key);
                                
                            }
                            res2[i2 - 1, 69] = rb;
                            updateres(res2, tpgvals, i2);
                            tpgdata.Remove(key);
                        }
                        else
                        {
                            kf = "n";
                            sw.WriteLine(i2.ToString() + "\tKYNF");
                        }
                        Debug.Print(" 1 spec: " + spec + " no: " + tpgdata.Count.ToString() + " key found: " + kf);
                        //Debug.Print("Benchmark_4");

                    }
                    else
                    {
                        sw.WriteLine(i2.ToString() + "\tSPNF");
                    }
                    //ignore first couple of lines. while writing start from c1;
                }
                //Debug.Print("Benchmark_5");

                xlrw2.setrangeval("A" + firstrow.ToString(), res2);
                //xlrw2.save();
                if (sz + firstrow >= lr2)
                {
                    break;
                }
                lr2_current = sz * (loopvar + 1) + 3 - 1;
                firstrow = sz * loopvar + 3;
                lr2_current = (lr2_current > lr2) ? lr2 : lr2_current - 1;
                //Debug.Print("lr2: "+lr2+" cur: "+lr2_current );
                loopvar++;
            } while (true);

            // end of loop .. 
            int totalnl = 0;
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in alldatafromtpg)
            {
                totalnl+=kvp.Value.Count;
            }
            //string[,] restoadd = new string[totalnl, lc2];
            List<string[]> restoadd = new List<string[]>();
            int nlcount = 1; 
            foreach (KeyValuePair<string, Dictionary<string,List<string>>> kvp in alldatafromtpg)
            {
                foreach(KeyValuePair<string,List<string>>kvp2 in kvp.Value)
                {
                    string[] ln = new string[lc2];
                    string[] krem = kvp2.Key.Split('#');// key
                    List<string> tpgvals = kvp2.Value;
                    ln[1] = kvp.Key;
                    ln[ 3] = krem[0];
                    ln[ 27] = krem[1];
                    ln[ 410] = "A";

//                    restoadd[nlcount - 1, 1] = kvp.Key; // spec
//                    restoadd[nlcount - 1, 3] = krem[0];
//                    restoadd[nlcount - 1, 27] = krem[1];
//                    restoadd[nlcount - 1, 408] = "A";

                    string rb = "N";

                    try
                    {
                        rb = (rbboolval[kvp.Key][kvp2.Key]) ? "Y" : "N";
                    }
                    catch (Exception)
                    {
                        lg.err("NKF rbbool. spec: " + kvp.Key + " key: " + kvp2.Key);

                    }
                    //restoadd[nlcount - 1, 69] = rb;
                    //restoadd[nlcount - 1, 408] = "A";
                    ln[69]= rb;
                    ln[410] = "A";
                    

                    addres(restoadd,ln, tpgvals,nlcount);
                    nlcount++;
                }
            }
            string[,] restoaddarr = new string[restoadd.Count(), lc2];
            int iarr = 0;
            int jarr = 0;
            foreach (string[] sarr in restoadd)
            {
                foreach(string ss in sarr)
                {
                    restoaddarr[iarr, jarr] = ss;
                    jarr++;
                }
                jarr = 0;
                iarr++;
            }
            //xlrw2.setrangeval("A1",res2);
            //xlrw2.save();
            //Debug.Print((lr2+1).ToString());
            xlrw2.setrangeval("A"+(lr2+1).ToString(),restoaddarr);
            xlrw2.save();
            xlrw.closefile();
            xlrw2.closefile();
            sw.Close();
            
        }

        public static string Area(string spec, string TC)
        {
            string outVal = "";
            string[] TCNumList = TC.Split('.');

            //RF:
            if (spec == "3GPP TS 36.521-1")
            {
                outVal = "RF";
            }
            else if (spec == "3GPP TS 34.121-1" && TCNumList[0] != "8")
            {
                outVal = "RF";
            }
            else if (spec == "3GPP TS 51.010-1" && (TCNumList[0] == "12" || TCNumList[0] != "13" || TCNumList[0] != "14" || TCNumList[0] != "16" || TCNumList[0] != "18" || TCNumList[0] != "21" || TCNumList[0] != "22"))
            {
                outVal = "RF";
            }

            //RRM:
            if (spec == "3GPP TS 36.521-3")
            {
                outVal = "RRM";
            }
            else if (spec == "3GPP TS 34.122")
            {
                outVal = "RRM";
            }
            else if (spec == "3GPP TS 34.121-1" && TCNumList[0] == "8")
            {
                outVal = "RRM";
            }

            //PS:
            if (spec == "PTCRB Bearer Agnostic AT-Command TS")
            {
                outVal = "PTCRB";
            }
            else if (spec == "PTCRB Bearer-Agnostic TTY TS")
            {
                outVal = "TTY";
            }
            else if (spec == "3GPP TR 37.901")
            {
                outVal = "TP";
            }
            else if (spec == "3GPP TS 31.124")
            {
                outVal = "SIM";
            }
            else if (spec == "ETSI TS 102 230")
            {
                outVal = "SIM";
            }
            else if (spec == "3GPP TS 34.171")
            {
                outVal = "GPS";
            }
            else if (spec == "3GPP TS 34.229-1")
            {
                outVal = "IMS";
            }
            else if (spec == "3GPP TS 37.571-1")
            {
                outVal = "GPS";
            }
            else if (spec == "3GPP TS 31.121")
            {
                outVal = "SIM";
            }
            else if (spec == "3GPP TS 37.571-2")
            {
                outVal = "GPS";
            }
            // Multiple criteria for 51.010-1:
            else if (spec == "3GPP TS 51.010-1" && (TCNumList[0] == "30"))
            {
                outVal = "AUD";
            }
            else if (spec == "3GPP TS 51.010-1" && (TCNumList[0] == "70"))
            {
                outVal = "GPS";
            }
            else if (spec == "3GPP TS 51.010-1" && (TCNumList[0] == "27"))
            {
                outVal = "SIM";
            }
            else if (spec == "3GPP TS 51.010-1" && (TCNumList[0] == "81" || TCNumList[0] == "82" || TCNumList[0] == "83" || TCNumList[0] == "84"))
            {
                outVal = "GAN";
            }
            else if (spec == "3GPP TS 51.010-1")
            {
                outVal = "PROT";
            }
            else if (spec == "3GPP TS 34.123-1" && (TCNumList[0] == "17" && TCNumList[1] == "2"))
            {
                outVal = "GPS";
            }
            else if (spec == "3GPP TS 34.123-1")
            {
                outVal = "PROT";
            }
            else if (spec == "3GPP TS 36.523-1" && (TCNumList[0] == "11" && TCNumList[1] == "2"))
            {
                outVal = "IMS";
            }
            else if (spec == "3GPP TS 36.523-1" && (TC == "10.4.2" || TC == "8.1.2.11" || TC == "8.1.2.12" || TC == "9.2.1.1.28" || TC == "9.2.1.1.28a" || TC == "9.2.1.1.29"))
            {
                outVal = "IMS";
            }
            else if (spec == "3GPP TS 36.523-1")
            {
                outVal = "PROT";
            }
            return outVal;
        }

        public static string Mode(string spec, string bandCombo, Dictionary<string, string> configSpecRAT, string TC, string area)
        {
            string outVal = "";
            string[] bandList = bandCombo.Split('#');

            string[] TC0State = TC.Split('.');
            string TCSub = TC0State[0];
            string keyForArea = spec + "#" + TCSub;

            string RAT = "";
            if (configSpecRAT.ContainsKey(keyForArea))
                RAT = configSpecRAT[keyForArea];
            
            string str1 = "", str2 = "", str3 = "";

            str1 = RAT;

            if (RAT == "4G")
            {
                string bandToCheckStr = bandList[0];

                string patt = @"[EUV]?(\d+)$";
                Match mc = Regex.Match(bandToCheckStr, patt);
                if (mc.Success)
                {
                    int bandToCheck = Int32.Parse(mc.Groups[1].ToString());
                    if (bandToCheck >= 33 && bandToCheck <= 48)
                    {
                        str2 = "T_";
                    }
                    else
                    {
                        str2 = "F_";
                    }
                }
                else
                {
                    str2 = "M_";
                }
            }
            else if (RAT == "3G")
            {
                string patt3G = @"U([AEF])";
                Match mc3G = Regex.Match(bandList[0], patt3G);
                if (mc3G.Success)
                {
                    //string TDD3GBands = mc3G.Groups[1].ToString();
                    string TDD3GBands = mc3G.Groups[1].ToString();
                    if (TDD3GBands == "A" || TDD3GBands == "E" || TDD3GBands == "F")
                        str2 = "T_";
                }
                else
                {
                    str2 = "F_";
                }
            }
            else if (RAT == "2G")
            {
                str1 = "2G";
                str2 = "_";
            }
            //Debug.Print("Before: "+ RAT);
            if (RAT != "2G")
            {
               // Debug.Print("RAT Not 2G Info: " + bandList.Length);
                string b1Temp = bandList[0];
                string b1 = "";
                if (b1Temp != "")
                    b1 = b1Temp.Substring(0, 1);

                //string b1 = b1Temp.Substring(0, 1);

                string b2Temp = bandList[1];
                string b2 = "";
                if (b2Temp != "")
                    b2 = b2Temp.Substring(0, 1);

                string b3Temp = bandList[2];
                string b3 = "";
                if (b3Temp != "")
                    b3 = b3Temp.Substring(0, 1);

                int bandNELength = arrayNELength(bandList);

                if (bandNELength == 1 || b1Temp.Contains("M"))
                {
                    str3 = "SM";
                    //Debug.Print("str3 in cond 1: " + str3);
                }
                else if (bandNELength == 2)
                {
                    if (b1 != b2)
                    {
                        str3 = "DM";
                    }
                    else
                    {
                        str3 = "SM";
                    }
                }
                else if (bandNELength == 3)
                {
                    // Address DM and SM in this logic
                    str3 = "TM";
                    List<string> checkList = new List<string>() { b1, b2, b3 };
                    int idxE = 0;
                    foreach (string ch in checkList)
                    {
                        if (ch == "E")
                        {
                            idxE += 1;
                        }
                    }
                    int idxU = 0;
                    foreach (string ch in checkList)
                    {
                        if (ch == "U")
                        {
                            idxU += 1;
                        }
                    }
                    int idxG = 0;
                    foreach (string ch in checkList)
                    {
                        if (ch == "G")
                        {
                            idxG += 1;
                        }
                    }

                    if (idxE == 3 || idxU == 3 || idxG == 3)
                    {
                        str3 = "SM";
                    }
                    else if (((idxE == 2) && (idxU == 1)) || ((idxE == 1) && (idxU == 2)) || ((idxE == 2) && (idxG == 1)) || ((idxE == 1) && (idxG == 2)) || ((idxG == 2) && (idxU == 1)) || ((idxG == 1) && (idxU == 2)))
                    {
                        str3 = "DM";
                    }
                    else if ((idxE == 1) && (idxU == 1) && (idxG == 1))
                    {
                        str3 = "TM";
                    }
                }
                //Debug.Print("str3 after all cond: " + str3);
            }
            else
            {
                // Write Code for GSM, TTY, GPRS, EGPRS, AGPS, GAN
                str3 = "";
                if (area == "TTY")
                {
                    str3 = "TTY";
                }
                else if (area == "GPS")
                {
                    str3 = "AGPS";
                }
                else if (area == "GAN")
                {
                    str3 = "GAN";
                }
                else if ((TC0State[0] == "20" && TC0State[1] == "22") || (TC0State[0] == "34" && TC0State[1] == "4") || (TC0State[0] == "15" && TC0State[1] == "6") || (TC0State[0] == "15" && TC0State[1] == "9") || TC0State[0] == "41" || TC0State[0] == "42" || TC0State[0] == "43" || TC0State[0] == "44" || TC0State[0] == "45" || TC0State[0] == "46" || TC0State[0] == "47")
                {
                    str3 = "GPRS";
                }
                else if ((TC0State[0] == "15" && TC0State[1] == "8") || (TC0State[0] == "58" && TC0State[1] == "a") || (TC0State[0] == "58" && TC0State[1] == "b") || TC0State[0] == "51" || TC0State[0] == "52" || TC0State[0] == "53" || TC0State[0] == "57")
                {
                    str3 = "EGPRS";
                }
                else
                {
                    str3 = "GSM";
                }
            }
            if (bandList[0] == "BI" || bandList[0] == "BA")
            {
                outVal = "RI_SM";
            }
            else if (bandList[0] == "NI")
            {
                outVal = "NI";
            }
            outVal = str1 + str2 + str3;

            if (spec == "ETSI TS 102 230")
            {
                outVal = "NI";
            }
            else if (spec == "PTCRB Bearer Agnostic AT-Command TS")
            {
                outVal = "RI-SM";
            }
            else if (spec == "PTCRB Bearer-Agnostic TTY TS")
            {
                if (bandList[0].Contains("G"))
                {
                    outVal = "2G_TTY";
                }
                else if (bandList[0].Contains("U"))
                {
                    outVal = "3G_SM";
                }
            }

            return outVal;
        }

        public static string appCondMod(string inStr)
        {
            string outStr = "";
            string outStrTemp = "";
            string[] outStrFull = inStr.Split(',');
            string patt = @"TC\:\s+?([a-zA-Z\/]+)";
            Match mc = Regex.Match(inStr, patt);
            if (mc.Success)
            {
                outStrTemp = mc.Groups[1].ToString();
                outStr = appCondModNormalize(outStrTemp);
            }
            return outStr;
        }

        public static string appCondModNormalize(string inStr)
        {
            string outStr = "";
            if (inStr == "R" || inStr == "A")
            {
                outStr = "Y";
            }
            else if (inStr == "NR")
            {
                outStr = "N";
            }
            else if (inStr.ToLower() == "rel")
            {
                outStr = "Not Found";
            }
            return outStr;
        }

        public static string ICEBandResMod(string inStr)
        {
            string outStr = "";
            if (inStr == "S")
            {
                outStr = "R";
            }
            else if (inStr == "NS")
            {
                outStr = "NR";
            }
            return outStr;
        }

        public static string finalApp(string PICSApp, string bandSupp, string reqBand)
        {
            string outStr = "";

            if (PICSApp == "Y" && bandSupp == "S" && reqBand == "Y")
            {
                outStr = "Y";
            }
            else
            {
                outStr = "N";
            }
            return outStr;
        }

        private static int arrayNELength(string[] inL)
        {
            int count = 0;
            for (int i = 0; i < inL.Length; i++)
            {
                if (inL[i] == null || inL[i] == "")
                {
                    
                }
                else
                {
                    count += 1;
                }
            }
            return count;
        }

        public static string FDDorTDD(string inB)
        {
            string outB = "";
            string patt = @"[EUG]?(\d+)$";
            Match mc = Regex.Match(inB, patt);
            if (mc.Success)
            {
                int inBInt = Int32.Parse(mc.Groups[1].ToString());
                outB = (inBInt >= 33 && inBInt <= 48) ? "TDD" : "FDD";
            }
            else
            {
                outB = "M2M";
                if (inB == "BI")
                {
                    outB = "BI";
                }
                else if (inB == "NI")
                {
                    outB = "NI";
                }
                else if (inB == "BA")
                {
                    outB = "BA";
                }
                else if (inB == "GAN")
                {
                    outB = "GAN";
                }
            }
            return outB;
        }

        public static List<string> BandSplit(string inputband, string TC)
        {
            List<string> allBands = new List<string>() { "", "", "", "", "", "", "", "", "", "" };
            List<string> allBWs = new List<string>() { "", "", "", "", "" };

            List<string> allBandsBWs = new List<string>();

            // configDict has to come from a config file.
            Dictionary<string, string> configDict = new Dictionary<string, string>();

            string debugVal = "";

            if (inputband == null)
            {
                debugVal = "Null";
                //return allBands;
                //return allBWs;
                allBandsBWs.AddRange(allBands);
                allBandsBWs.AddRange(allBWs);
                //return allBandsBWs;
            }
            else if (inputband.Trim() == "")
            {
                debugVal = "NA";
                //return allBands;
                //return allBWs;
                allBandsBWs.AddRange(allBands);
                allBandsBWs.AddRange(allBWs);
                //return allBandsBWs;
            }
            else if (inputband.Trim().StartsWith("CA"))
            {
                debugVal = "CA";
                string bpatt = @"CA_(\w+)\-?(\w+)?\-?(\w+)?\-?(\w+)?\-?(\w+)?";
                Match bc = Regex.Match(inputband, bpatt);
                string B1 = "", B2 = "", B3 = "", B4 = "", B5 = "";
                int i = 0;
                if (bc.Success)
                {
                    B1 = bc.Groups[1].ToString();
                    string res1 = CACCCheck(B1);
                    string resBW1 = CABWCheck(configDict, B1, TC);
                    allBands[i] = res1;
                    allBWs[i] = resBW1;
                    i++;

                    if (bc.Groups[2].Success)
                    {
                        B2 = bc.Groups[2].ToString();
                        string res2 = CACCCheck(B2);
                        string resBW2 = CABWCheck(configDict, B2, TC);
                        allBands[i] = res2;
                        allBWs[i] = resBW2;
                        i++;
                    }
                    if (bc.Groups[3].Success)
                    {
                        B3 = bc.Groups[3].ToString();
                        string res3 = CACCCheck(B3);
                        string resBW3 = CABWCheck(configDict, B3, TC);
                        allBands[i] = res3;
                        allBWs[i] = resBW3;
                        i++;
                    }
                    if (bc.Groups[4].Success)
                    {
                        B4 = bc.Groups[4].ToString();
                        string res4 = CACCCheck(B4);
                        string resBW4 = CABWCheck(configDict, B4, TC);
                        allBands[i] = res4;
                        allBWs[i] = resBW4;
                        i++;
                    }
                    if (bc.Groups[5].Success)
                    {
                        B5 = bc.Groups[5].ToString();
                        string res5 = CACCCheck(B5);
                        string resBW5 = CABWCheck(configDict, B5, TC);
                        allBands[i] = res5;
                        allBWs[i] = resBW5;
                        i++;
                    }

                }
                allBandsBWs.AddRange(allBands);
                allBandsBWs.AddRange(allBWs);
                //return allBandsBWs;
            }
            else
            {
                string pattMTC = @"\d+M$";
                Match mcMTC = Regex.Match(inputband, pattMTC);
                if (mcMTC.Success)
                {
                    debugVal = "M2M";
                    //mcTemp = true;
                    allBands[0] = inputband;
                    //Debug.Print("After Regex:" + allBands[0]);
                }
                else
                {
                    debugVal = "REST";
                    string[] ba = inputband.Split('-');
                    int i = 0;
                    foreach (string b in ba)
                    {
                        allBands[i] = b.Trim();
                        i++;
                    }
                }
                allBandsBWs.AddRange(allBands);
                allBandsBWs.AddRange(allBWs);
            }
            //Debug.Print("Debug BandAll Func: " + inputband + " : " + debugVal);
            return allBandsBWs;
        }

        private static string CACCCheck(string CACC)
        {
            string outBands = "";

            string bpatt = @"(\w+)([a-zA-Z]+)";
            Match bc = Regex.Match(CACC, bpatt);

            string CCLetter, CCValTemp, CCVal;
            if (bc.Success)
            {
                CCLetter = bc.Groups[2].ToString();
                CCValTemp = bc.Groups[1].ToString();
                if (CCValTemp.Length == 1)
                {
                    CCVal = "E0" + CCValTemp;
                }
                else
                {
                    CCVal = "E" + CCValTemp;
                }
                // Irrespective of any CA combos, it will be 1 single band inclusion.
                outBands = CCVal;
            }
            return outBands;
        }

        private static string CABWCheck(Dictionary<string, string> TCBandBW, string CACC, string TC = "")
        {
            //Sample entry of configVal: 10-5#20-20#20#20#20
            //Sample entry of TCBandBW: TCBandBW[TC#Band] = BW
            string outVal = "";
            string bpatt = @"(\w+)([a-zA-Z]+)";
            Match bc = Regex.Match(CACC, bpatt);

            string CCLetter, CCValTemp, CCVal;
            if (bc.Success)
            {
                CCLetter = bc.Groups[2].ToString();
                CCValTemp = bc.Groups[1].ToString();
                if (CCValTemp.Length == 1)
                {
                    CCVal = "E0" + CCValTemp;
                }
                else
                {
                    CCVal = "E" + CCValTemp;
                }

                if (TCBandBW.ContainsKey(TC + "#" + CACC))
                {
                    outVal = TCBandBW[TC + "#" + CACC];
                }
            }

            return outVal;
        }

        //Extra Function:
  


    }
}
