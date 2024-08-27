using Hjmos.Common.Interfaces;
using Hjmos.Helper;
using Hjmos.Lcdp.Plugins.NccControls.Views.EventHandling;
using Hjmos.Ncc.Common.Services;
using Hjmos.TempEntity.Common.Interface;
using Hjmos.TempEntity.EmergencyResource;
using Hjmos.TempService.EmergencyResource;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels.EventHandling
{
    public class CommandGroupViewModel : BindableBase, IRegionAware
    {
        #region 变量，属性
        private DelegateCommand<object> _AddCommand;
        /// <summary>
        /// 添加命令
        /// </summary>
        public DelegateCommand<object> AddCommand =>
            _AddCommand ?? (_AddCommand = new DelegateCommand<object>(ExecuteAddCommand));

        private List<Organization> _LinePersons;
        /// <summary>
        /// 线路人员列表
        /// </summary>
        public List<Organization> LinePersons
        {
            get { return _LinePersons; }
            set { SetProperty(ref _LinePersons, value); }
        }

        private List<CommandGroupMember> _SelectedGroupLeader;
        /// <summary>
        /// 已选择的组长列表
        /// </summary>
        public List<CommandGroupMember> SelectedGroupLeader
        {
            get { return _SelectedGroupLeader; }
            set { SetProperty(ref _SelectedGroupLeader, value); }
        }

        private List<CommandGroupMember> _SelectedDeputyGroupLeader;
        /// <summary>
        /// 已选择的副组长列表
        /// </summary>
        public List<CommandGroupMember> SelectedDeputyGroupLeader
        {
            get { return _SelectedDeputyGroupLeader; }
            set { SetProperty(ref _SelectedDeputyGroupLeader, value); }
        }

        private List<CommandGroupMember> _SelectedGroupMembers;
        /// <summary>
        /// 已选择的组员列表
        /// </summary>
        public List<CommandGroupMember> SelectedGroupMembers
        {
            get { return _SelectedGroupMembers; }
            set { SetProperty(ref _SelectedGroupMembers, value); }
        }

        private string _GroupLeaderText;
        /// <summary>
        /// 组长显示文本
        /// </summary>
        public string GroupLeaderText
        {
            get { return _GroupLeaderText; }
            set { SetProperty(ref _GroupLeaderText, value); }
        }

        private string _DeputyGroupLeaderText;
        /// <summary>
        /// 副组长显示文本
        /// </summary>
        public string DeputyGroupLeaderText
        {
            get { return _DeputyGroupLeaderText; }
            set { SetProperty(ref _DeputyGroupLeaderText, value); }
        }

        private string _GroupMembersText;
        /// <summary>
        /// 成员显示文本
        /// </summary>
        public string GroupMembersText
        {
            get { return _GroupMembersText; }
            set { SetProperty(ref _GroupMembersText, value); }
        }

        /// <summary>
        /// 可选择的最多人数
        /// </summary>
        int _MostSelectedNum;
        /// <summary>
        /// 事件id
        /// </summary>
        long _EventId;
        /// <summary>
        /// 指挥小组id
        /// </summary>
        long _GroupId;
        /// <summary>
        /// 编辑前的组长列表
        /// </summary>
        List<CommandGroupMember> _OldGroupLeader;
        /// <summary>
        /// 编辑前的副组长列表
        /// </summary>
        List<CommandGroupMember> _OldDeputyGroupLeader;
        /// <summary>
        /// 编辑前的组员列表
        /// </summary>
        List<CommandGroupMember> _OldGroupMembers;
        #endregion

        async void ExecuteAddCommand(object parameter)
        {
            if (LinePersons.IsEmpty())
            {
                LinePersons = await EmergencyResourceService.GetOrganizationAndPerson(TempEntity.Enums.OrganizationType.Line);
            }

            if (!(parameter is int type))
                return;

            switch (type)
            {
                case 1:
                    _MostSelectedNum = 1;
                    break;
                case 2:
                    _MostSelectedNum = 5;
                    break;
                case 0:
                    _MostSelectedNum = 50;
                    break;
                default:
                    _MostSelectedNum = 50;
                    break;
            }

            var para = new DialogParameters
                        {
                            { "ItemsSource",LinePersons.ToList<IOrganization>()},
                            { "MostSelectedNum",_MostSelectedNum},
                            { "SelectedPersons",(type == 1 ? SelectedGroupLeader : type == 2 ? SelectedDeputyGroupLeader : SelectedGroupMembers)?.ToList<IContact>() }
                        };
            
            DialogWindowService.ShowDialog(nameof(AddPersonView), para, cb =>
            {
                if (cb.Result == ButtonResult.OK)
                {
                    if (cb.Parameters.TryGetValue("Persons", out List<IContact> persons))
                    {
                        SetCommandGroupText(persons, type);
                    }
                }
            });
        }

        /// <summary>
        /// 设置TextBox显示内容
        /// </summary>
        /// <param name="list">人员列表</param>
        void SetCommandGroupText(List<IContact> list, int type)
        {
            var names = list?.Select(x => x.Name);

            string personsText;
            if (!names.IsEmpty())
            {
                personsText = string.Join(",", names);
            }
            else
            {
                personsText = string.Empty;
            }

            switch (type)
            {
                case 1:
                    SelectedGroupLeader = new List<CommandGroupMember>();
                    list?.ForEach(x => SelectedGroupLeader.Add(new CommandGroupMember { Id = x.Id, Name = x.Name }));
                    GroupLeaderText = personsText;
                    break;
                case 2:
                    SelectedDeputyGroupLeader = new List<CommandGroupMember>();
                    list?.ForEach(x => SelectedDeputyGroupLeader.Add(new CommandGroupMember { Id = x.Id, Name = x.Name }));
                    DeputyGroupLeaderText = personsText;
                    break;
                case 0:
                    SelectedGroupMembers = new List<CommandGroupMember>();
                    list?.ForEach(x => SelectedGroupMembers.Add(new CommandGroupMember { Id = x.Id, Name = x.Name }));
                    GroupMembersText = personsText;
                    break;
                default:
                    break;
            }
        }

        private bool _IsCmbEditable;
        /// <summary>
        /// 控件是否可编辑
        /// </summary>
        public bool IsCmbEditable
        {
            get { return _IsCmbEditable; }
            set { SetProperty(ref _IsCmbEditable, value); }
        }

        private DelegateCommand _EditCommand;
        /// <summary>
        /// 编辑命令
        /// </summary>
        public DelegateCommand EditCommand =>
            _EditCommand ?? (_EditCommand = new DelegateCommand(ExecuteEditCommand));

        void ExecuteEditCommand()
        {
            _OldGroupLeader = SelectedGroupLeader.DeepCopyByJson();
            _OldDeputyGroupLeader = SelectedDeputyGroupLeader.DeepCopyByJson();
            _OldGroupMembers = SelectedGroupMembers.DeepCopyByJson();

            IsCmbEditable = true;
        }

        private DelegateCommand _SaveCommand;
        /// <summary>
        /// 保存命令
        /// </summary>
        public DelegateCommand SaveCommand =>
            _SaveCommand ?? (_SaveCommand = new DelegateCommand(ExecuteSaveCommand));

        void ExecuteSaveCommand()
        {
            UpdateCommandGroup();
        }

        private DelegateCommand _CancelCommand;
        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand =>
            _CancelCommand ?? (_CancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            SelectedGroupLeader = _OldGroupLeader.DeepCopyByJson();
            SelectedDeputyGroupLeader = _OldDeputyGroupLeader.DeepCopyByJson();
            SelectedGroupMembers = _OldGroupMembers.DeepCopyByJson();

            SetCommandGroupText(SelectedGroupLeader?.ToList<IContact>(), 1);
            SetCommandGroupText(SelectedDeputyGroupLeader?.ToList<IContact>(),2);
            SetCommandGroupText(SelectedGroupMembers?.ToList<IContact>(), 0);

            IsCmbEditable = false;
        }

        /// <summary>
        /// 更新指挥小组数据
        /// </summary>
        /// <param name="situation"></param>
        private async void UpdateCommandGroup()
        {
            var req = new CommandGroupMembersReq
            {
                Members = new List<CommandGroupMember>(),
                EventId = _EventId,
                GroupId = _GroupId
            };

            SelectedGroupLeader?.ForEach(x =>
            {
                req.Members.Add(new CommandGroupMember
                {
                    Id = x.Id,
                    Type = 1
                });
            });

            SelectedDeputyGroupLeader?.ForEach(x =>
            {
                req.Members.Add(new CommandGroupMember
                {
                    Id = x.Id,
                    Type = 2
                });
            });

            SelectedGroupMembers?.ForEach(x =>
            {
                req.Members.Add(new CommandGroupMember
                {
                    Id = x.Id,
                    Type = 0
                });
            });

            var result = await EmergencyResourceService.UpdEventCommandGroup(req);

            if (!result.Success)
            {
                DialogWindowService.ShowDialog(nameof(MessageBoxView), new DialogParameters { { "Content", "保存失败" } });
                return;
            }

            IsCmbEditable = false;
        }

        public async void OnViewActived(IRegionParameters regionParameters)
        {
            regionParameters.TryGetValue("eventId", out long eventId);
            regionParameters.TryGetValue("planId", out long planId);
            regionParameters.TryGetValue("eventType", out TempEntity.Enums.EventType eventType);

            _EventId = eventId;
            //查询指挥小组对应的groupId
            var groupList = await EmergencyResourceService.GetCommandGroupTypes(planId);
            var group = groupList.FirstOrDefault();

            if (group != null) 
            {
                _GroupId = group.Id;
                var members = await EmergencyResourceService.GetEventCommandGroup(eventId, group.Id);
                if (members.IsEmpty())
                    return;

                SelectedGroupLeader = members.Where(x => x.Type == 1)?.ToList();
                SelectedDeputyGroupLeader = members.Where(x => x.Type == 2)?.ToList();
                SelectedGroupMembers = members.Where(x => x.Type == 0)?.ToList();

                SetCommandGroupText(SelectedGroupLeader?.ToList<IContact>(), 1);
                SetCommandGroupText(SelectedDeputyGroupLeader?.ToList<IContact>(), 2);
                SetCommandGroupText(SelectedGroupMembers?.ToList<IContact>(), 0);
            }
        }

        public CommandGroupViewModel()
        { }
    }
}
