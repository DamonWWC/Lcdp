using CefSharp;
using CefSharp.Wpf;
using Hjmos.Lcdp.Extensions;
using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.ILoger;
using Hjmos.Lcdp.Loger;
using Hjmos.Lcdp.VisualEditor.Core;
using Hjmos.Lcdp.VisualEditor.Core.Dialogs;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Hjmos.Lcdp.VisualEditor.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Hjmos.Lcdp.VisualEditor
{
    public partial class App
    {
        private ILogHelper _logHelper;

        public App()
        {
            Startup += App_Startup;

            // 初始化日志配置
            new LogConfiguration().Init();

            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                AssemblyName assemblyName = new AssemblyName(a.Name);

                // 排除资源文件
                if (assemblyName.Name.EndsWith(".resources") && !assemblyName.CultureName.EndsWith("neutral")) return null;

                #region CefSarp配置

                // 尝试从x86或x64子目录加载缺少的程序集
                // 在使用AnyCPU运行时，CefSharp要求加载非托管依赖项
                if (assemblyName.Name.StartsWith("CefSharp"))
                {
                    string name = assemblyName.Name + ".dll";
                    string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", name);

                    return File.Exists(archSpecificPath) ? Assembly.LoadFile(archSpecificPath) : null;
                }

                #endregion

                // 加载插件
                if (StateManager.PluginList.Exists(x => x.FullName == a.Name))
                {
                    return StateManager.PluginList.First(x => x.FullName == a.Name);
                }

                return null;
            };

            AppDomain.CurrentDomain.TypeResolve += (s, a) =>
            {
                // 加载插件
                if (StateManager.PluginList.Exists(x => x.ExportedTypes.Any(t => t.FullName == a.Name)))
                {
                    return StateManager.PluginList.First(x => x.ExportedTypes.Any(t => t.FullName == a.Name));
                }

                return null;
            };

            // 初始化CefSharp
            InitializeCefSharp();
        }

        /// <summary>
        /// CefSharp初始化设置
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeCefSharp()
        {
            if (!Cef.IsInitialized)
            {
                CefSettings settings = new CefSettings
                {
                    //设置语言
                    Locale = "zh-CN"
                };

                //忽略https证书的问题
                settings.CefCommandLineArgs.Add("--ignore-urlfetcher-cert-requests", "1");
                settings.CefCommandLineArgs.Add("--ignore-certificate-errors", "1");

                //禁止启用同源策略安全限制，禁止后不会出现跨域问题
                settings.CefCommandLineArgs.Add("--disable-web-security", "1");

                ////取消CefSharp打印浏览器的输出日志
                //settings.LogSeverity = LogSeverity.Disable;

                //禁用gpu,防止浏览器闪烁
                settings.CefCommandLineArgs.Add("disable-gpu", "1");

                //去掉代理，增加加载网页速度
                settings.CefCommandLineArgs.Add("proxy-auto-detect", "0");
                settings.CefCommandLineArgs.Add("no-proxy-serve", "1");

                // 在运行时根据系统类型（32/64位），设置BrowserSubProcessPath
                settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", "CefSharp.BrowserSubprocess.exe");

                // 确保设置performDependencyCheck为false
                Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            }
        }

        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void InitializeShell(Window shell)
        {
            if (Container.Resolve<LoginView>().ShowDialog() == false)
            {
                Current?.Shutdown();
            }
            else
            {
                base.InitializeShell(shell);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 批量注入Service层服务
            InjectingServices(ConfigurationManager.AppSettings["ServiceAssembly"], containerRegistry);

            containerRegistry.Register<Dispatcher>(() => Current.Dispatcher);

            // 注册日志服务
            containerRegistry.RegisterSingleton<ILogHelper, LogHelper>();

            // 注册平台API
            containerRegistry.RegisterSingleton<IPageApi, PageApi>();
            containerRegistry.RegisterSingleton<IWidgetApi, WidgetApi>();

            // 全局状态
            containerRegistry.RegisterSingleton<StateManager>();

            // TODO：把Dialog也改成自动注入
            // 注册弹出窗父窗口
            containerRegistry.RegisterDialogWindow<DialogWindow>();

            // 保存用于注册插件中的类型 TODO: 考虑其他方式代替，这样会一直保持着containerRegistry
            RegisterTypesHelper.Instance.ContainerRegistry = containerRegistry;

            _logHelper = Container.Resolve<ILogHelper>();
        }


        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<MainModule.MainModule>();
            moduleCatalog.AddModule<EditorManagement.MainModule.MainModule>("ManagementMainModule");
            moduleCatalog.AddModule<EditorManagement.PageModule.PageModule>();
        }


        /// <summary>
        /// 从配置文件读取程序集，注入服务
        /// </summary>
        /// <param name="assemblyString">程序集名称</param>
        /// <param name="containerRegistry"></param>
        private void InjectingServices(string assemblyString, IContainerRegistry containerRegistry)
        {
            Assembly serviceAssembly = Assembly.Load(assemblyString);

            var registrations =
                from types in serviceAssembly.GetExportedTypes()
                from interfaces in types.GetInterfaces()
                select new { interfaces, types };

            registrations.ToList().ForEach(x => containerRegistry.Register(x.interfaces, x.types));
        }


        private void App_Startup(object sender, StartupEventArgs e)
        {
            // 异常处理
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // 程序启动参数保存到全局变量
            StateManager.CommandLineArgs = e.Args;

            #region JSON全局序列化设置
            JsonSerializerSettings setting = new JsonSerializerSettings
            {
                //日期类型默认格式化处理
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateFormatString = "yyyy-MM-dd HH:mm:ss",

                // 忽略Null值
                NullValueHandling = NullValueHandling.Ignore,
                // 忽略默认值
                DefaultValueHandling = DefaultValueHandling.Ignore,
                // 首字母小写
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                //// 如何对字符串进行转义(EscapeNonAscii：所有非 ASCII 和控制字符（例如换行符）都被转义)
                //StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };

            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() => setting);

            #endregion

            #region 加载插件程序集

            // 插件路径
            List<string> paths = new List<string>();

            // 插件目录配置
            paths.AddRange(ConfigurationManager.AppSettings["PluginDirectorys"].Split(new char[] { ';', ',' }));

            // 插件程序集配置
            paths.AddRange(ConfigurationManager.AppSettings["PluginAssemblys"].Split(new char[] { ';', ',' }));

            // 获取路径下所有程序集
            foreach (string path in paths)
            {
                // 获取目录下所有程序集
                if (Directory.Exists(path))
                {
                    // 插件目录完整路径
                    string fullPath = Path.GetFullPath(path);
                    // 目录下所有程序集
                    List<Assembly> assemblies = new DirectoryInfo(fullPath).GetAssemblys();
                    // 添加到程序上下文
                    StateManager.PluginList.AddRange(assemblies);
                }
                // 获取单个程序集
                else if (File.Exists(path))
                {
                    FileInfo fileInfo = new FileInfo(path);

                    // 后缀过滤
                    if (!new string[] { ".dll", ".exe" }.Contains(fileInfo.Extension)) return;

                    // 获取程序集
                    Assembly assembly = Assembly.LoadFrom(path);
                    if (assembly == null) continue;

                    // 保存到全局插件列表
                    StateManager.PluginList.Add(assembly);
                }
            }
            #endregion
        }

        /// <summary>
        /// 捕获Task异常，Task异常在GC回收后捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string msg = e.Exception.InnerExceptions[0].Message;
            _logHelper.Error(this, "捕获Task异常", e.Exception);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 捕获整个应用程序的异常，除了Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string msg = ((Exception)e.ExceptionObject).Message;
            _logHelper.Error(this, "捕获应用程序异常", (Exception)e.ExceptionObject);
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 捕获主线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string msg = e.Exception.Message;
            _logHelper.Error(this, "捕获主线程异常", e.Exception);
            MessageBox.Show(msg);
            e.Handled = true;
        }
    }
}