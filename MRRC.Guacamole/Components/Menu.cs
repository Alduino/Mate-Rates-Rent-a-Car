using System;

namespace MRRC.Guacamole.Components
{
    /// <summary>
    /// Vertical list to select one of the specified items
    /// </summary>
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

        /// <summary>
        /// The currently active component
        /// </summary>
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
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow:
                    HighlightIndex = (HighlightIndex + 1).Mod(Items.Length);
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:
                    ev.State.ActiveComponent = ActiveItem;
                    break;
                case ConsoleKey.Backspace:
                case ConsoleKey.LeftArrow:
                    ev.State.ActiveComponent = ParentComponent;
                    break;
                    
                default:
                    shouldRender = false;
                    break;
            }

            ev.Cancel = true;
            ev.Rerender = shouldRender;
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (state.ActiveComponent != this) Console.ForegroundColor = ConsoleColor.DarkGray;
            DrawUtil.Outline(x, y, Width, Console.WindowHeight - 1, Name);

            for (var i = 0; i < Items.Length; i++)
            {
                var current = i == HighlightIndex;
                var item = Items[i];
                var str = item.ToString();
                if (str.Length > Width - 4) str = str.Substring(0, Width - 5) + '…';
                
                Console.SetCursorPosition(x + 1, y + i + 1);

                if (state.ActiveComponent == this)
                {
                    if (current)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Gray;
                    }
                }
                else
                {
                    if (current)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                }

                Console.Write((current ? '>' : ' ') + str);
                Console.ResetColor();
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}