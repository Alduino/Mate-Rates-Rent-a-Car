using System;
using System.Linq;
using MRRC.Guacamole.Components;

namespace MRRC.Guacamole.MenuGeneration
{
    /// <summary>
    /// Generates a menu from a class with <see cref="MenuItemAttribute"/>s
    /// </summary>
    public class MenuGenerator
    {
        /// <summary>
        /// Contains a menu that has been generated
        /// </summary>
        public class Manager : IManager
        {
            public Menu Menu { get; }
            
            public Manager(Menu menu)
            {
                Menu = menu;
            }
        }
        
        /// <summary>
        /// Generates a <see cref="Manager"/> instance from a class instance
        /// </summary>
        /// <param name="name">Display name of the menu</param>
        /// <param name="source">Instance of the menu class</param>
        public static IManager GenerateManager(string name, object source) => new MenuGenerator(name, source).Generate();

        private readonly string _name;
        private readonly object _manager;
        private readonly int _width;

        public MenuGenerator(string name, object manager)
        {
            _name = name;
            _manager = manager;
            _width = 32;
        }

        public MenuGenerator(string name, int width, object manager)
        {
            _name = name;
            _manager = manager;
            _width = width;
        }

        /// <inheritdoc cref="GenerateManager"/>
        public IManager Generate()
        {
            var type = _manager.GetType();
            var items =
                from property in type.GetProperties()
                where Attribute.IsDefined(property, typeof(MenuItemAttribute))
                select (Component) property.GetValue(_manager);

            return new Manager(new Menu(_name, items.ToArray(), _width));
        }
    }
}