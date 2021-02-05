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

        /// <summary>
        /// Creates intance of IGeneratorServiceBase based on operation type
        /// </summary>
        /// <param name="operation">Operation type</param>
        /// <param name="eventHandler">Event handler that provides showing progress informations on UI</param>
        /// <returns>Instance of IGeneratorServiceBase</returns>
        public static IGeneratorServiceBase CreateService(OperationType operation, EventHandler<string> eventHandler)
        {
            if(operation.Equals(OperationType.Unit_Test_Results_From_Tessy))
            {
                _generatorService = new GeneratorServiceForUnitResults(eventHandler);
            }
            else if(operation.Equals(OperationType.Module_Integration_Test_Results_From_Tessy))
            {
                _generatorService = new GeneratorServiceForIntegrationResults(eventHandler);
            }
            else
            {
                _generatorService = new GeneratorServiceForMtsTestSpec(eventHandler);
            }

            return _generatorService;
        }
    }
}
