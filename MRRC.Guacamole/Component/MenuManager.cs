using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole.Component
{
    public class MenuManager : IComponent
    {
        public event EventHandler<ConsoleKeyInfo> KeyPressed;
        
        public Menu<IComponent> RootMenu { get; }

        public MenuManager(Menu<IComponent> rootMenu)
        {
            RootMenu = rootMenu;
        }

        public void Render(int x, int y, bool active = true)
        {
            var menuItems = new List<IComponent>(new [] { RootMenu });

            while (true)
            {
                var currentItem = menuItems.Last();

                if (currentItem is Menu<IComponent> menuItem)
                {
                    var nextItem = menuItem.ActiveItem;
                    menuItems.Add(nextItem);
                }
                else break;
            }

            // subtract one from menu width so that they overlap on the border line
            var totalWidth = menuItems.Sum(v => v is Menu<IComponent> menu ? menu.Width - 1 : 16);
            var scroll = Math.Min(0, Console.WindowWidth - totalWidth);

            var offset = scroll;
            for (var i = 0; i < menuItems.Count; i++)
            {
                var item = menuItems[i];
                
                // if the item is a menu item, use its width
                // otherwise, if it is the last item, fill the available space
                // otherwise default to a width of 16
                var width = item is Menu<IComponent> menu ? menu.Width :
                    i == menuItems.Count - 1 ? Console.WindowWidth - totalWidth + 16 : 16;

                item.Render(x + offset, y, active && i == menuItems.Count - 1);
                offset += x;
            }
        }
    }
}