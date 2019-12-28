using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Interfaces.Providers.OutputProvider;
using to_doors_app.Models;
using to_doors_app.Models.Dtos;


namespace to_doors_app.Services
{
    public class GeneratorServiceBase<T> : IGeneratorServiceBase where T : Module
    {
        public List<T> ModulesData = new List<T>();
        public IExcelProviderBase<T> ExcelProvider { get; set; }
        protected IResultsProviderBase<T> ResultsProvider { get; set; } = null;
        protected IOutputProviderBase<T> OutputProvider { get; set; }
        protected OperationType Operation { get; set; }

        protected GeneratorServiceBase() { }

        protected virtual void GenerateTsvFiles()
        {
            ResultsProvider.FillResultsOfModules(ref ModulesData);

            OutputProvider.GenerateFiles(ref ModulesData);
        }

        public void RemoveModuleData(string moduleName)
        {
            ModulesData.Remove(ModulesData.FirstOrDefault(x => x.Name == moduleName));
        }

        public void LoadDataFromMts(List<string> modulesNames)
        {
            ExcelProvider.GetDataOfModules(modulesNames, ref ModulesData);
        }

        public void LoadDataFromMts()
        {
            ExcelProvider.GetDataOfModules(ref ModulesData);
        }

        public List<string> GetSheetNames(string pathToMts)
        {
            return ExcelProvider.GetSheetNames(pathToMts);
        }

        public void SetSheetName(string sheetName)
        {
            ExcelProvider.SetSheetName(sheetName);
        }

        public void CloseExcelDocument()
        {
            ExcelProvider.CloseDocument();
        }

        public List<ModuleToUiDto> GetDtosForUi()
        {
            List<ModuleToUiDto> modulesForUi = null;

            if (ModulesData != null && ModulesData.Count > 0)
            {
                modulesForUi = new List<ModuleToUiDto>();

                foreach (T module in ModulesData)
                {
                    modulesForUi.Add(new ModuleToUiDto(module.Name, module.Baseline, module.TrNumber));
                }
            }

            return modulesForUi;
        }

        /* updates data with new info typed in ui */
        public virtual void SaveEditedDataByUi(List<ModuleToUiDto> editedData)
        {
            try
            {
                /* if there is some edited data */
                if(editedData.Count > 0)
                { 
                    /* update data taken from excel and xml by data typed by user in ui */
                    foreach (ModuleToUiDto moduleFromUi in editedData)
                    {
                        Module moduleToUpdate = ModulesData.FirstOrDefault(x => x.Name == moduleFromUi.Name);

                        /* will be null only if user will change module name somehow */
                        if (moduleToUpdate != null)
                        {
                            moduleToUpdate.Baseline = moduleFromUi.Baseline;
                            moduleToUpdate.TrNumber = moduleFromUi.TrNumber;
                            moduleToUpdate.PathToOverviewReport = moduleFromUi.PathToOverviewReport;
                        }
                        else /* abort generating files if module was not found*/
                        {
                            throw new Exception($"Module {moduleFromUi.Name} was not found. Please restart the application and try again.");
                        }
                    }
                }

                /* if error did not occured all edited data is saved
                   application is ready for generating tsv files */
                GenerateTsvFiles();
            }
            catch (Exception ex) /* show message box with error */
            {
                MessageBox.Show(ex.Message);
            }
        }

        public virtual bool IsServiceReadyToGenerateFiles()
        {
            if (ExcelProvider != null && 
                ResultsProvider != null && 
                OutputProvider != null &&
                ModulesData.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
