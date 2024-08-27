using HandyControl.Data;
using HandyControl.Themes;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    /// <summary>
    /// 资源帮助类
    /// </summary>
    public class ResourceHelper
    {
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetResource<T>(string key) => Application.Current.TryFindResource(key) is T resource ? resource : default;

        public static T GetResourceInternal<T>(string key) => GetTheme()[key] is T resource ? resource : default;

        public static T GetResourceInternal<T>(string key, string uriString) => GetTheme(uriString)[key] is T resource ? resource : default;

        public static Theme GetTheme(string name, ResourceDictionary resourceDictionary)
        {
            if (string.IsNullOrEmpty(name) || resourceDictionary == null)
            {
                return null;
            }

            return resourceDictionary.MergedDictionaries.OfType<Theme>().FirstOrDefault(item => Equals(item.Name, name));
        }

        /// <summary>
        /// 获取皮肤
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="themePath"></param>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static ResourceDictionary GetSkin(Assembly assembly, string themePath, SkinType skin)
        {
            try
            {
                Uri uri = new($"pack://application:,,,/{assembly.GetName().Name};component/{themePath}/Skin{skin}.xaml");
                return new ResourceDictionary
                {
                    Source = uri
                };
            }
            catch
            {
                return new ResourceDictionary
                {
                    Source = new Uri($"pack://application:,,,/{assembly.GetName().Name};component/{themePath}/Skin{SkinType.Default}.xaml")
                };
            }
        }

        /// <summary>
        /// 获取HandyControl皮肤
        /// </summary>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static ResourceDictionary GetSkin(SkinType skin) => new()
        {
            Source = new Uri($"pack://application:,,,/HandyControl;component/Themes/Skin{skin}.xaml")
        };

        /// <summary>
        /// 获取HandyControl主题
        /// </summary>
        /// <returns></returns>
        public static ResourceDictionary GetTheme() => new Lazy<ResourceDictionary>(() => new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
        }).Value;


        /// <summary>
        /// 获取指定Uri的资源字典
        /// </summary>
        /// <param name="uriString">Uri字符串</param>
        /// <returns></returns>
        public static ResourceDictionary GetTheme(string uriString) => new Lazy<ResourceDictionary>(() => new ResourceDictionary
        {
            Source = new Uri(uriString)
        }).Value;
    }
}

