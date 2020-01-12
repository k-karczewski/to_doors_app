using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using to_doors_app.Interfaces;
using to_doors_app.Models.Dtos;
using to_doors_app.Services.Factories;
using to_doors_app.ViewModels.Helpers;
using System.Threading.Tasks;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.ViewModels
{
    public partial class GeneratorViewModel
    {
        #region SelectMtsFile
        /// <summary>
        /// This method loads mts (excel) file and loads needed excel sheets
        /// </summary>
        /// <returns>Async task</returns>
        private async Task SelectMtsFile()
        {
            /* get path*/
            string newMtsPath = GeneratorViewModelHelpers.GetMtsFilePath();

            try
            {
                /*if path was chosen*/
                if (newMtsPath != string.Empty)
                {
                    /* set new mts path */
                    MtsFilePath = newMtsPath;
                    /* create new instance of generator service by its factory */
                    _generatorService = GeneratorServiceFactory.CreateService(ActualOperation, ExcelProvider_ShowWorkProgressEvent);
                    /* load excel sheets  asynchronously */
                    MtsSheets = new BindableCollection<string>(await Task.Run(() => _generatorService.GetSheetNames(MtsFilePath)));
                    /* set first sheet as default one */
                    ActualMtsSheet = MtsSheets[0];

                    /* refresh view model */
                    GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "MtsFilePath");
                    GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "MtsSheets");
                    GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "ActualMtsSheet");
                    GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsSheetsDropdownEnabled");

                    if (ActualOperation == OperationType.Test_Specification_From_Module_Test_State)
                    {
                        OpenOutputDirectoryCommand.InvokeCanExecuteChanged();
                    }
                    else
                    {
                        OpenOverviewReportsCommand.InvokeCanExecuteChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
        }
        /// <summary>
        /// This method sets confirmed mts sheet and calls loading of mts data for specified modules
        /// </summary>
        /// <returns>async Task</returns>
        private async Task SaveChoosenMtsSheet()
        {
            if (_generatorService != null)
            {
                IsOperationTypeComboboxEnabled = false;
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsOperationTypeComboboxEnabled");
                _generatorService.SetSheetName(ActualMtsSheet);
                IsSheetChoosen = true;
                IsChoosingSheetAvailable = false;
                ConfirmMtsSheet.InvokeCanExecuteChanged();

                await LoadDataFromMts();

                //GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsChoosingSheetAvailable");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsSheetsDropdownEnabled");
            }
        }

        /// <summary>
        /// Loads data from mts for modules stored as reports paths 
        /// TODO: ---- TO BE SIMPLIFIED ------
        /// </summary>
        /// <returns>async Task</returns>
        private async Task LoadDataFromMts()
        {
            if (ReportsPaths != null)
            {
                List<string> moduleNames = await Task.Run(() => GeneratorViewModelHelpers.GetModuleNamesFromPaths(ReportsPaths, ActualOperation));

                if (_generatorService != null)
                {
                    await Task.Run(() => _generatorService.LoadDataFromMts(moduleNames));

                    List<ModuleToUiDto> itemsToAdd = _generatorService.GetDtosForUi();

                    GeneratorViewModelHelpers.SetOverviewReportsPathsToModules(ref itemsToAdd, ReportsPaths);

                    /* helper list to check if new loaded elements are already displayed on ui */
                    List<string> allModuleNamesOnUi = ModulesForUi.Select(x => x.Name).ToList();

                    /* remove dupplicated elements */
                    foreach (string moduleName in allModuleNamesOnUi)
                    {
                        itemsToAdd.RemoveAll(x => x.Name.Contains(moduleName));
                    }

                    /* show new loaded elements on ui */
                    ModulesForUi.AddRange(itemsToAdd);
                }

                _generatorService.CloseExcelDocument();

                RemoveObjectFromDataGridCommand.InvokeCanExecuteChanged();
                OpenOutputDirectoryCommand.InvokeCanExecuteChanged();

                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "ModulesForUi");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsSheetsDropdownEnabled");
            }
        }

        /// <summary>
        /// Checks if application state allows to load of module test state document
        /// </summary>
        /// <returns>true when button can be set to active - false otherwise </returns>
        private bool CanSearchForMts()
        {
            if(ReportsPaths != null && ReportsPaths.Count > 0 || ActualOperation == OperationType.Test_Specification_From_Module_Test_State)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region OverviewReportsDataGrid
        /// <summary>
        /// Stores paths to overview reports
        /// </summary>
        List<string> ReportsPaths { get; set; }

        /// <summary>
        /// This method allows picking of overview reports (.xml) by the dialog
        /// </summary>
        private void SelectOverviewReports()
        {
            ReportsPaths = GeneratorViewModelHelpers.GetOverviewReportPaths();

            if(ReportsPaths != null)
            { 
                if(ReportsPaths.Count > 0 && _generatorService != null)
                {
                    IsChoosingSheetAvailable = true;
                    ConfirmMtsSheet?.InvokeCanExecuteChanged();
                }
            }
            OpenMtsFileCommand?.InvokeCanExecuteChanged();
        }

        /// <summary>
        /// Method that allows "Add report" button be activated
        /// </summary>
        /// <returns>Method returns always true</returns>
        private bool CanSearchForReports()
        {
            return true;
        }

        /// <summary>
        /// This method removes selected row of the datagrid
        /// </summary>
        private void RemoveSelectedRow()
        {
            /* remove module from service */
            _generatorService.RemoveModuleData(SelectedObject.Name);
            /* remove module row from datagrid */
            ModulesForUi.Remove(SelectedObject);
            /* clear selected object */
            SelectedObject = null;
            /* refresh buttons if necessary */
            RemoveObjectFromDataGridCommand.InvokeCanExecuteChanged();
            OpenOutputDirectoryCommand.InvokeCanExecuteChanged();
            /* refresh ui controlls if necessary*/
            GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsSheetsDropdownEnabled");
            GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "ModulesForUi");
        }


        /// <summary>
        /// Checks if datagrid includes any row to be deleted
        /// </summary>
        /// <returns>true or false</returns>
        private bool CanRemoveRow()
        {
            return ModulesForUi.Count > 0 ? true : false;
        }

        #endregion

        #region OutputPath
        /// <summary>
        /// This method sets directory where output files will be generated
        /// </summary>
        public void SelectOutputDirectory()
        {
            OutputPath = GeneratorViewModelHelpers.GetOutputPath();
            _Settings.SetSetting(ActualOperation, "PathToTsv", OutputPath);

            GenerateFilesCommand.InvokeCanExecuteChanged();
            GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "OutputPath");
        }

        /// <summary>
        /// This method checks if selecting of output directory is allowed
        /// </summary>
        /// <returns>true or false</returns>
        private bool IsSelectOutputDirectoryButtonEnabled()
        {
            return (ModulesForUi.Count > 0 && ActualOperation != OperationType.Test_Specification_From_Module_Test_State) ||
                ActualOperation == OperationType.Test_Specification_From_Module_Test_State ? true : false;
        }
        #endregion

        #region GenerateFilesButton
        /// <summary>
        /// Starts main task of application
        /// </summary>
        /// <returns>async method</returns>
        public async Task GenerateFiles()
        {
            /* load data from mts for test spec */
            if (ActualOperation == OperationType.Test_Specification_From_Module_Test_State)
            {
               await Task.Run(_generatorService.LoadDataFromMts);
            }

            await Task.Run(() =>_generatorService.SaveEditedDataByUi(new List<ModuleToUiDto>(ModulesForUi)));
        }

        /// <summary>
        /// Checks if application is ready to generating output files (.tsv)
        /// </summary>
        /// <returns>true or false</returns>
        private bool IsGenerateFilesButtonEnabled()
        {
            if (_generatorService != null &&
                ModulesForUi != null &&
                ModulesForUi.Count > 0 &&
                Directory.Exists(OutputPath) && ActualOperation != OperationType.Test_Specification_From_Module_Test_State
                ||
                _generatorService != null &&
                ActualOperation == OperationType.Test_Specification_From_Module_Test_State)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if application is ready to generating output files (.tsv)
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsApplicationReadyToGenerateFiles()
        {
            if (IsGenerateFilesButtonEnabled())
            {
                if (_generatorService.IsServiceReadyToGenerateFiles())
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region EventActions
        /// <summary>
        /// Stores current progress of the application 
        /// </summary>
        public string ExcelProviderProgress { get; set; } = "";

        /// <summary>
        /// This method is an event listener which updates current progress of the application
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">progress information</param>
        public void ExcelProvider_ShowWorkProgressEvent(object sender, string progress)
        {
            ExcelProviderProgress = $"{progress}";
            GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "ExcelProviderProgress");
        }
        #endregion
    }
}
