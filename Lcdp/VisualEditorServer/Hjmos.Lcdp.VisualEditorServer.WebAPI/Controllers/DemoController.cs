using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Controllers
{
    /// <summary>
    /// 示例接口
    /// </summary>
    [Route("v1/demo")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DemoController : ControllerBase
    {
        /// <summary>
        /// 标签列表
        /// </summary>
        /// <returns></returns>
        [Route("basic_1")]
        [HttpGet]
        public IActionResult BasicColumnData1()
        {
            List<object> list = new();

            list.Add(new { Name = "Maria", Year = 2020, Value = 10 });
            list.Add(new { Name = "Susan", Year = 2020, Value = 50 });
            list.Add(new { Name = "Charles", Year = 2020, Value = 39 });
            list.Add(new { Name = "Frida", Year = 2020, Value = 50 });
            list.Add(new { Name = "Maria", Year = 2021, Value = 12 });
            list.Add(new { Name = "Susan", Year = 2021, Value = 48 });
            list.Add(new { Name = "Charles", Year = 2021, Value = 26 });
            list.Add(new { Name = "Frida", Year = 2021, Value = 56 });

            return Ok(Result<List<object>>.Success(list));
        }


        /// <summary>
        /// 标签列表
        /// </summary>
        /// <returns></returns>
        [Route("basic_2")]
        [HttpGet]
        public IActionResult BasicColumnData2()
        {

            List<object> list = new();

            list.Add(new { CName = "玛丽亚", Year = 2018, Value = 35 });
            list.Add(new { CName = "苏珊", Year = 2018, Value = 24 });
            list.Add(new { CName = "查尔斯", Year = 2018, Value = 68 });
            list.Add(new { CName = "弗里达", Year = 2018, Value = 10 });
            list.Add(new { CName = "玛丽亚", Year = 2019, Value = 57 });
            list.Add(new { CName = "苏珊", Year = 2019, Value = 12 });
            list.Add(new { CName = "查尔斯", Year = 2019, Value = 55 });
            list.Add(new { CName = "弗里达", Year = 2019, Value = 85 });

            return Ok(Result<List<object>>.Success(list));
        }

        /// <summary>
        /// 预警统计
        /// </summary>
        /// <returns></returns>
        [Route("alarm_warn_statistics")]
        [HttpGet]
        public IActionResult AlarmWarnStatistics()
        {
            List<object> list = new();

            list.Add(new { Total = 58, Num = 8, Name = string.Empty, Code = string.Empty, Ratio = "14%" });
            list.Add(new { Total = 58, Num = 28, Name = "一级", Code = "811001", Ratio = "48%" });
            list.Add(new { Total = 58, Num = 7, Name = "二级", Code = "811002", Ratio = "12%" });
            list.Add(new { Total = 58, Num = 3, Name = "三级", Code = "811003", Ratio = "5%" });
            list.Add(new { Total = 58, Num = 12, Name = "四级", Code = "811004", Ratio = "21%" });

            return Ok(Result<List<object>>.Success(list));
        }

        /// <summary>
        /// 预警统计
        /// </summary>
        /// <returns></returns>
        [Route("emergency_statistics")]
        [HttpGet]
        public IActionResult EmergencyStatistics()
        {
            List<object> list = new();

            list.Add(new { Total = 63, Num = 31, Name = "一级", Code = "811001", Ratio = "49%" });
            list.Add(new { Total = 63, Num = 10, Name = "二级", Code = "811002", Ratio = "16%" });
            list.Add(new { Total = 63, Num = 20, Name = "三级", Code = "811003", Ratio = "32%" });
            list.Add(new { Total = 63, Num = 2, Name = "四级", Code = "811004", Ratio = "3%" });

            return Ok(Result<List<object>>.Success(list));
        }


    }


}
