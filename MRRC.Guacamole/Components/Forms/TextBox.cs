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
        public bool ReadOnly { get; set; }
        
        public string Placeholder { get; set; }
        
        public int MaxLength { get; set; }

        private readonly Timer _blinkTimer = new Timer(300);
        private bool _blinkOn;

        public TextBox(string label = "")
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
            Console.Write(_blinkOn && !ReadOnly ? '_' : ' ');
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
                    e.State.ActiveComponent = ParentComponent;
                    e.Rerender = true;
                    e.Cancel = false;
                    return;
                case ConsoleKey.Backspace:
                {
                    if (ReadOnly)
                    {
                        e.Rerender = true;
                        return;
                    }
                    
                    if (Value.Length <= 0) e.Rerender = true;
                    else
                    {
                        Value = Value.Substring(0, Value.Length - 1);
                        DrawCursor();
                    }

                    if (Placeholder.Length > 0 && Value.Length == 0) e.Rerender = true;
                    if (Value.Length > Width - 4) e.Rerender = true;
                    return;
                }
                case ConsoleKey.Tab:
                    // this should go to the parent so that it can handle tabbing through children
                    e.Cancel = false;
                    return;
                case ConsoleKey.Delete:
                    // this can move the cursor, so we need to trigger a render to move it back
                    e.Rerender = true;
                    e.Cancel = true;
                    return;
            }

            if (ReadOnly)
            {
                e.Rerender = true;
                return;
            }

            if (e.Key.KeyChar == 0)
            {
                return;
            }

            Value += e.Key.KeyChar;

            if (Value.Length == 1) e.Rerender = true;

            if (MaxLength > 0 && Value.Length > MaxLength)
            {
                Value = Value.Substring(0, MaxLength);
                e.Rerender = true;
            }

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
            if (!active) Console.ForegroundColor = ConsoleColor.DarkGray;
            else if (ReadOnly) Console.ForegroundColor = ConsoleColor.Gray;
            
            DrawUtil.Outline(x, y, Width, 3, Label);

            if (Value.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(Placeholder);
                Console.ResetColor();
            }
            else
            {
                Console.Write(Value.Substring(Math.Max(0, Value.Length - Width + 3)), Value.Length);
            }

            if (state.ActiveComponent == this) DrawCursor();
        }

        public override string ToString() => Label;
        
        public void SetValue(object val)
        {
            Value = (string) val;
            TriggerRender();
        }
    }
}