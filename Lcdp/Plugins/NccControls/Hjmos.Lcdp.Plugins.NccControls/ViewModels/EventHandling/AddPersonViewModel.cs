using Hjmos.Common;
using Hjmos.Common.Interfaces;
using Hjmos.Helper;
using Hjmos.Lcdp.Plugins.NccControls.Views.EventHandling;
using Hjmos.Ncc.Common.Services;
using Hjmos.TempEntity.Common.Interface;
using Hjmos.TempEntity.EmergencyResource;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels.EventHandling
{
    public class AddPersonViewModel : ViewModelBase
    {
        #region 变量，属性
        private int _MostSelectedNum = 50;
        /// <summary>
        /// 可选择的最多人数
        /// </summary>
        public int MostSelectedNum
        {
            get => _MostSelectedNum;
            set => SetProperty(ref _MostSelectedNum, value);
        }

        private List<TreeItemData> _TreeDataSource;
        /// <summary>
        /// 树控件数据源
        /// </summary>
        public List<TreeItemData> TreeDataSource
        {
            get { return _TreeDataSource; }
            set { SetProperty(ref _TreeDataSource, value); }
        }

        private ObservableCollection<TreeItemData> _SelectedPersons;
        /// <summary>
        /// 选择的节点数据
        /// </summary>
        public ObservableCollection<TreeItemData> SelectedPersons
        {
            get { return _SelectedPersons; }
            set { SetProperty(ref _SelectedPersons, value); }
        }

        private int _SelectedCount;
        /// <summary>
        /// 选择的人员数量
        /// </summary>
        public int SelectedCount
        {
            get { return _SelectedCount; }
            set { SetProperty(ref _SelectedCount, value); }
        }

        private string _SearchKeys;
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKeys
        {
            get { return _SearchKeys; }
            set
            {
                SetProperty(ref _SearchKeys, value);
                SearchNames(TreeDataSource, _SearchKeys);
            }
        }

        /// <summary>
        /// 点击CheckBox时累计已选择人数
        /// </summary>
        int _TotalAddPersonCount = 0;

        /// <summary>
        /// 叶子节点列表
        /// </summary>
        List<TreeItemData> _LeafTreeItems;
        #endregion

        public AddPersonViewModel() { }

        #region 方法
        /// <summary>
        /// 接收参数
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            //树数据源
            if (!parameters.TryGetValue("ItemsSource", out List<IOrganization> linePersonlist))
                return;

            //最多可选择人数
            if (parameters.TryGetValue("MostSelectedNum", out int num))
            {
                MostSelectedNum = num;
            }

            //已选择的人员
            parameters.TryGetValue("SelectedPersons", out List<IContact> hasSelectedPersons);

            InitData(linePersonlist, hasSelectedPersons);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="list"></param>
        private void InitData(List<IOrganization> list, List<IContact> hasSelectedPersons)
        {
            if (list.IsEmpty())
                return;

            //创建树
            TreeDataSource = new List<TreeItemData>();
            foreach (var item in list)
            {
                TreeDataSource.Add(GetTreeItem(item, new Thickness(0, 0, 0, 0), new Thickness(0, 0, 0, 0)));
            }

            SelectedPersons = new ObservableCollection<TreeItemData>();

            //在树中勾选已选择的人员
            if (!hasSelectedPersons.IsEmpty())
            {
                _LeafTreeItems = new List<TreeItemData>();
                TreeDataSource.ForEach(t => GetLeafTreeItems(t));

                if (_LeafTreeItems.IsEmpty())
                    return;

                hasSelectedPersons.ForEach(x =>
                {
                    var treeItem = _LeafTreeItems.FirstOrDefault(t => t.AddressBook.Id == x.Id);

                    if (treeItem != null)
                    {
                        treeItem.IsChecked = true;
                        AddSelectedPerson(treeItem);
                        SetParentChecked(treeItem);
                    }
                });
            }

            SelectedCount = SelectedPersons.Count();
        }

        /// <summary>
        /// 获取叶子节点
        /// </summary>
        /// <param name="data"></param>
        private void GetLeafTreeItems(TreeItemData data)
        {
            if (data.IsLeafNode)
            {
                _LeafTreeItems.Add(data);
            }
            else if (!data.Children.IsEmpty())
            {
                data.Children.ForEach(x => GetLeafTreeItems(x));
            }
        }

        /// <summary>
        /// 根据姓名搜索
        /// </summary>
        /// <param name="list"></param>
        /// <param name="keyword"></param>
        private void SearchNames(List<TreeItemData> list, string keyword)
        {
            if (list.IsEmpty())
                return;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                list.ForEach(x =>
                {
                    x.Visibility = Visibility.Visible;

                    if (x.Children != null)
                        SearchNames(x.Children, keyword);
                });
                return;
            }

            list.ForEach(x =>
            {
                if (x.IsLeafNode && x.Name.Contains(keyword))
                {
                    x.Visibility = Visibility.Visible;
                    x.IsExpanded = true;
                    SetParentNodeVisible(x.Parent);
                }
                else
                {
                    x.Visibility = Visibility.Collapsed;
                }

                if (!x.Children.IsEmpty())
                    SearchNames(x.Children, keyword);
            });
        }

        /// <summary>
        /// 设置父节点可见
        /// </summary>
        /// <param name="parent">父节点</param>
        private void SetParentNodeVisible(TreeItemData parent)
        {
            parent.Visibility = Visibility.Visible;
            parent.IsExpanded = true;

            if (parent.Parent != null)
                SetParentNodeVisible(parent.Parent);
        }

        /// <summary>
        /// 创建树节点
        /// </summary>
        /// <param name="organization">组织实例</param>
        /// <param name="gridMargin">节点外边距</param>
        /// <param name="toggleButtonMargin">展开按钮外边距</param>
        /// <returns></returns>
        private TreeItemData GetTreeItem(IOrganization organization, Thickness gridMargin, Thickness toggleButtonMargin)
        {
            var treeViewSource = new TreeItemData(organization.Name);
            //线路组织
            if (organization is Organization LineOrganization)
            {
                if (LineOrganization.Persons != null)
                {
                    foreach (var personItem in LineOrganization.Persons)
                    {
                        var treeViewSource1 = new TreeItemData(personItem.Name)
                        {
                            AddressBook = personItem,
                            IsLeafNode = true,
                            Children = null
                        };

                        treeViewSource1.GridMargin = new Thickness(gridMargin.Left - 12, 0, gridMargin.Right - 12, 0);
                        treeViewSource1.ToggleButtonMargin = new Thickness(toggleButtonMargin.Left + 12, 0, toggleButtonMargin.Right + 12, 0);
                        treeViewSource.CreateTreeWithChildren(treeViewSource1);
                    }
                }
                if (LineOrganization.Children.IsEmpty())
                {
                    treeViewSource.GridMargin = gridMargin;
                    treeViewSource.ToggleButtonMargin = toggleButtonMargin;
                    return treeViewSource;
                }
                else
                {
                    foreach (var childrenItem in LineOrganization.Children)
                    {
                        treeViewSource.CreateTreeWithChildren(GetTreeItem(childrenItem, new Thickness(gridMargin.Left - 12, 0, gridMargin.Right - 12, 0), new Thickness(toggleButtonMargin.Left + 12, 0, toggleButtonMargin.Right + 12, 0)));
                    }
                }
            }
            //专家组织
            if (organization is ExpertOrganization expertOrganization)
            {
                if (expertOrganization.Persons != null)
                {
                    foreach (var personItem in expertOrganization.Persons)
                    {
                        var treeViewSource1 = new TreeItemData(personItem.Name)
                        {
                            AddressBook = personItem,
                            IsLeafNode = true,
                            Children = null
                        };
                        treeViewSource1.GridMargin = new Thickness(gridMargin.Left - 12, 0, gridMargin.Right - 12, 0);
                        treeViewSource1.ToggleButtonMargin = new Thickness(toggleButtonMargin.Left + 12, 0, toggleButtonMargin.Right + 12, 0);
                        treeViewSource.CreateTreeWithChildren(treeViewSource1);
                    }
                }
                if (expertOrganization.Children.IsEmpty())
                {
                    treeViewSource.GridMargin = gridMargin;
                    treeViewSource.ToggleButtonMargin = toggleButtonMargin;
                    return treeViewSource;
                }
                else
                {
                    foreach (var childrenItem in expertOrganization.Children)
                    {
                        treeViewSource.CreateTreeWithChildren(GetTreeItem(childrenItem, new Thickness(gridMargin.Left - 12, 0, gridMargin.Right - 12, 0), new Thickness(toggleButtonMargin.Left + 12, 0, toggleButtonMargin.Right + 12, 0)));
                    }
                }
            }
            return treeViewSource;
        }

        private DelegateCommand<TreeItemData> _DelCommand;
        /// <summary>
        /// 删除命令
        /// </summary>
        public DelegateCommand<TreeItemData> DelCommand =>
            _DelCommand ?? (_DelCommand = new DelegateCommand<TreeItemData>(ExecuteDelCommand));

        void ExecuteDelCommand(TreeItemData data)
        {
            if (data != null)
            {
                data.IsChecked = false;
                SetParentChecked(data);
                SelectedPersons.Remove(data);
                SelectedCount = SelectedPersons.Count();
            }
        }

        private DelegateCommand _ConfirmCommand;
        /// <summary>
        /// 确定命令
        /// </summary>
        public DelegateCommand ConfirmCommand =>
            _ConfirmCommand ?? (_ConfirmCommand = new DelegateCommand(ExecuteConfirmCommand));

        void ExecuteConfirmCommand()
        {
            var persons = SelectedPersons?.Select(x => x.AddressBook).ToList();

            CloseWindow(new DialogResult(ButtonResult.OK, new DialogParameters
            {
               { "Persons", persons}
            }));
        }

        private DelegateCommand _CancelCommand;
        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand =>
            _CancelCommand ?? (_CancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            CloseWindow(new DialogResult(ButtonResult.Cancel));
        }

        private DelegateCommand<object> _CheckCommand;
        /// <summary>
        /// CheckBox点击命令
        /// </summary>
        public DelegateCommand<object> CheckCommand =>
            _CheckCommand ?? (_CheckCommand = new DelegateCommand<object>(ExecuteCheckCommand, CanExecuteCheckCommand));

        private bool CanExecuteCheckCommand(object obj)
        {
            if (obj is TreeItemData data)
            {
                if (data.IsChecked == true)
                {
                    _TotalAddPersonCount = 0;
                    AddTotalCount(data, 0);

                    //判断已选择人数+要选择的人数是否大于最大可选择人数，大于则弹出提示，使选择无效
                    if (SelectedPersons.Count + _TotalAddPersonCount > MostSelectedNum)
                    {
                        var para = new DialogParameters
                        {
                            { "Content",$"最多可选择{MostSelectedNum}名人员"},
                        };

                        DialogWindowService.ShowDialog(nameof(MessageBoxView), para);

                        data.IsChecked = !data.IsChecked;
                        return false;
                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        void ExecuteCheckCommand(object obj)
        {
            if (obj is TreeItemData data)
            {
                if (data.IsChecked == true)
                {
                    AddSelectedPerson(data);
                }
                else if (data.IsChecked == false)
                {
                    RemoveSelectedPerson(data);
                }
                SelectedCount = SelectedPersons.Count();

                SetChildChecked(data);
                SetParentChecked(data);
            }
        }

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="data"></param>
        private void AddSelectedPerson(TreeItemData data)
        {
            if (data.IsLeafNode)
            {
                SelectedPersons.Add(data);
            }
            else if (!data.Children.IsEmpty())
            {
                data.Children.ForEach(x => AddSelectedPerson(x));
            }
        }

        /// <summary>
        /// 点击CheckBox移除人员
        /// </summary>
        /// <param name="data"></param>
        private void RemoveSelectedPerson(TreeItemData data)
        {
            if (data.IsLeafNode)
            {
                SelectedPersons.Remove(data);
            }
            else if (!data.Children.IsEmpty())
            {
                data.Children.ForEach(x => RemoveSelectedPerson(x));
            }
        }

        /// <summary>
        /// 当前节点及其子节点累计选择人数
        /// </summary>
        /// <param name="data">当前节点数据</param>
        /// <param name="hasCount">当前累计已选择人数</param>
        private void AddTotalCount(TreeItemData data, int hasCount)
        {
            if (data.IsLeafNode)
            {
                _TotalAddPersonCount = hasCount + 1;
            }
            else if (!data.Children.IsEmpty())
            {
                _TotalAddPersonCount += data.Children.Where(x => x.IsLeafNode).Count();

                data.Children.Where(c => !c.Children.IsEmpty()).ToList().ForEach(x =>
                  {
                      AddTotalCount(x, _TotalAddPersonCount);
                  });
            }
        }

        /// <summary>
        /// 设置父节点CheckBox状态
        /// </summary>
        /// <param name="data"></param>
        private void SetParentChecked(TreeItemData data)
        {
            if (data.Parent == null)
                return;

            if (data.Parent.Children.Exists(x => x.IsChecked == null))
            {
                data.Parent.IsChecked = null;
            }
            else if (data.Parent.Children.Where(x => x.IsChecked == true).Count() == 0)
            {
                data.Parent.IsChecked = false;
            }
            else if (data.Parent.Children.Where(x => x.IsChecked == true).Count() == data.Parent.Children.Count())
            {
                data.Parent.IsChecked = true;
            }
            else if (data.Parent.Children.Where(x => x.IsChecked == true).Count() < data.Parent.Children.Count())
            {
                data.Parent.IsChecked = null;
            }

            if (data.Parent.Parent != null)
            {
                SetParentChecked(data.Parent);
            }
        }

        /// <summary>
        /// 设置子节点CheckBox状态
        /// </summary>
        /// <param name="data"></param>
        private void SetChildChecked(TreeItemData data)
        {
            if (data == null || data.Children.IsEmpty())
                return;

            data.Children.ForEach(x =>
            {
                x.IsChecked = data.IsChecked;

                if (!x.Children.IsEmpty())
                {
                    SetChildChecked(x);
                }
            });
        }
        #endregion
    }

    /// <summary>
    /// 节点数据类
    /// </summary>
    public class TreeItemData : BindableBase
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public TreeItemData Parent { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeItemData> Children { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 通讯录
        /// </summary>
        public IContact AddressBook { get; set; }

        /// <summary>
        /// 是否叶子节点（Person为叶子节点）
        /// </summary>
        public bool IsLeafNode { get; set; }

        /// <summary>
        /// 节点的Grid外边距
        /// </summary>
        public Thickness GridMargin { get; set; }

        /// <summary>
        /// 展开按钮外边距
        /// </summary>
        public Thickness ToggleButtonMargin { get; set; }

        public TreeItemData(string name)
        {
            Name = name;
            Children = new List<TreeItemData>();
        }
        public TreeItemData()
        { }

        private bool _IsSelected;
        /// <summary>
        /// 树节点是否被选中
        /// </summary>
        public bool IsSelected
        {
            get => _IsSelected;
            set => SetProperty(ref _IsSelected, value);
        }

        private bool? _IsChecked = false;
        /// <summary>
        /// CheckBox是否被选中
        /// </summary>
        public bool? IsChecked
        {
            get => _IsChecked;
            set => SetProperty(ref _IsChecked, value);
        }

        private bool _IsExpanded;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get => _IsExpanded;
            set => SetProperty(ref _IsExpanded, value);
        }

        private Visibility _Visibility;
        /// <summary>
        /// 是否可见
        /// </summary>
        public Visibility Visibility
        {
            get => _Visibility;
            set => SetProperty(ref _Visibility, value);
        }

        /// <summary>
        /// 创建带子节点的节点数据
        /// </summary>
        /// <param name="children"></param>
        public void CreateTreeWithChildren(TreeItemData children)
        {
            Children.Add(children);
            children.Parent = this;
        }
    }
}
