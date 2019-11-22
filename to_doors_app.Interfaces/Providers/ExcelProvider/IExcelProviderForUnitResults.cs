using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers
{
    public interface IExcelProviderForUnitResults : IExcelProviderBase
    {
        List<UnitModule> GetDataOfUnitModules(List<string> moduleNames);
    }
}
