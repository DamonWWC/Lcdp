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
    public class RecommendExpertsViewModel : BindableBase, IRegionAware
    {
        private List<ExpertOrganization> _ExpertPersons;
        /// <summary>
        /// 专家列表
        /// </summary>
        public List<ExpertOrganization> ExpertPersons
        {
            get { return _ExpertPersons; }
            set { SetProperty(ref _ExpertPersons, value); }
        }

        /// <summary>
        /// 编辑前已选择的人员
        /// </summary>
        List<IContact> _OldSelectedPersons;

        private List<IContact> _SelectedPersons;
        /// <summary>
        /// 已选择人员列表
        /// </summary>
        public List<IContact> SelectedPersons
        {
            get { return _SelectedPersons; }
            set { SetProperty(ref _SelectedPersons, value); }
        }

        private string _ExpertsText;
        /// <summary>
        /// TextBox显示内容
        /// </summary>
        public string ExpertsText
        {
            get { return _ExpertsText; }
            set { SetProperty(ref _ExpertsText, value); }
        }

        /// <summary>
        /// 最多可选择10个专家
        /// </summary>
        static readonly int MOSTSELECTEDNUM = 10;

        long _EventId;
        long _PlanId;

        public RecommendExpertsViewModel()
        {
            GetAllExperts();
        }

        private DelegateCommand<string> _AddCommand;
        /// <summary>
        /// 添加命令
        /// </summary>
        public DelegateCommand<string> AddCommand =>
            _AddCommand ?? (_AddCommand = new DelegateCommand<string>(ExecuteAddCommand));

        async void ExecuteAddCommand(string type)
        {
            if (ExpertPersons.IsEmpty())
            {
                ExpertPersons = await EmergencyResourceService.GetExpertOrganizationAndPerson();
            }

            var para = new DialogParameters
                        {
                            { "ItemsSource", ExpertPersons?.ToList<IOrganization>()},
                            { "MostSelectedNum", MOSTSELECTEDNUM},
                            { "SelectedPersons", SelectedPersons}
                        };

            DialogWindowService.ShowDialog(nameof(AddPersonView), para, cb =>
            {
                if (cb.Result == ButtonResult.OK)
                {
                    if (cb.Parameters.TryGetValue("Persons", out List<IContact> persons))
                    {
                        SelectedPersons = persons;
                        SetExpertText(SelectedPersons);
                    }
                }
            });
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
            _OldSelectedPersons = new List<IContact>();
            SelectedPersons?.ForEach(x => _OldSelectedPersons.Add(x));

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
            UpdateEventExperts();
        }

        private DelegateCommand _CancelCommand;
        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand =>
            _CancelCommand ?? (_CancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            SelectedPersons = new List<IContact>();
            _OldSelectedPersons?.ForEach(x => SelectedPersons.Add(x));
            SetExpertText(SelectedPersons);

            IsCmbEditable = false;
        }

        /// <summary>
        /// 更新推荐专家数据
        /// </summary>
        /// <param name="situation"></param>
        private async void UpdateEventExperts()
        {
            var req = new EventPlanExpertForReq
            {
                ExpertList = new List<EventPlanExpert>(),
                EventId = _EventId
            };

            SelectedPersons?.ForEach(x =>
            {
                req.ExpertList.Add(new EventPlanExpert
                {
                    PlanId = _PlanId,
                    srcId = x.Id,
                    SrcCode = "YJ001"
                });
            });

            var result = await EmergencyResourceService.UpdEventExperts(req);

            if (!result.Success)
            {
                DialogWindowService.ShowDialog(nameof(MessageBoxView), new DialogParameters { { "Content", "保存失败" } });
                return;
            }

            IsCmbEditable = false;
        }

        public void OnViewActived(IRegionParameters regionParameters)
        {
            regionParameters.TryGetValue("eventId", out long eventId);
            regionParameters.TryGetValue("planId", out long planId);

            _EventId = eventId;
            _PlanId = planId;

            GetEventExperts(eventId);
        }

        /// <summary>
        /// 查询专家
        /// </summary>
        /// <param name="eventId"></param>
        async void GetEventExperts(long eventId)
        {
            var result = await EmergencyResourceService.GetEventExperts(eventId);
            if (!result.IsEmpty())
            {
                SelectedPersons = result.ToList<IContact>();
                SetExpertText(SelectedPersons);
            }
        }

        /// <summary>
        /// 加载专家数据
        /// </summary>
        async void GetAllExperts()
        {
            ExpertPersons = await EmergencyResourceService.GetExpertOrganizationAndPerson();
        }

        /// <summary>
        /// 设置TextBox显示内容
        /// </summary>
        /// <param name="list">专家列表</param>
        void SetExpertText(List<IContact> list)
        {
            var names = list?.Select(x => x.Name);

            if (!names.IsEmpty())
            {
                ExpertsText = string.Join(",", names);
            }
            else
            {
                ExpertsText = string.Empty;
            }
        }
    }
}
