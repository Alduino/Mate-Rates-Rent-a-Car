using System;
using System.ComponentModel;

namespace MRRC.Guacamole
{
    /// <summary>
    /// Event args for key presses
    /// </summary>
    public class KeyPressEvent : CancelEventArgs
    {
        /// <summary>
        /// The current state of the application
        /// </summary>
        public ApplicationState State;
        
        /// <summary>
        /// Information about the keys that were pressed
        /// </summary>
        public ConsoleKeyInfo Key;
        
        /// <summary>
        /// Set this to true to trigger a render once the event completes
        /// </summary>
        public bool Rerender;
    }
}