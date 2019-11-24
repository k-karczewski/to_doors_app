using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class Scenario
    {
        /* name of scenario */
        public string Name { get; }
        /* list of methods that were reached by scenario */
        public string Specification { get; }
        /* verdict of scenario */
        public string Verdict { get; }

        /* constructor */
        public Scenario(string name, string specification, string verdict)
        {
            Name = name;
            Specification = specification;
            Verdict = verdict;
        }
    }
}
