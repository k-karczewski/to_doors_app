using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;
namespace to_doors_app.Providers.ResultsProvider
{
    /* generic base class of Results Provider */
    public class ResultsProviderBase<T> : IResultsProviderBase<T> where T : Module
    {
        /* list of files or list of scenarios from xml file (depending on operation type) */
        protected List<XElement> TestObjectResults { get; set; } = null;
        /* xml report with all results */
        protected XElement ResultsReport { get; set; } = null;

        /* public method which fills modules passed by parameter with results stored in xml files */
        public void FillResultsOfModules(ref List<T> modulesToFillResults)
        {
            /* for every module in list */
            for (int numberOfModule = 0; numberOfModule < modulesToFillResults.Count; numberOfModule++)
            {
                /* load xml file with results  */
                if(LoadXmlFile(modulesToFillResults[numberOfModule].Name))
                {
                    FillTestObjectResults(modulesToFillResults[numberOfModule]);
                }
                else
                {
                    /*error during loading of xml file - stop the loop */
                    break;
                }
            }
        }

        /* loads xml report with results for module and operation type specified in parameters*/
        private bool LoadXmlFile(string moduleName)
        {
            /* try to load xml file*/
            try
            {
                /* if settings are loaded */
                if (!_Settings.container.Equals(null))
                {
                    /* new object of settings */
                    SettingsProvider.Settings operationData = new SettingsProvider.Settings();
                    /* get settings from dictionary with key value equal to operation type */
                    _Settings.container.OperationTypeData.TryGetValue(_Settings.CurrentOperationType, out operationData);
                    /* load xml file*/
                    ResultsReport = XElement.Load($"{operationData.PathToOverviewReports}{moduleName}{operationData.OverviewReportSufix}");
                    /* get test object results from xml file*/
                    TestObjectResults = GetTestObjectResults();
                    /* operation succeeded - return true */
                    return true;
                }
                else
                {
                    /* show error */
                    throw new Exception("Index of module exceeded size of modules to proceed list");
                }
            }
            catch (Exception ex)
            {
                /* show error during reading of xml file*/
            }
            /* operation failed - return false */
            return false;
        }

        /* gets test objects results (files or scenarios) depending on operation type - implementation not needed in base class */
        protected virtual List<XElement> GetTestObjectResults()
        {
            throw new NotImplementedException();
        }
        /* sets results to model - implementation not needed in base class*/
        protected virtual void FillTestObjectResults(T module)
        {
            throw new NotImplementedException();
        }
    }
}
