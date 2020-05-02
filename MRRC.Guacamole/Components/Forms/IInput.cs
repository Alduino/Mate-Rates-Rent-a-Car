namespace MRRC.Guacamole.Components.Forms
{
    public interface IInput<out T> : IComponent
    {
        T Value { get; }
        
        int Width { get; }
        int Height { get; }
    }
}