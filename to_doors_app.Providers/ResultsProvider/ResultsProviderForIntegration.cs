using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using to_doors_app.Models;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers.ResultsProvider
{
    /* results provider for integration tests*/
    public class ResultsProviderForIntegration : ResultsProviderBase<IntegrationModule>
    {
        /* default constructor */
        public ResultsProviderForIntegration() { }

        /* fills summary of the module (e.g number of scenarios, reached function coverage)
         * and generates scenario models using integration test results for every created scenario */
        protected sealed override void FillTestObjectResults(IntegrationModule moduleToFillResults)
        {
            /* get summary of the module */
            moduleToFillResults.TotalNumberOfTestcases = int.Parse(ResultsReport.Descendants("statistic").FirstOrDefault().Attribute("total").Value);
            moduleToFillResults.NumberOfSuccessfulTestcases = int.Parse(ResultsReport.Descendants("statistic").FirstOrDefault().Attribute("ok").Value);
            moduleToFillResults.NumberOfFailedTestcases = int.Parse(ResultsReport.Descendants("statistic").FirstOrDefault().Attribute("notok").Value);
            moduleToFillResults.NumberOfNotExecutedTestcases = int.Parse(ResultsReport.Descendants("statistic").FirstOrDefault().Attribute("notexecuted").Value);
            moduleToFillResults.ValueOfFunctionCoverage = int.Parse(ResultsReport.Descendants("fc").FirstOrDefault().Attribute("percentage").Value);

            /* flags for correct evaluation of test verdict */
            bool fcVerdict = false;
            bool functionalVerdict = false;

            /* if function coverage is o.k.*/
            if (moduleToFillResults.ValueOfFunctionCoverage.Equals(100))
            {
                /* set flag */
                fcVerdict = true;
            }
            /* if every scenario is "green" */
            if (moduleToFillResults.TotalNumberOfTestcases.Equals(moduleToFillResults.NumberOfSuccessfulTestcases))
            {
                /* set flag */
                functionalVerdict = true;
            }
            /* if both of flags are set - assign o.k. verdict value */
            if (fcVerdict.Equals(true) && functionalVerdict.Equals(true))
            {
                moduleToFillResults.Verdict = _Settings.container.OkVerdictValue;
            }
            else /* set n.o.k. otherwise*/
            {
                moduleToFillResults.Verdict = _Settings.container.NokVerdictValue;
            }

            /* list of scenario results*/
            List<Scenario> scenarios = new List<Scenario>();

            /* for every scenario */
            foreach(var scenario in TestObjectResults)
            {
                /* get its name */
                string name = scenario.Attribute("name").Value;
                string specification = string.Empty;
                string verdict = string.Empty;

                try /* try to read scenario specification */
                {
                    specification = scenario.Attribute("specification").Value;
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Warning: Scenario {scenario.Name} has no filled specification in Tessy tests\n {ex}");
                }
                
                /* evaluate verdict of scenario */
                if(scenario.Attribute("success").Value.Equals("ok"))
                {
                    verdict = _Settings.container.OkVerdictValue;
                }
                else if(scenario.Attribute("success").Value.Equals("nok"))
                {
                    verdict = _Settings.container.NokVerdictValue;
                }
                else
                {
                    verdict = _Settings.container.NotTestedVerdictValue;
                }

                /* add new scenario to list */
                scenarios.Add(new Scenario(name, specification, verdict));
            }

            /* assign scenarios to module */
            moduleToFillResults.Scenarios = scenarios;
        }

        /* gets scenarios from test report */
        protected override List<XElement> GetTestObjectResults()
        {
            return ResultsReport.Descendants("testcase").ToList();
        }
    }
}
