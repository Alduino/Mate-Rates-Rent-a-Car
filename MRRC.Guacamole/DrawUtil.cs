using System;

namespace MRRC.Guacamole
{
    public static class DrawUtil
    {
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
        public static void Outline(int x, int y, int w, int h)
        {
            Console.SetCursorPosition(x, y);

            var widthLine = '─'.Repeat(w - 2);
            var widthSpace = ' '.Repeat(w - 2);
            
            // draw top line
            Console.Write('╭' + widthLine + '╮');

            // edges
            for (var i = 0; i < h - 2; i++)
            {
                Console.SetCursorPosition(x, y + i + 1);
                Console.Write('│' + widthSpace + '│');
            }
            
            // bottom line
            Console.SetCursorPosition(x, y + h - 1);
            Console.WriteLine('╰' + widthLine + '╯');
            
            // mov the cursor to x,y to make sure it doesn't scroll
            Console.SetCursorPosition(x, y);
            
            // move to inside the box
            Console.SetCursorPosition(x + 1, y + 1);
        }
    }
}