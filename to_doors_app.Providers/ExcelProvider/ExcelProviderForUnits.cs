using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers.ExcelProvider
{
    public class ExcelProviderForUnits : ExcelProviderBase<UnitModule>
    {
        public ExcelProviderForUnits() : base() { }

        public override void GetDataOfModules(List<string> moduleNames, ref List<UnitModule> modulesToReturn)
        {
            /* start search from const coordinates*/
            int row = 12;
            const int col = 1;
            ChangeProgressInfo("Getting data from module test state");

            _Settings.SetSwBaseline(GetSwBaseline());

            while (ReadCell(row, col) != "Total:")
            {
                string currentModuleName = ReadCell(row, col);

                if (!currentModuleName.Equals(string.Empty))
                {
                    if (moduleNames.Contains(currentModuleName))
                    {
                        ChangeProgressInfo($"Reading from mts data of module {currentModuleName}");
                        string moduleBaseline = ReadCell(row, col + 1);

                        List<File> files = GetFilesInModule(row);

                        string trNumber = GetTrNumber(row);

                        modulesToReturn.Add(new UnitModule(currentModuleName, moduleBaseline, trNumber, row, col, files));

                        if (modulesToReturn.Count >= moduleNames.Count)
                        {
                            break;
                        }
                    }
                }
                row++;
            }
            ChangeProgressInfo("Done");
        }

        public void GetDataOfModules(ref List<UnitModule> allModulesToReturn)
        {
            /* start search from const coordinates*/
            int row = 12;
            const int col = 1;

            ChangeProgressInfo("Getting all modules data from module test state");

            _Settings.SetSwBaseline(GetSwBaseline());

            while (ReadCell(row, col) != "Total:")
            {
                string currentModuleName = ReadCell(row, col);

                if (!currentModuleName.Equals(string.Empty))
                {
                    ChangeProgressInfo($"Reading from mts data of module {currentModuleName}");

                    string moduleBaseline = ReadCell(row, col + 1);

                    List<File> files = GetFilesInModule(row);

                    string trNumber = GetTrNumber(row);

                    allModulesToReturn.Add(new UnitModule(currentModuleName, moduleBaseline, trNumber, row, col, files));

                }
                row++;
            }

            ChangeProgressInfo("Done");
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
                if (!fileName.Equals(string.Empty) && fileName.Contains(".c") == true)
                {
                    fileName = fileName.Replace(".c", "");

                    string fileRevision = ReadCell(fileRow, col + 3);

                    List<Function> functions = GetFunctionsOfFile(fileRow);

                    if(functions.Count > 0)
                    {
                        files.Add(new File(fileName, fileRevision, functions));
                    }
                }

                fileRow++;
            }

            return files;
        }

        private List<Function> GetFunctionsOfFile(int functionRow)
        {
            List<Function> functions = new List<Function>();

            do
            {
                functions.Add(new Function(ReadCell(functionRow, 5)));
                functionRow++;
            }
            while (ReadCell(functionRow, 3).Equals(string.Empty));

            return functions;
        }
    }
}
