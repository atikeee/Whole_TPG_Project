using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using XLReadWrite;
using System.Data;

namespace MTP_Project1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string folderPath = Application.StartupPath;
            //string fileName = "CT1_PSRRM_SD_XMM7480_Test_Request.xlsb";
            //string fullPathSource = Path.Combine(folderPath, fileName);
            //
            //string fullPathDest = Path.Combine(folderPath, fileName);
            //
            //writeExcelFile(fullPathSource, fullPathDest);
            string fullPathSource = Path.Combine(Application.StartupPath, "test.xlsx");
            XLRdWr xlrw = new XLRdWr();
            /*
            xlrw.addbook();
            xlrw.setsheet(1);
            //DataTable dt = new DataTable();
            //dt.Columns.Add("a", typeof(string));
            //dt.Columns.Add("b", typeof(string));
            //dt.Rows.Add("1","2");
            //dt.Rows.Add("11", "22");
            //string[] a = { "1", "2" };
            //string[] b = { "11", "22" };
            //string[][] c = { a, b };

            xlrw.writeExcelFile();
            xlrw.save(fullPathSource);
            */
            xlrw.update();
        }


        public void createExcelFile()
        {
            Excel.Application oXL = new Excel.Application();

            Excel.Workbook oWB = oXL.Workbooks.Add(Missing.Value);

            oWB.SaveAs(Application.StartupPath + "\\PROJEKTSTATUS_GESAMT_neues_Layout.xlsm", Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            oWB.Close(true, Missing.Value, Missing.Value);
        }

        public void openExcelFile()
        {
            Excel.Application oXL = new Excel.Application();

            Excel.Workbook oWB = oXL.Workbooks.Open(Application.StartupPath + "PROJEKTSTATUS_GESAMT_neues_Layout.xlsm", Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            //read Excel sheets 
            foreach (Excel.Worksheet ws in oWB.Sheets)
            {
                MessageBox.Show(ws.Name);
            }

            //save as separate copy 
            oWB.SaveAs(Application.StartupPath + "\\PROJEKTSTATUS_GESAMT_neues_Layout_neu.xlsm", Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            oWB.Close(true, Missing.Value, Missing.Value);
        }

        public void writeExcelFile(string fullPathSource, string fullPathDest)
        {
            Excel.Application oXL = new Excel.Application();
            Excel.Workbook oWB = oXL.Workbooks.Open(fullPathSource, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            Excel.Worksheet oWS = oWB.Worksheets[1] as Excel.Worksheet;

            //rename the Sheet name 
            oWS.Name = "CT1_CT_Results_PS_1";

            for (int i = 1; i < 10; i++)
            {
                oWS.Cells[i, 2] = "Imtiaz Atiq Cell: " + i.ToString();
            }
            oWB.SaveAs(fullPathDest, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            Process.Start(fullPathDest);
        }

        public void readExcelFile()
        {
            Excel.Application oXL = new Excel.Application();

            Excel.Workbook oWB = oXL.Workbooks.Open(Application.StartupPath + "\\PROJEKTSTATUS_GESAMT_neues_Layout.xlsm", Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            Excel.Worksheet oWS = oWB.Worksheets[1] as Excel.Worksheet;

            Excel.Range range;

            range = oWS.UsedRange;

            //read first row, first cell value 
            MessageBox.Show((string)(range.Cells[1, 1] as Excel.Range).Value2);
        }



    }
}
