using System;
using System.Collections.Generic;

namespace MRRC.Guacamole.Components
{
    /// <summary>
    /// Displays a dialogue in the centre of the window
    /// </summary>
    public class Dialogue : Component
    {
        private int _highlightIndex;
        
        public string Title { get; }
        public string Contents { get; }
        public string[] Buttons { get; }

        /// <summary>
        /// Triggered when one of the buttons is pressed
        /// </summary>
        public event EventHandler<string> ButtonPressed;
        
        public Dialogue(string title, string contents, string[] buttons)
        {
            Title = title;
            Contents = contents;
            Buttons = buttons;

            Focused += (_, ev) => ev.Cancel = false;

            KeyPressed += (_, ev) =>
            {
                ev.Rerender = true;
                
                switch (ev.Key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        _highlightIndex = (_highlightIndex - 1).Mod(Buttons.Length);
                        break;
                    case ConsoleKey.RightArrow:
                        _highlightIndex = (_highlightIndex + 1).Mod(Buttons.Length);
                        break;
                    case ConsoleKey.Enter:
                        ButtonPressed?.Invoke(this, Buttons[_highlightIndex]);
                        break;
                    
                    default:
                        ev.Rerender = false;
                        break;
                }
            };
        }

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            // disregard x and y
            // this component draws always in the middle of the window

            var width = Math.Max(Title.Length, Contents.Length % Math.Max(Title.Length, 80)) + 2;

            var contentsLines = new List<string>();
            var lastLineEnd = 0;
            for (var i = 0; i < Contents.Length; i++)
            {
                var character = Contents[i];
                if (character != ' ' || Contents.IndexOf(' ', i + 1) < width) continue;
                contentsLines.Add(Contents.Substring(lastLineEnd, i));
                lastLineEnd = i + 1;
            }

            // when this is still 0 that means we didn't find any spaces to split the lines at
            if (lastLineEnd == 0) contentsLines.Add(Contents);
            
            var height = 3 + contentsLines.Count;

            var left = (Console.WindowWidth - width) / 2;
            var top = (Console.WindowHeight - height) / 2;

            Console.ResetColor();
            DrawUtil.Outline(left, top, width, height, Title);
            DrawUtil.Lines(left + 1, top + 1, contentsLines);

            Console.SetCursorPosition(left + 1, top + height - 2);
            for (var i = 0; i < Buttons.Length; i++)
            {
                var button = Buttons[i];

                if (i == _highlightIndex) Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write(button);
                if (i != Buttons.Length - 1) Console.Write(" ");
            }
        }

        public override string ToString() => Title;
    }
}