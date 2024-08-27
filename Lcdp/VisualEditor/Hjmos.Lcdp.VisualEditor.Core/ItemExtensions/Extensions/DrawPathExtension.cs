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
    public class DrawPathExtension : BehaviorExtension, IDrawItemExtension
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

        public bool CanItemBeDrawn(Type createItemType) => createItemType == typeof(Path);

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

            PathFigure figure = new();
            PathGeometry geometry = new();
            DesignItem geometryDesignItem = createdItem.Services.Component.RegisterComponentForDesigner(geometry);
            DesignItem figureDesignItem = createdItem.Services.Component.RegisterComponentForDesigner(figure);
            createdItem.Properties[Path.DataProperty].SetValue(geometry);
            //geometryDesignItem.Properties[PathGeometry.FiguresProperty].CollectionElements.Add(figureDesignItem);
            figureDesignItem.Properties[PathFigure.StartPointProperty].SetValue(new Point(0, 0));

            new DrawPathMouseGesture(figure, createdItem, clickedOn.View, this.ExtendedItem.GetCompleteAppliedTransformationToView()).Start(panel, (MouseButtonEventArgs)e);
        }

        #endregion
    }
}
