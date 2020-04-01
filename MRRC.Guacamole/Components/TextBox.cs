using System;

namespace MRRC.Guacamole.Components
{
    public class TextBox : Component
    {
        public string Label { get; }
        public string Value { get; private set; } = "";

        private int _width = 30;
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                TriggerRender();
            }
        }

        public TextBox(string label)
        {
            Label = label;

            Focused += (_, ev) => ev.Cancel = false;
            KeyPressed += UpdateValue;
        }

        private void UpdateValue(object sender, KeyPressEvent e)
        {
            e.Cancel = true;
            
            switch (e.Key.Key)
            {
                case ConsoleKey.LeftArrow when Value.Length == 0:
                    // Exit out of focus to the parent component
                    e.State.ActiveComponent = Parent;
                    e.Rerender = true;
                    return;
                case ConsoleKey.Backspace:
                {
                    if (Value.Length <= 0) e.Rerender = true;
                    else Value = Value.Substring(0, Value.Length - 1);
                    if (Value.Length > Width - 4) e.Rerender = true;
                    return;
                }
                case ConsoleKey.Enter:
                    e.Rerender = true;
                    return;
            }

            if (e.Key.KeyChar == 0)
            {
                return;
            }
            
            Value += e.Key.KeyChar;

            if (Value.Length > Width - 3) e.Rerender = true;
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            DrawUtil.Outline(x, y, Width, 3, Label);
            Console.Write(Value.Substring(Math.Max(0, Value.Length - Width + 3)), Value.Length);
        }

        public override string ToString() => Label;
    }
}