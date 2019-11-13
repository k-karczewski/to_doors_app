using System.Collections.Generic;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;

namespace to_doors_app.Providers
{
    public class ExcelProviderForUnitResults : ExcelProviderBase, IExcelProviderForTestResults
    {
        public ExcelProviderForUnitResults(string path, string sheetName): base(path, sheetName) { }

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

                        List<File> files = GetFilesInModule(row);

                        string trNumber = GetTrNumber(row);
                        modulesToReturn.Add(new Module(currentModuleName, moduleBaseline, files, trNumber, row, col));

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

        private List<File> GetFilesInModule(int moduleRow)
        {
            List<File> files = new List<File>();

            int fileRow = moduleRow + 1;
            int col = 1;

            string fileName;

            /* search for .c files*/
            while (ReadCell(fileRow, col).Equals(string.Empty))
            {
                fileName = ReadCell(fileRow, col + 2);

                /* if file found */
                if (!fileName.Equals(string.Empty) && fileName.Contains(".c"))
                {
                    fileName = fileName.Replace(".c", "");
                    string fileRevision = ReadCell(fileRow, col + 3);

                    files.Add(new File(fileName, fileRevision));
                }

                fileRow++;
            }

            return files;
        }
    }
}
