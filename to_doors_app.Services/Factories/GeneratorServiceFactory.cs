using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;

namespace to_doors_app.Services.Factories
{
    public static class GeneratorServiceFactory
    {
        private static IGeneratorServiceBase _generatorService;

        public static IGeneratorServiceBase CreateService(OperationType operation)
        {
            if(operation.Equals(OperationType.Unit_Test_Resuls_From_Tessy))
            {
                _generatorService = new GeneratorServiceForUnitResults();
            }
            else if(operation.Equals(OperationType.Module_Integration_Test_Results_From_Tessy))
            {
                _generatorService = new GeneratorServiceForIntegrationResults();
            }
            else
            {
                _generatorService = new GeneratorServiceForMtsTestSpec();
            }

            return _generatorService;
        }
    }
}
