using System;
using System.Collections.Generic;
using System.Text;
using to_doors_app.Models;
using to_doors_app.Models.Dtos;

namespace to_doors_app.Interfaces.Providers
{
    public interface IGeneratorServiceBase
    {
        void LoadDataFromMts(List<string> moduleNames);
        void LoadDataFromMts();
        void SaveEditedDataByUi(List<ModuleToUiDto> editedModules);
        void CloseExcelDocument();
        void SetSheetName(string sheetName);
        bool IsServiceReadyToGenerateFiles();
        List<string> GetSheetNames(string pathToMts);
        List<ModuleToUiDto> GetDtosForUi();
        void RemoveModuleData(string moduleName);
    }
}
