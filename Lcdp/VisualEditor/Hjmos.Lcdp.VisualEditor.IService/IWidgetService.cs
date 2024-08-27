using Hjmos.Lcdp.VisualEditor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.IService
{
    public interface IWidgetService
    {
        /// <summary>
        /// 获取组件库列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<WidgetLibModel>> GetLibs();

        /// <summary>
        /// 上传组件库
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        Task<bool> UploadLib(string fileName, string fullPath);

        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <param name="id">组件库ID</param>
        /// <returns></returns>
        Task<IEnumerable<WidgetModel>> GetWidgets(int id);
    }
}
