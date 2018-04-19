using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.Data.OleDb;
using xl = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace TestPlanGen
{
    class ExcelProcess
    {
        // this file name should be complete path. 
        private string FileName = "";
        private xl.Application oXL;
        private xl._Workbook oWB;
        public ExcelProcess(string fn, bool vis = true, bool DisplayAlerts = true)
        {
            FileName = fn;
            oXL = new xl.Application() ; 
            oWB= (xl._Workbook)(oXL.Workbooks.Add("")); 
            oXL.Visible = vis;
            oXL.UserControl = false;
            oXL.DisplayAlerts = DisplayAlerts;


        }

        //public void AddWorkSheet(string sheetName)
        //{
        //    xl.Worksheet newWorkSheet;
        //    newWorkSheet = (xl.Worksheet)Globals.ThisWorkbook.Worksheets.Add();
        //}


        public  void CreateExcelFileFromDataTableOriginal(DataTable tbl)
        {
            xl._Worksheet oSheet;
            
            xl.Range oRng;
            object misvalue = System.Reflection.Missing.Value;
            oSheet = (xl._Worksheet)oWB.ActiveSheet;
            int idxpp = 1;
            int columnCount = 0;
            List<string> listColumn = new List<string>();
            foreach (DataColumn column in tbl.Columns)
            {
                columnCount += 1;
                listColumn.Add(column.ToString());
            }
            string columnCountString;
            columnCountString = GetExcelColumnName(columnCount);

            foreach (String val in listColumn)
            {
                oSheet.Cells[1, idxpp] = val;
                idxpp += 1;
            }
            oSheet.get_Range("A1", String.Format("{0}1", columnCountString)).Font.Bold = true;
            oSheet.get_Range("A1", String.Format("{0}1", columnCountString)).VerticalAlignment =
               xl.XlVAlign.xlVAlignCenter;

            // Create an array to multiple values at once.
            var totalRow = tbl.Rows.Count;
            int rowno = 0;

            string[,] saNames = new string[totalRow, columnCount];
            Debug.Print("totalrow" + totalRow.ToString());
            if (tbl.Rows.Count > 0)
            {
                foreach (DataRow dr in tbl.Rows)
                {

                    for (int idx = 0; idx < columnCount; idx += 1)
                    {
                        saNames[rowno, idx] = dr[idx].ToString();
                    }
                    rowno += 1;
                }

                //Debug.Print("rowcount: "+tbl.Rows.Count+ " colcount: "+tbl.Columns.Count+" columncount: "+columnCountString +" total row: "+totalRow);
                //Debug.Print(string.Format("{0}{1}", columnCountString, totalRow));
                oSheet.get_Range("A2", string.Format("{0}{1}", columnCountString, totalRow+1)).Value2 = saNames;
            }
            oRng = oSheet.get_Range("A1", string.Format("{0}1", columnCountString));
            oRng.EntireColumn.AutoFit();
            

            oWB.SaveAs(FileName, xl.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, xl.XlSaveAsAccessMode.xlNoChange);

            oWB.Close();
            oXL = null;
        }

        // Start: Function for Excel WorkBook and WorkSheet Creation:


        //public void CreateExcelFileFromDataTable(DataTable tbl)
        public void converttoexcelmultisheet(DataTable tbl,int sheetidx)
        {
            
            Dictionary<string, object> alldata = new Dictionary<string, object>();
            int idxpp = 1;
            int columnCount = 0;
            List<string> datac = new List<string>();
            foreach (DataColumn column in tbl.Columns)
            {
                columnCount += 1;
                datac.Add(column.ToString());
            }
            
            List<List<string>> newlist;
            if (tbl.Rows.Count > 0)
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    DataRow myRow = tbl.Rows[i];
                    string dickey= myRow[sheetidx].ToString();
                    
                    if (alldata.ContainsKey(myRow[sheetidx].ToString()))
                    {
                        newlist = (List<List<string>>)alldata[dickey] ;
                    }
                    else
                    {
                        newlist = new List<List<string>>();
                    }
                    List<string> oneline = new List<string>();
                    for (int j = 0; j < tbl.Columns.Count; j++)
                    {
                        oneline.Add(myRow[j].ToString());
                    }
                    if (dickey == "tty")
                    {
                        Debug.Print("line " + string.Join(" | ", oneline));
                    }
                    newlist.Add(oneline);
                    alldata[dickey] = newlist;
                }

            
            }
            foreach (KeyValuePair<string, object> kvp in alldata)
            {
                string shname = kvp.Key;
                
                List<List<string>> datarc = (List<List<string>>)kvp.Value;
                string[,] stringarr = new string[datarc.Count,datac.Count];
                int i =0; 
                int j =0; 
                foreach(List<string>dr in datarc)
                {
                    j = 0;
                    foreach(string d in dr)
                    {
                        stringarr[i, j] = d;
                        j++;
                    }
                    i++;
                }
                writeToExcelList(stringarr, datac, shname);
            }
            
        }
        // End: Function for Excel WorkBook and WorkSheet Creation:

        public void writeToExcelList(string[,] datarc, List<string> datac, string shname)
        {
            object missing = Type.Missing;
            xl.Range oRng;
            object misvalue = System.Reflection.Missing.Value;
            xl._Worksheet oSheet = oWB.Sheets.Add(missing, missing, 1, missing)
                            as xl._Worksheet;
            oSheet.Name = shname;
            int idxpp = 1;
            string columnCountString = GetExcelColumnName(datac.Count);

            foreach (String val in datac)
            {
                oSheet.Cells[1, idxpp] = val;
                idxpp += 1;
            }
            oSheet.get_Range("A1", String.Format("{0}1", columnCountString)).Font.Bold = true;
            oSheet.get_Range("A1", String.Format("{0}1", columnCountString)).VerticalAlignment = xl.XlVAlign.xlVAlignCenter;
            string lastrc = string.Format("{0}{1}", columnCountString, (datarc.GetLength(0)+1).ToString());
            oSheet.get_Range("A2", lastrc).Value2 = datarc;
            oRng = oSheet.get_Range("A1", string.Format("{0}1", columnCountString));
            oRng.EntireColumn.AutoFit();
        }
        public void savefile()
        {
            try
            {
                oWB.SaveAs(FileName, xl.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    false, false, xl.XlSaveAsAccessMode.xlNoChange);
                oWB.Close();

            }
            catch(Exception ex)
            {
                MessageBox.Show("Please remove the existing file or save with different name.");
            }
            oXL.Quit();

            oXL = null;
        }
        
        public string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        







    }
}
