using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

namespace Utility
{

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
            return "[ "+toCommaList()+" ]";
        }
        public  string toCommaList()
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
        public _List(int i) : base(i)
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
        public bool chkmatch(List<T> otherlist,List<int> ind)
        {
            if (this.Count != otherlist.Count)
                return false;
            //for (int i = 0; i < this.Count; i++)
            foreach(int i in ind)
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
        public bool chkmatch(List<_List<T>>listoflist, List<int> ind)
        {
            foreach(_List<T> onelist in listoflist)
            {
                bool oneMatch = this.chkmatch(onelist,ind);
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


}
