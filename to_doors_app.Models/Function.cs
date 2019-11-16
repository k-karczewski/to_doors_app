using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class Function
    {
        public string Name { get; }
        public int NumberOfTestcases { get; set; } = 0;
        public int NumberOfSuccessfulTestcases { get; set; } = 0;
        public int NumberOfFailedTestcases { get; set; } = 0;
        public int NumberOfNotExecutedTestcases { get; set; } = 0;
        public double ValueOfC1Coverage { get; set; } = 0;
        public double ValueOfMcDcCoverage { get; set; } = 0;
        public string Verdict { get; set; } = "test not done";

        public Function(string name)
        {
            Name = name;
        }
    }
}
