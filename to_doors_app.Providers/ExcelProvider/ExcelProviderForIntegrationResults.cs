using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Interfaces.Providers.ExcelProvider;
using to_doors_app.Models;

namespace to_doors_app.Providers.ExcelProvider
{
    public class ExcelProviderForIntegrationResults: ExcelProviderBase, IExcelProviderForIntegrationResults
    {
        public ExcelProviderForIntegrationResults(string path, string sheetName): base(path, sheetName) { }

        public override List<IntegrationModule> GetDataOfIntegrationModules(List<string> moduleNames, string sheetName)
        {
            List<IntegrationModule> modulesToReturn = new List<IntegrationModule>();
            /* start search from const coordinates*/
            int row = 12;
            const int col = 1;

            while (ReadCell(row, col) != "Total:")
            {
                string currentModuleName = ReadCell(row, col);

                if (!currentModuleName.Equals(string.Empty))
                {
                    if (moduleNames.Contains(currentModuleName))
                    {
                        string moduleBaseline = ReadCell(row, col + 1);

                        string trNumber = GetTrNumber(row);

                        modulesToReturn.Add(new IntegrationModule(currentModuleName, moduleBaseline, trNumber, row, col));

                        if (modulesToReturn.Count >= moduleNames.Count)
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
