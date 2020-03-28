using System;
using System.ComponentModel;

namespace MRRC.Guacamole
{
    public abstract class Component
    {
        public Component Parent { get; private set; }

        private bool IsActive(ApplicationState state) => this == state.ActiveComponent || 
                                                         (Parent?.IsActive(state) ?? false);
        
        /// <summary>
        /// Render this component to stdout
        /// </summary>
        /// <param name="state">The current state of the application</param>
        /// <param name="x">The left offset</param>
        /// <param name="y">The top offset</param>
        public void Render(ApplicationState state, int x, int y)
        {
            var active = IsActive(state);
            Draw(x, y, active, state);
        }

        protected abstract void Draw(int x, int y, bool active, ApplicationState state);

        /// <summary>
        /// Sets parent to be the parent of this component
        /// </summary>
        public void SetChildOf(Component parent)
        {
            Parent = parent;
        }
        
        protected event EventHandler<KeyPressEvent> KeyPressed;

        /// <summary>
        /// Handle a key press. Will trigger key press event and bubble to all components. Any component that has
        /// children should bubble this event downwards to them.
        /// </summary>
        public void HandleKeyPress(object sender, KeyPressEvent keyInfo)
        {
            KeyPressed?.Invoke(sender, keyInfo);
            if (!keyInfo.Cancel) Parent?.HandleKeyPress(sender, keyInfo);
        }
        
        /// <summary>
        /// This event is invoked when the component requires a re-render. Any component that has children should bubble
        /// this event upwards.
        /// </summary>
        public event EventHandler MustRender;

        protected void TriggerRender() => TriggerRender(this, EventArgs.Empty);

        protected void TriggerRender(object sender, EventArgs args)
        {
            MustRender?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This event is triggered when the component is brought into focus.
        /// </summary>
        /// <remarks>Cancelling this is enabled by default to prevent any situations where the user cannot exit some
        /// focus state, so to enable it a handler must be set to allow the event.</remarks>
        public event EventHandler<CancelEventArgs> Focused;

        public bool HandleFocused(object sender)
        {
            var args = new CancelEventArgs(true);
            Focused?.Invoke(sender, args);
            return args.Cancel;
        }

        /// <summary>
        /// This event is triggered when the component stops being focused
        /// </summary>
        public event EventHandler Blurred;

        public void HandleBlurred(object sender)
        {
            Blurred?.Invoke(sender, EventArgs.Empty);
        }
    }
}