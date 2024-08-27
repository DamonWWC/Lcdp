using Hjmos.Lcdp.VisualEditor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.IService
{
    public interface IModuleService
    {
        /// <summary>
        /// 获取应用项目
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FileModel>> GetModules();

        /// <summary>
        /// 获取文件夹子项
        /// </summary>
        /// <param name="directoryId"></param>
        /// <returns></returns>
        Task<IEnumerable<FileModel>> GetSubItems(int directoryId);

        /// <summary>
        /// 新增页面
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> AddPage(int directoryId, string name);

        /// <summary>
        /// 新增目录
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> AddDirectory(int directoryId, string name);


        /// <summary>
        /// 删除文件或文件夹
        /// </summary>
        /// <param name="fileModel"></param>
        /// <returns></returns>
        Task<bool> DeleteFileOrDirectory(FileModel fileModel);

    }
}
