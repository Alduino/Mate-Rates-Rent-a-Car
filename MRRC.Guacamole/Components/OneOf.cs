using System;
using System.Collections.Generic;

namespace MRRC.Guacamole.Components
{
    public class OneOf : Component
    {
        private readonly Dictionary<string, Component> _components;

        public OneOf(Dictionary<string, Component> components, string initial)
        {
            _components = components;

            ActiveComponent = initial;

            foreach (var c in _components)
            {
                c.Value.SetChildOf(this);
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
        
        public string ActiveComponent { get; set; }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!_components.ContainsKey(ActiveComponent)) 
                throw new IndexOutOfRangeException("Invalid active component");
            
            var component = _components[ActiveComponent];
            component.Render(state, x, y);
        }

        public override string ToString() => CurrentComponent.ToString();
    }
}