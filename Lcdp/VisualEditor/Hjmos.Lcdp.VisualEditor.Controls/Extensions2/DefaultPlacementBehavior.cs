﻿using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    [ExtensionFor(typeof(Panel))]
    [ExtensionFor(typeof(Control))]
    [ExtensionFor(typeof(Border))]
    [ExtensionFor(typeof(Viewbox))]
    public class DefaultPlacementBehavior : BehaviorExtension, IPlacementBehavior
    {
        static DefaultPlacementBehavior() { }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ExtendedItem.ContentProperty == null || Metadata.IsPlacementDisabled(ExtendedItem.ComponentType))
                return;
            ExtendedItem.AddBehavior(typeof(IPlacementBehavior), this);
        }

        public virtual bool CanPlace(IEnumerable<DesignItem> childItems, PlacementType type, PlacementAlignment position) => true;

        public virtual void BeginPlacement(PlacementOperation operation)
        {
        }

        public virtual void EndPlacement(PlacementOperation operation) => InfoTextEnterArea.Stop(ref infoTextEnterArea);

        public virtual Rect GetPosition(PlacementOperation operation, DesignItem item) => GetPositionRelativeToContainer(operation, item);

        public Rect GetPositionRelativeToContainer(PlacementOperation operation, DesignItem item)
        {
            if (item.View == null)
                return Rect.Empty;
            Point p = item.View.TranslatePoint(new Point(), operation.CurrentContainer.View);

            return new Rect(p, PlacementOperation.GetRealElementSize(item.View));
        }

        public virtual void BeforeSetPosition(PlacementOperation operation)
        {
        }

        public virtual bool CanPlaceItem(PlacementInformation info) => true;

        public virtual void SetPosition(PlacementInformation info)
        {
            if (info.Operation.Type != PlacementType.Move
                && info.Operation.Type != PlacementType.MovePoint
                && info.Operation.Type != PlacementType.MoveAndIgnoreOtherContainers)
                ModelTools.Resize(info.Item, info.Bounds.Width, info.Bounds.Height);

            //if (info.Operation.Type == PlacementType.MovePoint)
            //	ModelTools.Resize(info.Item, info.Bounds.Width, info.Bounds.Height);
        }

        public virtual bool CanLeaveContainer(PlacementOperation operation) => true;

        public virtual void LeaveContainer(PlacementOperation operation)
        {
            foreach (PlacementInformation info in operation.PlacedItems)
            {
                DesignItemProperty parentProperty = info.Item.ParentProperty;
                if (parentProperty.IsCollection)
                {
                    parentProperty.CollectionElements.Remove(info.Item);
                }
                else
                {
                    parentProperty.Reset();
                }
            }
        }

        private static InfoTextEnterArea infoTextEnterArea;

        public virtual bool CanEnterContainer(PlacementOperation operation, bool shouldAlwaysEnter)
        {
            bool canEnter = InternalCanEnterContainer(operation);

            if (canEnter && !shouldAlwaysEnter && !Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt))
            {
                FrameworkElement element = ExtendedItem.View as FrameworkElement;
                Rect b = new(0, 0, element.ActualWidth, element.ActualHeight);
                InfoTextEnterArea.Start(ref infoTextEnterArea, this.Services, this.ExtendedItem.View, b, Translations.Instance.PressAltText);

                return false;
            }

            return canEnter;
        }

        private bool InternalCanEnterContainer(PlacementOperation operation)
        {
            InfoTextEnterArea.Stop(ref infoTextEnterArea);

            if (ExtendedItem.Component is Expander expander)
            {
                if (!expander.IsExpanded)
                {
                    expander.IsExpanded = true;
                }
            }

            if (ExtendedItem.Component is UserControl && ExtendedItem.ComponentType != typeof(UserControl))
                return false;

            if (ExtendedItem.Component is Decorator decorator)
                return decorator.Child == null;

            if (ExtendedItem.ContentProperty.IsCollection)
                return true;
                // TODO: 这里需要判断集合中是否能添加子元素
                //return CollectionSupport.CanCollectionAdd(ExtendedItem.ContentProperty.ReturnType, operation.PlacedItems.Select(p => p.Item.Component));

            if (ExtendedItem.ContentProperty.ReturnType == typeof(string))
                return false;

            if (!ExtendedItem.ContentProperty.IsSet)
                return true;

            object value = ExtendedItem.ContentProperty.ValueOnInstance;
            // don't overwrite non-primitive values like bindings
            return ExtendedItem.ContentProperty.Value == null && (value is string && string.IsNullOrEmpty(value as string));
        }

        public virtual void EnterContainer(PlacementOperation operation)
        {
            if (ExtendedItem.ContentProperty.IsCollection)
            {
                foreach (PlacementInformation info in operation.PlacedItems)
                {
                    ExtendedItem.ContentProperty.CollectionElements.Add(info.Item);
                }
            }
            else
            {
                ExtendedItem.ContentProperty.SetValue(operation.PlacedItems[0].Item);
            }

            if (operation.Type == PlacementType.AddItem)
            {
                foreach (PlacementInformation info in operation.PlacedItems)
                {
                    SetPosition(info);
                }
            }
        }

        public virtual Point PlacePoint(Point point) => new Point(Math.Round(point.X), Math.Round(point.Y));
    }
}
