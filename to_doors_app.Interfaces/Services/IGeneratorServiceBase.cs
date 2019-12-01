using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers
{
    public interface IGeneratorServiceBase
    {
        void Run();
        List<string> GetSheetNames(string pathToMts);
        void SetSheetName(string sheetName);
    }
}
