using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Hjmos.Lcdp.VisualEditorServer.ICommon;
using Hjmos.Lcdp.VisualEditorServer.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Controllers
{
    /// <summary>
    /// 组件
    /// </summary>
    [Route("v1/widgets")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class WidgetController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IConfiguration _configuration;

        public WidgetController(IConfiguration configuration, IPageService pageService)
        {
            _configuration = configuration;
            _pageService = pageService;
        }

        /// <summary>
        /// 获取组件库列表
        /// </summary>
        /// <returns></returns>
        [Route("~/v1/libs")]
        [HttpGet]
        public IActionResult GetLibs()
        {
            var result = _pageService.Query<WidgetLib>().ToList();

            return Ok(Result<List<WidgetLib>>.Success(result));
        }

        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <param name="id">组件库ID</param>
        /// <returns></returns>
        [Route("~/v1/libs/{id}/widgets")]
        [HttpGet]
        public IActionResult GetWidgets(int id)
        {
            var result = _pageService.Query<Widget>(x => x.LibId == id).ToList();

            return Ok(Result<List<Widget>>.Success(result));
        }

        /// <summary>
        /// 上传组件
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Route("~/v1/libs")]
        [HttpPost]
        public IActionResult Upload([FromForm] IFormCollection formCollection)
        {
            FormFileCollection filelist = (FormFileCollection)formCollection.Files;
            if (filelist.Count > 0)
            {
                // 文件名
                string fileName = filelist[0].FileName;
                // 请求参数
                dynamic model = JsonConvert.DeserializeObject<dynamic>(formCollection["model"]);
                // 文件存放位置
                // TODO：文件要改名称，并且按日期分目录保存
                string path = Path.Combine(_configuration.Read("FileFolder"), "Upload");

                DirectoryInfo di = new(path);
                if (!di.Exists) di.Create();

                using (FileStream fs = System.IO.File.Create(Path.Combine(path, fileName)))
                {
                    // 复制文件
                    filelist[0].CopyToAsync(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }

                // 更新或新增到数据库
                WidgetLib lib = new()
                {
                    Name = fileName,
                    MD5 = model.mD5,
                    UploadTime = DateTime.Now
                };
                _pageService.AddOrUpdate(lib);
            }
            // TODO：改为返回已上传组件的详情
            return Ok(Result<dynamic>.Success(new { count = filelist.Count, len = filelist.Sum(f => f.Length) }));
        }
    }
}
