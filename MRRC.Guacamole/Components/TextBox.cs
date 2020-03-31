using System;

namespace MRRC.Guacamole.Components
{
    public class TextBox : Component
    {
        public string Label { get; }
        public string Value { get; private set; }

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
            if (e.Key.Key == ConsoleKey.Backspace && Value.Length > 0)
            {
                Value = Value.Substring(0, Value.Length - 1);
            }
            else
            {
                Value += e.Key.KeyChar;
            }

            TriggerRender();
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            DrawUtil.Outline(x, y, Width, 3, Label);
            Console.Write(Value);
        }

        public override string ToString() => Label;
    }
}