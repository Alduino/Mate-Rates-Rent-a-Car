namespace MRRC.Guacamole.Components.Forms
{
    public interface IInput<out T>
    {
        T Value { get; }
        
        int Width { get; }
        int Height { get; }
    }
}