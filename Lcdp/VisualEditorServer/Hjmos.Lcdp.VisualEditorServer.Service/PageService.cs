using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using Hjmos.Lcdp.VisualEditorServer.Entities.Enums;
using Hjmos.Lcdp.VisualEditorServer.ICommon;
using Hjmos.Lcdp.VisualEditorServer.IService;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditorServer.Service
{
    public class PageService : BaseService, IPageService
    {
        public PageService(IConnectionFactory contextFactory) : base(contextFactory) { }

        /// <summary>
        /// 新增或更新组件库信息
        /// </summary>
        /// <param name="lib"></param>
        /// <returns></returns>
        public int AddOrUpdate(WidgetLib lib)
        {
            List<int> query = (from q in Context.Set<WidgetLib>() where q.Name == lib.Name select q.Id).ToList();

            var entity = Context.Entry(lib);
            if (query.Any())
            {
                lib.Id = query[0];
                entity.State = EntityState.Modified;
            }
            else
            {
                entity.State = EntityState.Added;
            }
            return Context.SaveChanges();
        }

        /// <summary>
        /// 获取文件夹子项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FileDTO> GetSubItems(int id)
        {
            // 文件夹列表
            IEnumerable<FileDTO> directorys = Context.Set<Directory>().Where(x => x.ParentId == id).OrderBy(x => x.Sort)
                .Select(x => new FileDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    Icon = x.Icon,
                    Sort = x.Sort,
                    Level = x.Level,
                    Suffix = null,
                    FileType = FileType.Directory
                });


            // 文件列表
            IEnumerable<FileDTO> files = Context.Set<File>().Where(x => x.FolderId == id).OrderBy(x => x.Sort)
                .Select(x => new FileDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.FolderId,
                    Icon = x.Icon,
                    Sort = x.Sort,
                    Suffix = x.Suffix,
                    FileType = FileType.File,
                    Guid = x.Guid
                });

            return directorys.Concat(files).ToList();
        }
    }
}
