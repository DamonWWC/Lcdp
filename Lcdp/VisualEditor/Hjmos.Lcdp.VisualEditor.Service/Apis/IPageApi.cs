using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.Service.Apis
{
    /// <summary>
    /// 组件接口
    /// </summary>
    public interface IPageApi
    {
        /// <summary>
        /// 保存页面
        /// </summary>
        /// <returns></returns>
        [Put("/pages")]
        Task<Result<bool>> SavePage(File page);

        /// <summary>
        /// 根据Guid加载页面
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [Get("/pages/{guid}")]
        Task<Result<File>> LoadPage(string guid);

        /// <summary>
        /// 根据文件ID获取页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/pages/getbyid/{id}")]
        Task<Result<File>> GetPage(string id);

        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Post("/parameters")]
        Task<Result<bool>> AddParameter(ParameterModel parameter);

        /// <summary>
        /// 加载所有参数
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        [Get("/parameters/{appid}")]
        Task<Result<List<ParameterModel>>> LoadParameter(int appid);

        /// <summary>
        /// 删除参数
        /// </summary>
        /// <param name="parameter">参数模型</param>
        /// <returns></returns>
        [Delete("/parameters")]
        Task<Result<bool>> DeleteParameter(ParameterModel parameter);
    }
}
