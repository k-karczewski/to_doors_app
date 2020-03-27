using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;

namespace to_doors_app.Providers.ResultsProvider
{
    /* generic base class of Results Provider */
    public abstract class ResultsProviderBase<T> : IResultsProviderBase<T> where T : Module
    {
        protected int levelValue = 0;

        public event EventHandler<string> ShowWorkProgressEvent;

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
                ChangeProgressInfo($"Reading .xml test results of module {modulesToFillResults[numberOfModule].Name}");
                /* load xml file with results  */
                LoadXmlFile(modulesToFillResults[numberOfModule].PathToOverviewReport, modulesToFillResults[numberOfModule].Name);
                FillTestObjectResults(modulesToFillResults[numberOfModule]);
            }
        }

        /* loads xml report with results for module and operation type specified in parameters*/
        private void LoadXmlFile(string pathToReport, string moduleName)
        {
            /* try to load xml file*/
            try
            {
                /* load xml file*/
                ResultsReport = XElement.Load(pathToReport);
                /* get test object results from xml file*/
                TestObjectResults = GetTestObjectResults();
            }
            catch (Exception ex)
            {
                /* show error during reading of xml file*/
                MessageBox.Show($"Error during reading xml files: {ex}");
            }
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

        protected void ChangeProgressInfo(string message)
        {
            ShowWorkProgressEvent?.Invoke(this, message);
        }
    }
}
