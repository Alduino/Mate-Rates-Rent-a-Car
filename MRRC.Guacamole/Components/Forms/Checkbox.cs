using System;
using System.Threading.Tasks;

namespace MRRC.Guacamole.Components.Forms
{
    public class Checkbox : Component, IInput<object>
    {
        public Checkbox(bool def, TimeSpan activeTime)
        {
            Value = def;
            ActiveTime = activeTime;

            Focused += (_, e) => e.Cancel = false;
            KeyPressed += OnKeyPressed;
        }
        
        public Checkbox(TimeSpan activeTime) : this(false, activeTime) {}
        public Checkbox(bool def = false) : this(def, TimeSpan.FromMilliseconds(600)) {}

        private void OnKeyPressed(object sender, KeyPressEvent e)
        {
            if (e.Key.Key != ConsoleKey.Enter) return;
            Value = !(bool) Value;
            Console.Beep();
            e.Cancel = true;
            e.Rerender = true;
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!active) Console.ForegroundColor = ConsoleColor.DarkGray;
            DrawUtil.Outline(x, y, 5, 3);
            if ((bool) Value) Console.Write(" ✓");
            Console.ResetColor();
        }

        public object Value { get; private set; }
        public TimeSpan ActiveTime { get; }
        public int Width => 5;
        public int Height => 3;
        
        public void SetValue(object val)
        {
            Value = (bool) val;
        }
    }
}