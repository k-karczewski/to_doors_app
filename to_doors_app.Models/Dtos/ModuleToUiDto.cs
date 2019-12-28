using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models.Dtos
{
    public class ModuleToUiDto
    {
        /* property that stores module name */
        public string Name { get; }

        /* property that stores module label / baseline */
        public string Baseline { get; set; }

        /* property that stores test report number */
        public string TrNumber { get; set; }

        /* path to overview results report (.xml) - will be set by the viewmodel*/
        public string PathToOverviewReport { get; set; } = string.Empty; /* data not loaded yet*/

        public ModuleToUiDto(string name, string baseline, string trNumber)
        {
            Name = name;
            Baseline = baseline;
            TrNumber = trNumber;
        }
    }
}
