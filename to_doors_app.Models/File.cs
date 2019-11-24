using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class File
    {
        /* name of .c file */
        public string Name { get; set; }
        /* version of .c file */
        public string Revision { get; set; }
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
        /* list of functions that are implemented in file */
        public List<Function> Functions { get; set; } = null;

        /*first constructor */
        public File(string name, string revision)
        {
            Name = name;
            Revision = revision;
        }

        /* second constructor */
        public File(string name, string revision, List<Function> functions)
        {
            Name = name;
            Revision = revision;
            Functions = functions;
        }
    }
}
