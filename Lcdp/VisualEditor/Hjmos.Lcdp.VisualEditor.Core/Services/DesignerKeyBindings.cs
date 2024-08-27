using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    internal class DesignerKeyBindings : IKeyBindingService
    {
        private readonly DesignSurface _surface;
        private readonly Collection<KeyBinding> _bindings;

        public DesignerKeyBindings(DesignSurface surface)
        {
            Debug.Assert(surface != null);
            _surface = surface;
            _bindings = new Collection<KeyBinding>();
        }

        public void RegisterBinding(KeyBinding binding)
        {
            if (binding != null)
            {
                _surface.InputBindings.Add(binding);
                _bindings.Add(binding);
            }
        }

        public void DeregisterBinding(KeyBinding binding)
        {
            if (_bindings.Contains(binding))
            {
                _surface.InputBindings.Remove(binding);
                _bindings.Remove(binding);
            }
        }

        public KeyBinding GetBinding(KeyGesture gesture) => _bindings.FirstOrDefault(binding => binding.Key == gesture.Key && binding.Modifiers == gesture.Modifiers);

        public object Owner => _surface;

    }
}
