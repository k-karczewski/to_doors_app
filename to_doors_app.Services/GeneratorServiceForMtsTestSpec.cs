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
        public GeneratorServiceForMtsTestSpec()
        {
            Operation = Interfaces.OperationType.Test_Specification_From_Module_Test_State;
            SettingsProvider.SetOperationType(Operation);
            ExcelProvider = new ExcelProviderForUnits();
            OutputProvider = new OutputProviderForUnits();
        }

        protected override void GenerateTsvFiles()
        {
            OutputProvider.GenerateFiles(ref ModulesData);
        }

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
