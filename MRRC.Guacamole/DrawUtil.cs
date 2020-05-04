using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole
{
    public static class DrawUtil
    {
        public readonly struct Size
        {
            public Size(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public int Width { get; }
            public int Height { get; }
        }
    
        public static string Repeat(this char character, int count)
        {
            return new string(character, count);
        }
        
        /// <summary>
        /// Draws an outline around the specified area. Cursor ends in top-left corner. Draws outline inside bounds.
        /// </summary>
        /// <param name="x">Left offset</param>
        /// <param name="y">Top offset</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="title">Title to display at the top of the outline</param>
        public static void Outline(int x, int y, int w, int h, string title = "")
        {
            Console.SetCursorPosition(x, y);

            var widthLine = '─'.Repeat(w - 2);
            var widthSpace = ' '.Repeat(w - 2);
            
            // draw top line
            Console.Write('┌' + widthLine + '┐');

            // edges
            for (var i = 0; i < h - 2; i++)
            {
                Console.SetCursorPosition(x, y + i + 1);
                Console.Write('│' + widthSpace + '│');
            }
            
            // bottom line
            Console.SetCursorPosition(x, y + h - 1);
            Console.WriteLine('└' + widthLine + '┘');
            
            // draw the title
            if (title.Length > 0)
            {
                Console.SetCursorPosition(x + 1, y);
                Console.Write(" ");
                Console.Write(title.Length > w - 4 ? title.Substring(0, w - 5) + "…" : title);
                Console.Write(" ");
            }

            // mov the cursor to x,y to make sure it doesn't scroll
            Console.SetCursorPosition(x, y);
            
            // move to inside the box
            Console.SetCursorPosition(x + 1, y + 1);
        }

        /// <summary>
        /// Draws text lines at the specified position. Don't include newlines in the strings.
        /// </summary>
        /// <remarks>
        /// Normally you could just use <see cref="Console.WriteLine(string)"/>, however this will not work when
        /// needing to reset to a different x position.
        /// If you have just a string, instead of splitting it up into lines use <see cref="Text"/>.
        /// </remarks>
        /// <param name="x">Left offset</param>
        /// <param name="y">Top offset</param>
        /// <param name="lines">List of text lines</param>
        public static void Lines(int x, int y, IEnumerable<string> lines)
        {
            var linesArr = lines.ToArray();

            for (var i = 0; i < linesArr.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(linesArr[i]);
            }
        }

        /// <summary>
        /// Same as <see cref="Lines"/>, however this automatically splits the string into lines for you.
        /// </summary>
        public static void Text(int x, int y, string text)
        {
            var lines = text.Split('\n');
            Lines(x, y, lines);
        }

        /// <summary>
        /// Returns the width and height of the supplied text
        /// </summary>
        public static Size MeasureText(string text)
        {
            var lines = text.Split('\n');
            var maxWidth = lines.Max(line => line.Length);
            return new Size(maxWidth, lines.Length);
        }
    }
}