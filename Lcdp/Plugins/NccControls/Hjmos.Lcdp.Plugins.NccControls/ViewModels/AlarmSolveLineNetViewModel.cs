using Hjmos.BaseControls.Tools;
using Hjmos.Common;
using Hjmos.Helper;
using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.Plugins.NccControls.Views.EventHandling;
using Hjmos.Lcdp.VisualEditor.Core;
using Hjmos.Ncc.Common.Enums;
using Hjmos.Ncc.Common.Services;
using Hjmos.Ncc.Common.ViewModels;
using Hjmos.Ncc.Config.WebConfig;
using Hjmos.Restful;
using Hjmos.TempEntity.Enums;
using Hjmos.TempService.ServiceName.Restful;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels
{
    public class AlarmSolveLineNetViewModel : ViewModelBase, IPageAware
    {

        /// <summary>
        /// TODO: 这里的PageLoaded是指PageShell加载完毕后调用此方法传上下文过来
        /// 后续把名称重新改一下
        /// </summary>
        /// <param name="pageContext"></param>
        public void OnPageLoaded(PageContext pageContext)
        {
            long eventId = pageContext.Parameters.GetValue<long>("eventId");
            long planId = pageContext.Parameters.GetValue<long>("planId");
            EventType eventType = pageContext.Parameters.GetValue<EventType>("eventType");

            lineCode = pageContext.Parameters.GetValue<string>("lineCode");
            stationCode = pageContext.Parameters.GetValue<string>("stationCode");
            AlarmSolveHeaderViewModel.Topic = pageContext.Parameters.GetValue<string>("topic");


            AlarmSolveHeaderViewModel.Type = eventType == EventType.预警事件 ? "预警" : "突发";
            AlarmSolveHeaderViewModel.TypeBackground = eventType == EventType.预警事件 ? ResourceHelper.GetResource<Brush>("LBrush_Orange3_Orange1") : ResourceHelper.GetResource<Brush>("LBrush_Pink2_Red3");

            _param = new RegionParameters
            {
                {"eventId",eventId },
                {"planId",planId },
                {"eventType",eventType }
            };

            ContainerLocator.Container.Resolve<IRegionService>().SendMsgToChildren(pageContext.PageShell, _param);
        }

        public AlarmSolveSwitcherViewModel AlarmSolveSwitcherViewModel
        {
            get
            {
                if (_alarmSolveSwitcherViewModel is null)
                {
                    var switcher = new AlarmSolveSwitcherViewModel();
                    // 注册切换线网线路事件
                    switcher.ChangeLineAction += ChangeLineAction;
                    return switcher;
                }
                else
                    return _alarmSolveSwitcherViewModel;
            }

            set => SetProperty(ref _alarmSolveSwitcherViewModel, value);
        }
        private AlarmSolveSwitcherViewModel _alarmSolveSwitcherViewModel;

        public AlarmSolveHeaderViewModel AlarmSolveHeaderViewModel
        {
            get => _alarmSolveHeaderViewModel ?? new AlarmSolveHeaderViewModel();
            set => SetProperty(ref _alarmSolveHeaderViewModel, value);
        }
        private AlarmSolveHeaderViewModel _alarmSolveHeaderViewModel = new AlarmSolveHeaderViewModel();

        public AlarmSolveLineNetViewModel()
        {
            /// <summary>
            /// 获取Token
            /// </summary>
            /// <returns></returns>
            RestApi.OAuthorize(RestfulServiceNames.GetOAuthToken, "admin", "admin", RestfulServiceNames.GetOAuthRefreshToken);


            MapViewModel = new LineNetMapGisViewModel();

            TypeViewModel = new LineNetTypeViewModel();
            GisViewModel = new LineNetMapViewModel();
            MapViewModel.Address = WebConfig.WireGisNetWorkAddress;
            GisViewModel.Address = WebConfig.WireGisAddress;

            //执行
            ChangeLayerAction = MapViewModel.ChangeLayerAction;
            ChangeOperaAction = MapViewModel.ChangeOperationAction;
            ChangeReflashWebAction = MapViewModel.ChangeReflashWebAction;
            ChangeLineAction = MapViewModel.ChangeLineAction;


            //Gis执行
            GisChangeLayerAction = GisViewModel.ChangeLayerAction;
            GisChangeOperaAction = GisViewModel.ChangeOperationAction;
            GisChangeReflashWebAction = GisViewModel.ChangeReflashWebAction;

            //监听           

            TypeViewModel.LineNetTypeChangeAction += TypeViewModel_LineNetTypeChangeAction;
            GisViewModel.Visibility = Visibility.Collapsed;

            LineNetTypeVisibility = Visibility.Collapsed;

            ViewType = EnumHelper.ToList<ViewType>();
            SelectedViewType = TempEntity.Enums.ViewType.线网视图;

            // 注入类型到宿主程序

            RegisterTypesHelper.Instance.RegisterTypes(containerRegistry =>
            {
                if (!containerRegistry.IsRegistered<IRegionService>())
                {
                    containerRegistry.Register<IRegionService, RegionService>();
                }

                containerRegistry.RegisterDialogWindow<Ncc.Resource.Controls.CustomDialogWindow>();

                containerRegistry.RegisterDialog<MessageBoxView>(); //处置过程提示框
                containerRegistry.RegisterDialog<AddPersonView>();  //处置过程添加人员


                //创建DialogWindowService实例
                ContainerLocator.Container.Resolve<DialogWindowService>();
            });
        }

        private List<ViewType> _ViewType;
        public List<ViewType> ViewType
        {
            get { return _ViewType; }
            set { SetProperty(ref _ViewType, value); }
        }

        private ViewType _SelectedViewType;
        public ViewType SelectedViewType
        {
            get { return _SelectedViewType; }
            set
            {
                if (SetProperty(ref _SelectedViewType, value))
                {
                    MapViewModel.Address = WebConfig.WireGisNetWorkAddress;
                    if (value == TempEntity.Enums.ViewType.线路视图)
                    {
                        ChangeLineAction?.Invoke(lineCode);
                    }
                    else if (value == TempEntity.Enums.ViewType.线网视图)
                    {
                        ChangeLineAction?.Invoke("all");
                    }
                }
            }
        }

        /// <summary>
        /// 线网地址类型改变触发
        /// </summary>
        /// <param name="obj"></param>
        private void TypeViewModel_LineNetTypeChangeAction(LineNetType obj)
        {
            switch (obj)
            {
                case LineNetType.地铁线网:
                    MapViewModel.Visibility = Visibility.Visible;
                    GisViewModel.Visibility = Visibility.Collapsed;
                    break;
                case LineNetType.卫星影像:
                    break;
                case LineNetType.GIS地图:
                    MapViewModel.Visibility = Visibility.Collapsed;
                    GisViewModel.Visibility = Visibility.Visible;
                    break;
                case LineNetType.暗黑风格:
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 数据更新
        /// </summary>
        public event Action<string> EventUpdateAction;
        public Action<LineNetLayer, bool, bool> ChangeLayerAction;
        public Action<LineNetOperation> ChangeOperaAction;
        public Action ChangeReflashWebAction;
        /// <summary>
        /// 切换线网线路的
        /// </summary>
        private Action<string> ChangeLineAction;


        public Action<LineNetLayer, bool> GisChangeLayerAction;
        public Action<LineNetOperation> GisChangeOperaAction;
        public Action GisChangeReflashWebAction;

        private LineNetMapGisViewModel _MapViewModel;
        /// <summary>
        /// LineNetMap视图模型
        /// </summary>
        public LineNetMapGisViewModel MapViewModel
        {
            get { return _MapViewModel; }
            set { SetProperty(ref _MapViewModel, value); }
        }

        private LineNetTypeViewModel _TypeViewModel;
        /// <summary>
        /// LineNetMap视图模型
        /// </summary>
        public LineNetTypeViewModel TypeViewModel
        {
            get { return _TypeViewModel; }
            set { SetProperty(ref _TypeViewModel, value); }
        }

        private LineNetMapViewModel _GisViewModel;
        /// <summary>
        /// LineNetGsMap视图模型
        /// </summary>
        public LineNetMapViewModel GisViewModel
        {
            get { return _GisViewModel; }
            set { SetProperty(ref _GisViewModel, value); }
        }

        private Visibility _LineNetTypeVisibility = Visibility.Hidden;

        /// <summary>
        /// 右下角图例显示
        /// </summary>
        public Visibility LineNetTypeVisibility
        {
            get { return _LineNetTypeVisibility; }
            set { SetProperty(ref _LineNetTypeVisibility, value); }
        }

        #region Command


        private DelegateCommand<object> _OperationCommand;
        /// <summary>
        /// 线网图操作命令
        /// </summary>
        public DelegateCommand<object> OperationCommand =>
            _OperationCommand ?? (_OperationCommand = new DelegateCommand<object>(Operation));

        private void Operation(object parameter)
        {
            if (parameter is string param)
            {
                var parameters = param.Split('_');
                switch (parameters[0])
                {
                    case "刷新":
                        ChangeReflashWebAction?.Invoke();
                        GisChangeReflashWebAction?.Invoke();
                        break;
                    case "全屏":
                        //LeftRightRegionVisibility = LeftRightRegionVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                        //LineNetTypeVisibility = LeftRightRegionVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                        break;
                    case "客流":
                        ChangeLayerAction?.Invoke(LineNetLayer.客流, parameters[1] == "1", false);
                        GisChangeLayerAction?.Invoke(LineNetLayer.客流, parameters[1] == "1");
                        break;
                    case "行车":
                        ChangeLayerAction?.Invoke(LineNetLayer.行车, parameters[1] == "1", false);
                        GisChangeLayerAction?.Invoke(LineNetLayer.行车, parameters[1] == "1");
                        break;
                    default:
                        break;
                }
            }
        }

        private object _view;

        private DelegateCommand<object> _LoadedCommand;
        /// <summary>
        /// 确定发布预警信息
        /// </summary>
        public DelegateCommand<object> LoadedCommand =>
            _LoadedCommand ?? (_LoadedCommand = new DelegateCommand<object>(Loaded));


        private void Loaded(object parameter)
        {
            _view = parameter;
            ContainerLocator.Container.Resolve<IRegionService>().SendMsgToChildren(parameter, _param);
        }

        #endregion
        private RegionParameters _param;
        private string lineCode;
        private string stationCode;
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            if (parameters == null) return;

            parameters.TryGetValue("eventId", out long eventId);
            parameters.TryGetValue("planId", out long planId);
            parameters.TryGetValue("eventType", out EventType eventType);
            parameters.TryGetValue("lineCode", out lineCode);
            parameters.TryGetValue("stationCode", out stationCode);
            parameters.TryGetValue("topic", out string topic);
            AlarmSolveHeaderViewModel.Type = eventType == EventType.预警事件 ? "预警" : "突发";
            AlarmSolveHeaderViewModel.TypeBackground = eventType == EventType.预警事件 ? ResourceHelper.GetResource<Brush>("LBrush_Orange3_Orange1") : ResourceHelper.GetResource<Brush>("LBrush_Pink2_Red3");
            AlarmSolveHeaderViewModel.Topic = topic;
            //测试
            //eventId = 205406502651904;
            //eventType = EventType.预警事件;
            //eventId = 205752079747072;
            //eventType = EventType.突发事件;

            _param = new RegionParameters
            {
                {"eventId",eventId },
                {"planId",planId },
                {"eventType",eventType }
            };
        }
    }
}