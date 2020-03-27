using System.Collections.Generic;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers.OutputProvider
{
    public sealed class OutputProviderForIntegration: OutputProviderBase<IntegrationModule>
    {
        public OutputProviderForIntegration() : base(){ }

        protected override void PrepareTestData(IntegrationModule module)
        {
            List<string> buffer = new List<string>
            {
                "Module " + CurrentProceededModuleName,
                "SW version label: baseline " + _Settings.SwBaseline + "\nmodule version label:\n" + module.Baseline,
                _Settings.GetSetting(_Settings.CurrentOperationType, "TestConditionsNote"),
                _Settings.GetSetting(_Settings.CurrentOperationType, "TestRealizationProcedureNote"),
                _Settings.GetSetting(_Settings.CurrentOperationType, "TestFacilitiesEquipmentNote"),
                _Settings.GetSetting(_Settings.CurrentOperationType, "TestExpectedResultNoteDynamic"),
            };

            string tmp =  module.TrNumber + "\nComplete Tessy Test:\nTotal Scenarios: " + module.TotalNumberOfTestcases + "\nSuccessful: " 
                + module.NumberOfSuccessfulTestcases + "\nFailed: " + module.NumberOfFailedTestcases + "\nNot Executed: " + module.NumberOfNotExecutedTestcases + 
                "\nFunction Coverage: " + module.ValueOfFunctionCoverage + "\nCall Coverage: 100";

            buffer.Add(tmp);
            buffer.Add(module.Verdict);
            FillListWithEmptyObjects(ref buffer);
            SendElementsToFile(buffer);

            foreach(Scenario scenario in module.Scenarios)
            {
                buffer = new List<string>
                {
                    scenario.Name,
                    "",
                    "",
                    _Settings.GetSetting(_Settings.CurrentOperationType, "TestRealizationProcedureNote"),
                    "",
                    _Settings.GetSetting(_Settings.CurrentOperationType, "TestExpectedResultNoteStatic"),
                    "",
                    scenario.Verdict,
                    "",
                    scenario.Specification,
                };
                SendElementsToFile(buffer);
            }
        }

        /* temporary solution */
        protected override string GetTestType()
        {
            return "Module Integration Test";
        }
    }
}
