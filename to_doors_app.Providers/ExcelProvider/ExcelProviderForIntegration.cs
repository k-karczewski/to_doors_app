using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers.ExcelProvider
{
    public class ExcelProviderForIntegration: ExcelProviderBase<IntegrationModule>
    {
        public ExcelProviderForIntegration(): base() { }

        public override void GetDataOfModules(List<string> moduleNames, ref List<IntegrationModule> modulesToReturn)
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
                        ChangeProgressInfo($"Processing module {currentModuleName}");
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
            ChangeProgressInfo("Done");
        }
    }
}
