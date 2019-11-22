using System.Collections.Generic;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;

namespace to_doors_app.Providers
{
    public class ExcelProviderForUnitResults : ExcelProviderBase, IExcelProviderForUnitResults
    {
        public ExcelProviderForUnitResults(string path, string sheetName): base(path, sheetName) { }

        public override List<UnitModule> GetDataOfUnitModules(List<string> moduleNames)
        {
            List<UnitModule> modulesToReturn = new List<UnitModule>();
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

                        List<File> files = GetFilesInModule(row, false);

                        string trNumber = GetTrNumber(row);
                        
                        modulesToReturn.Add(new UnitModule(currentModuleName, moduleBaseline, trNumber, row, col, files));

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
