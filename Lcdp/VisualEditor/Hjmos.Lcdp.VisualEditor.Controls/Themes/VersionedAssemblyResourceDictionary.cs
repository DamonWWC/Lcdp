using System;
using System.ComponentModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Themes
{
    internal class VersionedAssemblyResourceDictionary : ResourceDictionary, ISupportInitialize
    {
        private static readonly string _uriStart;

        private static readonly int _subLength;

        static VersionedAssemblyResourceDictionary()
        {
            var assemblyName = typeof(VersionedAssemblyResourceDictionary).Assembly.GetName();
            _uriStart = string.Format(@"/{0};v{1};component/", assemblyName.Name, assemblyName.Version);
            _subLength = assemblyName.Name.Length + 1;
        }

        public string RelativePath { get; set; }

        void ISupportInitialize.EndInit()
        {
            this.Source = new Uri(_uriStart + this.RelativePath, UriKind.Relative);
            base.EndInit();
        }

        public static string GetXamlNameForType(Type t)
        {
            return _uriStart + t.FullName.Substring(_subLength).Replace(".", "/").ToLower() + ".xaml";
        }
    }
}
