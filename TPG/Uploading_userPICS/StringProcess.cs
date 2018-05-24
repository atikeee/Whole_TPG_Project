using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml;
using Utility;
namespace Parse_Pixit_Table
{
    public class StringProcess
    {

        public static Logging lgstr;
        public static _Dictionary<string, string> PICSmnemonicDic; 

        public static bool evalBooleanExpr(string strinput)
        {
            int count1 = strinput.Count(f => f == '(');
            int count2 = strinput.Count(f => f == ')');
            if (count1 == count2 + 1)
            {
                strinput =  strinput+')';
            }
            else if (count1== count2 - 1)
            {
                strinput = '(' + strinput;

            }else if(count1!=count2)
            {
                lgstr.cri("{evalBooleanExpr}"+ String.Format("Bracket mismatch more than 1 {0} ", strinput));
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
        /// This function will be used to get value from a dictionary. no key found treated as False. 
        /// </summary>
        /// <param name="a">
        /// this is the string will be searched in the dictionary
        /// </param>
        /// <param name="b">
        /// The dictionary
        /// </param>
        /// <returns></returns>
        public static string convertBoolHelper(string a, Dictionary<string, string> b)
        {
            string c;
            if (b.ContainsKey(a))
            {
                c = b[a];
            }
            else
            {
                c = "False";
                lgstr.war("{convertBoolHelper} KeyNotFound " + "-> " + a);
            }

            return c;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PICSoutput">
        /// _Dictionary that contains 
        /// itemno/pnemonic vs true false. 
        /// </param>
        /// <param name="strinput"></param>
        /// <returns></returns>
        public static string convertToBoolExpr(_Dictionary<string, string> PICSoutput, string strinput)
        {
            string strop = strinput;
            //string pattern = @"\b\PICSoutputd+#[AEG]\.\d[\d\-\/\.a-z]+\b";
            string pattern = @"\d+#\b[AEDGX]\.\d[\d\-\/\.a-z]+\b";
            Regex itemRegex = new Regex(pattern, RegexOptions.IgnoreCase);
            strop = itemRegex.Replace(strinput, m => convertBoolHelper(m.ToString(), PICSoutput));
            lgstr.inf("{convertToBoolExpr} " + strinput + " => " + strop);
            return strop;
        }
        public static string convertToBoolExprMnemonic(_Dictionary<string, string> PICSoutput, string strinput)
        {
            string strop = strinput;
            //string pattern = @"\b\PICSoutputd+#[AEG]\.\d[\d\-\/\.a-z]+\b";
            string pattern = @"\d+#\b[ADEGX]\.\d[\d\-\/\.a-z]+\b";
            Regex itemRegex = new Regex(pattern, RegexOptions.IgnoreCase);
            strop = itemRegex.Replace(strinput, m => convertBoolHelper(m.ToString(), PICSoutput));
            lgstr.inf("{convertToBoolExprMnemonic} " + strinput + " => " + strop);
            return strop;
        }

        /// <summary>
        /// this will take a pics item as input and generate true false statement. 
        /// 
        /// </summary>
        /// <param name="picsitem"></param>
        /// <param name="pics_support">
        /// (NOT(7#A.4.3-4a/1) AND 7#A.4.1-1/1 AND 7#A.4.3-3a/1) 
        /// </param>
        /// <returns>
        /// (NOT(False) AND True AND True) 
        /// </returns>
        public static string logicstrProcess(string picsitem, _Dictionary<string, string> pics_support)
        {
            string dbg = " In: " + picsitem;
            string boolstr = picsitem;
            boolstr = convertToBoolExpr(pics_support, picsitem);
            dbg += "\tBoolExpr: " + boolstr;
            string boolstrverify = boolstr.Replace("AND", "").Replace("OR", "").Replace("NOT", "").Replace("True", "").Replace("False", "");
            Match m = Regex.Match(boolstrverify, @"\w+");
            if (!(m.Success))
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

       // public static string condSimple(string condVal, _Dictionary<string, string> pics_support)
       // {
       //     // this method must return true or false only. 
       //     string boolstr = condVal;
       //     string strPatt = @"if([(\bA\.\S+|\bB\.\S+|\bD\.\S+|\bG\.\S+|\bX\.\S+|AND|OR|NOT|\s+)]+)then\s+[roam]\s+else\s+n\/a";
       //     Match matchPartGr = Regex.Match(condVal, strPatt, RegexOptions.IgnoreCase);
       //     if (matchPartGr.Success)
       //     {
       //         boolstr = logicstrProcess(matchPartGr.Groups[1].ToString(), pics_support);
       //     }
       //     else
       //     {
       //         lgstr.err(" {condSimple} LogicalExprError: " + condVal);
       //     }
       //     lgstr.inf(" {condSimple} " + condVal + " => " + boolstr);
       //     return boolstr;
       // }

       

        public static string condCompl(string cstring, _Dictionary<string, string> c_logic_map, _Dictionary<string, string> pics_support)
        {
            //update c_logic map
            //
            string logicstring = c_logic_map[cstring];
            //lgstr.deb("\t{condCompl} eval start for  " + String.Format("[{0}] -> {1}", cstring,logicstring));
            string outstr = "";
            string condResRecur = "";
            string condResRecur2 = "";
            //Debug.Print(cstring);
            //Debug.Print("xxxx: " + logicstring);

            if (logicstring.ToLower() == "true")
            {
                outstr = "True";
            }
            else if (logicstring.ToLower() == "false")
            {
                outstr = "False";
            }
            else if ((logicstring.Trim().ToLower() == "void") || (logicstring.Trim().ToLower() == @"n/a") || (logicstring.Trim().ToLower().Contains("ffs")))
            {
             //   lgstr.inf("Void Information: " + logicstring);
                outstr = "False";
            }
            else
            {
                var regex = new Regex(@"(and|or|not)", RegexOptions.IgnoreCase);
                logicstring = regex.Replace(logicstring, m => m.ToString().ToUpper());
                //simple pattern need to correct
                string pattSimple = @"^if(?:(?:\d+#\S+)|AND|OR|NOT|\(|\)|\s)+then\s+[RAM]\s+else\s+n\/a";
                Match mcSimple = Regex.Match(logicstring, pattSimple, RegexOptions.IgnoreCase);

                string pattMnemonic = @"if((?:(?:O_\S+)|AND|OR|NOT|\(|\)|\s)+)then";
                Match mcMnemonic = Regex.Match(logicstring, pattMnemonic, RegexOptions.IgnoreCase);

                //string pattMnemonic = @"if(?:(?:O_\S+)|AND|OR|NOT|\(|\)|\s)+then\s+[RAM]\s+else\s+n\/a";
                //@"if\s*(.+)\s+then\s+o\s+else\s+(?:\()?(?:\s+)?if\s*(.+)\s+then(.+)"; //07-25-2017
                string pattComplGen =  @"if\s * (.+)\s + then\s + o\s +else\s + (?:\() ? (?:\s +)?if\s *\(?\s * (.+?)\s *\)?\s + then(.+)";
                Match mcComplGen = Regex.Match(logicstring, pattComplGen, RegexOptions.IgnoreCase);

                string pattComplCTPSExtra = @"if((?:(?:c\S+)|(?:\d+#\S+)|AND|OR|NOT|\(|\)|\s)+)then\s+[MORA]\s+else\s+n\/a";
                Match mcComplCTPSxtra = Regex.Match(logicstring, pattComplCTPSExtra, RegexOptions.IgnoreCase);

                if (mcSimple.Success)
                {
                    //outstr = condSimple(logicstring, pics_support);
                    lgstr.inf("\t{condCompl} pattern mcComplGen # "+logicstring);

                }
                

                else if (mcComplGen.Success)
                {

                    // if the logic string is a complex type 
                    condResRecur = mcComplGen.Groups[1].ToString();
                    condResRecur2 = mcComplGen.Groups[2].ToString();
                    lgstr.inf("\t{condCompl} pattern mcComplGen # " + condResRecur + " # " + condResRecur2 + " # " + logicstring);
                    // After processing condResRecur, get true/false
                    if (condResRecur != "")
                    {
                        string pattIn = @"C(?:\%\d+\%)?[\w-_]+";
                        var mcIn = Regex.Matches(condResRecur, pattIn);
                        //string tempres ;
                        foreach (var mcInTemp in mcIn)
                        {
                            string tempres = c_logic_map[mcInTemp.ToString()];
                            lgstr.inf("\t{condCompl}" + condResRecur + " match: " + mcInTemp.ToString() + " 1stResult: " + tempres);
                            if ((tempres != "True") && (tempres != "False"))
                            {
                                lgstr.inf("\tRecursive call:  " + mcInTemp.ToString());
                                tempres = condCompl(mcInTemp.ToString(), c_logic_map, pics_support);
                            }
                            condResRecur = condResRecur.Replace(mcInTemp.ToString(), tempres);
                            string boolstrverify = condResRecur.Replace("AND", "").Replace("OR", "").Replace("NOT", "").Replace("True", "").Replace("False", "");
                            Match m = Regex.Match(boolstrverify, @"\w+");
                            if (!(m.Success))
                            {
                                lgstr.inf("\t{condCompl} part1Change1: " + condResRecur + " match: " + m.ToString());
                                if (tempres.Trim() != "")
                                    condResRecur = evalBooleanExpr(tempres).ToString();
                            }
                            lgstr.inf("\t{condCompl} part1change2:" + condResRecur);
                        }
                    }
                    if (condResRecur.ToLower() == "true")
                    {
                        outstr = "True";
                    }
                    else if (condResRecur.ToLower() == "false")
                    {
                        //evaluate 2nd part
                        outstr = logicstrProcess(condResRecur2, pics_support);
                    }
                    else
                    {
                        //return the problematic string. 
                        lgstr.cri("\t{condcompl} Improper Condition" + condResRecur);
                        outstr = condResRecur;
                    }
                }
                else if (mcComplCTPSxtra.Success)
                {

                    condResRecur = mcComplCTPSxtra.Groups[1].ToString();
                    lgstr.inf("\t{condcompl} pattern mcComplCTPSxtra # " + condResRecur + " # " + logicstring);
                    if (condResRecur != "")
                    {
                        string pattIn = @"C(?:\%\d+\%)?[\w-_]+";
                        string pattItem = @"\d+#\b[AEG]\.\d[\d\-\/\.a-z]+\b";
                        var mcIn = Regex.Matches(condResRecur, pattIn);
                        var mcItem = Regex.Matches(condResRecur, pattItem);
                        foreach (var mcItemTemp in mcItem)
                        {
                            string tempres = convertToBoolExpr(pics_support, mcItemTemp.ToString());
                            //condResRecur =  convertToBoolExpr(pics_support, condResRecur);
                            condResRecur = condResRecur.Replace(mcItemTemp.ToString(), tempres);
                            lgstr.inf("\t{condcompl} tempres: "+ tempres+"#"+ condResRecur);
                            //logicstring =  convertToBoolExpr(pics_support, logicstring);
                        }
                        lgstr.inf("\t{condCompl} replace item with TF# " + condResRecur);
                        //c_logic_map[cstring] = logicstring;
                        //string tempres ;
                        foreach (var mcInTemp in mcIn)
                        {
                            lgstr.inf("\tregexmatch"+ mcInTemp.ToString());
                            if (!c_logic_map.ContainsKey(mcInTemp.ToString()))
                            {
                                lgstr.inf(c_logic_map.ToString());
                            }
                            string tempres= c_logic_map[mcInTemp.ToString()];
                            lgstr.inf("\t{condCompl}" + condResRecur + "C match: " + mcInTemp.ToString() + " 1stResult: " + tempres);
                            if ((tempres != "True") && (tempres != "False"))
                            {
                                lgstr.inf("\trecursive call:  " + mcInTemp.ToString());
                                tempres = condCompl(mcInTemp.ToString(), c_logic_map, pics_support);
                            }
                            condResRecur = condResRecur.Replace(mcInTemp.ToString(), tempres);
                            string boolstrverify = condResRecur.Replace("AND", "").Replace("OR", "").Replace("NOT", "").Replace("True", "").Replace("False", "");
                            Match m = Regex.Match(boolstrverify, @"\w+");
                            if (!(m.Success))
                            {
                                lgstr.inf("\t{condCompl} part1Change1: " + condResRecur + " match: " + m.ToString());
                                if (tempres.Trim() != "")
                                    condResRecur = evalBooleanExpr(tempres).ToString();
                            }
                            //lgstr.inf(" {condCompl} part1change2:" + condResRecur);
                        }
                        lgstr.inf("\t{condCompl} Final val TF# " + condResRecur);
                        outstr = condResRecur;
                    }


                }
                else if (mcMnemonic.Success)
                {
                    condResRecur = mcMnemonic.Groups[1].ToString();
                    lgstr.inf("\t{condcompl} pattern mcMnemonic # " + condResRecur + " # " + logicstring);
                    if (condResRecur != "")
                    {
                        string patMnemonic = @"\bO_\S+\b";
                        var mcItem = Regex.Matches(condResRecur, patMnemonic);
                        Regex itemRegex = new Regex(patMnemonic, RegexOptions.IgnoreCase);
                        condResRecur = itemRegex.Replace(condResRecur, m => convertBoolHelper(m.ToString(), PICSmnemonicDic));
                        lgstr.inf("\t{condCompl} mnemonic replace item with TF# " + condResRecur);
                        string boolstrverify = condResRecur.Replace("AND", "").Replace("OR", "").Replace("NOT", "").Replace("True", "").Replace("False", "");

                        Match mtmp = Regex.Match(boolstrverify, @"\w+");
                        if (!(mtmp.Success))
                        {
                            lgstr.inf("\t{condCompl} pnemonic: " + condResRecur + " match: " + mtmp.ToString());
                            if (condResRecur.Trim() != "")
                                condResRecur = evalBooleanExpr(condResRecur).ToString();
                        }
                        outstr = condResRecur;
                    }

                    lgstr.inf("\t{condCompl} final: " + condResRecur + " match: " + outstr);
                }
                else
                {
                    lgstr.err("\t{condCompl} no matching pattern: " + logicstring);
                    outstr = "False";

                }
            }
            // For End:
            c_logic_map[cstring] = outstr;
            return outstr;
        }


        public static void combinedCond(_Dictionary<string, string> c_logic_map, _Dictionary<string, string> pics_support)
        {
            _List<string> cval = new _List<string>();
            foreach (KeyValuePair<string, string> temp in c_logic_map)
            {
                cval.Add(temp.Key);
            }
            foreach (string C in cval)
            {
                lgstr.inf("Key Find: " + C + "  Val: " + c_logic_map[C]);
                string evlstr = condCompl(C, c_logic_map, pics_support);
                lgstr.inf("Key Eval: " + C + " Val: "+ evlstr);
                //lgstr.inf(c_logic_map.ToString());

            }

        }
        public static _Dictionary<string, string> specToSpec = new _Dictionary<string, string>();
        public static _Dictionary<string, string> specIDMap = new _Dictionary<string, string>();
        public static TwoKeyDictionary<string, string, string> specRefMap = new TwoKeyDictionary<string, string, string>();
        public static void readconfigpics()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string curDir = System.Environment.CurrentDirectory;
            string fullPath = Path.Combine(curDir, "Config_PICS.xml");
            xmlDoc.Load(fullPath);
            //lgstr.info(fullPath);

            // Let's convert dash1 spec into dash2 spec from the config file:
            // Start:
            XmlNodeList spectospecalltogether = xmlDoc.GetElementsByTagName("spectospec");
            XmlElement spectospec = (XmlElement)spectospecalltogether[0];
            XmlNodeList smallElementDash1 = spectospec.GetElementsByTagName("specdash1");
            XmlNodeList smallElementDash2 = spectospec.GetElementsByTagName("specdash2");
            //_Dictionary<string, string> specToSpec = new _Dictionary<string, string>();
            for (int i = 0; i < smallElementDash1.Count; i++)
            {
                string K = smallElementDash1[i].InnerText.Trim();
                string V = smallElementDash2[i].InnerText.Trim();
                specToSpec[K] = V;
            }
            // End:
            //lgstr.inf("specdash1\n" + specDash1);
            lgstr.inf("spectospec\n" + specToSpec.ToString());

            //string spec = specToSpec[specDash1];


            XmlNodeList specrefalltogether = xmlDoc.GetElementsByTagName("specref");
            XmlElement specref = (XmlElement)specrefalltogether[0];
            Debug.Print(specref.InnerXml);
            //lgstr.cri(specref.InnerXml);
            //XmlNode xmln = bigElement[0].Inner;
            XmlNodeList smallElementSpec = specref.GetElementsByTagName("specrefIN");
            XmlNodeList smallElementRefID = specref.GetElementsByTagName("ref_id");
            XmlNodeList smallElementSpecID = specref.GetElementsByTagName("spec_id");

            //TwoKeyDictionary<string, string, string> specRefMap = new TwoKeyDictionary<string, string, string>();
            for (int i = 0; i < smallElementSpec.Count; i++)
            {
                string K1 = smallElementSpec[i].InnerText.Trim();
                string K2 = smallElementRefID[i].InnerText.Trim();
                string V = smallElementSpecID[i].InnerText.Trim();
                specRefMap[K1, K2] = V;

            }

            XmlNodeList allp = xmlDoc.GetElementsByTagName("mappingspec");
           // _Dictionary<string, string> specIDMap = new _Dictionary<string, string>();
            foreach (XmlNode pl in allp)
            {
                string specno = pl.ChildNodes[0].InnerText.Trim();
                string id = pl.ChildNodes[1].InnerText.Trim();
                specIDMap[specno] = id;
            }

            lgstr.inf("specidmap\n" + specIDMap.ToString());

            xmlDoc = null;
            lgstr.inf("specrefmap\n" + specRefMap.ToString());
        }
        public static string specDash1 = "";
        public static string addIDtoLogicStr(string fullLogic)
        {
            string spec = specToSpec[specDash1];
            // Replacing references first and then without reference. 
            string patt1 = @"(\[[\d\.\-]+\])\s*\b([ABEDGX]\.\d\S*)\b";
            var regex1 = new Regex(patt1, RegexOptions.IgnoreCase);
            //fullLogic = regex1.Replace(fullLogic, m => item1 + "#" + m);

            MatchCollection mcc = Regex.Matches(fullLogic, patt1);
            foreach (Match temp in mcc)
            {
                string item1 = temp.Groups[1].ToString();
                string item2 = specRefMap[spec, item1];
                //fullLogic = regex1.Replace(fullLogic, m => item1 + "#" + m.Groups[2]);
                fullLogic = fullLogic.Replace(item1, item2 + "#");
            }
            // Replacing the items without reference 
            string patt2 = @"(?<!#)\b[ABEDGX]\.\d\S*\b";
            var regex2 = new Regex(patt2, RegexOptions.IgnoreCase);
            Match mc2 = Regex.Match(fullLogic, patt2);
            if (mc2.Success)
            {
                string item2 = specIDMap[spec];
                fullLogic = regex2.Replace(fullLogic, m => item2 + "#" + m);
            }
            return fullLogic;
        }
        // Start to process C dictionary: Adding the spec number with Item
        //public static void itemProcess()
        public static void itemProcess(_Dictionary<string, string> CDictRaw, string specDash1)
        {
            /*
            XmlDocument xmlDoc = new XmlDocument();
            string curDir = System.Environment.CurrentDirectory;
            string fullPath = Path.Combine(curDir, "Config_PICS.xml");
            xmlDoc.Load(fullPath);
            //lgstr.info(fullPath);

            // Let's convert dash1 spec into dash2 spec from the config file:
            // Start:
            XmlNodeList spectospecalltogether = xmlDoc.GetElementsByTagName("spectospec");
            XmlElement spectospec = (XmlElement)spectospecalltogether[0];
            XmlNodeList smallElementDash1 = spectospec.GetElementsByTagName("specdash1");
            XmlNodeList smallElementDash2 = spectospec.GetElementsByTagName("specdash2");
            _Dictionary<string, string> specToSpec = new _Dictionary<string, string>();
            for (int i = 0; i < smallElementDash1.Count; i++)
            {
                string K = smallElementDash1[i].InnerText.Trim();
                string V = smallElementDash2[i].InnerText.Trim();
                specToSpec[K] = V;
            }
            // End:
            lgstr.inf("specdash1\n"+specDash1);
            lgstr.inf("spectospec\n"+specToSpec.ToString());

            


            XmlNodeList specrefalltogether = xmlDoc.GetElementsByTagName("specref");
            XmlElement specref = (XmlElement)specrefalltogether[0];
            Debug.Print(specref.InnerXml);
            //lgstr.cri(specref.InnerXml);
            //XmlNode xmln = bigElement[0].Inner;
            XmlNodeList smallElementSpec = specref.GetElementsByTagName("specrefIN");
            XmlNodeList smallElementRefID = specref.GetElementsByTagName("ref_id");
            XmlNodeList smallElementSpecID = specref.GetElementsByTagName("spec_id");

            TwoKeyDictionary<string, string, string> specRefMap = new TwoKeyDictionary<string, string, string>();
            for (int i = 0; i < smallElementSpec.Count; i++)
            {
                string K1 = smallElementSpec[i].InnerText.Trim();
                string K2 = smallElementRefID[i].InnerText.Trim();
                string V = smallElementSpecID[i].InnerText.Trim();
                specRefMap[K1, K2] = V;

            }

            XmlNodeList allp = xmlDoc.GetElementsByTagName("mappingspec");
            _Dictionary<string, string> specIDMap = new _Dictionary<string, string>();
            foreach (XmlNode pl in allp)
            {
                string specno = pl.ChildNodes[0].InnerText.Trim();
                string id = pl.ChildNodes[1].InnerText.Trim();
                specIDMap[specno] = id;
            }

            lgstr.inf("specidmap\n"+specIDMap.ToString());

            xmlDoc = null;
            lgstr.inf("specrefmap\n"+specRefMap.ToString());
            */
            //processedif = re.sub(r'(?<!#)\bA\.', s + r'#A.', processedif)
            //rpatt = r'(\[\d+\]\s*)'c
            string spec = specToSpec[specDash1];
            _List<string> newKey = new _List<string>();
            foreach (KeyValuePair<string, string> temp in CDictRaw)
            {
                // Filtering C string. All the invalid conditions are removerd (e.g., Note 1, etc.)
                newKey.Add(temp.Key);
            }
            foreach (string C in newKey)
            {
                string fullLogic = CDictRaw[C];
                //string patt1 = @"\bA\.\d\S*\b";

                string patt = @"C(?:\%\d+\%)?[\w-_]+";
                Match var = Regex.Match(C, patt);
                string specialpatt = @"C%(\d+)%\S+";
                Match varSpecial = Regex.Match(C, specialpatt);
                if (varSpecial.Success)
                {
                    string patt2 = @"(?<!#)\b[ABDGX]\.\d\S*\b";
                    var regex2 = new Regex(patt2, RegexOptions.IgnoreCase);
                    Match mc2 = Regex.Match(fullLogic, patt2);
                    string item2 = varSpecial.Groups[1].ToString();
                    if (mc2.Success)
                    {

                        fullLogic = regex2.Replace(fullLogic, m => item2 + "#" + m);
                    }
                    CDictRaw[C] = fullLogic;
                }

                else if (var.Success)
                {
                    // Replacing references
                    string patt1 = @"(\[[\d\.\-]+\])\s*\b([ABDGX]\.\d\S*)\b";
                    var regex1 = new Regex(patt1, RegexOptions.IgnoreCase);
                    //fullLogic = regex1.Replace(fullLogic, m => item1 + "#" + m);

                    MatchCollection mcc = Regex.Matches(fullLogic, patt1);
                    foreach (Match temp in mcc)
                    {
                        string item1 = temp.Groups[1].ToString();
                        string item2 = specRefMap[spec, item1];
                        //fullLogic = regex1.Replace(fullLogic, m => item1 + "#" + m.Groups[2]);
                        fullLogic = fullLogic.Replace(item1, item2+ "#");
                    }

                    //string item1 = Regex.Match(fullLogic,patt1);


                    // Adding spec no and hash
                    string patt2 = @"(?<!#)\b[ABDGX]\.\d\S*\b";
                    var regex2 = new Regex(patt2, RegexOptions.IgnoreCase);
                    Match mc2 = Regex.Match(fullLogic, patt2);
                    if (mc2.Success)
                    {

                        string item2 = specIDMap[spec];
                        fullLogic = regex2.Replace(fullLogic, m => item2 + "#" + m);
                    }
                    CDictRaw[C] = fullLogic;
                }
                else
                {
                    CDictRaw.Remove(C);
                }





            }


        }
        // End

        public static string TCVsTF(string complexCondStr, _Dictionary<string, string> CondVsTF)
        {
            string res = "";
            var regex = new Regex(@"C(?:\%\d+\%)?[\w-_]+", RegexOptions.IgnoreCase);
            string resTemp = regex.Replace(complexCondStr, m => convertBoolHelper(m.ToString(), CondVsTF));


            string boolstrverify = resTemp.Replace("AND", "").Replace("OR", "").Replace("NOT", "").Replace("True", "").Replace("False", "");
            Match mc = Regex.Match(boolstrverify, @"\w+");
            if (!(mc.Success))
            {
                if (resTemp.Trim() != "")
                {
                    res = evalBooleanExpr(resTemp).ToString();
                    lgstr.deb("{TCvsTF} More than 1 conditions found and being evaluated for" + complexCondStr + ", Res: " + res);
                }
            }
            else
            {
                res = resTemp;
            }
            lgstr.inf("{TCvsTF} " + String.Format(" In: {0} \tRes: {1}", complexCondStr, res));
            return res;
        }
        public static List<string> mismatchedbandbandsupporthelper = new List<string>();

        public static bool bandSupportHelper(string inputBand, _List<string> PICSBandSupportList)
        {

            if ((inputBand.ToUpper() == "BI") || (inputBand.ToUpper() == "NI") || (inputBand.ToUpper() == "BA"))
            {
                return true;
            }
            //start code here
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

            string pattEG = @"^[EG]\d+$";
            string pattU = @"^[U]\d+$";
            Match mcEG = Regex.Match(inputBand, pattEG);
            Match mcU = Regex.Match(inputBand, pattU);
            if (mcEG.Success)
            {
                inputBand = inputBand.TrimStart('E').TrimStart('G').TrimStart('0');
            }
            else if (mcU.Success)
            {
                inputBand = inputBand.TrimStart('U').TrimStart('0');
                inputBand = BandProcess.getwbandinroman(inputBand);

            }
            string pattSC = @"^(\d|[IVX])+$";
            Match mcSC = Regex.Match(inputBand, pattSC);
            //string pattSCU = @"^[IVX]+$";
            //Match mcSCU = Regex.Match(inputBand, pattSCU);
            string outputBand = inputBand.Trim();
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
                    //
                    //lgstr.err("{bandSupportHelper-stringprocess} Band Not Match: " + inputBand);
                    if (!mismatchedbandbandsupporthelper.Contains(inputBand))
                    {
                        mismatchedbandbandsupporthelper.Add(inputBand);
                    }
                }
            }

            //lgstr.cri("Input band: " + inputBand + ", Output band: " + outputBand + ", Result: " + finaloutputBand);
            return finaloutputBand;
        }

        public static _List<string> getValIntiligent(string k, _Dictionary<string, string> dic)
        {
            _List<string> retkeyval = new _List<string>();
            bool notfound = true;
            string containkey = "0";
            string retkey = k;
            string retval = "";
            if (dic.ContainsKey(k))
            {
                notfound = false;
                retval = dic[k];
                containkey = "1";
            }
            if (notfound)
            {

                // _v1
                Match m1 = Regex.Match(k, @"(.+)\s*\(v(\d)\)", RegexOptions.IgnoreCase);
                if (m1.Success)
                {
                    string newkey = m1.Groups[1].ToString().Trim() + "-" + m1.Groups[2].ToString();
                    if (dic.ContainsKey(newkey) && notfound)
                    {
                        notfound = false;
                        retval = dic[newkey];
                        retkey = newkey;
                        containkey = "2";
                    }
                    //lgstr.deb("matched " + k + ":" + newkey+": "+notfound.ToString());
                    newkey = m1.Groups[1].ToString().Trim();
                    if (dic.ContainsKey(newkey) && notfound)
                    {
                        notfound = false;
                        retval = dic[newkey];
                        retkey = newkey;
                        containkey = "3";
                    }
                    //lgstr.deb("matched " + k + ":" + newkey + ": " + notfound.ToString());
                }

            }
            if (notfound)
            {
                //FDDIV
                Match m1 = Regex.Match(k, @"[FT]DD\s*(\w+)", RegexOptions.IgnoreCase);
                string newkey = m1.Groups[1].ToString().Trim();
                if (dic.ContainsKey(newkey))
                {
                    notfound = false;
                    retval = dic[newkey];
                    retkey = newkey;
                    containkey = "4";
                }
            }

            retkeyval.Add(containkey);
            retkeyval.Add(retkey);
            retkeyval.Add(retval);
            retkeyval.Add(k);
            return retkeyval;
        }

    }
}
