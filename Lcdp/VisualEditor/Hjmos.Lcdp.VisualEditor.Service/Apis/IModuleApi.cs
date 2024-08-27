using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.Service.Apis
{
    /// <summary>
    /// 应用、目录接口
    /// </summary>
    public interface IModuleApi
    {
        /// <summary>
        /// 应用列表
        /// </summary>
        /// <returns></returns>
        [Get("/modules")]
        Task<Result<IEnumerable<FileModel>>> GetModules();

        /// <summary>
        /// 获取文件夹子项
        /// </summary>
        /// <returns></returns>
        [Get("/modules/{directoryId}/items")]
        Task<Result<IEnumerable<FileModel>>> GetSubItems(int directoryId);

        /// <summary>
        /// 新增页面
        /// </summary>
        /// <returns></returns>
        [Post("/pages")]
        Task<Result<bool>> AddPage(object page);


        /// <summary>
        /// 新增目录
        /// </summary>
        /// <returns></returns>
        [Post("/modules")]
        Task<Result<bool>> AddDirectory(object directory);

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <returns></returns>
        [Delete("/modules/{id}")]
        Task<Result<bool>> DeleteDirectory(int id);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        [Delete("/pages/{id}")]
        Task<Result<bool>> DeletePage(int id);

    }
}
