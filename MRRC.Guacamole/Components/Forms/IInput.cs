namespace MRRC.Guacamole.Components.Forms
{
    /// <summary>
    /// Interface that supports any input types
    /// </summary>
    public interface IInput<out T> : IComponent
    {
        /// <summary>
        /// The current value of the input
        /// </summary>
        T Value { get; }
        
        int Width { get; }
        int Height { get; }

        /// <summary>
        /// Sets the input's value. May throw casting errors if the inputted type is incorrect
        /// </summary>
        void SetValue(object val);
    }
}