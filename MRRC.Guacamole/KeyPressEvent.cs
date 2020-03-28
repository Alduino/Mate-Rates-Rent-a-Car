using System;
using System.ComponentModel;

namespace MRRC.Guacamole
{
    public class KeyPressEvent : CancelEventArgs
    {
        public ApplicationState State;
        public ConsoleKeyInfo Key;
        public bool Rerender;
    }
}