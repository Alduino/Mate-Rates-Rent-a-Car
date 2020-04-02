using System.ComponentModel;

namespace MRRC.Guacamole
{
    public class FocusEventArgs : CancelEventArgs
    {
        public ApplicationState State;
        public bool Rerender;

        public FocusEventArgs(ApplicationState state, bool rerender = false) : base(true)
        {
            State = state;
            Rerender = rerender;
        }
    }
}