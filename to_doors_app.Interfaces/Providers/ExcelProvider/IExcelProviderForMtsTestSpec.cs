using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers
{
    public interface IExcelProviderForMtsTestSpec : IExcelProviderBase
    {
        List<Module> GetDataOfAllUnitModules();
    }
}
