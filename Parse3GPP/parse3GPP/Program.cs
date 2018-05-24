using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using excelparse;
using System.Data;
using Utility;
using System.Diagnostics;
using System.IO;

namespace parse3GPP
{
    class Program
    {

        
        static void Main(string[] args)
        {
            /*
            TwoKeyDictionary<string, string, string> readconf = new TwoKeyDictionary<string, string, string>();
            MISC.readconfig_2kdict("conf\\GCF_BI_IRAT_TC.conf",readconf);
            Debug.Print(readconf.ToString());
            Dictionary<
                string,string> ab = readconf["36.523-1"];
            _Dictionary<string,string> abc = new _Dictionary<string, string>(ab);
            Debug.Print(abc.ToString());
            Dictionary<string, List<string>> t1 = MISC.readconfiglist("conf\\PTCRB_IRAT_band.conf",'\t');
            foreach (KeyValuePair<string , List<string>> kvp in t1)
            {
                Debug.Print(kvp.Key);
                Debug.Print("\t\t"+string.Join(",",kvp.Value));
            }
            */
            Logging lg = new Logging("log.log",0);
            MISC.lgstr = lg;
            _Dictionary<string, string> configdic = MISC.readconfig(@"conf\\config.conf");
            string[] excelfilearr_env = configdic["ecfile"].Split(',');
            string[] sheetnamearr_env = configdic["ecsheet"].Split(',');
            string printlevelforenvcondition = configdic["envdebprint"];
            string bandsupportfile = configdic["bandsupportfile"];
            string bandsupportproject = configdic["bandsupportproject"];
            bool sheetspecwise = Convert.ToBoolean(configdic["sheetspecwise"]);
            string[] gcfsheetarr =  configdic["gcfsheetarr"].Split(',');
            string[] gcfarrBI =  configdic["gcfbandlistforBI"].Split(',');
            string[] gcfarrBIM =  configdic["gcfbandlistforBIM"].Split(',');
            string[] gcfarrBIW =  configdic["gcfbandlistforBIW"].Split(',');
            string[] ptcrbspecarr =  configdic["ptcrbspecarr"].Trim().Split(',');
            string[] ptcrbarrBI =  configdic["ptcrbbandlistforBI"].Trim().Split(',');
            string[] ptcrbarrBIW =  configdic["ptcrbbandlistforBIW"].Trim().Split(',');
            
            _Dictionary<string, string> bandlistdic = MISC.readconfig(bandsupportfile);
            Debug.Print(bandlistdic.ToString());
            Dictionary<string, string[]> prjsupbandlist = MISC.prjsupbl(bandlistdic, bandsupportproject);
            _List<string> allband = new _List<string>();
            _List<string> ulcaband = new _List<string>();
            foreach (KeyValuePair<string,string[]> kvp in prjsupbandlist)
            {
                if (kvp.Key.ToUpper() == "ULCA")
                {
                    ulcaband.AddRange(kvp.Value);
                }else
                {
                    allband.AddRange(kvp.Value);
                }
                lg.deb("key: " + kvp.Key + "  val: " + String.Join("   ",kvp.Value));
            }
            lg.inf("All together" + allband.ToString());
            MISC.PICSBandSupportList = allband;
            Dictionary<string, List<string>> spec_tc_env = new Dictionary<string, List<string>>();
            for (int xl = 0; xl < excelfilearr_env.Count(); xl++)
            {
                string excelfile_env = excelfilearr_env[xl];
                string sheetname_env = sheetnamearr_env[xl];
                string[] filenamepart = excelfile_env.Split('_');
                string spec = filenamepart[0];// +"."+ filenamepart[1];
                Debug.Print("spec:"+spec+":");
                spec_tc_env.Add(spec, new List<string>());
                ParseExcel envpe = new ParseExcel(excelfile_env);
                envpe.lgx = lg;
                DataTable dt_env = envpe.GetExcelData(sheetname_env+"$");
                envpe.processenv(dt_env, spec_tc_env, printlevelforenvcondition);
            }
            foreach (KeyValuePair<string,List<string>> kvp in spec_tc_env)
            {
                lg.war("spec: "+ kvp.Key);
                lg.war("bandlist extreme" +String.Join("#",kvp.Value.ToArray()));

            } 

            
            //ParseExcel envcond = new ParseExcel(excelfile_env);

            Console.WriteLine("This program will parse and process GCF/PTCRB file." );
            Console.WriteLine("First argument is g/p g=> GCF, p=> PTCRB." );
            Console.WriteLine("Second argument is file name");
            Console.WriteLine("========================================================\n\n");
            if (args.Length != 2)
            {
                Console.WriteLine("Please add exactly two argument " );
            }else
            {
                
                string file = args[1];
                if (File.Exists(file))
                {
                    if (args[0].ToLower() == "g")
                    {
                        //code for gcf here. 
                        Console.WriteLine("Now GCF file processing ...");
                        ParseExcel.BIlist = gcfarrBI;
                        ParseExcel.BIlistW = gcfarrBIW;
                        ParseExcel.BIlistM = gcfarrBIM;
                        ParseExcel pex = new ParseExcel(file,"g");

                        pex.lgx = lg;
                        pex.cleanupfolder();
                        //List<string> sheetlist = pex.GetExcelsheetslist();

                        foreach (string sh in gcfsheetarr)
                        {
                            lg.inf("processing Sheet: " + sh);
                            DataTable dt = pex.GetExcelData(sh+"$");
                            pex.processgcffile(dt, spec_tc_env);
                        }
                        pex.writeoutput();
                        Console.WriteLine("Processing done");
                    }
                    else if (args[0].ToLower() == "p")
                    {
                        //code for ptcrb here. 
                        Console.WriteLine("Now PTCRB file processing ...");
                        ParseExcel.BIlist = ptcrbarrBI;
                        ParseExcel.BIlistW = ptcrbarrBIW;
                        ParseExcel pex = new ParseExcel(file,"p");
                        pex.lgx = lg;
                        pex.cleanupfolder();
                        DataTable dt = pex.GetExcelData("Sheet$");
                        pex.processptcrbfile(dt, spec_tc_env, ptcrbspecarr);
                        pex.writeoutput();
                        Console.WriteLine("Processing done");
                    }
                    else
                    {
                        Console.WriteLine("Please use letter g/p as the first argument ");
                    }
                }
                else
                {
                    Console.WriteLine("The file path is invalid");
                    Debug.Print("file not found");
                    lg.cri("file not found");
                }
                
            }

          
        }
    }
}
