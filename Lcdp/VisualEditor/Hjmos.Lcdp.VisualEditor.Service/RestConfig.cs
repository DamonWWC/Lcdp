using System.Configuration;

namespace Hjmos.Lcdp.VisualEditor.Service
{
    public class RestConfig
    {
        public static string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
    }
}
