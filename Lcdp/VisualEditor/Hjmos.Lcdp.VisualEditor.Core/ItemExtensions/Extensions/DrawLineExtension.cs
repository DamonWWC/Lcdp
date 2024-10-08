﻿using Hjmos.Lcdp.VisualEditor.Core.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(Canvas))]
    [ExtensionFor(typeof(Grid))]
    public class DrawLineExtension : BehaviorExtension, IDrawItemExtension
    {
        DesignItem CreateItem(DesignContext context, Type componentType)
        {
            object newInstance = context.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory(componentType, null);
            DesignItem item = context.Services.Component.RegisterComponentForDesigner(newInstance);
            //changeGroup = item.OpenGroup("Draw Line");
            context.Services.ExtensionManager.ApplyDefaultInitializers(item);
            return item;
        }

        //private ChangeGroup changeGroup;

        #region IDrawItemBehavior implementation

        public bool CanItemBeDrawn(Type createItemType)
        {
            return createItemType == typeof(Line);
        }

        public void StartDrawItem(DesignItem clickedOn, Type createItemType, IDesignPanel panel, MouseEventArgs e, Action<DesignItem> drawItemCallback)
        {
            DesignItem createdItem = CreateItem(panel.Context, createItemType);

            Point startPoint = e.GetPosition(clickedOn.View);
            PlacementOperation operation = PlacementOperation.TryStartInsertNewComponents(clickedOn,
                new DesignItem[] { createdItem },
                new Rect[] { new Rect(startPoint.X, startPoint.Y, double.NaN, double.NaN) },
                PlacementType.AddItem);

            if (operation != null)
            {
                createdItem.Services.Selection.SetSelectedComponents(new DesignItem[] { createdItem });
                operation.Commit();
            }

            createdItem.Properties[Shape.StrokeProperty].SetValue(Brushes.Black);
            createdItem.Properties[Shape.StrokeThicknessProperty].SetValue(2d);
            createdItem.Properties[Shape.StretchProperty].SetValue(Stretch.None);
            drawItemCallback?.Invoke(createdItem);

            LineHandlerExtension lineHandler = createdItem.Extensions.OfType<LineHandlerExtension>().First();
            lineHandler.DragListener.ExternalStart();

            new DrawLineMouseGesture(lineHandler, clickedOn.View).Start(panel, (MouseButtonEventArgs)e);
        }

        #endregion

        sealed class DrawLineMouseGesture : ClickOrDragMouseGesture
        {
            private readonly LineHandlerExtension l;
            //private ChangeGroup changeGroup;

            public DrawLineMouseGesture(LineHandlerExtension l, IInputElement relativeTo)
            {
                this.l = l;
                this._positionRelativeTo = relativeTo;
                //this.changeGroup = changeGroup;
            }

            protected override void OnMouseMove(object sender, MouseEventArgs e)
            {
                base.OnMouseMove(sender, e);
                l.DragListener.ExternalMouseMove(e);
            }

            protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
            {
                l.DragListener.ExternalStop();
                //if (changeGroup != null)
                //{
                //    changeGroup.Commit();
                //    changeGroup = null;
                //}
                base.OnMouseUp(sender, e);
            }

            protected override void OnStopped()
            {
                //if (changeGroup != null)
                //{
                //    changeGroup.Abort();
                //    changeGroup = null;
                //}
                if (services.Tool.CurrentTool is CreateComponentTool)
                {
                    services.Tool.CurrentTool = services.Tool.PointerTool;
                }
                base.OnStopped();
            }
        }
    }
}
