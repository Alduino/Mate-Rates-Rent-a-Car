using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole.Components.Forms
{
    /// <summary>
    /// Form that holds any amount of inputs and a submit button
    /// </summary>
    public class Form : Component
    {
        /// <summary>
        /// Represents one of the input fields
        /// </summary>
        public readonly struct Item
        {
            public Item(string title, IInput<object> component)
            {
                Title = title;
                Component = component;
            }

            public string Title { get; }
            public IInput<object> Component { get; }
        }

        /// <summary>
        /// Allows access of data in the form
        /// </summary>
        public class FormData
        {
            private readonly Item[] _items;

            public FormData(Item[] items)
            {
                _items = items;
            }

            /// <summary>
            /// Returns the value of the field with the specified title
            /// </summary>
            public T Get<T>(string title)
            {
                var el = _items.First(v => v.Title == title);
                return (T) el.Component.Value;
            }

            /// <summary>
            /// Attempts to output the value of the field with the specified title to <see cref="result"/> 
            /// </summary>
            /// <returns>True when successful, false if the field doesn't exist or the value is null</returns>
            public bool TryGet<T>(string title, out T result)
            {
                var has = _items.Any(v => v.Title == title);
                if (!has)
                {
                    result = default;
                    return false;
                }
                
                var el = _items.First(v => v.Title == title);
                var res = el.Component.Value;

                if (res == null)
                {
                    result = default;
                    return false;
                }

                result = (T) res;
                
                return true;
            }
        }

        /// <summary>
        /// Event args for when the form is submitted
        /// </summary>
        public class SubmittedEventArgs : EventArgs
        {
            public SubmittedEventArgs(FormData data)
            {
                Data = data;
            }

            public FormData Data { get; }
            
            /// <summary>
            /// Message displayed beside the submit button
            /// </summary>
            public string Result { get; set; } = "";
        }
        
        private readonly string _title;
        private readonly Item[] _items;
        private readonly Button _submit;

        private int _tabIndex;
        private string _result = "";

        /// <summary>
        /// Triggered when the form is submitted
        /// </summary>
        public event EventHandler<SubmittedEventArgs> Submitted;

        protected override IEnumerable<IComponent> Children => 
            _items
                .Select(v => (IComponent) v.Component)
                .Append(_submit);

        public Form(string title, IEnumerable<Item> items, Button submit)
        {
            _title = title;
            _items = items.ToArray();
            _submit = submit;

            foreach (var item in _items)
            {
                item.Component.SetChildOf(this);
                item.Component.MustRender += TriggerRender;
            }

            _submit.SetChildOf(this);
            _submit.MustRender += TriggerRender;

            _submit.Activated += (sender, _) =>
            {
                var ev = new SubmittedEventArgs(new FormData(_items));
                Submitted?.Invoke(sender, ev);
                _result = ev.Result;
            };
            
            KeyPressed += OnKeyPressed;
            Focused += OnFocused;
        }

        private void SetFocus(ApplicationState state)
        {
            var component = _tabIndex >= _items.Length ? (IComponent) _submit : _items[_tabIndex].Component;
            state.ActiveComponent = component;
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            _tabIndex = -1;
            e.Cancel = false;
            e.Rerender = true;
        }

        private void OnKeyPressed(object sender, KeyPressEvent e)
        {
            switch (e.Key.Key)
            {
                case ConsoleKey.LeftArrow:
                    e.State.ActiveComponent = Parent;
                    e.Rerender = true;
                    e.Cancel = true;
                    break;
                case ConsoleKey.Enter:
                    _tabIndex = _items.Length;
                    SetFocus(e.State);
                    break;
                case ConsoleKey.Tab:
                    _tabIndex = (_tabIndex + 1) % (_items.Length + 1);
                    SetFocus(e.State);
                    e.Rerender = true;
                    e.Cancel = true;
                    break;
            }
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            var maxTextWidth = _items.Max(v => v.Title.Length);
            var maxComponentWidth = _items.Max(v => v.Component.Width);

            var width = Math.Max(maxTextWidth + maxComponentWidth, _submit.Width) + 3;
            var height = _items.Select(v => v.Component.Height).Append(_submit.Height).Sum() + 2;

            if (!active) Console.ForegroundColor = ConsoleColor.DarkGray;
            DrawUtil.Outline(x, y, width, height, _title);

            var yOffset = 0;
            foreach (var item in _items)
            {
                var textSize = DrawUtil.MeasureText(item.Title);

                if (active) Console.ResetColor();
                else Console.ForegroundColor = ConsoleColor.DarkGray;
                DrawUtil.Text(x + 1, y + yOffset + item.Component.Height / 2 + 1, $"{item.Title}:");
                item.Component.Render(state, x + maxTextWidth + 2, y + yOffset + 1);

                yOffset += Math.Max(textSize.Height, item.Component.Height);
            }
            
            _submit.Render(state, x + 1, yOffset + 1);
            
            DrawUtil.Text(x + maxTextWidth + 2, yOffset + _submit.Height / 2 + 1, _result);
        }

        /// <summary>
        /// Sets the value of the specified field
        /// </summary>
        public void Set(string title, object value)
        {
            var item = _items.FirstOrDefault(v => v.Title == title);
            item.Component?.SetValue(value);
        }

        public override string ToString() => _title;
    }
}