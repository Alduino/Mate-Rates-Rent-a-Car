using System;
using System.Linq;

namespace MRRC.Guacamole.Component
{
    public class Menu<T> : IComponent
    {
        public event EventHandler<ConsoleKeyInfo> KeyPressed;
        public event EventHandler<T> ItemEntered;

        private int _highlightIndex;
        
        public string Name { get; }
        public T[] Items { get; }
        public int Width { get; }

        public T ActiveItem => Items[_highlightIndex];
        
        public Menu(string name, T[] items, int width = 32)
        {
            Name = name;
            Items = items;
            Width = width;
        }

        public void Render(int x, int y, bool active = true)
        {
            Console.CursorVisible = false;

            DrawUtil.Outline(x, y, Width, Console.WindowHeight);
            DrawUtil.Lines(x + 2, y, 
                Items
                    .Select(item => item.ToString())
                    .Select(item => item.Length > Width - 4 ? 
                        item.Substring(0, Width - 5) + "…" : 
                        item));
            
            Console.CursorVisible = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}