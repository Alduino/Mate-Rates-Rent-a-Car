using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole.Components
{
    /// <summary>
    /// Manages menu instances
    /// </summary>
    public class MenuManager : Component
    {
        public Menu RootMenu { get; }

        public MenuManager(Menu rootMenu)
        {
            RootMenu = rootMenu;
            rootMenu.SetChildOf(this);

            RootMenu.MustRender += TriggerRender;

            Focused += OnFocused;
        }

        private void OnFocused(object _, FocusEventArgs ev)
        {
            ev.State.ActiveComponent = RootMenu;
            ev.Rerender = true;
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            var menuItems = new List<Component>(new [] { RootMenu });

            while (true)
            {
                var currentItem = menuItems.Last();

                if (currentItem is Menu menuItem)
                {
                    var nextItem = menuItem.ActiveItem;
                    menuItems.Add(nextItem);
                }
                else break;
            }

            // subtract one from menu width so that they overlap on the border line
            var totalWidth = menuItems.Sum(v => v is Menu menu ? menu.Width - 1 : 16);
            var scroll = Math.Min(0, Console.WindowWidth - totalWidth);

            var offset = scroll;
            for (var i = 0; i < menuItems.Count; i++)
            {
                var item = menuItems[i];
                
                // if the item is a menu item, use its width
                // otherwise, if it is the last item, fill the available space
                // otherwise default to a width of 16
                var width = item is Menu menu ? menu.Width :
                    i == menuItems.Count - 1 ? Console.WindowWidth - totalWidth + 16 : 16;

                item.Render(state, x + offset, y);
                offset += width;
            }
        }
    }
}