using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
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
}
