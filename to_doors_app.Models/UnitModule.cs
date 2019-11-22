using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Models
{
    public class UnitModule : Module
    {
        public List<File> Files { get; set; }

        public UnitModule(string name, string baseline, string trNumber, int rowInMts, int colInMts, List<File> files) : base(name, baseline, trNumber, rowInMts, colInMts)
        {
            Files = files;
        }
    }
}
