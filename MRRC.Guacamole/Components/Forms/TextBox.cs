using System;
using System.Timers;

namespace MRRC.Guacamole.Components.Forms
{
    /// <summary>
    /// A box that allows user input
    /// </summary>
    public class TextBox<TRenderHookState> : Component, IInput<string>
    {
        /// <summary>
        /// Hooks can be used on text boxes to do custom rendering
        /// </summary>
        public interface IContentsRenderer
        {
            /// <summary>
            /// Called whenever a key is pressed in the text box
            /// </summary>
            /// <param name="newText">The new text in the text box</param>
            /// <param name="state">Some state to pass to Render. Initialised as default</param>
            /// <returns>True to trigger the <see cref="Render"/> function</returns>
            bool RequiresRender(string newText, ref TRenderHookState state);
            
            /// <summary>
            /// Called to render new text. Should render text with the same length, and shouldn't move the cursor.
            /// </summary>
            /// <remarks>
            /// This method will be called on every render, even if it was not triggered by
            /// <see cref="RequiresRender"/>. You may wish to set a property on <see cref="TRenderHookState"/> if you only want
            /// to run this when RequiresRender has been called.
            /// </remarks>
            /// <param name="active">When this is true, the text box is highlighted</param>
            /// <param name="textBox">The text box that this is connected to</param>
            /// <param name="state">State returned from <see cref="RequiresRender"/></param>
            /// <param name="startOffset">The offset that the text rendering should start at to fit the input</param>
            void Render(bool active, TextBox<TRenderHookState> textBox, TRenderHookState state, int startOffset);
        }

        private class DefaultContentsRenderer : IContentsRenderer
        {
            public bool RequiresRender(string newText, ref TRenderHookState state) => false;

            public void Render(bool active, TextBox<TRenderHookState> textBox, TRenderHookState state, int startOffset)
            {
                Console.Write(
                    textBox.Value.Substring(startOffset), 
                    textBox.Value.Length);
            }
        }
        
        /// <summary>
        /// Text that is displayed above the input, or empty to not display it
        /// </summary>
        public string Label { get; }
        
        /// <summary>
        /// The current value of the input
        /// </summary>
        public string Value { get; private set; } = "";

        private int _width = 30;
        
        /// <summary>
        /// The width of the input
        /// </summary>
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
        
        /// <summary>
        /// When true, the input value cannot be modified
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Some value to display while the input is empty
        /// </summary>
        public string Placeholder { get; set; } = "";
        
        /// <summary>
        /// The maximum amount of characters allowed
        /// </summary>
        public int MaxLength { get; set; }

        private readonly Timer _blinkTimer = new Timer(300);
        private bool _blinkOn;

        private IContentsRenderer _contentsRenderer;
        private TRenderHookState _renderHookState;

        public TextBox(string label = "", IContentsRenderer contentsRenderer = null)
        {
            Label = label;

            _contentsRenderer = contentsRenderer ?? new DefaultContentsRenderer();

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
            if (ReadOnly) return;
            
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            var displayPlaceholder = Placeholder?.Length > 0 && Value.Length == 0;
            
            if (displayPlaceholder)
            {
                Console.SetCursorPosition(Console.CursorLeft - Math.Min(Width - 2, Placeholder.Length), 
                    Console.CursorTop);

                if (!_blinkOn) Console.ForegroundColor = ConsoleColor.Gray;
                else Console.ResetColor();
            }
            
            Console.Write(_blinkOn && !ReadOnly ? '_' : displayPlaceholder ? Placeholder[0] : ' ');
            // clear old line for backspace
            if (Value.Length < Width - 3)
            {
                if (displayPlaceholder) Console.ForegroundColor = ConsoleColor.Gray;
                else Console.ResetColor();
                
                Console.Write(displayPlaceholder && Placeholder?.Length > 1 ? Placeholder[1] : ' ');
            }
            
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        private void TriggerRenderHook(KeyPressEvent e)
        {
            if (_contentsRenderer.RequiresRender(Value, ref _renderHookState)) e.Rerender = true;
        }
        
        private void UpdateValue(object sender, KeyPressEvent e)
        {
            // prevent it from propagating up
            e.Cancel = true;
            
            // keep the cursor visible while typing
            _blinkOn = true;

            switch (e.Key.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    e.Cancel = false;
                    return;
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

                    TriggerRenderHook(e);
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

            TriggerRenderHook(e);
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
                var startOffset = Math.Max(0, Value.Length - Width + 3);
                Console.ResetColor();
                _contentsRenderer.Render(active, this, _renderHookState, startOffset);
                Console.ResetColor();
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

    /// <summary>
    /// Non-generic version of <see cref="TextBox{TRenderHookState}"/>, doesn't allow a render hook to be passed
    /// </summary>
    public class TextBox : TextBox<object>
    {
        public TextBox(string label = "") : base(label)
        {
        }
    }
}