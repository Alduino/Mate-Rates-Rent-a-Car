using System;
using System.Linq;
using Pastel;

namespace MRRC.Guacamole.Components
{
    public class Menu : Component
    {
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
        public Component[] Items { get; }
        public int Width { get; }

        public Component ActiveItem => Items[HighlightIndex];
        
        public Menu(string name, Component[] items, int width = 32)
        {
            Name = name;
            Items = items;
            Width = width;
            
            foreach (var component in Items)
            {
                component.SetChildOf(this);
            }
            
            KeyPressed += OnKeyPressed;
            Focused += (_, ev) => ev.Cancel = false;
            HighlightIndex = 0;
        }

        private void OnKeyPressed(object sender, KeyPressEvent ev)
        {
            var key = ev.Key;
            if (ev.State.ActiveComponent != this) return;
            
            // we can move up and down or enter into the current child
            var shouldRender = true;
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    HighlightIndex = (HighlightIndex - 1).Mod(Items.Length);
                    break;
                case ConsoleKey.DownArrow:
                    HighlightIndex = (HighlightIndex + 1).Mod(Items.Length);
                    break;
                case ConsoleKey.RightArrow:
                    ev.State.ActiveComponent = ActiveItem;
                    break;
                case ConsoleKey.LeftArrow:
                    ev.State.ActiveComponent = Parent;
                    break;
                    
                default:
                    shouldRender = false;
                    break;
            }

            ev.Cancel = true;
            ev.Rerender = shouldRender;
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

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!active) Console.ForegroundColor = ConsoleColor.Gray;
            DrawUtil.Outline(x, y, Width, Console.WindowHeight - 1, Name);

            DrawUtil.Lines(x + 1, y + 1, 
                Items
                    .Select(item => item.ToString())
                    .Select(item => item.Length > Width - 4 ? 
                        item.Substring(0, Width - 5) + "…" : 
                        item)
                    .Select((v, i) => Highlight(v, i, active)));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}