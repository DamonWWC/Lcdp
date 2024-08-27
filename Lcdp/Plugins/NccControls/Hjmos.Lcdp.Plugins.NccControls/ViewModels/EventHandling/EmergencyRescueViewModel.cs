using Hjmos.Helper;
using Hjmos.Lcdp.Plugins.NccControls.Enums;
using Hjmos.Lcdp.Plugins.NccControls.Views.EventHandling;
using Hjmos.Log;
using Hjmos.Ncc.Common.Services;
using Hjmos.TempEntity.Common;
using Hjmos.TempEntity.EmergencyResource;
using Hjmos.TempEntity.Enums;
using Hjmos.TempService.EmergencyResource;
using Hjmos.TempService.WebSocket;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels.EventHandling
{
    public class EmergencyRescueViewModel : BindableBase, IRegionAware
    {
        #region 属性，变量

        /// <summary>
        /// 事件id
        /// </summary>
        long _EventId;
        /// <summary>
        /// 查询返回的应急救援数据
        /// </summary>
        List<RescueDisplay> _RescueDisplayList;

        /// <summary>
        /// 110救援状态数据id
        /// </summary>
        long _PoliceRescueId;
        /// <summary>
        /// 119救援状态数据id
        /// </summary>
        long _FirePoliceRescueId;
        /// <summary>
        /// 120救援状态数据id
        /// </summary>
        long _FirstAidesuceId;
        /// <summary>
        /// 专业救援状态数据id
        /// </summary>
        long _ProfessionalRescueId;

        /// <summary>
        /// 订阅WebSocket的Key
        /// </summary>
        string[] _SubKey;

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
            //保存数据
            UpdateEmergencyRescueStatus();
        }

        private DelegateCommand _CancelCommand;
        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand =>
            _CancelCommand ?? (_CancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            SetData(_RescueDisplayList);//还原为修改前的数据

            IsCmbEditable = false;
        }

        private EnumTypeClass _SelectedPoliceContactState = new EnumTypeClass();
        /// <summary>
        /// 选择的110联络状态
        /// </summary>
        public EnumTypeClass SelectedPoliceContactState
        {
            get { return _SelectedPoliceContactState; }
            set { SetProperty(ref _SelectedPoliceContactState, value); }
        }

        private List<EnumTypeClass> _PoliceContactStateList = new List<EnumTypeClass>();
        /// <summary>
        /// 110联络状态列表
        /// </summary>
        public List<EnumTypeClass> PoliceContactStateList
        {
            get { return _PoliceContactStateList; }
            set { SetProperty(ref _PoliceContactStateList, value); }
        }

        private EnumTypeClass _SelectedFirstAidContactState = new EnumTypeClass();
        /// <summary>
        /// 选择的120联络状态
        /// </summary>
        public EnumTypeClass SelectedFirstAidContactState
        {
            get { return _SelectedFirstAidContactState; }
            set { SetProperty(ref _SelectedFirstAidContactState, value); }
        }

        private List<EnumTypeClass> _FirstAidContactStateList = new List<EnumTypeClass>();
        /// <summary>
        /// 120联络状态列表
        /// </summary>
        public List<EnumTypeClass> FirstAidContactStateList
        {
            get { return _FirstAidContactStateList; }
            set { SetProperty(ref _FirstAidContactStateList, value); }
        }

        private EnumTypeClass _SelectedFirePoliceContactState = new EnumTypeClass();
        /// <summary>
        /// 选择的119联络状态
        /// </summary>
        public EnumTypeClass SelectedFirePoliceContactState
        {
            get { return _SelectedFirePoliceContactState; }
            set { SetProperty(ref _SelectedFirePoliceContactState, value); }
        }

        private List<EnumTypeClass> _FirePoliceContactStateList = new List<EnumTypeClass>();
        /// <summary>
        /// 119联络状态列表
        /// </summary>
        public List<EnumTypeClass> FirePoliceContactStateList
        {
            get { return _FirePoliceContactStateList; }
            set { SetProperty(ref _FirePoliceContactStateList, value); }
        }

        private EnumTypeClass _SelectedProfessionalRescueContactState = new EnumTypeClass();
        /// <summary>
        /// 选择的专业救援联络状态
        /// </summary>
        public EnumTypeClass SelectedProfessionalRescueContactState
        {
            get { return _SelectedProfessionalRescueContactState; }
            set { SetProperty(ref _SelectedProfessionalRescueContactState, value); }
        }

        private List<EnumTypeClass> _ProfessionalRescueContactStateList = new List<EnumTypeClass>();
        /// <summary>
        /// 专业联络状态列表
        /// </summary>
        public List<EnumTypeClass> ProfessionalRescueContactStateList
        {
            get { return _ProfessionalRescueContactStateList; }
            set { SetProperty(ref _ProfessionalRescueContactStateList, value); }
        }
        #endregion

        public EmergencyRescueViewModel()
        {
            InitData();
        }

        #region 方法
        /// <summary>
        /// 初始化数据
        /// </summary>
        void InitData()
        {
            EnumHelper.ToList<PoliceContactState>().ForEach(x => PoliceContactStateList.Add(new EnumTypeClass
            {
                OriginalName = x.ToString(),
                DisplayName = x.GetDescription(),
                Type = (int)x
            }));

            EnumHelper.ToList<FirstAidContactState>().ForEach(x => FirstAidContactStateList.Add(new EnumTypeClass
            {
                OriginalName = x.ToString(),
                DisplayName = x.GetDescription(),
                Type = (int)x
            }));

            EnumHelper.ToList<FirePoliceeContactState>().ForEach(x => FirePoliceContactStateList.Add(new EnumTypeClass
            {
                OriginalName = x.ToString(),
                DisplayName = x.GetDescription(),
                Type = (int)x
            }));

            EnumHelper.ToList<ProfessionalRescueContactState>().ForEach(x => ProfessionalRescueContactStateList.Add(new EnumTypeClass
            {
                OriginalName = x.ToString(),
                DisplayName = x.GetDescription(),
                Type = (int)x
            }));
        }

        /// <summary>
        /// 查询应急救援状态数据
        /// </summary>
        private async void GetEmergencyRescueData(long eventId)
        {
            _RescueDisplayList = await EmergencyResourceService.GetRescueDisplay(eventId);
            SetData(_RescueDisplayList);
        }

        /// <summary>
        /// 订阅WebSocket消息
        /// </summary>
        /// <param name="eventId">事件id</param>
        private void SubscribeWS(long eventId)
        {
            _SubKey = new string[] { $"rescue_{eventId}" };

            var wsClient = WsService.GetWebSocketClient();
            wsClient.Subscribe(CommandType.REALDATA.GetDescription(), _SubKey);
            wsClient.OnMessage = GetWsMessage;
        }

        void GetWsMessage(object obj)
        {
            try
            {
                if (obj is JArray jArr)
                {
                    foreach (var item in jArr)
                    {
                        var result = item?.ToObject<WSResponse<List<RescueDisplay>>>();

                        if (result == null)
                            continue;

                        if (result.Key == _SubKey[0])
                        {
                            SetData(result.Data);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("接收应急救援WebSocket数据发生异常", e);
            }
        }

        /// <summary>
        /// 设置应急救援状态数据
        /// </summary>
        /// <param name="list"></param>
        private void SetData(List<RescueDisplay> list)
        {
            if (list.IsEmpty())
            {
                SelectedPoliceContactState = new EnumTypeClass();
                SelectedFirePoliceContactState = new EnumTypeClass();
                SelectedFirstAidContactState = new EnumTypeClass();
                SelectedProfessionalRescueContactState = new EnumTypeClass();
                return;
            }

            list.ForEach(x =>
            {
                if (x.Code == RescueUnits.公安警察.GetDescription())
                {
                    SelectedPoliceContactState = PoliceContactStateList.FirstOrDefault(d => d.Type == Convert.ToInt32(x.RescueStatusCode));
                    _PoliceRescueId = x.Id;
                }
                else if (x.Code == RescueUnits.公安消防部队.GetDescription())
                {
                    SelectedFirePoliceContactState = FirePoliceContactStateList.FirstOrDefault(d => d.Type == Convert.ToInt32(x.RescueStatusCode));
                    _FirePoliceRescueId = x.Id;
                }
                else if (x.Code == RescueUnits.医疗卫生资源.GetDescription())
                {
                    SelectedFirstAidContactState = FirstAidContactStateList.FirstOrDefault(d => d.Type == Convert.ToInt32(x.RescueStatusCode));
                    _FirstAidesuceId = x.Id;
                }
                else if (x.Code == RescueUnits.专业求援队伍.GetDescription())
                {
                    SelectedProfessionalRescueContactState = ProfessionalRescueContactStateList.FirstOrDefault(d => d.Type == Convert.ToInt32(x.RescueStatusCode));
                    _ProfessionalRescueId = x.Id;
                }
            });
        }

        /// <summary>
        /// 更新应急救援状态
        /// </summary>
        /// <param name="situation"></param>
        private async void UpdateEmergencyRescueStatus()
        {
            try
            {
                var dic = new Dictionary<string, string>()
                {
                    { _PoliceRescueId.ToString(), SelectedPoliceContactState.Type.ToString()},
                    { _FirePoliceRescueId.ToString(), SelectedFirePoliceContactState.Type.ToString()},
                    { _FirstAidesuceId.ToString(), SelectedFirstAidContactState.Type.ToString()},
                    { _ProfessionalRescueId.ToString(), SelectedProfessionalRescueContactState.Type.ToString()},
                };
                var result = await EmergencyResourceService.UpdateRescueStatus(dic);

                if (!result.Success)
                {
                    DialogWindowService.ShowDialog(nameof(MessageBoxView), new DialogParameters { { "Content", "保存失败" } });
                    return;
                }

                GetEmergencyRescueData(_EventId);//获取更新后的数据
                IsCmbEditable = false;
            }
            catch (Exception e)
            {
                LogHelper.Error($"更新应急救援状态发生异常，4个救援状态id分别是:{_PoliceRescueId},{_FirstAidesuceId},{_FirePoliceRescueId},{_ProfessionalRescueId}", e);
                DialogWindowService.ShowDialog(nameof(MessageBoxView), new DialogParameters { { "Content", "保存失败" } });
            }
        }

        public void OnViewActived(IRegionParameters regionParameters)
        {
            regionParameters.TryGetValue("eventId", out long eventId);

            _EventId = eventId;
            GetEmergencyRescueData(eventId);
            SubscribeWS(eventId);
        }
        #endregion
    }
}
