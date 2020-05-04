using System;
using System.Collections.Generic;

namespace MRRC.Guacamole.Components
{
    /// <summary>
    /// Displays one of the specified components
    /// </summary>
    public class OneOf : Component
    {
        private readonly Dictionary<string, Component> _components;
        private string _activeComponent;

        /// <param name="components">List of components that can be displayed</param>
        /// <param name="initial">The key of the initial component to display</param>
        /// <param name="title">The title to display in menus</param>
        public OneOf(Dictionary<string, Component> components, string initial, string title)
        {
            _components = components;

            ActiveComponent = initial;
            Title = title;

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

        /// <summary>
        /// The current component being rendered
        /// </summary>
        public Component CurrentComponent => _components[ActiveComponent];

        /// <summary>
        /// The key of the currently active component
        /// </summary>
        public string ActiveComponent
        {
            get => _activeComponent;
            set
            {
                if (ActiveComponent != null) CurrentComponent.MustRender -= TriggerRender;
                
                _activeComponent = value;
                Focus(CurrentComponent);
                
                CurrentComponent.MustRender += TriggerRender;
            }
        }

        /// <summary>
        /// The title displayed in menus
        /// </summary>
        public string Title { get; }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!_components.ContainsKey(ActiveComponent)) 
                throw new IndexOutOfRangeException("Invalid active component");
            
            var component = _components[ActiveComponent];
            component.Render(state, x, y);
        }

        /// <summary>
        /// Gets one of the components by its key
        /// </summary>
        public T GetComponent<T>(string key) where T : Component
        {
            return (T) _components[key];
        }

        public override string ToString() => Title;
    }
}