using Hjmos.Lcdp.VisualEditor.Controls.Services;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 设计器上下文
    /// </summary>
    public class MyDesignContext : DesignContext
    {

        internal readonly MyComponentService _componentService;

        /// <summary>
        /// 获取正在设计的根节点。
        /// </summary>
        public override DesignItem RootItem => _rootItem;

        private readonly MyDesignItem _rootItem;

        public override void Save(XmlWriter writer) => throw new System.NotImplementedException();


        /// <summary>
        /// 实例化MyDesignContext
        /// </summary>
        /// <param name="actions">自定义服务注册的委托</param>
        public MyDesignContext(MyLoadSettings loadSettings)
        {
            // 注册选中项改变服务
            this.Services.AddService(typeof(ISelectionService), new DefaultSelectionService());
            //this.Services.AddService(typeof(IComponentPropertyService), new ComponentPropertyService());
            // 注册选择和移动元素的默认工具。
            this.Services.AddService(typeof(IToolService), new DefaultToolService(this));
            //this.Services.AddService(typeof(UndoService), new UndoService());
            //this.Services.AddService(typeof(IErrorService), new DefaultErrorService(this));
            //this.Services.AddService(typeof(IOutlineNodeNameService), new OutlineNodeNameService());
            this.Services.AddService(typeof(ViewService), new DefaultViewService(this));
            this.Services.AddService(typeof(OptionService), new OptionService());

            _componentService = new MyComponentService(this);
            this.Services.AddService(typeof(IComponentService), _componentService);


            // 注册自定义服务
            loadSettings.CustomServiceRegisterFunctions.ForEach(action => action(this));

            // 从设计器程序集注册扩展
            // 注意：注册程序集扩展要放在其他服务注册之后，不然有些扩展会找不到服务
            foreach (Assembly designerAssembly in loadSettings.DesignerAssemblies)
                this.Services.ExtensionManager.RegisterAssembly(designerAssembly);

            // 定义根节点的内容
            //Grid rootElement = new() { Name = "rootElement", Background = Brushes.White, Width = 1024d, Height = 800d };
            //Canvas rootElement = new() { Name = "rootElement", Background = Brushes.White, Width = 1024d, Height = 800d };

            PageShell rootElement = new(_componentService) { Name = "pageShell", Background = Brushes.White, Width = 2560d, Height = 1440d };




            //rootElement.Children.Add(new Grid()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    VerticalAlignment = VerticalAlignment.Top,
            //    Margin = new Thickness(100, 100, 0, 0),
            //    Background = Brushes.LightBlue,
            //    Width = 300d,
            //    Height = 300d
            //});

            // 设置根节点
            _rootItem = _componentService.RegisterComponentRecursive(rootElement);

            if (_rootItem != null)
            {
                new RootItemBehavior().Intialize(this);
            }
        }
    }
}
