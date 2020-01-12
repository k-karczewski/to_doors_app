using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Providers.ExcelProvider;
using to_doors_app.Providers.ResultsProvider;
using to_doors_app.Providers.SettingsProvider;
using to_doors_app.Providers.OutputProvider;
using to_doors_app.Models;

namespace to_doors_app.Services
{
    public class GeneratorServiceForMtsTestSpec : GeneratorServiceBase<UnitModule>
    {
        public GeneratorServiceForMtsTestSpec(EventHandler<string> eventHandler) : base()
        {
            Operation = Interfaces.OperationType.Test_Specification_From_Module_Test_State;
            SettingsProvider.SetOperationType(Operation);
            ExcelProvider = new ExcelProviderForUnits();
            OutputProvider = new OutputProviderForUnits();
            ListenToProgressEvents(eventHandler);
        }

        /// <summary>
        /// This function provides listening of progress events
        /// </summary>
        /// <param name="progressEventAction">Progress event action</param>
        public override void ListenToProgressEvents(EventHandler<string> progressEventAction)
        {
            ExcelProvider.ShowWorkProgressEvent += progressEventAction;
            OutputProvider.ShowWorkProgressEvent += progressEventAction;
        }

        /// <summary>
        /// Sends collected data to .tsv files
        /// </summary>
        protected override void GenerateTsvFiles()
        {
            OutputProvider.GenerateFiles(ref ModulesData);
        }

        /// <summary>
        /// Checks if all necessary objects are initialized
        /// </summary>
        /// <returns>true or false</returns>
        public override bool IsServiceReadyToGenerateFiles()
        {
            if (ExcelProvider != null &&
                OutputProvider != null)
            {
                return true;
            }

            return false;
        }
    }
}
