using System;
using System.ComponentModel;
using _IO = System.IO;
using to_doors_app.ViewModels;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;
using to_doors_app.ViewModels.ButtonHandlers;
using Microsoft.Win32;
using System.Windows;
using System.Collections.Generic;
using to_doors_app.Interfaces;
using System.Linq;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Services.Factories;
using to_doors_app.Services;
using to_doors_app.Models;
using to_doors_app.Providers;


namespace to_doors_app.ViewModels
{
    public class GeneratorViewModel : INotifyPropertyChanged
    {
        private IGeneratorServiceBase _generatorService;
        private ButtonHandler _openMtsFileCommand;
        public ButtonHandler OpenMtsFileCommand { get { return _openMtsFileCommand; } set { _openMtsFileCommand = value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public GeneratorViewModel()
        {
            _Settings.LoadSettingsFromFile();
            ActualOperationType = OperationType.Unit_Test_Resuls_From_Tessy; /* default value */
            _generatorService = GeneratorServiceFactory.CreateService(ActualOperationType);
            OpenMtsFileCommand = new ButtonHandler(GetMtsFilePath, CanSearchForMtsPath);

        }


        #region MtsFilePicker
        public string MtsFilePath
        {
            get { return _Settings.container.PathToModuleTestState; }
            set 
            { 
                _Settings.container.PathToModuleTestState = value;
                MtsSheets = _generatorService.GetSheetNames(value);
                ActualMtsSheet = MtsSheets[0];
                
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MtsSheets"));
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ActualMtsSheet"));
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsSheetsDropdownEnabled"));
                }
            }
        }

        public void GetMtsFilePath()
        {
            MtsFilePath = Helpers.ViewModelHelpers.GetMtsFilePath();

            if(PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MtsFilePath"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsSheetsDropdownEnabled"));
            }
        }

        public bool CanSearchForMtsPath()
        {
            return true;
        }

        #endregion

        #region MtsSheetsLoader
        private string _actualMtsSheet;
        public string ActualMtsSheet
        {
            get { return _actualMtsSheet; }
            set { _actualMtsSheet = value; }
        }
        
        private List<string> _mtsSheets;
        public List<string> MtsSheets
        {
            get { return _mtsSheets; }
            set { _mtsSheets = value; }
        }

        public bool IsSheetsDropdownEnabled
        {
            get { return ActualMtsSheet != null ? true : false; }
        }

        #endregion

        #region OperationTypeCombobox
        public OperationType ActualOperationType
        {
            get { return _Settings.CurrentOperationType; }
            set
            {
                _generatorService = GeneratorServiceFactory.CreateService(_Settings.CurrentOperationType);
                _Settings.CurrentOperationType = value;
            }
        }

        public List<OperationType> OperationsList
        {
            get { return Enum.GetValues(typeof(OperationType)).Cast<OperationType>().ToList(); }
        }
        #endregion

    }
}
