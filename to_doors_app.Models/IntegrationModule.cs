using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class IntegrationModule : Module
    {
        public List<Scenario> Scenarios { get; set; } = null;

        public IntegrationModule(string name, string baseline, string trNumber, int rowInMts, int colInMts) : base(name, baseline, trNumber, rowInMts, colInMts) { }
    }
}
