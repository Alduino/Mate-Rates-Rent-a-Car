using System;
using System.Threading.Tasks;

namespace MRRC.Guacamole.Components.Forms
{
    /// <summary>
    /// Allows triggering some action on press
    /// </summary>
    public class Button : Component
    {
        private bool _active;
        
        /// <summary>
        /// The text to be displayed on the button
        /// </summary>
        public string Text { get; }
        
        /// <summary>
        /// How long the button will be held down for
        /// </summary>
        public TimeSpan ActiveTime { get; }

        public int Width => DrawUtil.MeasureText(Text).Width + 2;
        public int Height => DrawUtil.MeasureText(Text).Height + 2;

        /// <summary>
        /// Triggered when the button is activated
        /// </summary>
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
            // Get out of this event so this can happen separately
            await Task.Delay(1);
            
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
                    e.Cancel = true;
                    if (_active) break;
                    Activate();
                    break;
                
                default:
                    // Take control while the button is pressed
                    e.Cancel = _active;
                    break;
            }
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!active) Console.ForegroundColor = ConsoleColor.DarkGray;
            
            if (_active) Console.BackgroundColor = ConsoleColor.DarkGray;
            DrawUtil.Outline(x, y, Width, Height);
            DrawUtil.Text(x + 1, y + 1, Text);
            Console.ResetColor();
        }

        public override string ToString() => Text;
    }
}