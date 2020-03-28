using System;
using System.Linq;
using Pastel;

namespace MRRC.Guacamole.Components
{
    public class Menu<T> : Component
    {
        public event EventHandler<T> ItemEntered;

        private int _highlightIndex;
        
        public string Name { get; }
        public T[] Items { get; }
        public int Width { get; }

        public bool Open { get; private set; }

        public T ActiveItem => Items[_highlightIndex];
        
        public Menu(string name, T[] items, int width = 32)
        {
            Name = name;
            Items = items;
            Width = width;

            KeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, ConsoleKeyInfo key)
        {
            if (Open)
            {
                var requiresRerender = false;
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.Escape:
                        Open = false;
                        requiresRerender = true;
                        break;
                    
                    default:
                        if (ActiveItem is Component component) component.HandleKeyPress(sender, key);
                        break;
                }
                if (requiresRerender) TriggerRender();
            }
            else
            {
                var requiresRender = true;
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        _highlightIndex = (_highlightIndex - 1).Mod(Items.Length);
                        break;
                    case ConsoleKey.DownArrow:
                        _highlightIndex = (_highlightIndex + 1).Mod(Items.Length);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.Enter:
                        Open = true;
                        break;
                    
                    default:
                        requiresRender = false;
                        break;
                }
                if (requiresRender) TriggerRender();
            }
        }

        private string Highlight(string item, int index)
        {
            var paddedItem = $" {item} ";
            return index == _highlightIndex ? paddedItem.Pastel("#ffffff").PastelBg("#333333") : paddedItem;
        }

        public override void Render(int x, int y, bool active = true)
        {
            Console.CursorVisible = false;

            DrawUtil.Outline(x, y, Width, Console.WindowHeight - 1);
            DrawUtil.Lines(x + 1, y + 1, 
                Items
                    .Select(item => item.ToString())
                    .Select(item => item.Length > Width - 4 ? 
                        item.Substring(0, Width - 5) + "…" : 
                        item)
                    .Select(Highlight));
            
            Console.CursorVisible = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}