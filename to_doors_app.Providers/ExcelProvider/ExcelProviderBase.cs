using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;
using _Excel = Microsoft.Office.Interop.Excel;
using _IO = System.IO;

namespace to_doors_app.Providers
{
    public class ExcelProviderBase : IExcelProviderBase
    {
        /* excel process */
        public _Excel.Application Excel { get; } = null;

        /* selected workbook */
        public _Excel.Workbook Workbook { get; } = null;

        /* selected worksheet */
        public _Excel.Worksheet Worksheet { get; } = null;

        /* path to excel document*/
        public string Path { get; set; }

        /* constructor */
        public ExcelProviderBase(string path, string sheetName)
        {
            Path = _IO.Directory.GetCurrentDirectory() + path;

            try
            {
                if(_IO.File.Exists(Path))
                {
                    Excel = new _Excel.Application();
                    Workbook = Excel.Workbooks.Open(Path);
                    Worksheet = (_Excel.Worksheet)Workbook.Worksheets[sheetName];
                }
            }
            catch(Exception ex)
            {
                /*MessageBox.Show($"Error during opening Mts excel sheet: {ex.Message}", "Error");*/
            }
        }

        /* kills all processes with name "EXCEL" */
        public virtual void CloseDocument()
        {
            Excel.Quit();

            var processes = Process.GetProcessesByName("EXCEL").ToList();

            foreach(var process in processes)
            {
                process.Kill();
            }
        }

        /* method not needed in base class */
        public virtual List<Module> GetDataOfAllUnitModules()
        {
            throw new NotImplementedException();
        }

        /* method not needed in base class */
        public virtual List<UnitModule> GetDataOfUnitModules(List<string> moduleNames)
        {
            throw new NotImplementedException();
        }
        
        /* method not needed in base class */
        public virtual List<IntegrationModule> GetDataOfIntegrationModules(List<string> moduleNames, string sheetName)
        {
            throw new NotImplementedException();
        }

        protected List<File> GetFilesInModule(int moduleRow, bool areFunctionsNeeded)
        {
            List<File> files = new List<File>();

            int fileRow = moduleRow + 1;
            int col = 1;

            string fileName;

            /* search for .c files*/
            while (ReadCell(fileRow, col).Equals(string.Empty))
            {
                fileName = ReadCell(fileRow, col + 2);

                /* if file found */
                if (!fileName.Equals(string.Empty) && fileName.Contains(".c"))
                {
                    fileName = fileName.Replace(".c", "");

                    string fileRevision = ReadCell(fileRow, col + 3);

                    List<Function> functions = null;
                    
                    if(areFunctionsNeeded)
                    {
                        functions = GetFunctionsOfFile(fileRow);
                    }

                    files.Add(new File(fileName, fileRevision, functions));
                }

                fileRow++;
            }

            return files;
        }

        private List<Function> GetFunctionsOfFile(int functionRow)
        {
            List<Function> functions = new List<Function>();

            do
            {
                functions.Add(new Function(ReadCell(functionRow, 5)));
                functionRow++;
            }
            while (ReadCell(functionRow + 1, 3).Equals(string.Empty));

            return functions;
        }

        public virtual string GetSwBaseline()
        {
            string swLabel = ReadCell(3, 3);
            swLabel = swLabel.Replace("SW version label: ", "");

            return swLabel;
        }

        /* gets all sheet names in excel file */
        public virtual List<string> GetSheetNames()
        {
            List<string> sheetNames = new List<string>();

            foreach(_Excel.Worksheet sheet in Workbook.Worksheets)
            {
                sheetNames.Add(sheet.Name);
            }

            return sheetNames;
        }

        public virtual string GetTrNumber(int moduleRow)
        {
            int trNumberColumn = 55; /* stub until settings class is implemented */

            string trNumber = ReadCell(moduleRow, trNumberColumn);     
            
            if(!trNumber.Equals(string.Empty))
            {
                return trNumber;
            }
            else
            {
                return "0";
            }
        }

        public virtual bool IsDocumentLoaded()
        {
            return !Excel.Equals(null) ? true : false;
        }

        /* tries to read cell in excel document at specific coordinates */
        public virtual string ReadCell(int row, int col)
        {
            try
            {
                if(row > 0 && col > 0)
                {    
                    if(((_Excel.Range)Worksheet.Cells[row, col]).Value2 != null)
                    {
                        return ((_Excel.Range)Worksheet.Cells[row, col]).Value2.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    throw new Exception("Excel row or column number was passed as a value lower than 1");
                }
            }
            catch(Exception ex)
            {
                return string.Empty;
                /*MessageBox.Show($"{ex.Message}", "Error");*/
            }
        }
    }
}
