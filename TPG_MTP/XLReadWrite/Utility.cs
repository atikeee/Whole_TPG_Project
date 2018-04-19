using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XLReadWrite
{

    public struct FourVal

    {

        public string w, x, y, z;

        // Constructor: 

        public FourVal(string w, string x, string y, string z)

        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;

        }

        public void setv(string w, string x, string y, string z)

        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }



        // Override the ToString method: 

        public override string ToString()

        {
            return (String.Format("({0},{1},{2},{3})", w, x, y, z));
        }

        public override bool Equals(object obj)

        {
            return base.Equals(obj);
        }

        public override int GetHashCode()

        {
            return base.GetHashCode();
        }



    }

    public class _List<T> : List<T>
    {
        public override string ToString()
        {
            //string str = "[  ";
            //bool b = false;
            //foreach (T k in this)
            //{
            //    if (b) str += ", ";
            //    str += k.ToString();
            //    b = true;
            //}
            //str += "  ]";
            return "[ " + toCommaList() + " ]";
        }
        public string toCommaList()
        {
            string str = "";
            bool b = false;
            foreach (T k in this)
            {
                if (b) str += ", ";
                str += k.ToString();
                b = true;
            }
            return str;
        }
        public _List() : base()
        {

        }
        public _List(List<T> l) : base(l)
        {

        }
        //check match with other list string by string. 
        public bool chkmatch(List<T> otherlist)
        {
            if (this.Count != otherlist.Count)
                return false;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].ToString() != otherlist[i].ToString())
                    return false;
            }
            return true;
        }
        //check match with other list string by string. 
        public bool chkmatch(_List<T> otherlist)
        {
            if (this.Count != otherlist.Count)
                return false;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].ToString() != otherlist[i].ToString())
                    return false;
            }
            return true;
        }
        // check match with other list with specific index list 
        public bool chkmatch(List<T> otherlist, List<int> ind)
        {
            if (this.Count != otherlist.Count)
                return false;
            //for (int i = 0; i < this.Count; i++)
            foreach (int i in ind)
            {
                if (this[i].ToString() != otherlist[i].ToString())
                    return false;
            }
            return true;
        }
        /// <summary>
        /// this will check over list of list and return bool if it exists. 
        /// </summary>
        /// <param name="listoflist"> this list element size should be the same ase this list</param>
        /// <param name="ind">only this indexes will be checked and compare</param>
        /// <returns>true or false</returns>
        public bool chkmatch(List<_List<T>> listoflist, List<int> ind)
        {
            foreach (_List<T> onelist in listoflist)
            {
                bool oneMatch = this.chkmatch(onelist, ind);
                if (oneMatch)
                {
                    return true;
                }
            }
            //for (int i = 0; i < this.Count; i++)

            return false;
        }

    }

    public class _Dictionary<T1, T2> : Dictionary<T1, T2>
    {
        private string dashline = String.Format("\t|{0,-50}{1,-100}\r\n", new String('-', 50), "|" + new String('-', 50));
        IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return this.GetEnumerator();
        }


        //public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        //{
        //    //return new MyEnum(this); // use your enumerator
        //                             // OR simply forget your own implementation and
        //    return this.GetEnumerator();
        //}
        public override string ToString()
        {
            string str = "\r\n";
            str += dashline;
            str += String.Format("\t|{0,-50}{1,-100}\r\n", "KEY", "|   VALUE");
            str += dashline;
            foreach (T1 k in this.Keys)
            {
                str += String.Format("\t|{0,-50}{1,-100}\r\n", k.ToString(), "|   " + this[k].ToString());
            }
            str += dashline;
            return str;
        }


        public _Dictionary(Dictionary<T1, T2> d) : base()
        {
            foreach (KeyValuePair<T1, T2> kvp in d)
            {
                this.Add(kvp.Key, kvp.Value);
            }

        }
        public _Dictionary() : base()
        {

        }

    }
    public class TwoKeyDictionary<T1, T2, T3> : IEnumerable<KeyValuePair<T1, Dictionary<T2, T3>>>
    {

        private Dictionary<T1, Dictionary<T2, T3>> wholedic;
        public Dictionary<T1, Dictionary<T2, T3>> getclone()
        {
            Dictionary<T1, Dictionary<T2, T3>> wholedicclone = new Dictionary<T1, Dictionary<T2, T3>>(wholedic);

            return wholedicclone;
        }
        private string dashline = String.Format("\t|{0,-50}{1,-100}\n", new String('-', 50), "|" + new String('-', 50));
        public int getCount()
        {
            return wholedic.Count;
        }
        public TwoKeyDictionary()
        {

            wholedic = new Dictionary<T1, Dictionary<T2, T3>>();


        }
        public void Clear()
        {
            wholedic.Clear();
        }
        public void Remove(T1 key1)
        {
            if (wholedic.ContainsKey(key1))
            {
                wholedic.Remove(key1);
            }
        }

        public void Add(T1 key1, T2 key2, T3 val)
        {
            Dictionary<T2, T3> k2val;
            if (wholedic.ContainsKey(key1))
            {
                k2val = (Dictionary<T2, T3>)wholedic[key1];

                if (!k2val.ContainsKey(key2))
                {
                    k2val.Add(key2, val);
                }
            }
            else
            {
                k2val = new Dictionary<T2, T3>();
                k2val.Add(key2, val);
                wholedic.Add(key1, k2val);
            }

        }
        public Dictionary<T2, T3> getdictionary(T1 key1)
        {
            Dictionary<T2, T3> valdict = new Dictionary<T2, T3>();
            if (wholedic.ContainsKey(key1))
            {
                valdict = (Dictionary<T2, T3>)wholedic[key1];
            }

            return valdict;
        }
        public T3 getelement(T1 key1, T2 key2)
        {

            T3 val;
            Dictionary<T2, T3> d_t = getdictionary(key1);
            val = d_t[key2];
            return val;

        }
        public T3 this[T1 i, T2 j]
        {
            get
            {
                return getelement(i, j);
            }
            set
            {
                Add(i, j, value);
            }
        }
        public Dictionary<T2, T3> this[T1 i]
        {
            get
            {
                return getdictionary(i);
            }
            set
            {
                wholedic[i] = value;

            }
        }
        public bool ContainsKey(T1 key1)
        {
            return wholedic.ContainsKey(key1);
        }
        public bool ContainsKey(T1 key1, T2 key2)
        {
            bool b = false;
            if (wholedic.ContainsKey(key1))
            {
                Dictionary<T2, T3> d1 = (Dictionary<T2, T3>)wholedic[key1];
                if (d1.ContainsKey(key2))
                {
                    b = true;
                }
            }
            return b;
        }
        public override string ToString()
        {
            string str = "";
            foreach (KeyValuePair<T1, Dictionary<T2, T3>> kvp in wholedic)
            {
                str += "\n### " + kvp.Key.ToString() + " ###\n";
                Dictionary<T2, T3> valdic = (Dictionary<T2, T3>)kvp.Value;

                str += dashline;
                str += String.Format("\t|{0,-50}{1,-100}\n", "KEY", "|   VALUE");
                str += dashline;
                //foreach (T1 k in this.Keys)
                //{
                //    str += String.Format("{0,-50}{1,-100}\n", k.ToString(), "|   " + this[k].ToString());
                //}


                foreach (KeyValuePair<T2, T3> kvp2 in valdic)
                {
                    str += String.Format("\t|{0,-50}{1,-100}\n", kvp2.Key.ToString(), "|   " + kvp2.Value.ToString());
                }
                str += dashline;
            }
            return str;
        }

        IEnumerator<KeyValuePair<T1, Dictionary<T2, T3>>> IEnumerable<KeyValuePair<T1, Dictionary<T2, T3>>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T1, Dictionary<T2, T3>>>)wholedic).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T1, Dictionary<T2, T3>>>)wholedic).GetEnumerator();
        }
    }


    public class ReadConfig
    {
        private string filename;
        private Dictionary<string, string> allconfig = new Dictionary<string, string>();
        public List<List<string>> stringarr = new List<List<string>>();

        // repeated key 0 = no repeatation , 1 = pick first , 2 pick last
        // repeated key 3 will make it as list. 
        public ReadConfig(string fn, int repeatedkey = 0)
        {
            filename = fn;
            string line;

            // Read the file and display it line by line.
            StreamReader file = new StreamReader(fn);
            while ((line = file.ReadLine()) != null)
            {
                if (repeatedkey == 3)
                {
                    string[] kv = line.Split('\t');
                    List<string> lst = kv.OfType<string>().ToList();
                    if (!line.StartsWith("#") && (kv.Count() > 1))
                    {
                        stringarr.Add(lst);
                    }

                }
                else
                {
                    string[] kv = line.Split('=');

                    if ((!line.StartsWith("#")) && (kv.Count() > 1))
                        if (repeatedkey == 0)
                        {
                            allconfig.Add(kv[0].Trim(), kv[1].Trim());
                        }
                        else if (repeatedkey == 1)
                        {
                            if (!allconfig.ContainsKey(kv[0]))
                            {
                                allconfig.Add(kv[0].Trim(), kv[1].Trim());
                            }
                        }
                        else if (repeatedkey == 2)
                        {
                            if (allconfig.ContainsKey(kv[0]))
                            {
                                allconfig[kv[0].Trim()] = kv[1].Trim();
                            }
                            else
                            {
                                allconfig.Add(kv[0].Trim(), kv[1].Trim());
                            }
                        }
                }

            }

            file.Close();
        }
        public List<List<string>> getitem_list(Dictionary<int, string> inputdict, bool regx = false)
        {
            // This function is returning the whole line of the info as list based on the dictionary input. 
            // dictionary key = index and value is value. it supports regex and non regex both. 
            // first column Y will make it regex. non regex is default. if the first column doesnt say 

            List<List<string>> matchedlist = new List<List<string>>();
            _Dictionary<string, string> matchedval = new _Dictionary<string, string>();
            foreach (List<string> lst in stringarr)
            {
                matchedval.Clear();
                //List<string> t = new List<string>();
                //List<string> d = new List<string>(); 
                // this will override regex value if it is defined in the first column of the text file. 
                if (lst[0].ToLower() == "y")
                {
                    regx = true;
                }
                else if (lst[0].ToLower() == "n")
                {
                    regx = false;
                }
                if (regx)
                {
                    bool matchflag = true;
                    foreach (KeyValuePair<int, string> kvp in inputdict)
                    {
                        string pattern = lst[kvp.Key];
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        Match mc = rgx.Match(kvp.Value);

                        if (mc.Success)
                        {
                            if (kvp.Key == 2)
                            {
                                matchedval.Add("#t1#", mc.Groups[1].Value);
                                matchedval.Add("#t2#", mc.Groups[2].Value);
                                matchedval.Add("#t3#", mc.Groups[3].Value);
                                matchedval.Add("#t4#", mc.Groups[4].Value);
                            }
                            else if (kvp.Key == 3)
                            {
                                matchedval.Add("#d1#", mc.Groups[1].Value);
                                matchedval.Add("#d2#", mc.Groups[2].Value);
                                matchedval.Add("#d3#", mc.Groups[3].Value);
                                matchedval.Add("#d4#", mc.Groups[3].Value);
                            }
                        }
                        else
                        {

                            Debug.Print("false");
                            matchflag = false;
                            continue;
                        }
                    }
                    if (matchflag)
                    {
                        List<string> token = new List<string>() { "#t1#", "#t2#", "#t3#", "#d1#", "#d2#", "#d3#" };



                        for (int i = 4; i < lst.Count - 1; i += 2)
                        {
                            if (lst[i].Trim() != "")
                            {
                                List<string> oneres = new List<string>();
                                string newtc = lst[i];
                                string newdesc = lst[i + 1];
                                //inputdict[2]
                                foreach (string t in token)
                                {
                                    if (newtc.Contains(t))
                                        newtc = newtc.Replace(t, matchedval[t]);
                                    if (newdesc.Contains(t))
                                        newdesc = newdesc.Replace(t, matchedval[t]);
                                }

                                oneres.Add(newtc);
                                oneres.Add(newdesc);
                                matchedlist.Add(oneres);
                            }
                        }
                        //matchedlist = lst;
                        break;
                    }

                }
                else
                {
                    bool match = true;
                    foreach (KeyValuePair<int, string> kvp in inputdict)
                    {

                        if (lst[kvp.Key] != kvp.Value)
                        {
                            match = false;
                            continue;
                        }
                    }
                    if (match)
                    {

                        for (int i = 5; i < lst.Count - 1; i += 2)
                        {
                            if (lst[i].Trim() != "")
                            {
                                List<string> oneres = new List<string>();
                                string newtc = lst[i];
                                string newdesc = lst[i + 1];
                                oneres.Add(newtc);
                                oneres.Add(newdesc);
                                matchedlist.Add(oneres);
                            }
                        }

                        //matchedlist = lst;
                        break;
                    }
                }
            }
            Debug.Print(matchedval.ToString());
            if (matchedlist.Count == 0)
            {
                matchedlist.Add(new List<string>() { inputdict[2], inputdict[3], "" });
            }
            return matchedlist;

        }
        public string getvalue(string k)
        {
            string v = "";
            if (allconfig.ContainsKey(k))
                v = allconfig[k];
            else
                throw new KeyNotFoundException("Key is not in config");

            return v;
        }
        public List<string> getList(string k, char c = ',')
        {
            string v = "";
            if (allconfig.ContainsKey(k))
            {
                v = allconfig[k];
            }
            else
            {
                throw new KeyNotFoundException("Key is not in config");
            }
            string[] vsplit = v.Split(c);
            List<string> retval = new List<string>();
            foreach (string e in vsplit)
            {
                retval.Add(e.Trim());
            }

            return retval;
        }
        public Dictionary<string, string> getdictionary(string k, char c = ',', char kvsep = ':')
        {
            Dictionary<string, string> retval = new Dictionary<string, string>();
            List<string> kv = this.getList(k, c);
            foreach (string oneitem in kv)
            {
                string[] dickv = oneitem.Split(kvsep);
                if (dickv.Count() == 2)
                {
                    retval.Add(dickv[0], dickv[1]);
                }
                else
                {
                    throw new Exception("Improperformat for dictionary");
                }
            }
            return retval;
        }
        public List<string> getkeys()
        {
            return allconfig.Keys.ToList<string>();
        }
    }
    public class Logging
    {
        private TextWriter w;
        //private StreamWriter w;
        private int lvl;
        private string fn;
        private string[] logprefix = { "INFO ", "DEBUG", "WARN ", "ERROR", "CRITI" };
        private int[] loglvl = { 10, 20, 30, 40, 50 };

        public Logging(string fn, int lvl)
        {
            this.fn = fn;
            w = new StreamWriter(fn);
            //w = File.AppendText(fn);
            this.lvl = lvl;
        }
        public void resetlog()
        {
            //w.Close()
            File.WriteAllText(this.fn, string.Empty);
        }
        /*
           CRITICAL	50
            ERROR	40
            WARNING	30
            INFO	20
            DEBUG	10
            NOTSET	0
         */
        //public void close()
        ~Logging()
        {
            //w.Close();
        }
        public void logprint(object logMessage, int i)
        {
            if (this.lvl <= this.loglvl[i])
            {
                if (logMessage.GetType() == typeof(string))
                {
                    logMessage = this.logprefix[i] + ":\t" + logMessage;
                    w.WriteLine(logMessage);
                    w.Flush();
                }
                else if (logMessage.GetType() == typeof(Dictionary<string, string>))
                {
                    string convertedlogmessage = this.logprefix[i] + ": (Dictionary)";
                    w.WriteLine(convertedlogmessage);
                    foreach (KeyValuePair<string, string> t in (Dictionary<string, string>)logMessage)
                    {
                        convertedlogmessage = String.Format("\t\t{0,-50}:{1}", t.Key, t.Value);
                        //Debug.Print(t.Key);
                        w.WriteLine(convertedlogmessage);
                    }
                    w.Flush();
                }
                else if (logMessage.GetType() == typeof(Dictionary<string, object>))
                {
                    string valuemessage = "";
                    string convertedlogmessage = this.logprefix[i] + ":(Dictionary) - value as list of string";
                    w.WriteLine(convertedlogmessage);
                    foreach (KeyValuePair<string, object> t in (Dictionary<string, object>)logMessage)
                    {
                        //Debug.Print("st ob"+t.Key);
                        if (t.Value.GetType() == typeof(List<string>))
                        {
                            foreach (string ts in (List<string>)t.Value)
                            {
                                valuemessage += "\t" + ts;
                            }
                            convertedlogmessage = String.Format("\t\t{0,-50}:{1}", t.Key, valuemessage);
                            valuemessage = "";
                            w.WriteLine(convertedlogmessage);
                        }
                    }
                    w.Flush();

                }


            }

        }
        public void inf(object logMessage)
        {
            this.logprint(logMessage, 0);
        }
        public void deb(object logMessage)
        {
            this.logprint(logMessage, 1);
        }
        public void war(object logMessage)
        {
            this.logprint(logMessage, 2);
        }
        public void err(object logMessage)
        {
            this.logprint(logMessage, 3);
        }
        public void cri(object logMessage)
        {
            this.logprint(logMessage, 4);
        }


    }
    public static class Util
    {
        public static List<string> CASplit(string inputband)
        {
            List<string> allBands = new List<string>() { "", "", "", "", "", "", "", "", "", "" };
            if (inputband == null)
            {
                return allBands;
            }
            else  if (inputband.Trim() == "")
            {
                return allBands;
            }else if (inputband.Trim().StartsWith("CA"))
            {
                string bpatt = @"CA_(\w+)\-?(\w+)?\-?(\w+)?\-?(\w+)?\-?(\w+)?";
                Match bc = Regex.Match(inputband, bpatt);
                string B1 = "", B2 = "", B3 = "", B4 = "", B5 = "";
                int i = 0; 
                if (bc.Success)
                {
                    B1 = bc.Groups[1].ToString();
                    List<string> res1 = CACCCheck(B1);
                    foreach (string val in res1)
                    {
                        //allBands.Add(val);
                        allBands[i] = val;
                        i++;
                    }

                    if (bc.Groups[2].Success)
                    {
                        B2 = bc.Groups[2].ToString();
                        List<string> res2 = CACCCheck(B2);
                        foreach (string val in res2)
                        {
                            //allBands.Add(val);
                            allBands[i] = val;
                            i++;

                        }
                    }
                    if (bc.Groups[3].Success)
                    {
                        B3 = bc.Groups[3].ToString();
                        List<string> res3 = CACCCheck(B3);
                        foreach (string val in res3)
                        {
                            //allBands.Add(val);
                            allBands[i] = val;
                            i++;
                        }
                    }
                    if (bc.Groups[4].Success)
                    {
                        B4 = bc.Groups[4].ToString();
                        List<string> res4 = CACCCheck(B4);
                        foreach (string val in res4)
                        {
                            //allBands.Add(val);
                            allBands[i] = val;
                            i++;
                        }
                    }
                    if (bc.Groups[5].Success)
                    {
                        B5 = bc.Groups[5].ToString();
                        List<string> res5 = CACCCheck(B5);
                        foreach (string val in res5)
                        {
                            //allBands.Add(val);
                            allBands[i] = val;
                            i++;
                        }
                    }

                }
                return allBands;

            }else
            {
                string[] ba = inputband.Split('-');
                int i = 0; 
                foreach(string b in ba)
                {
                    allBands[i]= b.Trim();
                    i++;
                }
                return allBands;
            }
            

            

        }

        private static List<string> CACCCheck(string CACC)
        {
            List<string> outBands = new List<string>();

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
                switch (CCLetter)
                {
                    case "A":
                        outBands.Add(CCVal);
                        break;
                    case "B":
                        for (int i = 1; i <= 2; i++)
                        {
                            outBands.Add(CCVal);
                        }
                        break;
                    case "C":
                        for (int i = 1; i <= 2; i++)
                        {
                            outBands.Add(CCVal);
                        }
                        break;
                    case "D":
                        for (int i = 1; i <= 3; i++)
                        {
                            outBands.Add(CCVal);
                        }
                        break;
                    case "E":
                        for (int i = 1; i <= 4; i++)
                        {
                            outBands.Add(CCVal);
                        }

                        break;
                    case "F":
                        for (int i = 1; i <= 5; i++)
                        {
                            outBands.Add(CCVal);
                        }

                        break;
                }
            }
            return outBands;
        }
        // This is for geting CA band 
        //public void bandCombine(string bandCombo, string BCS, string BWCombo)
        public static void bandCombine(string[,] res, int rowidx, int[] colidx)
        {
            //colidx 12 element array of int /last one for updating the value of band
            // bandCombo 5 element array
            // BWCombo 5 element array
            // BCS = 1 string element
            //Debug.Print("rowidx: "+ rowidx);
            rowidx--;
            string outVal = "";
            string outVal1 = "CA_";
            //string[] bandsAll = bandCombo.Split('#');
            //string[] BWAll = BWCombo.Split('#');
            string[] bandsAll= { "", "", "", "", ""};
            string[] BWAll= { "", "", "", "" };
            for (int i = 0; i < 5; i++)
            {
                bandsAll[i] = res[rowidx, colidx[i]-1];
            }
            for (int i = 5; i < 9; i++)
            {
                BWAll[i-5]=res[rowidx, colidx[i]-1];
            }
            string BCS = res[rowidx, colidx[9] - 1];
            //Debug.Print("bandsAll: " +string.Join(" , " ,bandsAll));
            //Debug.Print("BWAll: " + string.Join(" , " , BWAll));
            //Debug.Print("BCS: " + BCS);
            List<string> bandsDL = new List<string>();
            //List<string> bandsUL = new List<string>();
            int numDLBand = 5;
            for (int i = 0; i < numDLBand; i++)
            {
                if ((bandsAll[i] != "")&&(bandsAll[i]!=null))
                    bandsDL.Add(bandsAll[i]);
            }
            //BCS = RemoveSpecialCharacters(BCS);
            BCS = (BCS == null) ? "" : BCS;
            if (BCS != "")
            {
                //Debug.Print("bcs!=null:" + BCS);
                List<string> outVal2List = new List<string>();
                for (int i = 0; i < bandsDL.Count; i++)
                {
                    string CAForBandTemp = CAType(BWAll[i]);
                    //Debug.Print("i: " + i.ToString() + " bandsdl " + BWAll[i]+" caforbandtemp "+ CAForBandTemp);
                    string temp = "";
                    if (CAForBandTemp != "")
                    {
                        temp = removeBandString(bandsDL[i]) + CAForBandTemp;
                    }
                    outVal2List.Add(temp);

                }
                string temp3 = "";
                for (int i = 0; i < outVal2List.Count; i++)
                {
                    if (i == 0)
                    {
                        temp3 = outVal2List[i];
                    }
                    else
                    {
                        temp3 += "-" + outVal2List[i];
                    }
                }
                outVal = outVal1 + temp3;
            }
            else
            {
                // Single Bands and Handover and Multiple Bands
                outVal=""; 
                foreach(string banddl in bandsAll)
                {
                    if ((banddl == "")||(banddl==null))
                        break;
                    outVal += "-" + banddl;
                    //Debug.Print("ov: " + outVal);
                    
                }
                
                
                outVal=(outVal.Trim() != "")? outVal.Substring(1):outVal;
            }
            res[rowidx, (colidx[10]-1)] = outVal.TrimEnd('-');
            //return outVal;
            //Debug.Print("result: "+outVal);
           
        }

        private static string CAType(string BWCur)
        {
            BWCur = (BWCur == null) ? "" : BWCur;
            string outVal = "";

            string pattF = @"(\d+)\-(\d+)-(\d+)\-(\d+)-(\d+)";
            Match mcF = Regex.Match(BWCur, pattF);

            string pattE = @"(\d+)\-(\d+)-(\d+)\-(\d+)";
            Match mcE = Regex.Match(BWCur, pattE);

            string pattD = @"(\d+)\-(\d+)-(\d+)";
            Match mcD = Regex.Match(BWCur, pattD);

            string pattBorC = @"(\d+)\-(\d+)";
            Match mcBorC = Regex.Match(BWCur, pattBorC);

            string pattA = @"(\d+)";
            Match mcA = Regex.Match(BWCur, pattA);

            if (mcF.Success)
            {
                outVal = "F";
            }
            else if (mcE.Success)
            {
                outVal = "E";
            }
            else if (mcD.Success)
            {
                outVal = "D";
            }
            else if (mcBorC.Success)
            {
                string CAContiguousBW = mcBorC.ToString();
                string[] CAContiguousBWSplit = CAContiguousBW.Split('-');
                int numCC = CAContiguousBWSplit.Length;
                List<int> CACCBWVal = new List<int>();
                int sumCCBW = 0;
                for (int i = 0; i < numCC; i++)
                {
                    //Debug.Print(sumCCBW.ToString());
                    sumCCBW += Int32.Parse(CAContiguousBWSplit[i]);
                    Debug.Print(sumCCBW.ToString());
                }
                if (sumCCBW <= 20)
                {
                    outVal = "B";
                }
                else if (sumCCBW > 20 && sumCCBW <= 40)
                {
                    outVal = "C";
                }
            }
            else if (mcA.Success)
            {
                outVal = "A";
            }

            return outVal;
        }
        //private static string RemoveSpecialCharacters(string str)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (char c in str)
        //    {
        //        if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
        //        {
        //            sb.Append(c);
        //        }
        //    }
        //    return sb.ToString();
        //}
        private static string removeBandString(string inVal)
        {
            string outVal = "";
            string tempAt1 = "";
            string patt1 = @"[EUG](\d+)";
            Match mc1 = Regex.Match(inVal, patt1);
            if (mc1.Success)
            {
                tempAt1 = mc1.Groups[1].ToString();
            }
            string patt2 = @"[0](\d+)";
            Match mc2 = Regex.Match(tempAt1, patt2);
            if (mc2.Success)
            {
                outVal = mc2.Groups[1].ToString();
            }
            else
            {
                outVal = tempAt1;
            }
            return outVal;
        }






    }
}
