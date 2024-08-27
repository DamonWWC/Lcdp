using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Adorners
{
    /// <summary>
    /// 显示装饰面板的控件。
    /// </summary>
    public sealed class AdornerLayer : Panel, IAdornerLayer
    {
        private readonly UIElement _designPanel;

        public AdornerLayer(UIElement designPanel)
        {
            _designPanel = designPanel;

            LayoutUpdated += OnLayoutUpdated;

            Adorners = new AdornerPanelCollection(this);
        }

        /// <summary>
        /// 布局改变时更新所有装饰器
        /// </summary>
        private void OnLayoutUpdated(object sender, EventArgs e) => UpdateAllAdorners(false);

        /// <summary>
        /// 大小改变时更新所有装饰器
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateAllAdorners(true);
        }

        internal AdornerPanelCollection Adorners { get; }

        private sealed class AdornerInfo
        {
            internal readonly List<AdornerPanel> adorners = new();
            internal bool isVisible;
            internal Rect position;
        }

        // 保存被装饰元素和装饰面板的字典
        // 被装饰元素 => 装饰信息（包含装饰面板列表）
        private Dictionary<UIElement, AdornerInfo> _dict = new();

        /// <summary>
        /// 清空装饰层中的装饰器
        /// </summary>
        internal void ClearAdorners()
        {
            if (_dict.Count == 0)
                return;

            this.Children.Clear();
            _dict = new Dictionary<UIElement, AdornerInfo>();

        }

        /// <summary>
        /// 获取或创建被装饰元素的装饰信息
        /// </summary>
        /// <param name="adornedElement">被装饰元素</param>
        /// <returns></returns>
        private AdornerInfo GetOrCreateAdornerInfo(UIElement adornedElement)
        {
            if (!_dict.TryGetValue(adornedElement, out AdornerInfo info))
            {
                // 不存在时创建新的装饰信息类
                info = _dict[adornedElement] = new AdornerInfo();
                // 当被装饰元素是设计面板的子项时，设置为可见
                info.isVisible = adornedElement.IsDescendantOf(_designPanel);
            }
            return info;
        }

        /// <summary>
        /// 获取已经存在的装饰信息
        /// </summary>
        /// <param name="adornedElement">被装饰元素</param>
        /// <returns></returns>
        private AdornerInfo GetExistingAdornerInfo(UIElement adornedElement)
        {
            _dict.TryGetValue(adornedElement, out AdornerInfo info);
            return info;
        }

        /// <summary>
        /// 添加装饰面板
        /// </summary>
        /// <param name="adornerPanel">装饰面板</param>
        internal void AddAdorner(AdornerPanel adornerPanel)
        {
            if (adornerPanel.AdornedElement == null)
                throw new DesignerException("必须设置adornerPanel.AdornedElement");

            // 保存到字典中
            AdornerInfo info = GetOrCreateAdornerInfo(adornerPanel.AdornedElement);
            info.adorners.Add(adornerPanel);

            // 如果可见，就添加到设计面板中
            if (info.isVisible)
            {
                AddAdornerToChildren(adornerPanel);
            }

        }

        /// <summary>
        /// 把装饰面板添加到设计面板中
        /// </summary>
        /// <param name="adornerPanel">装饰面板</param>
        private void AddAdornerToChildren(AdornerPanel adornerPanel)
        {
            UIElementCollection children = this.Children;
            int i;
            for (i = 0; i < children.Count; i++)
            {
                AdornerPanel p = (AdornerPanel)children[i];
                if (p.Order > adornerPanel.Order)
                {
                    break;
                }
            }
            children.Insert(i, adornerPanel);
        }

        /// <summary>
        /// 布局时的尺寸测量
        /// </summary>
        /// <param name="availableSize">可以为子元素分配的可用空间</param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // 无穷大的尺寸
            Size infiniteSize = new(double.PositiveInfinity, double.PositiveInfinity);

            // 更新子项的DesiredSize
            foreach (AdornerPanel adorner in this.Children)
            {
                adorner.Measure(infiniteSize);
            }

            // 返回所需尺寸(排列的时候用不到，所以返回任意Size就行了)
            return new Size(0, 0);
        }

        /// <summary>
        /// 排列每个元素
        /// </summary>
        /// <param name="finalSize">测量后的尺寸</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (AdornerPanel adorner in this.Children)
            {
                // 如果被装饰元素属于设计面板的子项
                if (adorner.AdornedElement.IsDescendantOf(_designPanel))
                {
                    // 获取被装饰元素的转换：GeneralTransform是一个提供通用变换支持的抽象类
                    GeneralTransform generalTransform = adorner.AdornedElement.TransformToAncestor(_designPanel);
                    // 矩阵转换
                    MatrixTransform matrixTransform = generalTransform as MatrixTransform;

                    if (matrixTransform != null && adorner.AdornedDesignItem != null // 矩阵转换不为空、被装饰的设计项不为空
                        && adorner.AdornedDesignItem.Parent != null && adorner.AdornedDesignItem.Parent.View is Canvas // 被装饰的设计项父类是画布
                        && adorner.AdornedElement.RenderSize.Height == 0 && adorner.AdornedElement.RenderSize.Width == 0 // 被装饰元素的呈现大小是0
                    )
                    {
                        double width = ((FrameworkElement)adorner.AdornedElement).Width;
                        width = width > 0 ? width : 2d;
                        double height = ((FrameworkElement)adorner.AdornedElement).Height;
                        height = height > 0 ? height : 2d;
                        double xOffset = matrixTransform.Matrix.OffsetX - (width / 2);
                        double yOffset = matrixTransform.Matrix.OffsetY - (height / 2);
                        matrixTransform = new MatrixTransform(new Matrix(matrixTransform.Matrix.M11, matrixTransform.Matrix.M12, matrixTransform.Matrix.M21, matrixTransform.Matrix.M22, xOffset, yOffset));
                    }
                    else if (generalTransform is GeneralTransformGroup group)
                    {
                        #region TODO: WPFDesigner里本来注释掉的
                        //var intTrans = ((GeneralTransformGroup) transform).Children.FirstOrDefault(x => x.GetType().Name == "GeneralTransform2DTo3DTo2D");
                        //var prp = intTrans.GetType().GetField("_worldTransformation", BindingFlags.Instance | BindingFlags.NonPublic);
                        //var mtx = (Matrix3D) prp.GetValue(intTrans);
                        //var mtx2D = new Matrix(mtx.M11, mtx.M12, mtx.M21, mtx.M22, mtx.OffsetX, mtx.OffsetY);
                        //rt = new MatrixTransform(mtx2D); 
                        #endregion
                        matrixTransform = group.Children.OfType<MatrixTransform>().LastOrDefault();
                    }


                    adorner.RenderTransform = matrixTransform;
                }

                // 排列每个元素
                adorner.Arrange(new Rect(new Point(0, 0), adorner.DesiredSize));
            }
            return finalSize;
        }

        /// <summary>
        /// 移除装饰面板
        /// </summary>
        /// <param name="adornerPanel">装饰面板</param>
        internal bool RemoveAdorner(AdornerPanel adornerPanel)
        {
            if (adornerPanel.AdornedElement == null)
                return false;

            AdornerInfo info = GetExistingAdornerInfo(adornerPanel.AdornedElement);
            if (info == null)
                return false;

            if (info.adorners.Remove(adornerPanel))
            {
                if (info.isVisible)
                {
                    // 从当前装饰层中移除装饰面板
                    this.Children.Remove(adornerPanel);
                }

                // 当装饰信息中没有装饰面板了，从字典中移除
                if (info.adorners.Count == 0)
                {
                    _dict.Remove(adornerPanel.AdornedElement);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        /// <param name="element"></param>
        /// <param name="forceInvalidate"></param>
        public void UpdateAdornersForElement(UIElement element, bool forceInvalidate)
        {
            // 获取元素的装饰信息
            AdornerInfo info = GetExistingAdornerInfo(element);
            if (info != null)
            {
                // 更新布局
                UpdateAdornersForElement(element, info, forceInvalidate);
            }
        }


        /// <summary>
        /// 获取表示元素当前位置的矩形
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private Rect GetPositionCache(UIElement element)
        {
            GeneralTransform t = element.TransformToAncestor(_designPanel);
            Point p = t.Transform(new Point(0, 0));
            return new Rect(p, element.RenderSize);
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        /// <param name="element"></param>
        /// <param name="info"></param>
        /// <param name="forceInvalidate"></param>
        private void UpdateAdornersForElement(UIElement element, AdornerInfo info, bool forceInvalidate)
        {
            // 如果元素是设计面板的子项
            if (element.IsDescendantOf(_designPanel))
            {
                // 设置关联的装饰面板可见
                if (!info.isVisible)
                {
                    info.isVisible = true;
                    // 让装饰面板可见
                    info.adorners.ForEach(AddAdornerToChildren);
                }
                // 获取表示元素当前位置的矩形
                Rect rect = GetPositionCache(element);

                // 如果需要更新布局，并且元素的位置有所改变
                if (forceInvalidate || !info.position.Equals(rect))
                {
                    // 缓存元素原来的位置
                    info.position = rect;
                    foreach (AdornerPanel p in info.adorners)
                    {
                        // 使装饰面板的测量失效
                        p.InvalidateMeasure();
                    }
                    // 使装饰层重新布局
                    InvalidateArrange();
                }
            }
            else
            {
                if (info.isVisible)
                {
                    info.isVisible = false;
                    // 设置装饰面板不可见
                    info.adorners.ForEach(this.Children.Remove);
                }
            }
        }

        /// <summary>
        /// 更新所有装饰器的布局
        /// </summary>
        /// <param name="forceInvalidate"></param>
        private void UpdateAllAdorners(bool forceInvalidate)
        {
            foreach (KeyValuePair<UIElement, AdornerInfo> pair in _dict)
            {
                UpdateAdornersForElement(pair.Key, pair.Value, forceInvalidate);
            }
        }
    }
}
