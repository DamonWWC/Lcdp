using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditor.Service.Apis;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using Refit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.Service
{
    public class PageService : IPageService
    {
        /// <summary>
        /// 保存页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<bool> Save(File page)
        {
            var result = await RestService.For<IPageApi>(RestConfig.BaseUrl).SavePage(page);
            return result.Data;
        }

        /// <summary>
        /// 根据Guid加载页面
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async Task<File> Load(string guid)
        {
            var result = await RestService.For<IPageApi>(RestConfig.BaseUrl).LoadPage(guid);
            return result.Data;
        }

        /// <summary>
        /// 根据文件ID获取页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<File> GetPage(string id)
        {
            var result = await RestService.For<IPageApi>(RestConfig.BaseUrl).GetPage(id);
            return result.Data;
        }

        /// <summary>
        /// 保存参数
        /// TODO：应该放到不同Service里面
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<bool> Add(ParameterModel parameter)
        {
            var result = await RestService.For<IPageApi>(RestConfig.BaseUrl).AddParameter(parameter);
            return result.Data;
        }

        /// <summary>
        /// 加载所有参数
        /// </summary>
        /// <returns></returns>
        public async Task<List<ParameterModel>> LoadParameter(int appid)
        {
            var result = await RestService.For<IPageApi>(RestConfig.BaseUrl).LoadParameter(appid);

            return result.Data.Select((p, index) => new ParameterModel
            {
                Index = (index + 1).ToString("00"),
                Id = p.Id,
                Name = p.Name,
                Range = p.Range,
                Value = p.Value
            }).ToList();

        }

        /// <summary>
        /// 删除参数
        /// TODO：应该放到不同Service里面
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<bool> Delete(ParameterModel parameter)
        {
            var result = await RestService.For<IPageApi>(RestConfig.BaseUrl).DeleteParameter(parameter);
            return result.Data;
        }
    }
}
