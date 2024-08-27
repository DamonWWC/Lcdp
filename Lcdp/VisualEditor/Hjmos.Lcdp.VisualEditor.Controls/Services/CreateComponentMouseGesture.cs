using System;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.Services
{
    internal sealed class CreateComponentMouseGesture : ClickOrDragMouseGesture
    {
        private readonly DesignItem _createdItem;
        private PlacementOperation _operation;
        private readonly DesignItem _container;
        //ChangeGroup changeGroup;

        public CreateComponentMouseGesture(DesignItem clickedOn, DesignItem createdItem)
        {
            _container = clickedOn;
            _createdItem = createdItem;
            positionRelativeTo = clickedOn.View;
            //this.changeGroup = changeGroup;
        }

        //		GrayOutDesignerExceptActiveArea grayOut;
        //		SelectionFrame frame;
        //		AdornerPanel adornerPanel;

        private Rect GetStartToEndRect(MouseEventArgs e)
        {
            Point endPoint = e.GetPosition(positionRelativeTo);
            return new Rect(
                Math.Min(startPoint.X, endPoint.X),
                Math.Min(startPoint.Y, endPoint.Y),
                Math.Abs(startPoint.X - endPoint.X),
                Math.Abs(startPoint.Y - endPoint.Y)
            );
        }

        protected override void OnDragStarted(MouseEventArgs e)
        {
            _operation = PlacementOperation.TryStartInsertNewComponents(_container, new DesignItem[] { _createdItem }, new Rect[] { GetStartToEndRect(e).Round() }, PlacementType.Resize);
            if (_operation != null)
            {
                services.Selection.SetSelectedComponents(new DesignItem[] { _createdItem });
            }
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(sender, e);
            if (_operation != null)
            {
                foreach (PlacementInformation info in _operation.PlacedItems)
                {
                    info.Bounds = GetStartToEndRect(e).Round();
                    _operation.CurrentContainerBehavior.SetPosition(info);
                }
            }
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (hasDragStarted)
            {
                if (_operation != null)
                {
                    _operation.Commit();
                    _operation = null;
                }
            }
            else
            {
                CreateComponentTool.AddItemsWithDefaultSize(_container, new[] { _createdItem });
            }
            //if (changeGroup != null)
            //{
            //    changeGroup.Commit();
            //    changeGroup = null;
            //}

            if (designPanel.Context.Services.Component is MyComponentService service)
            {
                service.RaiseComponentRegisteredAndAddedToContainer(_createdItem);
            }

            base.OnMouseUp(sender, e);
        }

        protected override void OnStopped()
        {
            if (_operation != null)
            {
                _operation.Abort();
                _operation = null;
            }
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
