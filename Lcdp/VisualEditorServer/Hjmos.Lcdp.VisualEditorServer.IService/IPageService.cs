using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditorServer.IService
{
    public interface IPageService : IBaseService
    {
        /// <summary>
        /// 新增或更新组件库信息
        /// </summary>
        /// <param name="lib"></param>
        /// <returns></returns>
        int AddOrUpdate(WidgetLib lib);

        /// <summary>
        /// 获取文件夹子项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<FileDTO> GetSubItems(int id);
    }
}
