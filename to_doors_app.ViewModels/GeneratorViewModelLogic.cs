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
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.ViewModels
{
    public partial class GeneratorViewModel
    {

        #region SelectMtsFile
        /*button command*/
        private void SelectMtsFile()
        {
            string newMtsPath = GeneratorViewModelHelpers.GetMtsFilePath();

            try
            {
                /*if path was chosen*/
                if (newMtsPath != string.Empty)
                {
                    MtsFilePath = newMtsPath;
                    _generatorService = GeneratorServiceFactory.CreateService(ActualOperation);
                    List<string> tmpSheets = _generatorService.GetSheetNames(MtsFilePath);
                    MtsSheets = new ObservableCollection<string>(_generatorService.GetSheetNames(MtsFilePath));
                    ActualMtsSheet = MtsSheets[0];

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

        private bool CanSearchForMts()
        {
            return true; /* mts can be always changed */
        }
        #endregion

        #region OverviewReportsDataGrid
        private void SelectOverviewReports()
        {
            List<string> reportsPaths = GeneratorViewModelHelpers.GetOverviewReportPaths();

            /* null is possible when user will click "cancel" button*/
            if (reportsPaths != null)
            {
                List<string> moduleNames = GeneratorViewModelHelpers.GetModuleNamesFromPaths(reportsPaths, ActualOperation);

                if (_generatorService != null)
                {
                    _generatorService.LoadDataFromMts(moduleNames);

                    List<ModuleToUiDto> itemsToAdd = _generatorService.GetDtosForUi();

                    GeneratorViewModelHelpers.SetOverviewReportsPathsToModules(ref itemsToAdd, reportsPaths);

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

                RemoveObjectFromDataGridCommand.InvokeCanExecuteChanged();
                OpenOutputDirectoryCommand.InvokeCanExecuteChanged();

                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "ModulesForUi");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsSheetsDropdownEnabled");
            }
        }

        private bool CanSearchForReports()
        {
            return MtsSheets != null && MtsSheets.Count > 0 ? true : false;
        }

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

        private bool CanRemoveRow()
        {
            return ModulesForUi.Count > 0 ? true : false;
        }

        #endregion

        #region OutputPath
        public void SelectOutputDirectory()
        {
            OutputPath = GeneratorViewModelHelpers.GetOutputPath();
            _Settings.SetSetting(ActualOperation, "PathToTsv", OutputPath);

            GenerateFilesCommand.InvokeCanExecuteChanged();
            GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "OutputPath");
        }

        private bool IsSelectOutputDirectoryButtonEnabled()
        {
            return (ModulesForUi.Count > 0 && ActualOperation != OperationType.Test_Specification_From_Module_Test_State) ||
                ActualOperation == OperationType.Test_Specification_From_Module_Test_State ? true : false;
        }
        #endregion

        #region GenerateFilesButton
        private void GenerateFiles()
        {
            /* load data from mts for test spec */
            if (ActualOperation == OperationType.Test_Specification_From_Module_Test_State)
            {
                _generatorService.LoadDataFromMts();
            }

            _generatorService.SaveEditedDataByUi(new List<ModuleToUiDto>(ModulesForUi));
        }

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
    }
}
