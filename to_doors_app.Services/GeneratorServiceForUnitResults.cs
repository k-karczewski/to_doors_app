using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;
using to_doors_app.Providers.ExcelProvider;
using to_doors_app.Providers.ResultsProvider;
using to_doors_app.Providers.SettingsProvider;
using to_doors_app.Providers.OutputProvider;

namespace to_doors_app.Services
{
    public sealed class GeneratorServiceForUnitResults : GeneratorServiceBase<UnitModule>
    {
        public GeneratorServiceForUnitResults(EventHandler<string> eventHandler) : base()
        {
            Operation = Interfaces.OperationType.Unit_Test_Resuls_From_Tessy;
            SettingsProvider.SetOperationType(Operation);
            ExcelProvider = new ExcelProviderForUnits();
            ResultsProvider = new ResultsProviderForUnits();
            OutputProvider = new OutputProviderForUnits();
            ListenToProgressEvents(eventHandler);
        }
    }
}

