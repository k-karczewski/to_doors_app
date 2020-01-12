using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using _Excel = Microsoft.Office.Interop.Excel;

namespace to_doors_app.Interfaces.Providers.ExcelProvider
{
    public interface IExcelProviderBase<T>
    {
        event EventHandler<string> ShowWorkProgressEvent;

        // excel process 
        _Excel.Application Excel { get; set; }

        // selected workbook
        _Excel.Workbook Workbook { get; set; }

        // selected worksheet 
        _Excel.Worksheet Worksheet { get; set; }

        // path to excel document
        public string Path { get; set; }

        // abstract method - implementation is not needed in base class 
        public abstract void GetDataOfModules(List<string> moduleNames, ref List<T> modulesToReturn);

        // kills all processes with name "EXCEL" 
        void CloseDocument();

        // gets sw baseline
        string GetSwBaseline();

        // gets all sheet names in excel file 
        List<string> GetSheetNames (string path);

        // sets sheet name to be used
        void SetSheetName(string sheetName);

    }
}
