using System;
using System.Collections.Generic;
using System.Linq;
using Pastel;

namespace MRRC.Guacamole.Components
{
    public class Dialogue : Component
    {
        private int _highlightIndex;
        
        public string Title { get; }
        public string Contents { get; }
        public string[] Buttons { get; }

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
            
            var height = 4 + contentsLines.Count;

            var left = (Console.WindowWidth - width) / 2;
            var top = (Console.WindowHeight - height) / 2;

            Console.ResetColor();
            DrawUtil.Outline(left, top, width, height);
            DrawUtil.Text(left + 1, top + 1, Title);
            DrawUtil.Lines(left + 1, top + 2, contentsLines);

            DrawUtil.Text(left + 1, top + height - 2, string.Join(" ", 
                Buttons.Select((text, i) => i == _highlightIndex ? text.Pastel("#45f98a") : text)));
        }

        public override string ToString() => Title;
    }
}