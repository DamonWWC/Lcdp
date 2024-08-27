using Hjmos.Lcdp.VisualEditor.Core.Attached;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.CS6.Helpers
{
    internal class SampleDataHelper
    {

        /// <summary>
        /// 使用Json文件初始化JsonAttached
        /// </summary>
        public static void FillDataFromJsonFile(DependencyObject d, string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string jsonFile = $"{assembly.GetName().Name}.{path}";
            Stream stream = assembly.GetManifestResourceStream(jsonFile);

            if (stream is null)
            {
                MessageBox.Show("Json文件不存在，请检查路径");
                return;
            }

            using StreamReader streamReader = new(stream, Encoding.UTF8);
            var s = streamReader.ReadToEnd();
            d.SetCurrentValue(JsonAttached.JsonProperty, s);
        }
    }
}
