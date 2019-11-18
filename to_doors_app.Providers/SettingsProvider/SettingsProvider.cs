using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Interfaces;
using to_doors_app.Interfaces.Providers;

namespace to_doors_app.Providers.SettingsProvider
{
    public class Settings
    {
        public string TestConditionsNote { get; set; }
        public string TestRealizationProcedureNote { get; set; }
        public string TestFacilitiesEquipmentNote { get; set; }
        public string TestExpectedResultNoteStatic { get; set; }
        public string TestExpectedResultNoteDynamic { get; set; }
        public string PathToTsv { get; set; }
        public string PathToOverviewReports { get; set; }
        public string OverviewReportSufix { get; set; }
        public string TsvFileSufix { get; set; } = "";
        public int MaxNumberOfAttributes { get; set; }
    }

    public class SettingsContainer
    {
        public Dictionary<string, Settings> OperationTypeData { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string PathToModuleTestState { get; set; }
        public string OkVerdictValue { get; set; }
        public string NokVerdictValue { get; set; }
        public string NotTestedVerdictValue { get; set; } = "";
        public string ControllerName { get; set; }

        public SettingsContainer() { }
        public SettingsContainer(SettingsContainer container) 
        {
            OperationTypeData = container.OperationTypeData;
            Attributes = container.Attributes;
            PathToModuleTestState = container.PathToModuleTestState;
            OkVerdictValue = container.OkVerdictValue;
            NokVerdictValue = container.NokVerdictValue;
            NotTestedVerdictValue = container.NotTestedVerdictValue;
            ControllerName = container.ControllerName;
        }
    }

    public static class SettingsProvider
    {
        public static string PathToSettingsFile { get; } = "./settings/settings.json";
        public static SettingsContainer container = new SettingsContainer();
       
        public static void RestoreDefaultSettings()
        {
            SettingsContainer dataToFile = new SettingsContainer
            {
                OperationTypeData = new Dictionary<string, Settings>
                {
                    {OperationType.Unit_Test_Resuls_From_Tessy.ToString(), new Settings
                        {
                            TestConditionsNote = "- SW Module with the correct Version Label is available.\n- Test environment is configured correctly.",
                            TestRealizationProcedureNote = "see references in PTIP, chapter \"Software Unit Tests\", subchapter \"Test Procedure\"",
                            TestFacilitiesEquipmentNote = "see references in PTIP, chapter \"Software Unit Tests\", chapter \"Test Environment / Test Tools\"",
                            TestExpectedResultNoteStatic = "- Software Quality Metrics: Okay(Polyspace).\n- MISRA C:2012 Check: Okay(Polyspace).\n- Programming Guidelines Check: Okay. \n- Software Run-Time-Error check: Okay(Polyspace).",
                            TestExpectedResultNoteDynamic = "- Dynamic Software Unit Tests are Okay (Tessy)\n- Branch Coverage (C1 in Tessy) = 100%, if no rational\n- MC/DC Coverage (C2 in Tessy)  = 100%, if no rational - for SW units on ASIL x",

                            PathToTsv = "./Generated tsv files (Unit Tests)/",
                            PathToOverviewReports = "./Overviews(Unit Tests)/",
                            OverviewReportSufix = "_OverviewReport.xml",
                            TsvFileSufix = "_to_DOORS_units.tsv",
                            MaxNumberOfAttributes = 8
                        }
                    },
                    { OperationType.Module_Integration_Test_Results_From_Tessy.ToString(), new Settings
                        {
                            TestConditionsNote = "- SW Module with the correct Version Label is available.\n- Test environment is configured correctly.",
                            TestRealizationProcedureNote = "See references in PTIP, chapter \"3.6.1. Software Integration Test: Software Module Integration Tests\", subchapter \"3.6.1.4 Test Procedure\" (Tessy). See also SDV&V plan\n",
                            TestFacilitiesEquipmentNote = "See references in PTIP, chapter \"Software Integration Test: Software Module Integration Tests\", chapter \"Test Environment/Test Tools\"\n",
                            TestExpectedResultNoteStatic = "- Software Quality Metrics: Okay(Polyspace).\n- MISRA C:2012 Check: Okay(Polyspace).\n- Programming Guidelines Check: Okay. \n- Software Run-Time-Error check: Okay(Polyspace).",
                            TestExpectedResultNoteDynamic = "Software Functional Tests are Okay (Tessy) \nFunction Coverage 100%\n",

                            PathToTsv = "./Generated tsv files (Module Integration Tests)/",
                            PathToOverviewReports = "./Overviews (Module Integration)/",
                            OverviewReportSufix = "_DetailsReport_MIT.xml",
                            TsvFileSufix = "_to_DOORS_module_integration.tsv",
                            MaxNumberOfAttributes = 9
                        }
                    },
                    { OperationType.Test_Specification_From_Module_Test_State.ToString(), new Settings
                        {
                            TestConditionsNote = "- SW Module with the correct Version Label is available.\n- Test environment is configured correctly.",
                            TestRealizationProcedureNote = "see references in PTIP, chapter \"Software Unit Tests\", subchapter \"Test Procedure\"",
                            TestFacilitiesEquipmentNote = "see references in PTIP, chapter \"Software Unit Tests\", chapter \"Test Environment / Test Tools\"",
                            TestExpectedResultNoteStatic = "- Software Quality Metrics: Okay(Polyspace).\n- MISRA C:2012 Check: Okay(Polyspace).\n- Programming Guidelines Check: Okay. \n- Software Run-Time-Error check: Okay(Polyspace).",
                            TestExpectedResultNoteDynamic = "- Dynamic Software Unit Tests are Okay (Tessy)\n- Branch Coverage (C1 in Tessy) = 100%, if no rational\n- MC/DC Coverage (C2 in Tessy)  = 100%, if no rational - for SW units on ASIL x",

                            PathToTsv = "./Generated tsv files (MTS)/",
                            PathToOverviewReports = string.Empty,
                            OverviewReportSufix = string.Empty,
                            TsvFileSufix = "_to_DOORS_mts.tsv",
                            MaxNumberOfAttributes = 8
                        }
                    }
                },

                Attributes = new Dictionary<string, string>
                {
                    {"attribute1", "attr_test_parameter" },
                    {"attribute2", "attr_test_conditions" },
                    {"attribute3", "attr_test_procedure" },
                    {"attribute4", "attr_test_equipment_and_setup" },
                    {"attribute5", "attr_test_expected_results" },
                    {"attribute6", "attr_test_actual_results" },
                    {"attribute7", "attr_test_verdict" },
                    {"attribute8", "attr_safety_test_method" },
                    {"attribute9", "attr_specification" },
                },
                PathToModuleTestState = "./mts/module_test_state.xlsx", /* to be changed later*/
                OkVerdictValue = "o.k.",
                NokVerdictValue = "n.o.k.",
                NotTestedVerdictValue = "test not done",
                ControllerName = "Main Controller"
            };

            container = dataToFile;

            SaveSettingsInFile(dataToFile);
        }

        public static void LoadSettingsFromFile()
        {
            try
            {
                container = JsonConvert.DeserializeObject<SettingsContainer>(System.IO.File.ReadAllText(PathToSettingsFile));
            }
            catch(Exception ex)
            {
                /* show error during reading json file*/
            }
        }

        public static void SaveSettingsInFile(SettingsContainer settings)
        {
            string serializedDictionary = JsonConvert.SerializeObject(settings, Formatting.Indented);

            try
            {
                System.IO.File.WriteAllText(PathToSettingsFile, serializedDictionary);
            }
            catch (Exception ex)
            {
                /* show error during writing json file*/
            }
        }

    }
}
