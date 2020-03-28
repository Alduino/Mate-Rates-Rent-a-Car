using System;
using System.Linq;
using Pastel;

namespace MRRC.Guacamole.Components
{
    public class Menu<T> : Component
    {
        public event EventHandler<T> ItemEntered;

        private int _highlightIndex;

        private int HighlightIndex
        {
            get => _highlightIndex;
            set
            {
                if (ActiveItem is Component component) component.MustRender -= TriggerRender;
                _highlightIndex = value;
                if (ActiveItem is Component newComponent) newComponent.MustRender += TriggerRender;
            }
        }

        public string Name { get; }
        public T[] Items { get; }
        public int Width { get; }

        public bool Open { get; private set; }

        public T ActiveItem => Items[HighlightIndex];
        
        public Menu(string name, T[] items, int width = 32)
        {
            Name = name;
            Items = items;
            Width = width;
            
            KeyPressed += OnKeyPressed;
            HighlightIndex = 0;
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
                        HighlightIndex = (HighlightIndex - 1).Mod(Items.Length);
                        break;
                    case ConsoleKey.DownArrow:
                        HighlightIndex = (HighlightIndex + 1).Mod(Items.Length);
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

        private string Highlight(string item, int index, bool active)
        {
            var noHighlightFg = active ? "#f5f5ff" : "#aaaacc";
            const string noHighlightBg = "#000000";

            const string highlightFg = "#000000";
            var highlightBg = active ? "#f5f5ff" : "#aaaacc";

            var current = index == HighlightIndex;

            var paddedItem = $"{(current ? '>' : ' ')}{item} ";
            return current ? 
                paddedItem.Pastel(highlightFg).PastelBg(highlightBg) : 
                paddedItem.Pastel(noHighlightFg).PastelBg(noHighlightBg);
        }

        public override void Render(int x, int y, bool active = true)
        {
            Console.CursorVisible = false;

            DrawUtil.Outline(x, y, Width, Console.WindowHeight - 1, Name);
            DrawUtil.Lines(x + 1, y + 1, 
                Items
                    .Select(item => item.ToString())
                    .Select(item => item.Length > Width - 4 ? 
                        item.Substring(0, Width - 5) + "…" : 
                        item)
                    .Select((v, i) => Highlight(v, i, active && !Open)));
            
            Console.CursorVisible = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}