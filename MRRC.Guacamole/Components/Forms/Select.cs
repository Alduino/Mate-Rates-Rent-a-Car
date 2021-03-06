using System;
using System.Linq;

namespace MRRC.Guacamole.Components.Forms
{
    /// <summary>
    /// Selection input of items in an enumeration
    /// </summary>
    public class Select : Component, IInput<object>
    {
        private const string SelectOne = "Select one";

        // From https://stackoverflow.com/a/1082938
        private static int Mod(int x, int m) => (x % m + m) % m;
        
        private string[] _members;

        private bool _open;

        private int _selected = -1;

        public Select(string[] items)
        {
            _members = items;

            Focused += (_, ev) => ev.Cancel = false;
            KeyPressed += OnKeyPressed;
        }

        private string GetSelectedName() => _selected == -1 ? SelectOne : _members[_selected];

        private void Open()
        {
            _open = true;
            if (_selected == -1) _selected = 0;
        }

        private void Close()
        {
            _open = false;
            Value = GetSelectedName();
        }

        private void OnKeyPressed(object sender, KeyPressEvent e)
        {
            if (_open) e.Cancel = true;
            
            switch (e.Key.Key)
            {
                case ConsoleKey.Enter:
                    e.Cancel = true;
                    e.Rerender = true;
                    if (_open) Close();
                    else Open();
                    break;
                case ConsoleKey.UpArrow:
                    if (!_open) break;
                    _selected = Mod(_selected - 1, _members.Length);
                    e.Rerender = true;
                    break;
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow:
                    if (!_open) break;
                    _selected = Mod(_selected + 1, _members.Length);
                    e.Rerender = true;
                    break;
            }
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!active) Console.ForegroundColor = ConsoleColor.DarkGray;
            
            DrawUtil.Outline(x, y, Width, Height);
            DrawUtil.Text(x + 1, y + 1, GetSelectedName());
            
            Console.SetCursorPosition(x + Width - 2, y + 1);
            if (_open) Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write('*');
            
            Console.ResetColor();

            if (!_open) return;
            DrawUtil.Outline(x, y + 2, Width, _members.Length + 2);
            DrawUtil.Lines(x + 1, y + 3, _members);
                
            Console.SetCursorPosition(x + 1, y + _selected + 3);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write(GetSelectedName());
            
            Console.ResetColor();
        }

        public object Value { get; private set; }

        public int Width => _members.Append(SelectOne).Max(v => v.Length) + 4;
        public int Height => 3;
        
        public void SetValue(object val)
        {
            Value = val;
            _selected = Array.FindIndex(_members, m => m == val.ToString());
            TriggerRender();
        }

        /// <summary>
        /// Returns this instance with the value set
        /// </summary>
        public Select WithDefault(string value)
        {
            SetValue(value);
            return this;
        }

        public void SetNewMembers(string[] newMembers)
        {
            _members = newMembers;
            _selected = -1;
            _open = false;
            TriggerRender();
        }
    }
    
    /// <summary>
    /// Selection input of items in an enumeration
    /// </summary>
    public class Select<T> : Component, IInput<object> where T : Enum
    {
        private const string SelectOne = "Select one";

        // From https://stackoverflow.com/a/1082938
        private static int Mod(int x, int m) => (x % m + m) % m;
        
        private readonly Type _enum;
        private readonly string[] _members;

        private bool _open;

        private int _selected = -1;

        public Select()
        {
            _enum = typeof(T);
            _members = Enum.GetNames(_enum);

            Focused += (_, ev) => ev.Cancel = false;
            KeyPressed += OnKeyPressed;
        }

        private string GetSelectedName() => _selected == -1 ? SelectOne : _members[_selected];

        private void Open()
        {
            _open = true;
            if (_selected == -1) _selected = 0;
        }

        private void Close()
        {
            _open = false;
            Value = Enum.Parse(_enum, GetSelectedName());
        }

        private void OnKeyPressed(object sender, KeyPressEvent e)
        {
            if (_open) e.Cancel = true;
            
            switch (e.Key.Key)
            {
                case ConsoleKey.Enter:
                    e.Cancel = true;
                    e.Rerender = true;
                    if (_open) Close();
                    else Open();
                    break;
                case ConsoleKey.UpArrow:
                    if (!_open) break;
                    _selected = Mod(_selected - 1, _members.Length);
                    e.Rerender = true;
                    break;
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow:
                    if (!_open) break;
                    _selected = Mod(_selected + 1, _members.Length);
                    e.Rerender = true;
                    break;
            }
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            if (!active) Console.ForegroundColor = ConsoleColor.DarkGray;
            
            DrawUtil.Outline(x, y, Width, Height);
            DrawUtil.Text(x + 1, y + 1, GetSelectedName());
            
            Console.SetCursorPosition(x + Width - 2, y + 1);
            if (_open) Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write('*');
            
            Console.ResetColor();

            if (!_open) return;
            DrawUtil.Outline(x, y + 2, Width, _members.Length + 2);
            DrawUtil.Lines(x + 1, y + 3, _members);
                
            Console.SetCursorPosition(x + 1, y + _selected + 3);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write(GetSelectedName());
            
            Console.ResetColor();
        }

        public object Value { get; private set; }

        public int Width => _members.Append(SelectOne).Max(v => v.Length) + 4;
        public int Height => 3;
        
        public void SetValue(object val)
        {
            Value = val;
            _selected = Array.FindIndex(_members, m => m == val.ToString());
            TriggerRender();
        }

        /// <summary>
        /// Returns this instance with the value set
        /// </summary>
        public Select<T> WithDefault(T value)
        {
            SetValue(value);
            return this;
        }
    }
}