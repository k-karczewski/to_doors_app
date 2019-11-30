using System;
using System.Collections.Generic;
using System.Text;

namespace to_doors_app.Interfaces.Providers.OutputProvider
{
    public interface IOutputProviderBase<T>
    {
        void GenerateFiles(ref List<T> modulesToGenerate);
    }
}
