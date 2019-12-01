using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using _IO = System.IO;
using _Settings = to_doors_app.Providers.SettingsProvider.SettingsProvider;

namespace to_doors_app.ViewModels.Helpers
{
    public class ViewModelHelpers
    {
        public static string GetMtsFilePath()
        {
            string currentPath = string.Empty;

            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = "xlsx";
                dialog.InitialDirectory = _IO.Directory.GetCurrentDirectory();

                if (dialog.ShowDialog() != null && dialog.FileName != string.Empty)
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
    }
}
