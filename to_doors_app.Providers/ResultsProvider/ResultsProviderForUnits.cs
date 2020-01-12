using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using to_doors_app.Interfaces;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers.ResultsProvider
{
    /* results provider for unit tests*/
    public sealed class ResultsProviderForUnits : ResultsProviderBase<UnitModule>
    {   
        /* default constructor */
        public ResultsProviderForUnits() { }

        /* fills results of all tested files in module*/
        protected sealed override void FillTestObjectResults(UnitModule moduleToFillResults)
        {
            /* for each test object (File)*/
            foreach(var fileResults in TestObjectResults)
            {
                /* moduleToFillResults has Files property that was created in Excel Provider with MTS data (constains all available .c files)
                 * now it is time to get only those that were tested in Tessy (that are present in xml file)
                 * rest of them will have only default data (e.g name, baseline, names of .c file) */
                File currentFile = moduleToFillResults.Files.FirstOrDefault(x => x.Name.ToLower() == fileResults.Attribute("name").Value.ToLower());

                /* if file was found by its name */
                if (!currentFile.Equals(null))
                {
                    /* object that stores results of test for current .c file */
                    XElement testcase_statistics = fileResults.Element("testcase_statistics");

                    /* get results from report */
                    currentFile.NumberOfTestcases = int.Parse(testcase_statistics.Attribute("total").Value);

                    currentFile.NumberOfSuccessfulTestcases = int.Parse(testcase_statistics.Attribute("ok").Value);

                    currentFile.NumberOfFailedTestcases = int.Parse(testcase_statistics.Attribute("notok").Value);

                    currentFile.NumberOfNotExecutedTestcases = int.Parse(testcase_statistics.Attribute("notexecuted").Value);

                    /* try to get reached coverage values */
                    try
                    {
                        currentFile.ValueOfC1Coverage = Convert.ToDouble(fileResults.Element("coverage").Element("c1").Attribute("percentage").Value, CultureInfo.InvariantCulture);
                        currentFile.ValueOfMcDcCoverage = Convert.ToDouble(fileResults.Element("coverage").Element("mcdc").Attribute("percentage").Value, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Warning: C1/MCDC data of file {currentFile.Name} is not available. Tests may be not executed.\n {e}");
                    }

                    /* evaluate verdict value */
                    string verdict = testcase_statistics.Attribute("success").Value;

                    if (verdict == "notok")
                    {
                        currentFile.Verdict = _Settings.container.NokVerdictValue;

                    }
                    else if (currentFile.NumberOfTestcases.Equals(0))
                    {
                        currentFile.Verdict = _Settings.container.NotTestedVerdictValue;
                    }
                    else
                    {
                        currentFile.Verdict = _Settings.container.OkVerdictValue;
                    }

                    /* get results of functions placed in current .c file */
                    var functionResults = fileResults.Descendants("tessyobject").Where(x => x.Attribute("level").Value == "4");

                    /* list of Functions that will store fillded with results Funcion objects */
                    List<Function> functions = new List<Function>();

                    /* loop over every tested function */
                    foreach (var functionResult in functionResults)
                    {
                        /* create ready function object */
                        Function currentFunction = FillFunctionResults(functionResult);
                        /* add it to the list */
                        functions.Add(currentFunction);
                    }

                    /* when list has all functions assign it to the File object */
                    currentFile.Functions = functions;
                }
            }
        }

        /* gets test objects from xml report*/
        protected override List<XElement> GetTestObjectResults()
        {
            return ResultsReport.Descendants("tessyobject").Where(x => x.Attribute("level").Value == "3").ToList();
        }

        /* creates Function object with results */
        private Function FillFunctionResults(XElement functionResult)
        {
            /* create new Function object by its name in xml report */
            Function function = new Function(functionResult.Attribute("name").Value);

            /* load data about testcases created for current function */
            XElement test_object_statistics = functionResult.Element("testcase_statistics");

            /* get data about testcases - total number, number of "greens", number of "reds" and number of not executed */
            function.NumberOfTestcases = Convert.ToInt32(test_object_statistics.Attribute("total").Value);
            function.NumberOfSuccessfulTestcases = Convert.ToInt32(test_object_statistics.Attribute("ok").Value);
            function.NumberOfFailedTestcases = Convert.ToInt32(test_object_statistics.Attribute("notok").Value);
            function.NumberOfNotExecutedTestcases = Convert.ToInt32(test_object_statistics.Attribute("notexecuted").Value);

            /* try to read c1 and mc/dc value */
            /* sometimes it is not possible to run function with mc/dc coverage */
            try
            {
                function.ValueOfC1Coverage = Convert.ToDouble(functionResult.Element("coverage").Element("c1").Attribute("percentage").Value, CultureInfo.InvariantCulture);
                function.ValueOfMcDcCoverage = Convert.ToDouble(functionResult.Element("coverage").Element("mcdc").Attribute("percentage").Value, CultureInfo.InvariantCulture);
            }
            catch
            {
                /* show warning */
                /* to be changed later */
                Console.WriteLine("Warning: MC/DC value of function " + functionResult.Attribute("name").Value + " is missing in Overview Report");
            }

            /* evaluate function verdict */
            string verdict = functionResult.Attribute("success").Value;

            if (verdict.Equals("notok"))
            {
                function.Verdict = _Settings.container.NokVerdictValue;

            }
            else if (function.NumberOfTestcases.Equals(0))
            {
                function.Verdict = _Settings.container.NotTestedVerdictValue;
            }
            else
            {
                function.Verdict = _Settings.container.OkVerdictValue;
            }

            /* return function*/
            return function;
        }
    }
}
