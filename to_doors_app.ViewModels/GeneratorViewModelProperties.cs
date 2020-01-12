using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        public ButtonAsyncHandler OpenMtsFileCommand { get; set; }
        public ButtonHandler OpenOverviewReportsCommand { get; set; }
        public ButtonHandler RemoveObjectFromDataGridCommand { get; set; }
        public ButtonHandler OpenOutputDirectoryCommand { get; set; }
        public ButtonAsyncHandler GenerateFilesCommand { get; set; }
        public ButtonAsyncHandler ConfirmMtsSheet { get; set; }
        #endregion

        #region ServiceDeclaration
        private IGeneratorServiceBase _generatorService;
        #endregion

        public GeneratorViewModel()
        {
            _Settings.LoadSettingsFromFile();
            ActualOperation = AvailableOperationsList[0]; /* unit results by default */

            /*buttons commands*/
            OpenMtsFileCommand = new ButtonAsyncHandler(SelectMtsFile, CanSearchForMts);
            OpenOverviewReportsCommand = new ButtonHandler(SelectOverviewReports, CanSearchForReports);
            RemoveObjectFromDataGridCommand = new ButtonHandler(RemoveSelectedRow, CanRemoveRow);
            OpenOutputDirectoryCommand = new ButtonHandler(SelectOutputDirectory, IsSelectOutputDirectoryButtonEnabled);
            GenerateFilesCommand = new ButtonAsyncHandler(GenerateFiles, IsApplicationReadyToGenerateFiles);
            ConfirmMtsSheet = new ButtonAsyncHandler(SaveChoosenMtsSheet, CanSaveChoosenMtsSheet);
        }

        #region OperationTypesCombobox
        /// <summary>
        /// Operation type combobox property values (OperationType enum values)
        /// </summary>
        public List<OperationType> AvailableOperationsList { get; } = Enum.GetValues(typeof(OperationType)).Cast<OperationType>().ToList();
       
        /// <summary>
        /// private variable that stores actual operation type 
        /// </summary>
        private OperationType _actualOperation; 
        
        /// <summary>
        /// Getter and setter of _actualOperation variable
        /// </summary>
        public OperationType ActualOperation 
        { 
            get
            {
                /* return value */
                return _actualOperation;
            }
            set /* if actual operation is changed by user */
            {
                /* if user have chosen some report before operation change */
                if (ReportsPaths != null)
                {
                    ReportsPaths = null;
                    IsChoosingSheetAvailable = false;
                    ConfirmMtsSheet?.InvokeCanExecuteChanged();
                    OpenMtsFileCommand?.InvokeCanExecuteChanged();

                    ExcelProviderProgress = $"Operation has been changed. Overview reports has been deleted.";
                    GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "ExcelProviderProgress");
                    GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "ExcelProviderProgress");
                }

                // set new value of operation
                _actualOperation = value;

                /* clear mts path*/
                //MtsFilePath = string.Empty;
                
                /* if mts sheets are loaded (should be always true) */
                //if(MtsSheets != null && MtsSheets.Count > 0)
                //{
                //    /* clear them - mts is no longer loaded*/
                //    ActualMtsSheet = null;
                //    MtsSheets.Clear();
                //}

                /* clear service object */
                //if (_generatorService != null)
                //{
                //    _generatorService.CloseExcelDocument();
                //    _generatorService = null;
                //}

                //OpenMtsFileCommand?.InvokeCanExecuteChanged();

                /* update ui */
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "MtsFilePath");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsSheetsDropdownEnabled");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "OutputPath");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsReportsSectionVisible");
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsOperationTypeComboboxEnabled");
            }     
        }

        public bool IsOperationTypeComboboxEnabled { get; set; } = true;

        #endregion

        #region ModuleTestStatePicker
        /// <summary>
        /// Gets/sets path to mts file form/to settings
        /// </summary>
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
        /// <summary>
        /// Stores excel sheets names that can be used
        /// </summary>
        public BindableCollection<string> MtsSheets { get; set; }

        /// <summary>
        /// Actually choosen excel sheet
        /// </summary>
        private string _actualMtsSheet = string.Empty;

        /// <summary>
        /// Getter/setter of actual mts sheet
        /// </summary>
        public string ActualMtsSheet 
        { 
            get 
            { 
                return _actualMtsSheet; 
            }
            set
            {
                /* if value is set to default*/
                if(_actualMtsSheet.Equals(string.Empty))
                {
                    /* send chosen name to service */
                    _generatorService.SetSheetName(value);
                    /* allow changing of name */
                    IsChoosingSheetAvailable = true;
                    /* turn on confirm sheet button */
                    ConfirmMtsSheet.InvokeCanExecuteChanged();
                }

                /* set value to variable */
                _actualMtsSheet = value;
                /* refresh ui */
                GeneratorViewModelHelpers.RefreshViewModel(this, PropertyChanged, "IsRemovingRowAvailable");
            }
        }

        /// <summary>
        /// Information if user is able to choose mts sheet - default false because mts is not loaded yet
        /// </summary>
        public bool IsChoosingSheetAvailable { get; set; } = false;

        /// <summary>
        /// Gets information if is able to choose mts sheet
        /// </summary>
        /// <returns>IsChoosingSheetAvailable property</returns>
        private bool CanSaveChoosenMtsSheet()
        {
            return IsChoosingSheetAvailable;
        }

        /// <summary>
        /// Information if mts sheet was confirmed
        /// </summary>
        private bool IsSheetChoosen { get; set; } = false;
        
        /// <summary>
        /// Property that enables or disables combobox with mts sheets
        /// </summary>
        public bool IsSheetsDropdownEnabled
        {
            get { return _generatorService != null && ModulesForUi.Count == 0 && !IsSheetChoosen ?  true : false; }
        }
        #endregion

        #region DataGrid
        /// <summary>
        /// Selected row of datagrid
        /// </summary>
        private ModuleToUiDto _selectedObject = null;

        /// <summary>
        /// Property of selected datagrid object
        /// Returns private variable _selectedObject
        /// </summary>
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

        /// <summary>
        /// Collection of ModuleToUiDto objects that is binded to DataGrid
        /// </summary>
        public BindableCollection<ModuleToUiDto> ModulesForUi { get; set; } = new BindableCollection<ModuleToUiDto>();

        /// <summary>
        /// Enables or disables "Remove report" button
        /// </summary>
        public bool IsRemoveModuleButtonEnabled
        {
            get { return ModulesForUi != null ? true : false; }
        }

        /// <summary>
        /// Hides or shows "Add report", "Remove report" buttons and DataGrid
        /// </summary>
        public bool IsReportsSectionVisible 
        {   
            get 
            { 
                return ActualOperation != OperationType.Test_Specification_From_Module_Test_State ? true : false; 
            } 
        }

        /// <summary>
        /// Checks if removing of object is allowed. If some object is selected returns true, false otherwise
        /// </summary>
        public bool IsRemovingRowAvailable
        {
            get
            {
                return SelectedObject != null ? true : false;
            }
        }
        #endregion

        #region OutputPath
        /// <summary>
        /// Gets or sets output path from/to settings
        /// </summary>
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
