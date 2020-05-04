using System;

namespace MRRC.Guacamole
{
    /// <summary>
    /// Component interface, so that other interfaces can be components
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// The parent component
        /// </summary>
        IComponent Parent { get; }
        
        /// <summary>
        /// Triggered when the component needs to be rerendered
        /// </summary>
        event EventHandler MustRender;

        /// <summary>
        /// Renders the component
        /// </summary>
        /// <param name="state">The current state of the application</param>
        /// <param name="x">The x coordinate to render at</param>
        /// <param name="y">The y coordinate to render at</param>
        void Render(ApplicationState state, int x, int y);

        /// <summary>
        /// Sets the parent of this component
        /// </summary>
        void SetChildOf(IComponent parent);

        /// <summary>
        /// Triggers the internal key press event
        /// </summary>
        void HandleKeyPress(object sender, KeyPressEvent keyInfo);

        /// <summary>
        /// Triggers the internal focus event and returns the event args
        /// </summary>
        FocusEventArgs HandleFocused(object sender, ApplicationState state);

        /// <summary>
        /// Triggers the internal blur event
        /// </summary>
        void HandleBlurred(object sender);

        /// <summary>
        /// Returns true when this component or its children are active
        /// </summary>
        bool IsActive(ApplicationState state);

        /// <summary>
        /// Renders with the same parameters as the previous time
        /// </summary>
        void RenderLikePrevious();
    }
}