﻿using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Browser
{
    /// <summary>
    /// HJBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class HJBrowser : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public HJBrowser()
        {
            InitializeComponent();

            InitCefSharp();
            Loaded += CustomCefSharp_Loaded;
        }


        static HJBrowser()
        {
            //// 当程序集的解析失败时, 从x86或x64子目录加载缺少的程序集
            //  AppDomain.CurrentDomain.AssemblyResolve += Resolver;

            InitializeCefSharp();

        }

        /// <summary>
        /// CefSharp初始化设置
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
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

        /// <summary>
        /// 用于关闭窗体，注销当前CefSharp的实例
        /// </summary>
        private bool isResigeterClosing = false;
        private void CustomCefSharp_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isResigeterClosing && IsVisible)
            {
                Window.GetWindow(this).Closing -= CefSharpClosing;
                Window.GetWindow(this).Closing += CefSharpClosing;
                isResigeterClosing = true;
            }
        }

        /// <summary>
        /// 释放CEFsharp的资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CefSharpClosing(object sender, CancelEventArgs e)
        {
            this.CefSharp?.Dispose();
        }

        private ChromiumWebBrowser _CefSharp;

        public ChromiumWebBrowser CefSharp
        {
            get { return _CefSharp; }
            set { _CefSharp = value; UIPropertyChanged(nameof(CefSharp)); }
        }


        /// <summary>
        /// 初始化CefSharp
        /// </summary>
        private void InitCefSharp()
        {
            CefSharp = new ChromiumWebBrowser();
            CefSharp.KeyboardHandler = new KeyBoardHander();
            CefSharp.MenuHandler = new MenuHandler();
            MainGrid.Children.Add(CefSharp);
        }

        //设置地址的连接，支持绑定
        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.RegisterAttached("Address", typeof(string), typeof(HJBrowser), new PropertyMetadata(null, new PropertyChangedCallback(H5AddressChanged)));


        /// <summary>
        /// H5地址值改变,设置当前cef的H5地址
        /// </summary>
        /// <param name="d">当前控件</param>
        /// <param name="e">属性参数</param>
        private static void H5AddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newAddr = e.NewValue as string;

            if (d is HJBrowser customCefSharp && customCefSharp != null && customCefSharp.CefSharp != null && !customCefSharp.CefSharp.IsDisposed)
            {
                if (!string.IsNullOrWhiteSpace(newAddr))
                {
                    customCefSharp.CefSharp.Address = newAddr;
                }
                else
                {
                    customCefSharp.CefSharp.Address = null;
                }
            }

            //if (!string.IsNullOrWhiteSpace(newAddr) && d is HJBrowser customCefSharp)
            //{
            //    if (customCefSharp.CefSharp != null && !customCefSharp.CefSharp.IsDisposed)
            //    {
            //        customCefSharp.CefSharp.Address = newAddr;
            //    }
            //}
        }


        /// <summary>
        /// 前台传递参数需要通过浏览器发送消息给H5（Js的方式，Json字符串）
        /// </summary>
        public H5SendMsgModel SendH5Msg
        {
            get { return (H5SendMsgModel)GetValue(SendH5MsgProperty); }
            set { SetValue(SendH5MsgProperty, value); }
        }

        public static readonly DependencyProperty SendH5MsgProperty =
        DependencyProperty.RegisterAttached("SendH5Msg", typeof(H5SendMsgModel), typeof(HJBrowser), new PropertyMetadata(null, new PropertyChangedCallback(SendH5MsgChanged)));

        /// <summary>
        /// 通过值改变的方式发送给H5数据
        /// </summary>
        /// <param name="d">当前控件的实例</param>
        /// <param name="e">属性参数</param>
        private static void SendH5MsgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newMsgModel = e.NewValue as H5SendMsgModel;
            if (newMsgModel != null && d is HJBrowser customCefSharp)
            {
                var json = $"{newMsgModel.JavaScripMethodName}('{newMsgModel.SendJsonMsg}')";
                customCefSharp.SendMsgToH5(json);
            }
        }


        /// <summary>
        /// 要注册到浏览器上的Javascript对象的集合
        /// </summary>
        public IDictionary<string, object> JavascriptObjects
        {
            get { return (IDictionary<string, object>)GetValue(JavascriptObjectsProperty); }
            set { SetValue(JavascriptObjectsProperty, value); }
        }

        /// <summary>
        /// 要注册到浏览器上的Javascript对象的集合依赖属性
        /// </summary>
        public static readonly DependencyProperty JavascriptObjectsProperty =
          DependencyProperty.Register(nameof(JavascriptObjects), typeof(IDictionary<string, object>), typeof(HJBrowser), new PropertyMetadata(null, OnJavascriptObjectsPropertyChanged));

        /// <summary>
        /// Javascript对象的集合变化
        /// </summary>
        /// <param name="d">当前控件的实例</param>
        /// <param name="e">回调的属性参数</param>
        private static void OnJavascriptObjectsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HJBrowser customCefSharp)
            {
                IDictionary<string, object> oldJavascriptObjects = e.OldValue as IDictionary<string, object>;
                IDictionary<string, object> newJavascriptObjects = e.NewValue as IDictionary<string, object>;
                customCefSharp.InitJavascriptObjects(oldJavascriptObjects, newJavascriptObjects);
            }
        }

        /// <summary>
        /// 注册Javascript对象集合
        /// </summary>
        /// <param name="oldJavascriptObjects"></param>
        /// <param name="newJavascriptObjects"></param>
        private void InitJavascriptObjects(IDictionary<string, object> oldJavascriptObjects, IDictionary<string, object> newJavascriptObjects)
        {
            if (CefSharp == null || CefSharp.IsDisposed)
            {
                return;
            }

            if (oldJavascriptObjects != null && oldJavascriptObjects.Count > 0)
            {
                foreach (var oldJavascriptObject in oldJavascriptObjects)
                {
                    CefSharp.JavascriptObjectRepository.UnRegister(oldJavascriptObject.Key);
                }
            }

            if (newJavascriptObjects != null && newJavascriptObjects.Count > 0)
            {
                foreach (var newJavascriptObject in newJavascriptObjects)
                {
                    CefSharp.JavascriptObjectRepository.Register(newJavascriptObject.Key, newJavascriptObject.Value, true, BindingOptions.DefaultBinder);
                }
            }
        }

        /// <summary>
        /// wpf给H5发送js字符串通信
        /// </summary>
        /// <param name="json">发送到H5的json字符串</param>
        private void SendMsgToH5(string json)
        {
            if (CefSharp != null && !CefSharp.IsDisposed && CefSharp.CanExecuteJavascriptInMainFrame)
            {
                CefSharp.ExecuteScriptAsync(json);
            }
        }

        /// <summary>
        /// 通知界面值改变回调
        /// </summary>
        /// <param name="propertiName">属性名</param>
        private void UIPropertyChanged(string propertiName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertiName));
        }

    }

    /// <summary>
    /// 设置相应键盘事件
    /// </summary>
    public class KeyBoardHander : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            //按键事件会触发2次KeyType类型：RawKeyDown、KeyUp，为避免触发两次，需进行判断
            if (type != KeyType.KeyUp)
            {
                return false;
            }

            const int VK_F5 = 0x74;
            const int VK_F9 = 0x78;
            const int VK_F12 = 0x7B;

            if (windowsKeyCode == VK_F5)
            {
                browser.Reload();
            }
            else if (windowsKeyCode == VK_F12)
            {
                browser.ShowDevTools();
            }
            else if (windowsKeyCode == VK_F9)
            {
            }
            return false;
        }
    }


    /// <summary>
    /// 屏蔽浏览器的默认属性（点击右键显示属性菜单）
    /// </summary>
    public class MenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }
        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return false;
        }
        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }
        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }

    /// <summary>
    /// WPF发送H5的请求类
    /// </summary>
    public class H5SendMsgModel
    {
        /// <summary>
        /// 用于请求Js的方法名（例如wpfToH5）
        /// </summary>
        public string JavaScripMethodName
        {
            get; set;
        }

        /// <summary>
        /// 请求发送的字符串
        /// </summary>
        public string SendJsonMsg
        {
            get; set;
        }
    }

}
