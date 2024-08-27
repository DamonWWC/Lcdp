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
    public interface IWidgetApi
    {
        /// <summary>
        /// 获取组件库列表
        /// </summary>
        /// <returns></returns>
        [Get("/libs")]
        Task<Result<List<WidgetLib>>> GetLibs();


        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <param name="pid">组件库ID</param>
        /// <returns></returns>
        [Get("/libs/{pid}/widgets")]
        Task<Result<List<Widget>>> GetWidgets(int pid);


        [Multipart]
        [Post("/libs")]
        Task<Result<object>> UploadFiles([AliasAs("files")] IEnumerable<FileInfoPart> streams, object model);

    }
}
