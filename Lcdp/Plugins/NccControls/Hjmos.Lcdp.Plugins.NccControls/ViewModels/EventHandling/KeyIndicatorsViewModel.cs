using Hjmos.Helper;
using Hjmos.Lcdp.Plugins.NccControls.Views.EventHandling;
using Hjmos.Ncc.Common.Services;
using Hjmos.TempEntity.Common;
using Hjmos.TempEntity.EarlyWarning;
using Hjmos.TempEntity.Emergency;
using Hjmos.TempEntity.Enums;
using Hjmos.TempService.BasicData;
using Hjmos.TempService.EarlyWarning;
using Hjmos.TempService.Emergency;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels.EventHandling
{
    public class KeyIndicatorsViewModel : BindableBase, IRegionAware
    {
        private string _SelectedInterruptionTime;
        /// <summary>
        /// 选择的中断运营时间列表
        /// </summary>
        public string SelectedInterruptionTime
        {
            get { return _SelectedInterruptionTime; }
            set { SetProperty(ref _SelectedInterruptionTime, value); }
        }

        private List<DataDictionaryCode> _InterruptionTime;
        /// <summary>
        /// 中断运营时间列表
        /// </summary>
        public List<DataDictionaryCode> InterruptionTime
        {
            get { return _InterruptionTime; }
            set { SetProperty(ref _InterruptionTime, value); }
        }

        private string _SelectedDeaths;
        /// <summary>
        /// 选择的死亡人数
        /// </summary>
        public string SelectedDeaths
        {
            get { return _SelectedDeaths; }
            set { SetProperty(ref _SelectedDeaths, value); }
        }

        private List<DataDictionaryCode> _Deaths;
        /// <summary>
        /// 死亡人数
        /// </summary>
        public List<DataDictionaryCode> Deaths
        {
            get { return _Deaths; }
            set { SetProperty(ref _Deaths, value); }
        }

        private string _SelectedInjuredNum;
        /// <summary>
        /// 选择的受伤人数
        /// </summary>
        public string SelectedInjuredNum
        {
            get { return _SelectedInjuredNum; }
            set { SetProperty(ref _SelectedInjuredNum, value); }
        }

        private List<DataDictionaryCode> _InjuredNum;
        /// <summary>
        /// 受伤人数
        /// </summary>
        public List<DataDictionaryCode> InjuredNum
        {
            get { return _InjuredNum; }
            set { SetProperty(ref _InjuredNum, value); }
        }

        private string _SelectedLossAmount;
        /// <summary>
        /// 选择的损失金额
        /// </summary>
        public string SelectedLossAmount
        {
            get { return _SelectedLossAmount; }
            set { SetProperty(ref _SelectedLossAmount, value); }
        }

        private List<DataDictionaryCode> _LossAmount;
        /// <summary>
        /// 损失金额
        /// </summary>
        public List<DataDictionaryCode> LossAmount
        {
            get { return _LossAmount; }
            set { SetProperty(ref _LossAmount, value); }
        }

        /// <summary>
        /// 查询返回的预警事件数据
        /// </summary>
        EarlyWarningEvent _EarlyWarningEvent;

        /// <summary>
        /// 查询返回的突发事件数据
        /// </summary>
        UpdateEmergencyEvent _EmergencyEvent;

        /// <summary>
        /// 事件类型
        /// </summary>
        EventType _EventType;

        /// <summary>
        /// 事件id
        /// </summary>
        long _EventId;

        private bool _IsCmbEditable;
        /// <summary>
        /// ComboBox是否可编辑
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
            //暂存编辑前的数据
            if (_EventType == EventType.预警事件)
            {
                _EarlyWarningEvent = new EarlyWarningEvent()
                {
                    Id = _EventId,
                    InterruptTimeCode = SelectedInterruptionTime,
                    DeathsCode = SelectedDeaths,
                    InjuriesCode = SelectedInjuredNum,
                    EconomicLossCode = SelectedLossAmount
                };
            }
            else 
            {
                _EmergencyEvent = new UpdateEmergencyEvent()
                {
                    Id = _EventId,
                    InterruptTimeCode = SelectedInterruptionTime,
                    DeathsCode = SelectedDeaths,
                    InjuriesCode = SelectedInjuredNum,
                    EconomicLossCode = SelectedLossAmount
                };
            }

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
            if (_EventType == EventType.预警事件)
            {                
                var earlyWarningEvent = new EarlyWarningEvent
                {
                    Id = _EventId,
                    InterruptTimeCode = SelectedInterruptionTime,
                    DeathsCode = SelectedDeaths,
                    EconomicLossCode = SelectedLossAmount,
                    InjuriesCode = SelectedInjuredNum
                };

                UpdateEarlyWarningEvent(earlyWarningEvent);
            }
            else 
            {
                var emergencyEvent = new UpdateEmergencyEvent
                {
                    Id = _EventId,
                    InterruptTimeCode = SelectedInterruptionTime,
                    DeathsCode = SelectedDeaths,
                    EconomicLossCode = SelectedLossAmount,
                    InjuriesCode = SelectedInjuredNum
                };

                UpdateEmergencyEvent(emergencyEvent);
            }
        }

        private DelegateCommand _CancelCommand;
        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand =>
            _CancelCommand ?? (_CancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            IsCmbEditable = false;

            if (_EventType == EventType.预警事件)
            {
                SetKeyIndicatorsData(_EarlyWarningEvent);
            }
            else 
            {
                SetKeyIndicatorsData(_EmergencyEvent);
            }
        }

        public KeyIndicatorsViewModel()
        {
            InitData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async void InitData()
        {
            var result = await BasicDataService.GetDataByCodes(new string[]
            {
                BasicDataCode.中断运营时间.GetDescription(),
                BasicDataCode.死亡人数.GetDescription(),
                BasicDataCode.受伤人数.GetDescription(),
                BasicDataCode.损失金额.GetDescription()
            });

            if (result != null && result.Count == 4)
            {
                InterruptionTime = result[0].Children;
                Deaths = result[1].Children;
                InjuredNum = result[2].Children;
                LossAmount = result[3].Children;
            }
        }

        /// <summary>
        /// 更新预警事件关键指标
        /// </summary>
        private async void UpdateEarlyWarningEvent(EarlyWarningEvent earlyWarningEvent)
        {
            var result = await EarlyWaringEventService.AddEarlyWarningEvent(earlyWarningEvent, new string[] { "id", "levelInterruptTimeCode", "levelDeathsCode", "levelInjuriesCode", "levelEconomicLossCode" });

            if (!result.Success)
            {
                DialogWindowService.ShowDialog(nameof(MessageBoxView), new DialogParameters { { "Content", "保存失败" } });
                return;
            }

            IsCmbEditable = false;
        }

        /// <summary>
        /// 更新突发事件关键指标
        /// </summary>
        private async void UpdateEmergencyEvent(UpdateEmergencyEvent mergencyEvent)
        {
            var result = await EmergencyService.UpdateEmergencyEventScene(mergencyEvent);

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
            regionParameters.TryGetValue("eventType", out EventType eventType);

            _EventType = eventType;
            _EventId = eventId;
            GetEventKeyIndicators(eventId, eventType);
        }

        /// <summary>
        /// 查询事件关键指标
        /// </summary>
        /// <param name="eventId"></param>
        private async void GetEventKeyIndicators(long eventId, EventType eventType)
        {
            if (eventType == EventType.预警事件)
            {
                var warningEvent = await EarlyWaringEventService.GetEarlyWarningEventById(eventId);
                SetKeyIndicatorsData(warningEvent);
            }
            else
            {
                var emergencyEvent = await EmergencyService.GetEmergencyById(eventId);
                SetKeyIndicatorsData(emergencyEvent);
            }
        }     

        /// <summary>
        /// 设置突发事件指标数据
        /// </summary>
        /// <param name="keyIndicators">事件实体</param>
        private void SetKeyIndicatorsData(KeyIndicators keyIndicators)
        {
            SelectedInterruptionTime = keyIndicators?.InterruptTimeCode;
            SelectedDeaths = keyIndicators?.DeathsCode;
            SelectedInjuredNum = keyIndicators?.InjuriesCode;
            SelectedLossAmount = keyIndicators?.EconomicLossCode;
        }
    }
}
