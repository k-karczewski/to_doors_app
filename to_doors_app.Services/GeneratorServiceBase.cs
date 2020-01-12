using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Interfaces.Providers.ExcelProvider;
using to_doors_app.Interfaces.Providers.OutputProvider;
using to_doors_app.Models;
using to_doors_app.Models.Dtos;
using to_doors_app.Providers;
using to_doors_app.Providers.ExcelProvider;

namespace to_doors_app.Services
{
    public abstract class GeneratorServiceBase<T> : IGeneratorServiceBase where T : Module
    {
        /// <summary>
        /// List of object to be processed
        /// </summary>
        protected List<T> ModulesData = new List<T>();

        /// <summary>
        /// Object that provides excel data access
        /// </summary>
        protected IExcelProviderBase<T> ExcelProvider { get; set; }

        /// <summary>
        /// Object that provides xml data access
        /// </summary>
        protected IResultsProviderBase<T> ResultsProvider { get; set; } = null;

        /// <summary>
        /// Creates .tsv files with specific file structure 
        /// </summary>
        protected IOutputProviderBase<T> OutputProvider { get; set; }

        /// <summary>
        /// Operation type - value of OperationType enum
        /// </summary>
        protected OperationType Operation { get; set; }

        protected GeneratorServiceBase() {}

        /// <summary>
        /// Main task of service. Gets results from .xml files and sends them to .tsv file
        /// </summary>
        protected virtual void GenerateTsvFiles()
        {
            ResultsProvider.FillResultsOfModules(ref ModulesData);

            OutputProvider.GenerateFiles(ref ModulesData);
        }

        /// <summary>
        /// Removes object from ModuleData collection by its name
        /// </summary>
        /// <param name="moduleName">Name of of object to be removed</param>
        public void RemoveModuleData(string moduleName)
        {
            ModulesData.Remove(ModulesData.FirstOrDefault(x => x.Name == moduleName));
        }

        /// <summary>
        /// Reads data from excel file for every module name passed in parameter
        /// </summary>
        /// <param name="modulesNames">List of module names that data will be loaded</param>
        public void LoadDataFromMts(List<string> modulesNames)
        {
            ExcelProvider.GetDataOfModules(modulesNames, ref ModulesData);
        }

        /// <summary>
        /// Reads data of all modules contained in excel file. Used only for OperationType Test_Specification_From_Module_Test_State
        /// </summary>
        public void LoadDataFromMts()
        {
            if(ExcelProvider is ExcelProviderForUnits excelProviderForUnits && ModulesData is List<UnitModule> unitModules)
            {
                excelProviderForUnits.GetDataOfModules(ref unitModules);
            }
        }

        /// <summary>
        /// Gets sheet names from loaded excel file
        /// </summary>
        /// <param name="pathToMts">Path to excel file</param>
        /// <returns>List of sheet names</returns>
        public List<string> GetSheetNames(string pathToMts)
        {
            return ExcelProvider.GetSheetNames(pathToMts);
        }

        /// <summary>
        /// Sets sheet to be used
        /// </summary>
        /// <param name="sheetName">Sheet name to be used that reading of data will be done from</param>
        public void SetSheetName(string sheetName)
        {
            ExcelProvider.SetSheetName(sheetName);
        }

        /// <summary>
        /// Closes all excel processes
        /// </summary>
        public void CloseExcelDocument()
        {
            ExcelProvider.CloseDocument();
        }

        /// <summary>
        /// Converts module object (loaded from excel) to data transfer object that is show on User Interface
        /// </summary>
        /// <returns>List of dtos</returns>
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

        /// <summary>
        /// Updates data with new info (changed by user in UI) 
        /// </summary>
        /// <param name="editedData">Edited objects by user</param>
        public virtual void SaveEditedDataByUi(List<ModuleToUiDto> editedData)
        {
            try
            {
                // if there is some edited data
                if(editedData.Count > 0)
                { 
                    // update data taken from excel and xml by data typed by user in ui
                    foreach (ModuleToUiDto moduleFromUi in editedData)
                    {
                        Module moduleToUpdate = ModulesData.FirstOrDefault(x => x.Name == moduleFromUi.Name);

                        // will be null only if user will change module name somehow
                        if (moduleToUpdate != null)
                        {
                            moduleToUpdate.Baseline = moduleFromUi.Baseline;
                            moduleToUpdate.TrNumber = moduleFromUi.TrNumber;
                            moduleToUpdate.PathToOverviewReport = moduleFromUi.PathToOverviewReport;
                        }
                        else // abort generating files if module was not found
                        {
                            throw new Exception($"Module {moduleFromUi.Name} was not found. Please restart the application and try again.");
                        }
                    }
                }

                /* if error did not occured all edited data is saved
                   application is ready for generating tsv files */
                GenerateTsvFiles();
            }
            catch (Exception ex) // show message box with error
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Checks if all objects are initialized and if there is any data to be generated
        /// </summary>
        /// <returns>true or false</returns>
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

        /// <summary>
        /// This function provides listening of progress events
        /// </summary>
        /// <param name="progressEventAction">Progress event action</param>
        public virtual void ListenToProgressEvents(EventHandler<string> progressEventAction)
        {
            ExcelProvider.ShowWorkProgressEvent += progressEventAction;
            ResultsProvider.ShowWorkProgressEvent += progressEventAction;
            OutputProvider.ShowWorkProgressEvent += progressEventAction;
        }
    }

}
