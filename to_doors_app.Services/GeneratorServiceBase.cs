using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Interfaces.Providers.OutputProvider;
using _IO = System.IO;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Services
{
    public class GeneratorServiceBase<T> : IGeneratorServiceBase
    {
        protected List<T> ModulesData = new List<T>();
        public IExcelProviderBase<T> ExcelProvider { get; set; }
        protected IResultsProviderBase<T> ResultsProvider { get; set; } = null;
        protected IOutputProviderBase<T> OutputProvider { get; set; }
        protected OperationType Operation { get; set; }
        protected List<string> ModulesNames = new List<string>();
        
        protected GeneratorServiceBase() { }

        private void GetModulesNames(ref List<string> modulesNames)
        {
            string pathToOverviews = _Settings.GetSetting(Operation, "PathToOverviewReports");
            string fileNameSufix = _Settings.GetSetting(Operation, "OverviewReportSufix");

            /* check if directory exist */
            if (_IO.Directory.Exists(pathToOverviews))
            {
                string[] pathsToOvervievs = _IO.Directory.GetFiles(pathToOverviews);

                /* take module name from path */
                foreach (string path in pathsToOvervievs)
                {
                    string result = path.Replace(pathToOverviews, "");
                    result = result.Replace(fileNameSufix, "");

                    /* store module name in list*/
                    modulesNames.Add(result);
                }

                /* if no modules were found */
                if (modulesNames.Count <= 0)
                {
                   // Console.WriteLine("\"Overviews\" folder is empty");
                    /* show message that there is no overview repoert for current operation */
                }

            }
            else
            {
                //Console.WriteLine("Error during reading Overview reports (Overview folder does not exist)");
            }
        }

        public void Run()
        {
            if (!Operation.Equals(OperationType.Test_Specification_From_Module_Test_State))
            {
                GetModulesNames(ref ModulesNames);

                ExcelProvider.GetDataOfModules(ModulesNames, ref ModulesData);

                ResultsProvider.FillResultsOfModules(ref ModulesData);
            }
            else
            {
                ExcelProvider.GetDataOfModules(ref ModulesData);
            }

            OutputProvider.GenerateFiles(ref ModulesData);
        }

        public List<string> GetSheetNames(string pathToMts)
        {
            return ExcelProvider.GetSheetNames(pathToMts);
        }

        public void SetSheetName(string sheetName)
        {
            ExcelProvider.SetSheetName(sheetName);
        }
    }
}
