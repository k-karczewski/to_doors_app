using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models.Dtos;
using to_doors_app.ViewModels.ButtonHandlers;
using to_doors_app.ViewModels.Helpers;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.ViewModels
{
    public partial class GeneratorViewModel : INotifyPropertyChanged
    {
        #region InterfaceDeclarations
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region ButtonHandlers
        public ButtonHandler OpenMtsFileCommand { get; set; }
        public ButtonHandler OpenOverviewReportsCommand { get; set; }
        public ButtonHandler RemoveObjectFromDataGridCommand { get; set; }
        public ButtonHandler OpenOutputDirectoryCommand { get; set; }
        public ButtonHandler GenerateFilesCommand { get; set; }
        #endregion

        #region ServiceDeclaration
        private IGeneratorServiceBase _generatorService;
        #endregion

        public GeneratorViewModel()
        {
            _Settings.LoadSettingsFromFile();
            ActualOperation = AvailableOperationsList[0]; /* unit results by default */
            ModulesForUi = new BindableCollection<ModuleToUiDto>();

            /*buttons commands*/
            OpenMtsFileCommand = new ButtonHandler(SelectMtsFile, CanSearchForMts);
            OpenOverviewReportsCommand = new ButtonHandler(SelectOverviewReports, CanSearchForReports);
            RemoveObjectFromDataGridCommand = new ButtonHandler(RemoveSelectedRow, CanRemoveRow);
            OpenOutputDirectoryCommand = new ButtonHandler(SelectOutputDirectory, IsSelectOutputDirectoryButtonEnabled);
            GenerateFilesCommand = new ButtonHandler(GenerateFiles, IsApplicationReadyToGenerateFiles);
        }

        #region OperationTypesCombobox
        /* operation type combobox properties */
        public List<OperationType> AvailableOperationsList { get; } = Enum.GetValues(typeof(OperationType)).Cast<OperationType>().ToList();
        private OperationType _actualOperation; 
        public OperationType ActualOperation 
        { 
            get
            {
                return _actualOperation;
            }
            set /* if actual operation is changed by user */
            {
                /* set new value of operation */
                _actualOperation = value;

                /* clear mts path*/
                MtsFilePath = string.Empty;
                
                /* if mts sheets are loaded (should be always true) */
                if(MtsSheets != null && MtsSheets.Count > 0)
                {
                    /* clear them - mts is no longer loaded*/
                    ActualMtsSheet = null;
                    MtsSheets.Clear();
                }

                /* clear service object */
                if (_generatorService != null)
                {
                    _generatorService.CloseExcelDocument();
                    _generatorService = null;
                }

                /* update ui*/
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "MtsFilePath");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsSheetsDropdownEnabled");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "OutputPath");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsReportsSectionVisible");
            } 
        }
        #endregion

        #region ModuleTestStatePicker
        public string MtsFilePath
        {
            get
            {
                return _Settings.container.PathToModuleTestState;
            }
            set
            {
                _Settings.container.PathToModuleTestState = value;
            }
        }
        #endregion

        #region ModuleTestStateSheetCombobox

        public ObservableCollection<string> MtsSheets { get; set; }

        private string _actualMtsSheet;

        public string ActualMtsSheet 
        { 
            get 
            { 
                return _actualMtsSheet; 
            }
            set
            {
                _actualMtsSheet = value;
                
                if(_generatorService != null)
                {
                    _generatorService.SetSheetName(value);
                }

                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsRemovingRowAvailable");
            }
        }

        public bool IsSheetsDropdownEnabled
        {
            get { return _generatorService != null && ModulesForUi.Count == 0 ? true : false; }
        }
        #endregion

        #region DataGrid
        private ModuleToUiDto _selectedObject = null;
          
        public ModuleToUiDto SelectedObject 
        { 
            get
            {
                return _selectedObject;
            }
            set
            {
                _selectedObject = value;
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsRemovingRowAvailable");

            }
        }

        public BindableCollection<ModuleToUiDto> ModulesForUi { get; set; } = new BindableCollection<ModuleToUiDto>();

        public bool IsRemoveModuleButtonEnabled
        {
            get { return ModulesForUi != null ? true : false; }
        }

        public bool IsReportsSectionVisible 
        {   
            get 
            { 
                return ActualOperation != OperationType.Test_Specification_From_Module_Test_State ? true : false; 
            } 
        }

        public bool IsRemovingRowAvailable
        {
            get
            {
                return SelectedObject != null ? true : false;
            }
        }
        #endregion

        #region OutputPath
        public string OutputPath
        {
            get
            {
                return _Settings.GetSetting(ActualOperation, "PathToTsv");
            }
            set
            {
                _Settings.SetSetting(ActualOperation, "PathToTsv", value);
            }
        }
        #endregion
    }
}
