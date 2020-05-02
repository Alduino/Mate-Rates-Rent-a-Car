using System;

namespace MRRC.Guacamole
{
    public interface IComponent
    {
        IComponent Parent { get; }
        
        event EventHandler MustRender;

        void Render(ApplicationState state, int x, int y);

        void SetChildOf(IComponent parent);

        void HandleKeyPress(object sender, KeyPressEvent keyInfo);

        FocusEventArgs HandleFocused(object sender, ApplicationState state);

        void HandleBlurred(object sender);

        bool IsActive(ApplicationState state);

        void RenderLikePrevious();
    }
}