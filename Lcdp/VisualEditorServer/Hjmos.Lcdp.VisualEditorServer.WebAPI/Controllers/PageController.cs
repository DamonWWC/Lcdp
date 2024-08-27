using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Hjmos.Lcdp.VisualEditorServer.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Controllers
{
    /// <summary>
    /// 页面
    /// </summary>
    [Route("v1/pages")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class PageController : Controller
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService) => _pageService = pageService;

        /// <summary>
        /// 保存页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Save(File page)
        {
            // 根据文件ID查找数据库中的页面文件
            File pageFromDB = _pageService.Query<File>().FirstOrDefault(x => x.Id == page.Id);

            // 更新或保存页面
            if (pageFromDB != null)
            {
                pageFromDB.Name = page.Name;
                pageFromDB.Content = page.Content;
                _pageService.Update(pageFromDB);
            }

            return Ok(Result<bool>.Success(true));
        }

        /// <summary>
        /// 查找页面
        /// </summary>
        /// <param name="guid">页面GUID</param>
        /// <returns></returns>
        [Route("{guid}")]
        [HttpGet]
        public IActionResult Load(string guid)
        {
            // 根据Guid查找页面
            File page = _pageService.Query<File>(x => x.Guid == guid).First();

            return Ok(Result<File>.Success(page));
        }

        /// <summary>
        /// 删除页面
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // TODO：删除改成软删除，查询过滤已删除文件
            _pageService.Delete<File>(id);

            return Ok(Result<bool>.Success(true));
        }

        /// <summary>
        /// 根据文件ID查找页面
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        [Route("getbyid/{id}")]
        [HttpGet]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetById(int id)
        {
            // 根据文件ID查找页面
            var page = _pageService.Query<File>().Where(x => x.Id == id).OrderBy(x => x.Guid).LastOrDefault();

            return Ok(Result<File>.Success(page));
        }

        /// <summary>
        /// 新增页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddPage(JObject page)
        {
            int folderId = (int)page["id"];

            // 通过上级目录获取应用ID
            Directory folder = _pageService.Query<Directory>().FirstOrDefault(x => x.Id == folderId);

            if (folder is null)
            {
                // TODO：报错
            }

            // 添加页面文件
            _ = _pageService.Insert(new File()
            {
                Name = page["name"].ToString(),
                FolderId = folderId,
                Icon = ((char)int.Parse("e64d", NumberStyles.HexNumber)).ToString(),
                Sort = 0, // TODO：暂时都写成0
                Suffix = "jtf",
                Category = 1,// TODO：写成Enum
                Path = string.Empty,
                Content = string.Empty,
                AppId = folder.AppId,
                Guid = Guid.NewGuid().ToString(),
                CreateBy = 1, // TODO：改为用户ID
                CreateTime = DateTime.Now,
                UpdateBy = 1, // TODO：改为用户ID
                UpdateTime = DateTime.Now,
                Status = 1
            });

            return Ok(Result<bool>.Success(true));
        }
    }
}
