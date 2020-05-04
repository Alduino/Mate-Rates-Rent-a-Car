using System;

namespace MRRC.Guacamole.MenuGeneration
{
    /// <summary>
    /// Marks this property as a menu item
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MenuItemAttribute : Attribute
    {
        public MenuItemAttribute() {}
    }
}