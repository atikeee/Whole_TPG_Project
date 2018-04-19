using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.OleDb;
using System.Diagnostics;
using Utility;
namespace WritingToDB
{
    public class MySqlDb
    {
        public Logging lgsql;
        private bool logenable = true;
        // Holds our connection with the database
        private MySqlConnection dbConnection = new MySqlConnection();
        //public string UserName = "tpgwrite";
        //public string PassWord = "tpgwrite";
        public string ServerName = "";
        //public string ServerName = "10.232.81.133";
        //public string ServerName = "SD-CT-OPIOT.sn.intel.com";
        public string UserName = "";
        public string PassWord = "";
        public string DatabaseName = "testplandb";
        public string dbtablename = "";
        public string storedproc = "insertdata";
        public string tablename;
        
        public List<string> colname = new List<string>() { "st", "tcno", "des", "ba", "gcatp", "gcsdtp", "tcstat", "picsstat", "envcon", "bs", "icebs", "ver", "picsver", "sh" };
        public MySqlDb(string DatabaseName)
        {
            this.DatabaseName = DatabaseName;
            //connectToDatabase();
            readserverfile();

        }

        public MySqlDb()
        {
            //connectToDatabase();
            readserverfile();
        }
        private void readserverfile()
        {
            string line;
            StreamReader f = new StreamReader("server.conf");
            while ((line = f.ReadLine()) != null)
            {

                string[] words = line.Split('=');
                string k = words[0].Trim();
                string v = words[1].Trim();
                if (words.Count() > 1)
                {

                    if(k == "server")
                    {
                        ServerName = v;
                    }
                    else if (k == "user")
                    {
                        UserName= v;
                    }
                    else if (k == "pass")
                    {
                        PassWord= v;

                    }

                }
            }
            Debug.Print(this.ServerName+this.UserName+this.PassWord);
        }
        public void connectToDatabase()
        {

            string connectionString = String.Format("  Data Source={0};Initial Catalog={1};User ID={2};Password={3}", this.ServerName, this.DatabaseName, this.UserName, this.PassWord);
            if (logenable)
                lgsql.deb(" {mysqldb} " + connectionString);
            dbConnection = new MySqlConnection(connectionString);
            try
            {
                dbConnection.Open();
                Debug.Print("Connected to => " + this.DatabaseName);
                if (logenable)
                    lgsql.deb(" {mysqldb}" + "\tConnected to => " + this.DatabaseName);
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Debug.Print("!!! Connection failed !!! " + ex.ToString());
                        if (logenable)
                            lgsql.err(" {mysqldb}" + "!!! Connection failed !!! " + ex.ToString());
                        break;

                    case 1045:
                        Debug.Print(" {mysqldb}" + "!!!  Connection failed => Wrong User/Pass !!! " + ex.ToString());
                        if (logenable)
                            lgsql.err(" {mysqldb}" + "!!!  Connection failed => Wrong User/Pass !!! " + ex.ToString());
                        break;
                }
            }
        }
        //public Dictionary<string,List<string>> tcvsid; // used only the below function
        public object gettblaslist(List<string> colnames, string cond,string idcol = "id#picsmapping")
        {
            //tcvsid = 
            Dictionary<string, List<string>> tcvsid = new Dictionary<string, List<string>>(); ;
            Dictionary<string, List<string>> idvsrow = new Dictionary<string, List<string>>();

