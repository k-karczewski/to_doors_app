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

                    List<File> files = GetFilesInModule(row);

                    string trNumber = GetTrNumber(row);

                    allModulesToReturn.Add(new Module(currentModuleName, moduleBaseline, files, trNumber, row, col));

                }
                row++;
            }
            return allModulesToReturn;
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

                    List<string> functions = GetFunctionsOfFile(fileRow);

                    files.Add(new File(fileName, fileRevision, functions));
                }

                fileRow++;
            }

            return files;
        }

        private List<string> GetFunctionsOfFile(int functionRow)
        {
            List<string> functions = new List<string>();

            do
            {
                functions.Add(ReadCell(functionRow, 5));
                functionRow++;
            }
            while (ReadCell(functionRow + 1, 3).Equals(string.Empty));

            return functions;
        }
    }
}
