using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using to_doors_app.Interfaces.Providers.ExcelProvider;
using _Excel = Microsoft.Office.Interop.Excel;
using _IO = System.IO;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers
{
    public abstract class ExcelProviderBase<T> : IExcelProviderBase<T>
    {
        public event EventHandler<string> ShowWorkProgressEvent;

        /* excel process */
        public _Excel.Application Excel { get; set; } = null;

        /* selected workbook */
        public _Excel.Workbook Workbook { get;  set; } = null;

        /* selected worksheet */
        public _Excel.Worksheet Worksheet { get; set; } = null;

        /* path to excel document*/
        public string Path { get; set; }

        /* abstract method - implementation is not needed in base class */
        public abstract void GetDataOfModules(List<string> moduleNames, ref List<T> modulesToReturn);

        /* kills all processes with name "EXCEL" */
        public void CloseDocument()
        {
            ChangeProgressInfo("Closing excel document");

            Excel.Quit();

            var processes = Process.GetProcessesByName("EXCEL").ToList();

            foreach(var process in processes)
            {
                process.Kill();
            }
        }

        public string GetSwBaseline()
        {
            string swLabel = ReadCell(3, 3);
            swLabel = swLabel.Replace("SW version label: ", "");

            return swLabel;
        }

        /* gets all sheet names in excel file */
        public List<string> GetSheetNames(string path)
        {
            ChangeProgressInfo("Getting sheets names");

            Path = path;
            List<string> sheetNames = new List<string>();

            if (_IO.File.Exists(path))
            {
                Excel = new _Excel.Application();
                Workbook = Excel.Workbooks.Open(Path);
            }

            sheetNames = ReadSheetNames();
            
            ChangeProgressInfo("Done");

            return sheetNames;
        }

        private List<string> ReadSheetNames()
        {
            List<string> sheetNames = new List<string>();
            
            foreach (_Excel.Worksheet sheet in Workbook.Worksheets)
            {
                if (sheet.Name.Contains("diff"))
                    sheetNames.Add(sheet.Name);
            }

            return sheetNames;
        }

        public void SetSheetName(string sheetName)
        {
            Worksheet = (_Excel.Worksheet)Workbook.Worksheets[sheetName];
        }

        protected string GetTrNumber(int moduleRow)
        {
            int trNumberColumn = _Settings.container.TrNumberColumn;

            string trNumber = ReadCell(moduleRow, trNumberColumn);     
            
            if(!trNumber.Equals(string.Empty))
            {
                return trNumber;
            }
            else
            {
                return "--";
            }
        }

        protected bool IsDocumentLoaded()
        {
            return !Excel.Equals(null) ? true : false;
        }

        /* tries to read cell in excel document at specific coordinates */
        protected string ReadCell(int row, int col)
        {
            try
            {
                if(row > 0 && col > 0)
                {    
                    if(((_Excel.Range)Worksheet.Cells[row, col]).Value2 != null)
                    {
                        return ((_Excel.Range)Worksheet.Cells[row, col]).Value2.ToString();
                    }
                }
                else
                {
                    throw new Exception("Excel row or column number was passed as a value lower than 1");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
            return string.Empty;
        }

        protected void ChangeProgressInfo(string message)
        {
            ShowWorkProgressEvent?.Invoke(this, message);
        }
    }
}
