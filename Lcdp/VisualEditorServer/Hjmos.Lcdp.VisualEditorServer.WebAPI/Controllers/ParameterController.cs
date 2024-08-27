using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Hjmos.Lcdp.VisualEditorServer.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Controllers
{
    [Route("v1/parameters")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ParameterController : ControllerBase
    {
        private readonly IPageService _pageService;
        public ParameterController(IPageService templateService) => _pageService = templateService;

        /// <summary>
        /// 获取参数列表
        /// </summary>
        /// <returns></returns>
        [Route("{appid}")]
        [HttpGet]
        public IActionResult GetAllParameters(int appid)
        {
            var directorys = _pageService.Query<Parameter>().Where(x => x.AppId == appid);

            return Ok(Result<List<Parameter>>.Success(directorys.ToList()));
        }

        [HttpPost]
        public IActionResult Save(Parameter parameter)
        {

            _pageService.Insert(parameter);

            return Ok(Result<bool>.Success(true));
        }

        [HttpDelete]
        public IActionResult Delete(Parameter parameter)
        {

            _pageService.Delete(parameter);

            return Ok(Result<bool>.Success(true));
        }
    }
}
