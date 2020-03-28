using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole.Components
{
    public class MenuManager : Component
    {
        public Menu<Component> RootMenu { get; }

        public MenuManager(Menu<Component> rootMenu)
        {
            RootMenu = rootMenu;

            KeyPressed += RootMenu.HandleKeyPress;
            RootMenu.MustRender += TriggerRender;
        }

        public override void Render(int x, int y, bool active = true)
        {
            var menuItems = new List<Component>(new [] { RootMenu });

            while (true)
            {
                var currentItem = menuItems.Last();

                if (currentItem is Menu<Component> menuItem)
                {
                    var nextItem = menuItem.ActiveItem;
                    menuItems.Add(nextItem);
                }
                else break;
            }

            // subtract one from menu width so that they overlap on the border line
            var totalWidth = menuItems.Sum(v => v is Menu<Component> menu ? menu.Width - 1 : 16);
            var scroll = Math.Min(0, Console.WindowWidth - totalWidth);

            var offset = scroll;
            for (var i = 0; i < menuItems.Count; i++)
            {
                var item = menuItems[i];
                
                // if the item is a menu item, use its width
                // otherwise, if it is the last item, fill the available space
                // otherwise default to a width of 16
                var width = item is Menu<Component> menu ? menu.Width :
                    i == menuItems.Count - 1 ? Console.WindowWidth - totalWidth + 16 : 16;

                item.Render(x + offset, y, active && i == menuItems.Count - 1);
                offset += width;
            }
        }
    }
}