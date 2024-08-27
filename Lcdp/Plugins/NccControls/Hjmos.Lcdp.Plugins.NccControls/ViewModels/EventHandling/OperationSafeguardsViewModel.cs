using Hjmos.Helper;
using Hjmos.Lcdp.Plugins.NccControls.Views.EventHandling;
using Hjmos.Ncc.Common.Services;
using Hjmos.TempEntity.Emergency;
using Hjmos.TempService.Emergency;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels.EventHandling
{
    public class OperationSafeguardsViewModel : BindableBase, IRegionAware
    {
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
            //编辑前暂存数据
            _OriginalPassengerMeasures.Content = PassengerFlowMeasure?.Content;
            _OriginalTrainMeasures.Content = TrainMeasure?.Content;

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
            UpdateOperationSafeguards();
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

            PassengerFlowMeasure.Content = _OriginalPassengerMeasures?.Content;
            TrainMeasure.Content = _OriginalTrainMeasures?.Content;
        }

        private SafeguardMeasures _PassengerFlowMeasure;
        /// <summary>
        /// 客流调整措施
        /// </summary>
        public SafeguardMeasures PassengerFlowMeasure
        {
            get { return _PassengerFlowMeasure; }
            set { SetProperty(ref _PassengerFlowMeasure, value); }
        }

        private SafeguardMeasures _TrainMeasure;
        /// <summary>
        /// 车辆调配措施
        /// </summary>
        public SafeguardMeasures TrainMeasure
        {
            get { return _TrainMeasure; }
            set { SetProperty(ref _TrainMeasure, value); }
        }

        /// <summary>
        /// 修改前的客流措施
        /// </summary>
        SafeguardMeasures _OriginalPassengerMeasures = new SafeguardMeasures();
        /// <summary>
        /// 修改前的车辆措施
        /// </summary>
        SafeguardMeasures _OriginalTrainMeasures = new SafeguardMeasures();

        public OperationSafeguardsViewModel()
        { }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async void InitData(long evnetId)
        {
            var measures = await EmergencyService.GetPassengerAndTrainMeasures(evnetId);

            if (!measures.IsEmpty())
            {
                var passengerMeasures = measures.FirstOrDefault(x => x.Type == "passenger");
                PassengerFlowMeasure = new SafeguardMeasures
                {
                    Id = passengerMeasures?.Id,
                    Content = passengerMeasures?.Content
                };

                var trainMeasures = measures.FirstOrDefault(x => x.Type == "car");
                TrainMeasure = new SafeguardMeasures
                {
                    Id = trainMeasures?.Id,
                    Content = trainMeasures?.Content
                };
            }
        }

        /// <summary>
        /// 更新运营保障措施
        /// </summary>
        /// <param name="situation"></param>
        private async void UpdateOperationSafeguards()
        {
            var measures = new List<SafeguardMeasures>
            {
                PassengerFlowMeasure,
                TrainMeasure
            };

            var result = await EmergencyService.UpdatePassengerAndTrainMeasures(measures);

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
            InitData(eventId);
        }
    }
}
