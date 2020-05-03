using System;
using System.Collections.Generic;

namespace MRRC.Guacamole.Components
{
    public class OneOf : Component
    {
        private readonly Dictionary<string, Component> _components;
        private string _activeComponent;

        public OneOf(Dictionary<string, Component> components, string initial)
        {
            _components = components;

            ActiveComponent = initial;

            foreach (var c in _components)
            {
                c.Value.SetChildOf(this);
                c.Value.MustRender += TriggerRender;
            }

            Focused += (sender, ev) =>
            {
                ev.Cancel = false;
                
                if (sender == CurrentComponent)
                {
                    ev.State.ActiveComponent = Parent;
                }
                else
                {
                    ActiveComponent = initial;
                    ev.State.ActiveComponent = CurrentComponent;
                }
            };
        }

        public Component CurrentComponent => _components[ActiveComponent];

        public string ActiveComponent
        {
            get => _activeComponent;
            set
            {
                _activeComponent = value;
                Focus(CurrentComponent);
            }
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!_components.ContainsKey(ActiveComponent)) 
                throw new IndexOutOfRangeException("Invalid active component");
            
            var component = _components[ActiveComponent];
            component.Render(state, x, y);
        }

        public T GetComponent<T>(string key) where T : Component
        {
            return (T) _components[key];
        }

        public override string ToString() => CurrentComponent.ToString();
    }
}