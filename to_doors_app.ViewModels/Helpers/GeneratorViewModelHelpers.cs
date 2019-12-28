using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using to_doors_app.Interfaces;
using to_doors_app.Models.Dtos;
using _IO = System.IO;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.ViewModels.Helpers
{
    public class GeneratorViewModelHelpers
    {
        public static void RefreshViewModel(GeneratorViewModel model, PropertyChangedEventHandler propertyChanged, string nameOfField)
        {
            if(propertyChanged != null)
            {
                propertyChanged.Invoke(model, new PropertyChangedEventArgs(nameOfField));
            }
        }

        public static List<string> GetOverviewReportPaths()
        {
            List<string> currentPaths = null;

            try
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = _IO.Directory.GetCurrentDirectory();
                dialog.Multiselect = true;
                dialog.DefaultExtension = ".xml";

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileNames != null)
                {
                    currentPaths = new List<string>();
                    
                    foreach (string path in dialog.FileNames)
                    {
                        currentPaths.Add(path);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during setting path to mts file:\n {ex}");
            }

            return currentPaths;
        }

        public static string GetMtsFilePath()
        {
            string currentPath = string.Empty;

            try
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.DefaultExtension = ".xlsx";
                dialog.InitialDirectory = _IO.Directory.GetCurrentDirectory();
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
                {
                    currentPath = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during setting path to mts file:\n {ex}");
            }

            return currentPath;
        }

        public static string GetOutputPath()
        {
            string directoryPath = string.Empty;

            try
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = _IO.Directory.GetCurrentDirectory();
                dialog.IsFolderPicker = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
                {
                    directoryPath = dialog.FileName;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during output path:\n {ex}");
            }

            return directoryPath;
        }

        public static List<string> GetModuleNamesFromPaths(List<string> overviewReportPaths, OperationType actualOperation)
        {
            List<string> moduleNamesToReturn = new List<string>();
            
            foreach (string path in overviewReportPaths)
            {
                string moduleName = path.Substring(path.LastIndexOf("\\") + 1, path.Length - path.LastIndexOf("\\") - 1 - _Settings.GetSetting(actualOperation, "OverviewReportSufix").Length);
                moduleNamesToReturn.Add(moduleName);
            }

            return moduleNamesToReturn;
        }

        internal static void SetOverviewReportsPathsToModules(ref List<ModuleToUiDto> itemsToAdd, List<string> reportsPaths)
        {
            foreach (string path in reportsPaths)
            {
                foreach (ModuleToUiDto module in itemsToAdd)
                {
                    /* if path contains module name */
                    if (path.Contains(module.Name))
                    {
                        /* path was found */
                        module.PathToOverviewReport = path;
                        /* go to next path*/
                        break;
                    }
                }
            }
        }
    }
}
