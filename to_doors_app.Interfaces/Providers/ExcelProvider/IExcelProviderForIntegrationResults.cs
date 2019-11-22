using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers.ExcelProvider
{
    public interface IExcelProviderForIntegrationResults
    {
        List<IntegrationModule> GetDataOfIntegrationModules(List<string> moduleNames, string sheetName);
    }
}
