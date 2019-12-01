using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;
using to_doors_app.Providers.ExcelProvider;
using to_doors_app.Providers.ResultsProvider;
using to_doors_app.Providers.SettingsProvider;
using to_doors_app.Providers.OutputProvider;

namespace to_doors_app.Services
{
    public class GeneratorServiceForIntegrationResults : GeneratorServiceBase<IntegrationModule>
    {
        public GeneratorServiceForIntegrationResults()
        {
            Operation = Interfaces.OperationType.Module_Integration_Test_Results_From_Tessy;
            SettingsProvider.SetOperationType(Operation);
            ExcelProvider = new ExcelProviderForIntegration();
            ResultsProvider = new ResultsProviderForIntegration();
            OutputProvider = new OutputProviderForIntegration();
        }
    }
}
