using System;
using System.Threading.Tasks;

namespace MRRC.Guacamole.Components.Forms
{
    public class Button : Component
    {
        private bool _active;
        
        public string Text { get; }
        public TimeSpan ActiveTime { get; }

        public int Width => DrawUtil.MeasureText(Text).Width + 2;
        public int Height => DrawUtil.MeasureText(Text).Height + 2;

        public event EventHandler Activated;
        
        public Button(string text, TimeSpan activeTime)
        {
            Text = text;
            ActiveTime = activeTime;

            Focused += (_, ev) => ev.Cancel = false;
            KeyPressed += OnKeyPressed;
        }

        public Button(string text) : this(text, TimeSpan.FromMilliseconds(600))
        {
        }

        private async void Activate()
        {
            Activated?.Invoke(this, EventArgs.Empty);
            
            _active = true;
            TriggerRender();
            await Task.Delay(ActiveTime);
            _active = false;
            TriggerRender();
        }

        private void OnKeyPressed(object sender, KeyPressEvent e)
        {
            switch (e.Key.Key)
            {
                case ConsoleKey.Enter:
                    if (_active) break;
                    Activate();
                    break;
            }
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (state.ActiveComponent != this) Console.ForegroundColor = ConsoleColor.Gray;
            
            if (_active) Console.BackgroundColor = ConsoleColor.Gray;
            DrawUtil.Outline(x, y, Width, Height);
            DrawUtil.Text(x + 1, y + 1, Text);
            Console.ResetColor();
        }
    }
}