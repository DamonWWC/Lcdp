using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class ModifyLibConfigDialogViewModel : BindableBase, IDialogAware
    {
        #region Property

        string _title = "配置组件";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }


        public ObservableCollection<string> Icons { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Category { get; set; } = new ObservableCollection<string>();

        #endregion

        public ModifyLibConfigDialogViewModel()
        {
            // 类别列表
            // TODO：改成从数据库获取
            Category.Add("NCC");
            Category.Add("Demo");
            Category.Add("Function");

            // 图标列表
            // TODO：改成从数据库获取
            List<string> icons = new List<string>() { "e632", "e664", "e6c6", "e661", "e61a", "e614", "e657" };
            icons.ForEach(x => Icons.Add(((char)int.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToString()));

        }

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _parameters = parameters;

            if (_parameters.ContainsKey("Name"))
            {
                Title = $"配置组件[{_parameters.GetValue<string>("Name")}]";
            }

        }
    }
}
