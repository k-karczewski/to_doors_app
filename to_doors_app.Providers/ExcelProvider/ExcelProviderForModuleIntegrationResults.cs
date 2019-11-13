using System.Collections.Generic;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;

namespace to_doors_app.Providers
{
    public class ExcelProviderForModuleIntegrationResults : ExcelProviderBase, IExcelProviderForTestResults
    {
        public ExcelProviderForModuleIntegrationResults(string path, string sheetName) : base(path, sheetName) { }

        public override List<Module> GetDataOfModules(List<string> moduleNames)
        {
            List<Module> modulesToReturn = new List<Module>();

            /* start search from const coordinates*/
            int row = 12;
            const int col = 1;

            while (ReadCell(row, col) != "Total:")
            {
                string currentModuleName = ReadCell(row, col);

                if (!currentModuleName.Equals(string.Empty))
                {
                    if(moduleNames.Contains(currentModuleName))
                    {
                        string moduleBaseline = ReadCell(row, col + 1);

                        string trNumber = GetTrNumber(row);

                        modulesToReturn.Add(new Module(currentModuleName, moduleBaseline, trNumber, row, col));

                        if(modulesToReturn.Count >= moduleNames.Count)
                        {
                            break;
                        }
                    }
                }
                row++;
            }
            return modulesToReturn;
        }
    }
}
