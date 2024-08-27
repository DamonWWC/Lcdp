using Hjmos.Lcdp.Extensions;
using Hjmos.Lcdp.VisualEditor.Core;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.TempEntity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels
{
    public class AlarmSolveSwitcherViewModel : ViewModelBase, IPageAware
    {
        private string _lineCode;

        public AlarmSolveSwitcherViewModel()
        {
            // 将枚举填充到List<ViewType>
            ViewType = new List<ViewType>();
            ViewType.FillWithMembers();

            Enum.GetNames(typeof(ViewType)).ToList();
        }

        /// <summary>
        /// TODO: 这里的PageLoaded是指PageShell加载完毕后调用此方法传上下文过来
        /// 后续把名称重新改一下
        /// </summary>
        /// <param name="pageContext"></param>
        public void OnPageLoaded(PageContext pageContext) => _lineCode = pageContext.Parameters.GetValue<string>("lineCode");

        /// <summary>
        /// 可选的线网视图
        /// </summary>
        public List<ViewType> ViewType
        {
            get => _viewType;
            set => SetProperty(ref _viewType, value);
        }
        private List<ViewType> _viewType;

        /// <summary>
        /// 切换线网线路事件
        /// </summary>
        internal Action<string> ChangeLineAction;

        /// <summary>
        /// 当前选中的线网视图
        /// </summary>
        public ViewType SelectedViewType
        {
            get => _selectedViewType;
            set
            {
                if (SetProperty(ref _selectedViewType, value))
                {
                    if (value == TempEntity.Enums.ViewType.线路视图)
                    {
                        ChangeLineAction?.Invoke(_lineCode);
                    }
                    else if (value == TempEntity.Enums.ViewType.线网视图)
                    {
                        ChangeLineAction?.Invoke("all");
                    }
                }
            }
        }
        private ViewType _selectedViewType;
    }
}
