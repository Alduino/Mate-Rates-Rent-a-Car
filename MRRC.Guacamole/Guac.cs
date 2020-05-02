using System;
using System.Threading.Tasks;
using MRRC.Guacamole.Components;

namespace MRRC.Guacamole
{
    public class Guac
    {
        private Component _renderOverride;
        
        public Component Root { get; }
        
        public IComponent ActiveComponent { get; private set; }

        /// <summary>
        /// Triggered when a key is pressed.
        /// </summary>
        /// <remarks>The <see cref="KeyPressEvent.Rerender"/> property does not do anything for this event</remarks>
        public event EventHandler<KeyPressEvent> KeyPressed;

        public void Focus(Component component) => ActiveComponent = component;

        private ApplicationState MakeApplicationState() => new ApplicationState
        {
            ActiveComponent = ActiveComponent
        };

        private void Render()
        {
            // for now we need to re-render the whole screen, so we will clear it first to make sure nothing breaks
            Console.Clear();
            Console.ResetColor();
            Root.Render(MakeApplicationState(), 0, 0);
            _renderOverride?.Render(MakeApplicationState(), 0, 0);
        }
        
        public Guac(Component root)
        {
            Root = root;
            ActiveComponent = root;

            var focusEv = ActiveComponent.HandleFocused(this, MakeApplicationState());
            if (focusEv.State.ActiveComponent != ActiveComponent) ActiveComponent = focusEv.State.ActiveComponent;

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
            
            KeyPressed?.Invoke(this, ev);
            if (ev.Cancel) return;

            ActiveComponent.HandleKeyPress(this, ev);

            if (ev.State.ActiveComponent != ActiveComponent)
            {
                var oldComponent = ActiveComponent;
                var newComponent = ev.State.ActiveComponent;

                var focusEvent = newComponent.HandleFocused(oldComponent, MakeApplicationState());
                if (focusEvent.Rerender) ev.Rerender = true;

                if (!focusEvent.Cancel)
                {
                    oldComponent.HandleBlurred(newComponent);
                    ActiveComponent = newComponent;
                }
            }
            
            if (ev.Rerender) Render();
        }

        public Task<string> ShowDialogue(string title, string contents, string[] buttons)
        {
            var dialogue = new Dialogue(title, contents, buttons);
            var oldActiveComponent = ActiveComponent;
            oldActiveComponent.HandleBlurred(this);
            ActiveComponent = dialogue;
            _renderOverride = dialogue;
            Render();
            
            var completionSource = new TaskCompletionSource<string>();

            dialogue.ButtonPressed += (_, button) =>
            {
                _renderOverride = null;
                ActiveComponent = oldActiveComponent;
                Render();
                
                completionSource.TrySetResult(button);
            };
            
            return completionSource.Task;
        }
    }
}