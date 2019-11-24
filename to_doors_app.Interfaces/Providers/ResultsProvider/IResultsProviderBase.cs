using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using to_doors_app.Models;

namespace to_doors_app.Interfaces.Providers
{
    public interface IResultsProviderBase<T>
    {
        void FillResultsOfModules(ref List<T> modules, OperationType operationType);
    }
}
