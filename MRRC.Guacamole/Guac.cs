using System;
using System.Threading.Tasks;
using MRRC.Guacamole.Components;

namespace MRRC.Guacamole
{
    /// <summary>
    /// Guacamole main class, controls all the components
    /// </summary>
    public class Guac
    {
        private Component _renderOverride;
        
        /// <summary>
        /// The root component
        /// </summary>
        public Component Root { get; }
        
        /// <summary>
        /// The component that is currently focused
        /// </summary>
        public IComponent ActiveComponent { get; private set; }

        /// <summary>
        /// Triggered when a key is pressed.
        /// </summary>
        /// <remarks>The <see cref="KeyPressEvent.Rerender"/> property does not do anything for this event</remarks>
        public event EventHandler<KeyPressEvent> KeyPressed;

        /// <summary>
        /// Focus the specified component
        /// </summary>
        /// <remarks>This assumes the component is in the component tree</remarks>
        public bool Focus(IComponent component)
        {
            var oldComponent = ActiveComponent;
            var newComponent = component;
            var rerender = false;

            FocusEventArgs focusEvent = null;

            while (focusEvent?.State.ActiveComponent != newComponent)
            {
                focusEvent = newComponent.HandleFocused(oldComponent, new ApplicationState
                {
                    ActiveComponent = newComponent
                });
                if (focusEvent.Rerender) rerender = true;

                oldComponent = newComponent;
                newComponent = focusEvent.State.ActiveComponent;
            }

            if (focusEvent?.Cancel == false)
            {
                ActiveComponent.HandleBlurred(newComponent);
                ActiveComponent = newComponent;
            }

            return rerender;
        }

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
            
            // then render the active component again, to put the cursor in the correct place
            ActiveComponent.RenderLikePrevious();
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

            root.FocusRequested += (_, component) =>
            {
                if (Focus(component)) Render();
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
                ev.Rerender = Focus(ev.State.ActiveComponent) || ev.Rerender;
            }
            
            if (ev.Rerender) Render();
        }

        /// <summary>
        /// Shows a dialogue window in the centre of the screen
        /// </summary>
        /// <param name="title">The title of the dialogue</param>
        /// <param name="contents">The main text in the dialogue</param>
        /// <param name="buttons">A list of options that can be selected</param>
        /// <returns>Task that completes when a button is pressed, containing the text of that button</returns>
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
                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    
                    _renderOverride = null;
                    ActiveComponent = oldActiveComponent;
                    ActiveComponent.HandleFocused(this, MakeApplicationState());

                    Render();

                    completionSource.TrySetResult(button);
                });
            };
            
            return completionSource.Task;
        }
    }
}