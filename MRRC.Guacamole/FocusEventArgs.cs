using System.ComponentModel;

namespace MRRC.Guacamole
{
    /// <summary>
    /// Event args used for focus events
    /// </summary>
    public class FocusEventArgs : CancelEventArgs
    {
        /// <summary>
        /// The current application state. Set this to focus another component.
        /// </summary>
        public ApplicationState State;
        
        /// <summary>
        /// Set this to true to rerender the entire component tree
        /// </summary>
        public bool Rerender;

        public FocusEventArgs(ApplicationState state, bool rerender = false) : base(true)
        {
            State = state;
            Rerender = rerender;
        }
    }
}