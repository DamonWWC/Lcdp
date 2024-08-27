using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.IService
{
    public interface IPageService
    {
        /// <summary>
        /// 保存页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<bool> Save(File page);

        /// <summary>
        /// 根据Guid加载页面
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<File> Load(string guid);

        /// <summary>
        /// 根据文件ID获取页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<File> GetPage(string id);


        /// <summary>
        /// 保存参数
        /// TODO：应该放到不同Service里面
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<bool> Add(ParameterModel parameter);

        /// <summary>
        /// 加载所有参数
        /// </summary>
        /// <returns></returns>
        Task<List<ParameterModel>> LoadParameter(int appid);

        /// <summary>
        /// 删除参数
        /// TODO：应该放到不同Service里面
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<bool> Delete(ParameterModel parameter);
    }
}
