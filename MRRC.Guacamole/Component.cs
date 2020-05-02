using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole
{
    public abstract class Component : IComponent
    {
        private readonly struct RenderState
        {
            public RenderState(int x, int y, ApplicationState appState)
            {
                X = x;
                Y = y;
                AppState = appState;
            }

            public int X { get; }
            public int Y { get; }
            public ApplicationState AppState { get; }
        }

        private RenderState _previousRenderState;
            
        public IComponent Parent { get; private set; }

        public Component ParentComponent => Parent is Component cp ? cp : null;

        public bool IsActive(ApplicationState state) => this == state.ActiveComponent || HasActiveChildren(state);

        private bool HasActiveChildren(ApplicationState state) =>
            Children.Any(child => child.IsActive(state));
        
        protected virtual IEnumerable<IComponent> Children => Enumerable.Empty<Component>();

        /// <summary>
        /// Render this component to stdout
        /// </summary>
        /// <param name="state">The current state of the application</param>
        /// <param name="x">The left offset</param>
        /// <param name="y">The top offset</param>
        public void Render(ApplicationState state, int x, int y)
        {
            _previousRenderState = new RenderState(x, y, state);
            var active = IsActive(state);
            Draw(x, y, active, state);
        }

        /// <summary>
        /// Renders the component with the same parameters as the last render
        /// </summary>
        public void RenderLikePrevious()
        {
            Render(_previousRenderState.AppState, _previousRenderState.X, _previousRenderState.Y);
        }

        protected abstract void Draw(int x, int y, bool active, ApplicationState state);

        /// <summary>
        /// Sets parent to be the parent of this component
        /// </summary>
        public void SetChildOf(IComponent parent)
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
            if (!keyInfo.Cancel) ParentComponent?.HandleKeyPress(sender, keyInfo);
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
        public event EventHandler<FocusEventArgs> Focused;

        public FocusEventArgs HandleFocused(object sender, ApplicationState state)
        {
            var args = new FocusEventArgs(state);
            Focused?.Invoke(sender, args);
            return args;
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