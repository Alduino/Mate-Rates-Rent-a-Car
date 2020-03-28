using System;

namespace MRRC.Guacamole
{
    public abstract class Component
    {
        /// <summary>
        /// Render this component to stdout
        /// </summary>
        /// <param name="x">The left offset</param>
        /// <param name="y">The top offset</param>
        /// <param name="active">When true, this component is currently focused</param>
        public abstract void Render(int x, int y, bool active = true);

        protected event EventHandler<ConsoleKeyInfo> KeyPressed;
        
        /// <summary>
        /// This event is invoked when the component requires a re-render. Any component that has children should bubble
        /// this event upwards.
        /// </summary>
        public event EventHandler MustRender;

        protected void TriggerRender()
        {
            MustRender?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handle a key press. Will trigger key press event and bubble to all components. Any component that has
        /// children should bubble this event downwards to them.
        /// </summary>
        public void HandleKeyPress(object sender, ConsoleKeyInfo keyInfo)
        {
            KeyPressed?.Invoke(sender, keyInfo);
        }
    }
}