using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole
{
    public class Guac
    {
        public Component Root { get; }
        
        public Component ActiveComponent { get; private set; }

        public void Focus(Component component) => ActiveComponent = component;

        private ApplicationState MakeApplicationState() => new ApplicationState
        {
            ActiveComponent = ActiveComponent
        };

        private void Render()
        {
            // for now we need to re-render the whole screen, so we will clear it first to make sure nothing breaks
            Console.Clear();
            Root.Render(MakeApplicationState(), 0, 0);
        }
        
        public Guac(Component root)
        {
            Root = root;
            ActiveComponent = root;

            root.MustRender += delegate
            {
                Render();
            };

            Render();
        }

        /// <summary>
        /// Blocking function that should be called in a while loop.
        /// </summary>
        public void HandleEventLoop()
        {
            var character = Console.ReadKey();
            var ev = new KeyPressEvent
            {
                Key = character,
                State = MakeApplicationState()
            };
            
            ActiveComponent.HandleKeyPress(this, ev);

            if (ev.State.ActiveComponent != ActiveComponent)
            {
                var oldComponent = ActiveComponent;
                var newComponent = ev.State.ActiveComponent;

                var cancelled = newComponent.HandleFocused(oldComponent);

                if (!cancelled)
                {
                    oldComponent.HandleBlurred(newComponent);
                    ActiveComponent = newComponent;
                }
            }
            
            if (ev.Rerender) Render();
        }
    }
}