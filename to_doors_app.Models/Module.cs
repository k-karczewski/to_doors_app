using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class Module
    {
        /* property that stores module name */
        public string Name { get; }

        /* property that stores module label / baseline */
        public string Baseline { get; }

        /* list that stores files info */
        public List<File> Files { get; }
        
        /* property that stores test report number */
        public string TrNumber { get; }
        
        /* coordinates of module in MTS document */
        public int RowInMts { get; }
        public int ColInMts { get; }

        public Module(string name, string baseline, string trNumber, int rowInMts, int colInMts)
        {
            Name = name;
            Baseline = baseline;
            Files = null;
            TrNumber = trNumber;
            RowInMts = rowInMts;
            ColInMts = colInMts;
        }

        public Module(string name, string baseline, List<File> files, string trNumber, int rowInMts, int colInMts)
        {
            Name = name;
            Baseline = baseline;
            Files = files;
            TrNumber = trNumber;
            RowInMts = rowInMts;
            ColInMts = colInMts;
        }
    }
}
