namespace MRRC.Guacamole
{
    public interface IComponent
    {
        /// <summary>
        /// Render this component to stdout
        /// </summary>
        /// <param name="x">The left offset</param>
        /// <param name="y">The top offset</param>
        /// <param name="active">When true, this component is currently focused</param>
        void Render(int x, int y, bool active = true);
    }
}