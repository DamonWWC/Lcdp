﻿using System;
using System.Windows.Media;
namespace Hjmos.Lcdp.VisualEditor.Controls.Services
{
    // See IToolService for description.
    // 有关说明请参见IToolService。
    internal sealed class DefaultToolService : IToolService, IDisposable
    {
        private ITool _currentTool;
        private IDesignPanel _designPanel;

        public DefaultToolService(DesignContext context)
        {
            _currentTool = this.PointerTool;
            context.Services.RunWhenAvailable(
                delegate (IDesignPanel designPanel)
                {
                    _designPanel = designPanel;
                    _currentTool.Activate(designPanel);
                });
        }

        public HitTestFilterCallback DesignPanelHitTestFilterCallback
        {
            set => _designPanel.CustomHitTestFilterBehavior = value;
        }

        public void Dispose()
        {
            if (_designPanel != null)
            {
                _currentTool.Deactivate(_designPanel);
                _designPanel = null;
            }
        }

        public ITool PointerTool
        {
            get { return Services.PointerTool.Instance; }
        }

        public ITool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (_currentTool == value) return;
                if (_designPanel != null)
                {
                    _currentTool.Deactivate(_designPanel);
                }
                _currentTool = value;
                if (_designPanel != null)
                {
                    _currentTool.Activate(_designPanel);
                }
                if (CurrentToolChanged != null)
                {
                    CurrentToolChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CurrentToolChanged;

        public IDesignPanel DesignPanel
        {
            get { return _designPanel; }
        }
    }
}
