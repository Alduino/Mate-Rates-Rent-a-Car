namespace MRRC.Guacamole
{
    /// <summary>
    /// The current state of the application
    /// </summary>
    public class ApplicationState
    {
        /// <summary>
        /// The component that is currently focused. May be mutated in most events to focus a new component.
        /// </summary>
        public IComponent ActiveComponent;
    }
}