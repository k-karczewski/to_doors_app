using System.Collections.Generic;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;

namespace to_doors_app.Providers
{
    public class ExcelProviderForMtsTestSpec : ExcelProviderBase, IExcelProviderForMtsTestSpec
    {
        public ExcelProviderForMtsTestSpec(string path, string sheetName) : base(path, sheetName) { }

        public override List<Module> GetDataOfAllModules()
        {
            List<Module> allModulesToReturn = new List<Module>();
            /* start search from const coordinates*/
            int row = 12;
            const int col = 1;

            while (ReadCell(row, col) != "Total:")
            {
                string currentModuleName = ReadCell(row, col);

                if (!currentModuleName.Equals(string.Empty))
                {
                    string moduleBaseline = ReadCell(row, col + 1);

                    List<File> files = GetFilesInModule(row, true);

                    string trNumber = GetTrNumber(row);

                    allModulesToReturn.Add(new Module(currentModuleName, moduleBaseline, files, trNumber, row, col));

                }
                row++;
            }
            return allModulesToReturn;
        }
    }
}