            string colname = String.Join("`,`", colnames);
            colname = "`" + colname + "`";
            colname= colname+String.Format(", `{0}` ", idcol) ;
            string sqlCmd;
            if (cond == "" || cond == null)
            {
                sqlCmd = string.Format("SELECT  {0} from {1} ", colname, tablename);
            }
            else
            {
                sqlCmd = string.Format("SELECT  {0} from {1} where {2}", colname, tablename, cond);

            }
            Debug.Print("getdatatable1: " + sqlCmd);
            if (logenable)
                lgsql.deb(" {mysqldb:getdatatble1} sqlcmd: " + sqlCmd + "\tdbname: " + DatabaseName);
            MySqlDataAdapter adr = new MySqlDataAdapter(sqlCmd, dbConnection);
            adr.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)
            foreach (DataRow dr in dt.Rows)
            {
                string idno = dr[idcol].ToString();
                List<string> onerowdata = new List<string>();
                
                foreach(string colstr in colnames)
                {
                    string coldata = dr[colstr].ToString();
                    onerowdata.Add(coldata);
                    if(colstr=="Test Case Number")
                    {
                        if (tcvsid.ContainsKey(coldata))
                        {
                            tcvsid[coldata].Add(idno);
                            

                        }else
                        {
                            tcvsid.Add(coldata,new List<string>() { idno});
                        }
                    }
                }
                if (idvsrow.ContainsKey(idno))
                {
                    lgsql.war("duplicate entry with same colid: "+idno);
                }else
                {
                    idvsrow.Add(idno,onerowdata);
                }
                
            }
            return new List<object>() { tcvsid, idvsrow };
        }

        public DataTable getdatatble(List<string> colnames, string cond)
        {
            string colname = String.Join("`,`", colnames);
            colname = "`" + colname + "`";
            string sqlCmd;
            if (cond == "" || cond == null)
            {
                sqlCmd = string.Format("SELECT  {0} from {1} ", colname, tablename);
            }
            else
            {
                sqlCmd = string.Format("SELECT  {0} from {1} where {2}", colname, tablename, cond);

            }
            Debug.Print("getdatatable1: "+sqlCmd);
            if (logenable)
                lgsql.deb(" {mysqldb:getdatatble1} sqlcmd: " + sqlCmd + "\tdbname: " + DatabaseName);
            MySqlDataAdapter adr = new MySqlDataAdapter(sqlCmd, dbConnection);
            adr.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)
            return dt;
        }
        public DataTable getdatatble( string cond="")
        {
            string sqlCmd;
            if (cond == "" || cond == null)
            {
                sqlCmd = string.Format("SELECT  * from {0} ", tablename);
            }
            else
            {
                sqlCmd = string.Format("SELECT  * from {0} where {1}", tablename, cond);

            }
            Debug.Print("getdatatable2: " + sqlCmd);
            if (logenable)
                lgsql.deb(" {mysqldb:getdatatble2} sqlcmd: " + sqlCmd + "\tdbname: " + DatabaseName);
            MySqlDataAdapter adr = new MySqlDataAdapter(sqlCmd, dbConnection);
            adr.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)
            return dt;

        }
        public DataTable getDtFromSqlSt(string sqlCmd)
        {
            MySqlDataAdapter adr = new MySqlDataAdapter(sqlCmd, dbConnection);
            if (logenable)
                lgsql.deb(" {mysqldb:getdatatble2} sqlcmd: " + sqlCmd + "\tdbname: " + DatabaseName);
            adr.SelectCommand.CommandTimeout = 500;
            adr.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)
            return dt;
        }
        public List<string> getuniqueitem(string colname, string cond)
        {
            List<string> items = new List<string>();
            string sqlCmd;
            if (cond == "")
            {
                sqlCmd = string.Format("SELECT distinct {0} from {1}", colname, tablename);
            }
            else
            {

                sqlCmd = string.Format("SELECT distinct {0} from {1} WHERE {2}", colname, tablename, cond);
            }
            Debug.Print("SQL STATEMENT :" + sqlCmd + ":" + DatabaseName);
            if(logenable)
                lgsql.deb(" {mysqldb:getuniqueitem}" + sqlCmd + "\tdbname: " + DatabaseName);
            MySqlCommand sqlcmd = new MySqlCommand(sqlCmd, dbConnection);
            MySqlDataReader reader = sqlcmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(reader[0].ToString());
            }
            reader.Close();
            return items;
        }
        public int getcount(string colname, string colvalue)
        {
            string sqlCmd = string.Format("(SELECT COUNT(*) FROM {0} WHERE {1}= \"{2}\")", tablename, colname, colvalue);
            if (logenable)
                lgsql.deb(" {mysqldb:getdatatble2} sqlcmd: " + sqlCmd + "\tdbname: " + DatabaseName);
            MySqlCommand check_combconfig = new MySqlCommand(sqlCmd, dbConnection);
            string configexist = check_combconfig.ExecuteScalar().ToString();
            return Int32.Parse(configexist);
        }
        public void deleterows(string cond)
        {

            string sqlCmd = string.Format("DELETE FROM {0} WHERE {1}", tablename, cond);
            if (logenable)
                lgsql.deb(" {mysqldb:getdatatble2} sqlcmd: " + sqlCmd + "\tdbname: " + DatabaseName);
            MySqlCommand check_combconfig = new MySqlCommand(sqlCmd, dbConnection);
            check_combconfig.ExecuteNonQuery().ToString();
        }
        public void deletedatafromalltable(string ver)
        {
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = dbConnection;
            sqlCmd.CommandText = "deletetcinfo";
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("v", ver);
            sqlCmd.Parameters["v"].Direction = ParameterDirection.Input;
            sqlCmd.ExecuteNonQuery();
        }
        public int deletepicsdata( string gcfver, string picsver)
        {
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = dbConnection;
            sqlCmd.CommandText = "delete_picsdata";
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("gv1", gcfver);
            sqlCmd.Parameters.AddWithValue("pv2", picsver);
            sqlCmd.Parameters["gv1"].Direction = ParameterDirection.Input;
            sqlCmd.Parameters["pv2"].Direction = ParameterDirection.Input;
            return sqlCmd.ExecuteNonQuery();
            

        }
        public string getid_str(DataTable dttbl, Dictionary<string, string> toMatch,  string idname = "id")
        {
            string id = "-1";
            foreach (DataRow dr in dttbl.Rows)
            {
                bool match = true;
                foreach (KeyValuePair<string, string> kvp in toMatch)
                {
                   
                    if (!String.Equals(kvp.Value,dr[kvp.Key].ToString()))
                    {
                        match = false;
                        //lgsql.deb(":"+kvp.Value+":"+dr[kvp.Key].ToString()+":");
                        break;
                    }
                }
                if (match)
                {
                    id = dr[idname].ToString();
                    break;
                }


            }

            return id;
        }
        public int getid(string idname, string cond)
        {
            string sqlCmd = String.Format("SELECT {0} FROM {1} WHERE({2})", idname, tablename, cond);
            if (logenable)
                lgsql.deb("{mysqldb:getid:condonly}" + sqlCmd);
            MySqlCommand sqlcmd = new MySqlCommand(sqlCmd, dbConnection);
            //check_combconfig.ExecuteNonQuery();
            Debug.Print(sqlCmd);
            try
            {
                string rowid = sqlcmd.ExecuteScalar().ToString();
                return Int32.Parse(rowid);
            }
            catch
            {
                return -1;
            }

        }

        public string getid_str(string idname, string cond)
        {
            string sqlCmd = String.Format("SELECT {0} FROM {1} WHERE({2})", idname, tablename, cond);
            MySqlCommand sqlcmd = new MySqlCommand(sqlCmd, dbConnection);
            //check_combconfig.ExecuteNonQuery();
            Debug.Print(sqlCmd);
            string rowid;
            try
            {
                rowid = sqlcmd.ExecuteScalar().ToString();
               
            }
            catch
            {
                rowid= "-1";
            }
            if (logenable)
                lgsql.inf("{mysqldb: getid_str}" + sqlCmd+": rowid: "+rowid.ToString());
            return rowid;
        }

        public string getvalfromkey(string colname,string keyval, string keyname = "id")
        {
            string val = "";
            string sqlCmd = String.Format("SELECT {0} FROM {1} WHERE({2}={3})", colname, tablename,keyname,keyval );
            MySqlCommand sqlcmd = new MySqlCommand(sqlCmd, dbConnection);
            //check_combconfig.ExecuteNonQuery();
            //Debug.Print(sqlCmd);
            //string rowid;
            try
            {
                val = sqlcmd.ExecuteScalar().ToString();

            }
            catch
            {
                val = "";
            }
            if (logenable)
                lgsql.inf("{mysqldb: getcol_fromid}" + sqlCmd + ": rowid: " + val.ToString());
            return val; 
        }

        public void insertfile(string fn, string colsstr)
        {
            string sqlCmd = String.Format(@"load data local infile '{0}' into table {1} FIELDS TERMINATED BY '\t' LINES  TERMINATED BY '\n'({2}); ", fn, tablename, colsstr);
            Debug.Print(sqlCmd);
            if(logenable)
                lgsql.deb("{mysqldb:insertfile} " + sqlCmd);
            MySqlCommand cmd = new MySqlCommand(sqlCmd, dbConnection);
            cmd.ExecuteNonQuery();
        }
        public void insertdata(List<string> colvalue, List<string> insertcol)
        {
            string val = String.Join("','", colvalue);
            val = "'" + val + "'";

            string colname = String.Join("`,`", insertcol);
            colname = "`" + colname + "`";

            string sqlstring = String.Format("insert ignore into {0} ({1}) values ({2})", tablename, colname, val);
            //Debug.Print("{mysqldata:insertdata} " + sqlstring);
            if(logenable)
                lgsql.deb("{mysqldata:insertdata} " + sqlstring);
            MySqlCommand sqlcmd = new MySqlCommand(sqlstring, dbConnection);
            sqlcmd.ExecuteNonQuery();
        }

        public void insertdatatoalltable(List<string> wholerow)
        //public void insertdatatoalltable(string st, string tcno, string des, string ba, string sh, string id_tcconfig, string gcatp, string gcsdtp, string tcstat, string picsstat, string envcon, string bs, string icebs, string ver, string picver)
        {

            MySqlCommand sqlcmd = new MySqlCommand();
            sqlcmd.Connection = dbConnection;
            sqlcmd.CommandText = "insertdata";
            sqlcmd.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < colname.Count; i++)
            {
                sqlcmd.Parameters.AddWithValue(colname[i], wholerow[i]);
                sqlcmd.Parameters[colname[i]].Direction = ParameterDirection.Input;
            }

            sqlcmd.ExecuteNonQuery();
        }
        public void insertorupdate(string picsver, string bs)
        //public void insertdatatoalltable(string st, string tcno, string des, string ba, string sh, string id_tcconfig, string gcatp, string gcsdtp, string tcstat, string picsstat, string envcon, string bs, string icebs, string ver, string picver)
        {

            MySqlCommand sqlcmd = new MySqlCommand();
            sqlcmd.Connection = dbConnection;
            sqlcmd.CommandText = "insertorupdate";
            sqlcmd.CommandType = CommandType.StoredProcedure;

            sqlcmd.Parameters.AddWithValue("p1", picsver);
            sqlcmd.Parameters["p1"].Direction = ParameterDirection.Input;
            sqlcmd.Parameters.AddWithValue("p2", bs);
            sqlcmd.Parameters["p2"].Direction = ParameterDirection.Input;

            sqlcmd.ExecuteNonQuery();
        }
        public void insertorupdateicebandinfo(string iceproj, string bandlist, string bandlist_ulca)
        {
            string sqlstring = String.Format("insert ignore into {0} (iceproj,bandlist,bandlist_ulca) values (\"{1}\",\"{2}\",\"{3}\" ) ON DUPLICATE KEY UPDATE bandlist = \"{2}\",bandlist_ulca=\"{3}\"", tablename,  iceproj,bandlist,bandlist_ulca);
            Debug.Print("{mysqldata:insertdata} " + sqlstring);
            if (logenable)
                lgsql.deb("{mysqldata:insertdata} " + sqlstring);
            MySqlCommand sqlcmd = new MySqlCommand(sqlstring, dbConnection);
            sqlcmd.ExecuteNonQuery();
            

        }
        public static List<string> GetExcelsheetslist(string _FilePath, string Condition = "")
        {

            string excelConnString = @"provider=Microsoft.ACE.OLEDB.12.0;data source=" + _FilePath + ";extended properties=" + "\"excel 12.0;hdr=yes;IMEX=1\"";

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


        public static DataTable GetExcelData(string _FilePath, string ExcelSheetName = "", string Condition = "")
        {
            DataTable tbl = new DataTable();
            string excelConnString = @"provider=Microsoft.ACE.OLEDB.12.0;data source=" + _FilePath + ";extended properties=" + "\"excel 12.0;hdr=yes;IMEX=1\"";
            //Create Connection to Excel work book 
            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                ///----------------------------
                string sql = "Select * " + " from [" + ExcelSheetName + "] ";
                if (Condition.Trim().Length > 0)
                {
                    sql += Condition; 
                }
                OleDbDataAdapter da = new OleDbDataAdapter(sql, excelConnection);
                da.Fill(tbl);
            }
            return tbl;
        }
        public void closedb()
        {
            try
            {
                dbConnection.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void changedb()
        {
            Debug.Print("db changed to -> " + DatabaseName);
            dbConnection.ChangeDatabase(DatabaseName);
        }
        public bool getconnectionstat()
        {
            bool status = false;
            try
            {

                ConnectionState state = dbConnection.State;

                if (state == ConnectionState.Open)
                {
                    status = true;
                }
            }
            catch (Exception E)
            {
                status = false;
            }
            return status;
        }
    }
}
