using Hjmos.Lcdp.VisualEditor.Core.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(Canvas))]
    [ExtensionFor(typeof(Grid))]
    public class DrawPolyLineExtension : BehaviorExtension, IDrawItemExtension
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

        public bool CanItemBeDrawn(Type createItemType) => createItemType == typeof(Polyline) || createItemType == typeof(Polygon);

        public void StartDrawItem(DesignItem clickedOn, Type createItemType, IDesignPanel panel, MouseEventArgs e, Action<DesignItem> drawItemCallback)
        {
            var createdItem = CreateItem(panel.Context, createItemType);

            var startPoint = e.GetPosition(clickedOn.View);
            var operation = PlacementOperation.TryStartInsertNewComponents(clickedOn,
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

            if (createItemType == typeof(Polyline))
                createdItem.Properties[Polyline.PointsProperty].CollectionElements.Add(createdItem.Services.Component.RegisterComponentForDesigner(new Point(0, 0)));
            else
                createdItem.Properties[Polygon.PointsProperty].CollectionElements.Add(createdItem.Services.Component.RegisterComponentForDesigner(new Point(0, 0)));

            new DrawPolylineMouseGesture(createdItem, clickedOn.View, this.ExtendedItem.GetCompleteAppliedTransformationToView()).Start(panel, (MouseButtonEventArgs)e);
        }

        #endregion
    }
}
