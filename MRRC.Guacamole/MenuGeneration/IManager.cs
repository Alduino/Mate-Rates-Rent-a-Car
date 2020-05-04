using MRRC.Guacamole.Components;

namespace MRRC.Guacamole.MenuGeneration
{
    /// <summary>
    /// Contains a menu that has been generated
    /// </summary>
    public interface IManager
    {
        Menu Menu { get; }
    }
}