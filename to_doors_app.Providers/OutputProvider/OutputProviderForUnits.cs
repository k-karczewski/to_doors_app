using System.Collections.Generic;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers.OutputProvider
{
    public sealed class OutputProviderForUnits : OutputProviderBase<UnitModule>
    {
        public OutputProviderForUnits() : base() { }

        protected override void PrepareTestData(UnitModule module)
        {
            SendStaticTestData(module);
            SendDynamicTestData(module);
        }

        private void SendFunctions(File file, string trNumber)
        {
            foreach(Function function in file.Functions)
            {
                List<string> buffer = new List<string>
                {

                    /* add function name and constant notes */
                    function.Name,
                    "",
                    _Settings.GetSetting(_Settings.CurrentOperationType, "TestConditionsNote"),
                    _Settings.GetSetting(_Settings.CurrentOperationType, "TestRealizationProcedureNote"),
                    _Settings.GetSetting(_Settings.CurrentOperationType, "TestFacilitiesEquipmentNote"),
                    _Settings.GetSetting(_Settings.CurrentOperationType, "TestExpectedResultNoteDynamic"),
                    $"{trNumber}\nTotal Test Objects: { function.NumberOfTestcases }\nSuccessful: { function.NumberOfSuccessfulTestcases }\n" +
                    $"Failed: { function.NumberOfFailedTestcases }\nNot Executed: { function.NumberOfNotExecutedTestcases }\nC1 coverage: { function.ValueOfC1Coverage }%\n" +
                    $"MC/DC coverage: { function.ValueOfMcDcCoverage }%",
                    function.Verdict,
                    "Requirement Based Test"
                };

                SendElementsToFile(buffer);
            }
        }

        private void SendDynamicTestData(UnitModule module)
        {
            /* declaration of list that will store data to send*/
            List<string> buffer = new List<string>
            {
                /* add element */
                "Dynamic Test"
            };

            /* only one element is needed for current row */
            FillListWithEmptyObjects(ref buffer);
            SendElementsToFile(buffer);

            /* for each "tessyobject" tag which was found above */
            foreach (File file in module.Files)
            {
                /* clear data buffer */
                buffer = new List<string>
                {
                    /* add necessary data to buffer */
                    "File: " + file.Name,
                    "SW version label: baseline " + _Settings.SwBaseline + "\n module version label " +
                     module.Name + ": " + file.Revision,
                    "",
                    "",
                    "",
                    "",
                    $"{module.TrNumber}\n Complete Tessy Test:\nTotal Test Objects: { file.NumberOfTestcases }\n" +
                    $"Successful: { file.NumberOfSuccessfulTestcases }\nFailed: { file.NumberOfFailedTestcases }\n" +
                    $"Not Executed: { file.NumberOfNotExecutedTestcases }\n" +
                    $"C1 coverage: { file.ValueOfC1Coverage }%\nMC/DC coverage: { file.ValueOfMcDcCoverage }%",
                    file.Verdict,
                    "Requirement Based Test"
                };

                SendElementsToFile(buffer);
                SendFunctions(file, module.TrNumber);
            }
        }


        private void SendStaticTestData(UnitModule module)
        {
            string currentElement = "Static Test\n";

            /* combine first element */
            foreach (File file in module.Files)
            {
                currentElement += "File: " + file.Name + "\n";
            }

            /* declaration of list that will store data to send*/
            List<string> buffer = new List<string>
            {
                currentElement
            };

            currentElement = "SW version label: baseline " + _Settings.SwBaseline + "\n";

            foreach (File file in module.Files)
            {
                currentElement += "module version label: " + file.Name + ": " + module.Baseline + "\n";
            }

            buffer.Add(currentElement);
            buffer.Add(_Settings.GetSetting(_Settings.CurrentOperationType, "TestConditionsNote"));
            buffer.Add(_Settings.GetSetting(_Settings.CurrentOperationType, "TestRealizationProcedureNote"));
            buffer.Add(_Settings.GetSetting(_Settings.CurrentOperationType, "TestFacilitiesEquipmentNote"));
            buffer.Add(_Settings.GetSetting(_Settings.CurrentOperationType, "TestExpectedResultNoteStatic"));
            buffer.Add(string.Empty);
            buffer.Add(string.Empty);
            buffer.Add("Internal Interface Test");

            SendElementsToFile(buffer);
        }

        protected override string GetTestType()
        {
            return $"Module {CurrentProceededModuleName}";
        }
    }
}
