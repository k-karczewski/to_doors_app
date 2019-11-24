using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class IntegrationModule : Module
    {
        /* total number of scenarios */
        public int TotalNumberOfTestcases { get; set; } = 0;
        /* number of "green" scenarios */
        public int NumberOfSuccessfulTestcases { get; set; } = 0;
        /* number of "red" scenarios */
        public int NumberOfFailedTestcases { get; set; } = 0;
        /* number of not executed scenarios */
        public int NumberOfNotExecutedTestcases { get; set; } = 0;
        /* value of function coverage */
        public double ValueOfFunctionCoverage { get; set; } = 0;
        /* test verdict of module */
        public string Verdict { get; set; } = "test not done";
        /* list of scenarios*/
        public List<Scenario> Scenarios { get; set; } = null;

        /* constructor */
        public IntegrationModule(string name, string baseline, string trNumber, int rowInMts, int colInMts) : base(name, baseline, trNumber, rowInMts, colInMts) { }
    }
}
