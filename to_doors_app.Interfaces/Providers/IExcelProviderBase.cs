using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers
{
    public interface IExcelProviderBase
    {
        bool IsDocumentLoaded();
        
        List<string> GetSheetNames();

        string GetTrNumber(int moduleRow);

        string GetSwBaseline();

        string ReadCell(int row, int col);

        void CloseDocument();
    }
}
