using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Hjmos.Lcdp.VisualEditorServer.Entities.Enums;
using Hjmos.Lcdp.VisualEditorServer.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using System;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Controllers
{
    /// <summary>
    /// 模块
    /// </summary>
    [Route("v1/modules")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ModuleController : Controller
    {
        private readonly IPageService _pageService;

        public ModuleController(IPageService templateService) => _pageService = templateService;

        /// <summary>
        /// 获取页面项目列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Modules()
        {

            var directorys = _pageService.Query<Directory>().Where(x => x.ParentId == 0).OrderBy(x => x.Sort);

            List<FileDTO> SubItems = new();

            SubItems.AddRange(directorys.ToList().Select(x => new FileDTO()
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                Icon = x.Icon,
                Sort = x.Sort,
                Level = x.Level,
                Suffix = null,
                FileType = FileType.Directory
            }));

            return Ok(Result<List<FileDTO>>.Success(SubItems));
        }

        /// <summary>
        /// 获取文件夹子项
        /// </summary>
        /// <param name="id">文件夹ID</param>
        /// <returns></returns>
        [Route("{id}/items")]
        [HttpGet]
        public IActionResult GetSubItems(int id)
        {
            List<FileDTO> subItems = _pageService.GetSubItems(id);

            return Ok(Result<List<FileDTO>>.Success(subItems));
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="id">文件夹ID</param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // TODO：有子项不删除
            if (_pageService.GetSubItems(id).Count > 0)
            {

            }

            _pageService.Delete<Directory>(id);

            return Ok(Result<bool>.Success(true));
        }

        /// <summary>
        /// 新增目录
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddDirectory(JObject directory)
        {

            int parentId = (int)directory["id"];

            Directory dir;
            App app;

            if (parentId == 0)
            {
                // 添加根目录
                dir = _pageService.Insert(new Directory()
                {
                    Name = directory["name"].ToString(),
                    ParentId = 0,
                    Icon = ((char)int.Parse("e666", NumberStyles.HexNumber)).ToString(),
                    Sort = 0,
                    ParentIds = string.Empty,
                    Level = 1,
                    Remarks = string.Empty,
                    AppId = 0,
                    CreateBy = 1, // TODO：改为用户ID
                    CreateTime = DateTime.Now,
                    UpdateBy = 1, // TODO：改为用户ID
                    UpdateTime = DateTime.Now,
                    Status = 1
                });

                // 添加应用
                app = _pageService.Insert(new App()
                {
                    Name = directory["name"].ToString(),
                    RootDir = dir.Id,
                    CreateBy = 1, // TODO：改为用户ID
                    CreateTime = DateTime.Now,
                    UpdateBy = 1, // TODO：改为用户ID
                    UpdateTime = DateTime.Now,
                    Status = 1
                });

                dir.AppId = app.Id;
                dir.ParentIds = dir.Id.ToString();
            }
            else
            {
                Directory parentDir = _pageService.Find<Directory>(parentId);

                // 添加目录
                dir = _pageService.Insert(new Directory()
                {
                    Name = directory["name"].ToString(),
                    ParentId = parentId,
                    Icon = ((char)int.Parse("e666", NumberStyles.HexNumber)).ToString(),
                    Sort = 0,
                    ParentIds = string.Empty,
                    Level = parentDir.Level + 1,
                    Remarks = string.Empty,
                    AppId = parentDir.AppId,
                    CreateBy = 1, // TODO：改为用户ID
                    CreateTime = DateTime.Now,
                    UpdateBy = 1, // TODO：改为用户ID
                    UpdateTime = DateTime.Now,
                    Status = 1
                });
                dir.ParentIds = parentDir.ParentIds + "-" + dir.Id.ToString();
            }

            _pageService.Update(dir);

            return Ok(Result<bool>.Success(true));
        }
    }
}
