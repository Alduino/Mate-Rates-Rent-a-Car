namespace MRRC.Guacamole
{
    public interface IComponent
    {
        IComponent Parent { get; }

        void Render(ApplicationState state, int x, int y);

        void SetChildOf(IComponent parent);
    }
}