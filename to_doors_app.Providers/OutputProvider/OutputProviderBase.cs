using System;
using System.Collections.Generic;
using System.Linq;
using to_doors_app.Interfaces.Providers.OutputProvider;
using to_doors_app.Models;
using _IO = System.IO;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.Providers.OutputProvider
{
    public abstract class OutputProviderBase<T> : IOutputProviderBase<T> where T: Module
    {
        public event EventHandler<string> ShowWorkProgressEvent;

        protected List<T> ModulesToGenerate { get; set; } = null;
        protected string PathToTsvLocation { get; set; } = string.Empty;
        protected string TsvFileSufix { get; set; } = string.Empty;
        protected string CurrentProceededModuleName = string.Empty;
        
        private int MaxNumberOfElementsInRow = 0;
        
        protected OutputProviderBase() { }

        private void LoadSettings()
        {
            PathToTsvLocation = _Settings.GetSetting(_Settings.CurrentOperationType, "PathToTsv");
            TsvFileSufix = _Settings.GetSetting(_Settings.CurrentOperationType, "TsvFileSufix");
            MaxNumberOfElementsInRow = int.Parse(_Settings.GetSetting(_Settings.CurrentOperationType, "MaxNumberOfAttributes")) + 1;
        }

        public void GenerateFiles(ref List<T> modulesTogenerate)
        {
            ModulesToGenerate = modulesTogenerate;

            LoadSettings();

            // start generating .tsv files
            foreach (T module in ModulesToGenerate)
            {
                CurrentProceededModuleName = module.Name;
                ChangeProgressInfo($"Generating .tsv file of module {CurrentProceededModuleName}");

                // if file with this name exists in output folder
                if (_IO.File.Exists($"{PathToTsvLocation}\\{CurrentProceededModuleName}{TsvFileSufix}"))
                {
                    // delete it
                    _IO.File.Delete($"{PathToTsvLocation}\\{CurrentProceededModuleName}{TsvFileSufix}");
                }

                // start creating new file
                SendHeaderInfo();
                PrepareTestData(module);
            }
            ChangeProgressInfo("Done");
        }

        /* prepares header info (attributes, controller name) and sends it to file */
        protected void SendHeaderInfo()
        {
            /* declaration of list that will store data to send*/
            List<string> elements = new List<string>
            {
                "test_description"
            };

            elements.AddRange(_Settings.container.Attributes.Values.ToList().Take(MaxNumberOfElementsInRow-1));

            /* send attributes to file*/
            SendElementsToFile(elements);
            
            /* sent data is not needed now - clear it */
            elements = new List<string>
            {
                _Settings.container.ControllerName
            };
            
            /*for currnet row only one element is needed - rest of them should be empty*/
            FillListWithEmptyObjects(ref elements);
            
            /* send data to file */
            SendElementsToFile(elements);

            elements = new List<string>
            {
               GetTestType()
            };

            FillListWithEmptyObjects(ref elements);
            SendElementsToFile(elements);
        }

        /* temporary solution */
        protected virtual string GetTestType()
        {
            return string.Empty;
        }

        protected void CreateDirectory()
        {
            /* if folder with tsv files exists*/
            if (_IO.Directory.Exists(PathToTsvLocation).Equals(true))
            {
                /* check if directory contains some files */
                string[] fileNames = _IO.Directory.GetFiles(PathToTsvLocation);

                /* if directory is not empty */
                if (fileNames.Length > 0)
                {
                    /* delete all files*/
                    for(int i = 0; i < fileNames.Length; i++)
                    {
                        _IO.File.Delete($"{fileNames[i]}");
                    }
                }
            }
            else
            {
                /* create new empty folder*/
                _IO.Directory.CreateDirectory(PathToTsvLocation);
            }
        }

        /* sends list of elements to file. if element is equal to string.empty tab is provided instead */
        protected void SendElementsToFile(List<string> elements)
        {
            /* data buffer */
            string output = "";

            /* combine elements to one buffer */
            for (int i = 0; i < elements.Count; i++)
            {
                /* handle not empty objects */
                if (elements[i].Equals("") == false)
                {
                    if (i.Equals(elements.Count - 1) && elements[elements.Count - 1] != "")
                    {
                        output += "\"" + elements[i] + "\"\n";
                    }
                    else
                    {
                        output += "\"" + elements[i] + "\"\t";
                    }
                }
                else /* handle empty objects */
                {
                    if (i.Equals(elements.Count - 1))
                    {
                        output += "\n";
                    }
                    else
                    {
                        output += "\t";
                    }
                }
            }

            /* write buffer to file */
            using (_IO.StreamWriter sw = _IO.File.AppendText($"{PathToTsvLocation}\\{CurrentProceededModuleName}{TsvFileSufix}"))
            {
                sw.Write(output);
            }
        }

        protected virtual void PrepareTestData(T module)
        {
            throw new NotImplementedException();
        }

        /* fills list of strings with empty string object - size of list is always 9 (for unit tests and mts) and 10 for (module integration) */
        protected void FillListWithEmptyObjects(ref List<string> elements)
        {
            /* copy original size of list */
            int origCount = elements.Count;

            /* add empty string objects to list */
            for (int i = 0; i < (MaxNumberOfElementsInRow) - origCount; i++) /* + 1 because "test_description" is added by default */
            {
                elements.Add("");
            }
        }

        protected void ChangeProgressInfo(string message)
        {
            ShowWorkProgressEvent?.Invoke(this, message);
        }
    }
}
