using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole
{
    /// <summary>
    /// Base class for any component
    /// </summary>
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
            
        /// <summary>
        /// The component that owns this one
        /// </summary>
        public IComponent Parent { get; private set; }

        /// <summary>
        /// The parent component, but as a <see cref="Component"/> (or null if it is not one)
        /// </summary>
        public Component ParentComponent => Parent is Component cp ? cp : null;

        /// <summary>
        /// Returns true when this component or its children are active
        /// </summary>
        /// <param name="state">The current application state</param>
        public bool IsActive(ApplicationState state) => this == state.ActiveComponent || HasActiveChildren(state);

        private bool HasActiveChildren(ApplicationState state) =>
            Children.Any(child => child.IsActive(state));
        
        /// <summary>
        /// Should enumerate to every child of this component
        /// </summary>
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
        
        /// <summary>
        /// Triggered when a key is pressed
        /// </summary>
        /// <remarks>If the <see cref="KeyPressEvent.Cancel"/> property is false (the default), this event will
        /// propagate up through the parent components.</remarks>
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

        /// <summary>
        /// Trigger a render of the entire component tree
        /// </summary>
        protected void TriggerRender() => TriggerRender(this, EventArgs.Empty);

        /// <summary>
        /// Trigger a render of the entire component tree, with the event args specified
        /// </summary>
        protected void TriggerRender(object sender, EventArgs args)
        {
            MustRender?.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        /// This event is triggered when the component is brought into focus.
        /// </summary>
        /// <remarks>Cancelling this is enabled by default to prevent any situations where the user cannot exit some
        /// focus state, so to enable it a handler must be set to allow the event.</remarks>
        public event EventHandler<FocusEventArgs> Focused;

        /// <summary>
        /// Triggers the <see cref="Focused"/> event and returns the event args used.
        /// </summary>
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

        /// <summary>
        /// Triggers the internal blur event
        /// </summary>
        public void HandleBlurred(object sender)
        {
            Blurred?.Invoke(sender, EventArgs.Empty);
        }

        internal event EventHandler<IComponent> FocusRequested;

        /// <summary>
        /// Attempts to focus the specified component. No blur event will be triggered on this component.
        /// </summary>
        protected void Focus(IComponent component) => Focus(this, component);

        private void Focus(object sender, IComponent component)
        {
            FocusRequested?.Invoke(sender, component);
            ParentComponent?.Focus(sender, component);
        }
    }
}