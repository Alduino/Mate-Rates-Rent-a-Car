using System;
using System.Linq;
using MRRC.Guacamole.Components;

namespace MRRC.Guacamole.MenuGeneration
{
    public class MenuGenerator
    {
        public class Manager : IManager
        {
            public Menu Menu { get; }
            
            public Manager(Menu menu)
            {
                Menu = menu;
            }
        }
        
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