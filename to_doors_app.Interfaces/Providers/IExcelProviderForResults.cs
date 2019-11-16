using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers
{
    public interface IExcelProviderForResults : IExcelProviderBase
    {
        List<Module> GetDataOfModules(List<string> moduleNames, bool areFilesNeeded);
    }
}
