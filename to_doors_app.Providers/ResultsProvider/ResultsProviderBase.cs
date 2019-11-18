using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;
namespace to_doors_app.Providers.ResultsProvider
{
    public abstract class ResultsProviderBase : IResultsProviderBase
    {
        protected XElement XmlData { get; set; } = null;
        
        protected string ModuleName { get; } = "";

        protected ResultsProviderBase(string moduleName, OperationType operationType) 
        {
            ModuleName = moduleName;
            
            try
            {
                if (!_Settings.container.Equals(null))
                {
                    SettingsProvider.Settings operationData = new SettingsProvider.Settings();

                    if (operationType.Equals(OperationType.Unit_Test_Resuls_From_Tessy))
                    {                        
                        _Settings.container.OperationTypeData.TryGetValue(OperationType.Unit_Test_Resuls_From_Tessy.ToString(), out operationData);   
                    }
                    else
                    {
                        _Settings.container.OperationTypeData.TryGetValue(OperationType.Module_Integration_Test_Results_From_Tessy.ToString(), out operationData);
                    }

                    XmlData = XElement.Load($"{operationData.PathToOverviewReports}{moduleName}{operationData.OverviewReportSufix}");
                }
                else
                {
                    throw new Exception("Application settings were not loaded before operation of reading xml file");
                }
            }
            catch(Exception ex)
            {
                /* show error during reading of xml file*/
            }
        }

        public abstract void FillResultsOfModule(out Module module);

        protected abstract void LoadXmlFile();

        protected abstract int GetTotalNumberOfTestcases();
        
        protected abstract int GetNumberOfSuccesses();
        
        protected abstract int GetNumberOfFails();
        
        protected abstract int GetNumberOfNotExecuted();

        protected abstract string GetModuleVerdict();
    }
}
