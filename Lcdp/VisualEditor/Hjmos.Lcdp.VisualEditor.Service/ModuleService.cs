using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditor.Service;
using Hjmos.Lcdp.VisualEditor.Service.Apis;
using Hjmos.Lcdp.VisualEditorServer.Entities.Enums;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.EditorManagement.Service
{
    public class ModuleService : IModuleService
    {
        /// <summary>
        /// 获取应用列表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<FileModel>> GetModules()
        {
            var result = await RestService.For<IModuleApi>(RestConfig.BaseUrl).GetModules();
            return result.Data;
        }

        /// <summary>
        /// 获取文件夹子项
        /// </summary>
        /// <param name="directoryId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FileModel>> GetSubItems(int directoryId)
        {
            var result = await RestService.For<IModuleApi>(RestConfig.BaseUrl).GetSubItems(directoryId);
            return result.Data;
        }

        /// <summary>
        /// 新增页面
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> AddPage(int directoryId, string name)
        {
            var result = await RestService.For<IModuleApi>(RestConfig.BaseUrl).AddPage(new { id = directoryId, name });
            return result.Data;
        }

        /// <summary>
        /// 新增目录
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> AddDirectory(int directoryId, string name)
        {
            var result = await RestService.For<IModuleApi>(RestConfig.BaseUrl).AddDirectory(new { id = directoryId, name });

            return result.Data;
        }

        /// <summary>
        /// 删除文件或文件夹
        /// </summary>
        /// <param name="fileModel"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileOrDirectory(FileModel fileModel)
        {
            if (fileModel.FileType == FileType.Directory)
            {
                await RestService.For<IModuleApi>(RestConfig.BaseUrl).DeleteDirectory(fileModel.Id);
            }

            if (fileModel.FileType == FileType.File)
            {
                await RestService.For<IModuleApi>(RestConfig.BaseUrl).DeletePage(fileModel.Id);
            }

            return true;
        }
    }
}
