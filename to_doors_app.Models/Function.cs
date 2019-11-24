using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class Function
    {
        /* name of function */
        public string Name { get; }
        /* total number of testcases */
        public int NumberOfTestcases { get; set; } = 0;
        /* number of "green" testcases */
        public int NumberOfSuccessfulTestcases { get; set; } = 0;
        /* number of "red" testcases */
        public int NumberOfFailedTestcases { get; set; } = 0;
        /* number of not executed testcases */
        public int NumberOfNotExecutedTestcases { get; set; } = 0;
        /* value of c1 coverage in file */
        public double ValueOfC1Coverage { get; set; } = 0;
        /* value of mc/dc coverage in file */
        public double ValueOfMcDcCoverage { get; set; } = 0;
        /* test verdict of file */
        public string Verdict { get; set; } = "test not done";

        public Function(string name)
        {
            Name = name;
        }
    }
}
