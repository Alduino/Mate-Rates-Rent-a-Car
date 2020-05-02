using System;
using System.Timers;

namespace MRRC.Guacamole.Components.Forms
{
    public class TextBox : Component, IInput<string>
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

        public int Height => 3;

        private Timer _blinkTimer = new Timer(300);
        private bool _blinkOn;

        public TextBox(string label)
        {
            Label = label;

            Focused += (_, ev) => ev.Cancel = false;
            KeyPressed += UpdateValue;

            Focused += delegate { _blinkTimer.Enabled = true; };
            Blurred += delegate { _blinkTimer.Enabled = false; };

            _blinkTimer.Elapsed += delegate
            {
                _blinkOn = !_blinkOn;
                DrawCursor();
            };
        }

        private void DrawCursor()
        {
            var (cursorLeft, cursorTop) = (Console.CursorLeft, Console.CursorTop);
            Console.Write(_blinkOn ? '_' : ' ');
            // clear old line for backspace
            if (Value.Length < Width - 3) Console.Write(' ');
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
        
        private void UpdateValue(object sender, KeyPressEvent e)
        {
            // prevent it from propagating up
            e.Cancel = true;
            
            // keep the cursor visible while typing
            _blinkOn = true;
            
            switch (e.Key.Key)
            {
                case ConsoleKey.Enter:
                    // Exit out of focus to the parent component
                    e.State.ActiveComponent = Parent;
                    e.Rerender = true;
                    return;
                case ConsoleKey.Backspace:
                {
                    if (Value.Length <= 0) e.Rerender = true;
                    else
                    {
                        Value = Value.Substring(0, Value.Length - 1);
                        DrawCursor();
                    }
                    
                    if (Value.Length > Width - 4) e.Rerender = true;
                    return;
                }
            }

            if (e.Key.KeyChar == 0)
            {
                return;
            }

            Value += e.Key.KeyChar;

            if (Value.Length > Width - 3)
            {
                e.Rerender = true;
            }
            else
            {
                DrawCursor();
            }
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (state.ActiveComponent != this) Console.ForegroundColor = ConsoleColor.Gray;
            
            DrawUtil.Outline(x, y, Width, 3, Label);
            Console.Write(Value.Substring(Math.Max(0, Value.Length - Width + 3)), Value.Length);

            if (state.ActiveComponent == this) DrawCursor();
        }

        public override string ToString() => Label;
    }
}