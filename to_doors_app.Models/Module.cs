using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public abstract class Module
    {
        /* property that stores module name */
        public string Name { get; }

        /* property that stores module label / baseline */
        public string Baseline { get; set; }
        
        /* property that stores test report number */
        public string TrNumber { get; set; }
        
        /* coordinates of module in MTS document */
        public int RowInMts { get; }
        public int ColInMts { get; }

        /* path to overview results report (.xml)*/
        public string PathToOverviewReport { get; set; } = string.Empty; /* data not loaded yet*/

        public Module(string name, string baseline, string trNumber, int rowInMts, int colInMts)
        {
            Name = name;
            Baseline = baseline;
            TrNumber = trNumber;
            RowInMts = rowInMts;
            ColInMts = colInMts;
        }
    }
}
