using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hjmos.Lcdp.Extensions
{

    public static class DirectoryInfoExtension
    {
        /// <summary>
        /// 获取目录下指定后缀的所有文件
        /// </summary>
        /// <param name="dir">目录路径</param>
        /// <param name="extensions">后缀列表</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            // 获取目录下所有文件（包括子文件夹中的文件）
            IEnumerable<FileInfo> files = dir.EnumerateFiles("*", SearchOption.AllDirectories);
            return files.Where(f => extensions.Contains(f.Extension));
        }

        /// <summary>
        /// 获取目录下所有程序集文件
        /// </summary>
        /// <param name="dir">目录路径</param>
        /// <returns></returns>
        public static List<Assembly> GetAssemblys(this DirectoryInfo dir)
        {
            // 后缀过滤
            string[] extensions = new string[] { ".dll", ".exe" };

            // 获取目录下所有程序集文件（包括子文件夹中的文件）
            IEnumerable<FileInfo> fileInfoList = dir.EnumerateFiles("*", SearchOption.AllDirectories).Where(f => extensions.Contains(f.Extension));

            // TODO：排除项目探测路径下已有的程序集
            // 不排除的话，由于native image的存在，会优先加载插件目录中的程序集，会导致宿主程序集失效

            Assembly assembly;
            List<Assembly> assemblyList = new List<Assembly>();

            foreach (FileInfo fileInfo in fileInfoList)
            {
                string fullName = fileInfo.FullName;

                assembly = Assembly.LoadFrom(Path.Combine(dir.FullName, fileInfo.FullName));
                if (assembly != null)
                {
                    assemblyList.Add(assembly);
                }
            }
            return assemblyList;
        }
    }
}
