using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
namespace XLReadWrite
{
      public class XLRdWr
    {
        private string fn ;
        private Excel.Application oXL;
        private Excel.Workbook oWB;
        public Excel.Worksheet oWS;
        private string dirname ;
        public XLRdWr()
        {
            //fn = filepath;
            oXL = new Excel.Application();

        }
        public XLRdWr(string filepath)
        {
            fn = filepath;
            oXL = new Excel.Application();
            oXL.Visible = true; 

        }
        public void addbook()
        {
            oWB = oXL.Workbooks.Add(Missing.Value);
        }

        //private Excel.Application 
        public void saveas(string newfn)
        {
            oWB.SaveAs(newfn, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            closefile();
        }
        public void save()
        {
            oWB.Save();

            //closefile();
        }
        public void createExcelFile()
        {


        }
        public void setsheet(int i)
        {
            oWS = oWB.Worksheets[i] as Excel.Worksheet;
            
        }
        public void setsheet(string sh)
        {
            oWS = oWB.Worksheets[sh] as Excel.Worksheet;
        }
        public void closefile()
        {

            oWB.Close(true, Missing.Value, Missing.Value);
            oXL = null;
        }
        public void openfile()
        {
            oWB = oXL.Workbooks.Open(fn, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            //read Excel sheets 
            
        }
        public void write(int i , int j, string val)
        {
            oWS.Cells[i, j] = val;
        }
        public void write(string i, int j, string val)
        {
            oWS.Cells[i+j.ToString()] = val;
        }
        public void write(int i, int j)
        {
            var cell1 = oWS.Cells[i, j];
            var cell2 = oWS.Cells[i+1,j+1];
            //Excel.Range r = { 1, 2, 3, 4, 5, 6 };
            oWS.Range["A1:A10"].Value = "a";
            oWS.Range["B1:B10"].Value2 = "b";
            {

            }
           // Range r= oWS.Range[cell1, cell2];

        }
        public string[,] getrange(string fcell, int r, int c)
        {
            Debug.Print(fcell + " i: "+r + " j: "+c);
            string[,] res = new string[r,c];
            if (r == 0)
                return res;
            //int rr = res.GetLength(0);
            //int cc = res.GetLength(1);
            //oWS.Cells[fcell];
            //oWS.Range[fcell].Select();
            var arr = oWS.Range[fcell].Resize[r,c].Value2;
            for(int i = 1; i<=r; i++)
            {
                for (int j = 1; j <= c; j++)
                {
                    string s = arr[i, j] as string;
                    res[i-1, j-1] = s;
                }
            }
            return res;
        }
        public void setrangeval(string fcell, string[,] val)
        {
            int r = val.GetLength(0);
            int c = val.GetLength(1);
            if (r == 0)
                return;
            Debug.Print("set: "+fcell + " i: " + r + " j: " + c);
            oWS.Range[fcell].Select();
            oWS.Range[fcell].Resize[r, c].Value2 = val;
        }
        public Excel.Range get_lastrange()
        {
            return oWS.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
        }
        public List<string> getsheets()
        {
            List<string> allsheet = new List<string>();
            foreach (Excel.Worksheet worksheet in oWB.Worksheets)
            {
                allsheet.Add(worksheet.Name);
                Debug.Print("Sheetname: " + worksheet.Name);

            }
            return allsheet;
        }
        public int getlastrow()
        {
            return get_lastrange().Row;
        }
        public int getlastcol()
        {
            return get_lastrange().Column;
        }
        //public getcol(string fcell, int r)
        //{
        //    var arr = oWS.Range[fcell].Resize[r, 1].Value;
        //    for (int i = 1; i <= r; i++)
        //    {
        //        var txt = arr[i, 1] as string;
        //        Debug.Print(i.ToString() + " " + txt);
        //    }
        //}
        public void writeExcelFile()
        {
            //oWS = oWB.Worksheets[1] as Excel.Worksheet;

            //rename the Sheet name 
            oWS.Name = "testsheet";

            for (int i = 1; i < 1000; i++)
            {
                oWS.Cells[i, 6] = "7 " + i.ToString();
            }
         
        }
        public void update()
        {
            //String sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Temp\Book1.xls;Extended Properties='Excel 8.0;HDR=NO'";
            //OleDbConnection objConn = new OleDbConnection(sConnectionString);
            //objConn.Open();
            //OleDbCommand objCmdSelect = new OleDbCommand("UPDATE [Sheet1$A2:A2] SET F1=123456", objConn);
            //
            //
            //objCmdSelect.ExecuteNonQuery();
            //objConn.Close();

            String sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Temp\Book1.xls;Extended Properties='Excel 8.0;HDR=NO'";
            OleDbConnection objConn = new OleDbConnection(sConnectionString);
            objConn.Open();
            OleDbCommand objCmdSelect = new OleDbCommand("UPDATE [Sheet1$A2:A2] SET F1=123456", objConn);
            objCmdSelect.ExecuteNonQuery();
            objCmdSelect = new OleDbCommand("UPDATE [Sheet1$A4:A4] SET F1='hello'", objConn);
            objCmdSelect.ExecuteNonQuery();
            objCmdSelect = new OleDbCommand("UPDATE [Sheet1$A7:A8] SET F1=123456, F2='hello'", objConn);
            objCmdSelect.ExecuteNonQuery();
            objConn.Close();
        }

        public void readExcelFile()
        {
           oWS = oWB.Worksheets[1] as Excel.Worksheet;

            Excel.Range range;

            range = oWS.UsedRange;

            //read first row, first cell value 
        }
    }
}
