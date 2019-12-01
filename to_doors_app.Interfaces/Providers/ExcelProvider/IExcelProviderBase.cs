using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers
{
    public interface IExcelProviderBase<T>
    {
        List<string> GetSheetNames(string path);
        void SetSheetName(string sheetName);
        void GetDataOfModules(List<string> moduleNames, ref List<T> modulesToReturn);
        void GetDataOfModules(ref List<T> allModulesToReturn);
        void CloseDocument();
    }
}
