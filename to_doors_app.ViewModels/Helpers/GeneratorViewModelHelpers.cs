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
        /// <summary>
        /// Refreshes viewmodel if propertyChanged parameter is not null 
        /// </summary>
        /// <param name="model">sender object</param>
        /// <param name="propertyChanged">event handler</param>
        /// <param name="nameOfField">name of property that is needed to refresh</param>
        public static void RefreshViewModel(GeneratorViewModel model, PropertyChangedEventHandler propertyChanged, string nameOfField)
        {
            //if not null invoke event listener
            propertyChanged?.Invoke(model, new PropertyChangedEventArgs(nameOfField));
        }

        /// <summary>
        /// Gets ovierview reports' paths
        /// </summary>
        /// <returns>List of paths to overview reports</returns>
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

        /// <summary>
        /// This helper method opens open file dialog and allows picking of .xlsx document
        /// </summary>
        /// <returns>Path to module test state document (excel) or empty string when user canceled operation</returns>
        public static string GetMtsFilePath()
        {
            string currentPath = string.Empty;

            try
            {
                // Create dialog
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.DefaultExtension = ".xlsx";
                dialog.InitialDirectory = _IO.Directory.GetCurrentDirectory();
                dialog.Multiselect = false;

                // if file was chosen
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
                {
                    currentPath = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during setting path to mts file:\n {ex}");
            }

            // return path
            return currentPath;
        }

        /// <summary>
        /// Opens open directory dialog and allows picking of output folder
        /// </summary>
        /// <returns>Path to chosen folder</returns>
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

        /// <summary>
        /// Adds overview report path to correct ModuleToUiDto object
        /// </summary>
        /// <param name="itemsToAdd">list of processed objects</param>
        /// <param name="reportsPaths">list of overview reports paths</param>
        internal static void SetOverviewReportsPathsToModules(ref List<ModuleToUiDto> itemsToAdd, List<string> reportsPaths)
        {
            //for each path found 
            foreach (string path in reportsPaths)
            {
                //for each module
                foreach (ModuleToUiDto module in itemsToAdd)
                {
                    //if path contains module name
                    if (path.Contains(module.Name))
                    {
                        //path was found
                        module.PathToOverviewReport = path;
                        //go to next path
                        break;
                    }
                }
            }
        }
    }
}
