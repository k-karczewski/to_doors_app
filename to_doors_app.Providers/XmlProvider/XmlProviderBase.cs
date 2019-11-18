using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using to_doors_app.Interfaces.Providers;
using to_doors_app.Models;

namespace to_doors_app.Providers.XmlProvider
{
    public abstract class XmlProviderBase : IXmlProviderBase
    {
        protected XElement XmlData { get; set; } = null;
        
        protected string ModuleName { get; } = "";

        protected XmlProviderBase(string moduleName) 
        {
            ModuleName = moduleName;
        }

        public abstract void FillResultsOfModule(out Module module);

        protected abstract void LoadXmlFile();

        protected abstract int GetTotalNumberOfTestcases();
        
        protected abstract int GetNumberOfSuccesses();
        
        protected abstract int GetNumberOfFails();
        
        protected abstract int GetNumberOfNotExecuted();

        protected abstract string GetModuleVerdict();
    }
}
