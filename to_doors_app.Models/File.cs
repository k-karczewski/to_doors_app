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
        /* list of function names included in file - used only in MTS variant*/
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
