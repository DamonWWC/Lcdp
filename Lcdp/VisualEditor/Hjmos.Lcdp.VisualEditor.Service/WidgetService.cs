using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditor.Service.Apis;
using Hjmos.Lcdp.Loger;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.Service
{
    public class WidgetService : IWidgetService
    {
        private readonly LogHelper _logHelper = new LogHelper();

        /// <summary>
        /// 获取组件库列表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<WidgetLibModel>> GetLibs()
        {
            var result = await RestService.For<IWidgetApi>(RestConfig.BaseUrl).GetLibs();

            return result.Data.Select((lib, index) => new WidgetLibModel
            {
                Index = (index + 1).ToString("00"),
                Id = lib.Id,
                Name = lib.Name,
                UploadTime = lib.UploadTime
            }).ToList();
        }

        /// <summary>
        /// 上传组件库
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public async Task<bool> UploadLib(string fileName, string fullPath)
        {
            string strMD5 = GetMD5HashFromFile(fullPath);

            var api = RestService.For<IWidgetApi>(RestConfig.BaseUrl);
            var files = new List<FileInfoPart>()
            {
                new FileInfoPart(new FileInfo(fullPath),fileName)
            };

            _ = await api.UploadFiles(files, new { MD5 = strMD5 });


            return true;
        }

        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <param name="pid">组件库ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<WidgetModel>> GetWidgets(int pid)
        {
            var result = await RestService.For<IWidgetApi>(RestConfig.BaseUrl).GetWidgets(pid);

            return result.Data.Select((w, index) => new WidgetModel
            {
                Index = (index + 1).ToString("00"),
                Id = w.Id,
                Name = w.Name,
                DisplayName = w.DisplayName,
                Category = w.Category,
                Icon = w.Icon,
                RenderAsSample = w.RenderAsSample
            }).ToList();
        }

        /// <summary>
        /// 获取文件MD5
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string msg = "获取文件MD5时发生异常。";
                _logHelper.Error(msg, ex);
                throw new Exception(msg);
            }
        }
    }
}
