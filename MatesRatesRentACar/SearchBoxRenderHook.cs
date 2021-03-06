using System;
using System.Collections.Generic;
using System.Linq;
using MRRC.Guacamole.Components.Forms;
using MRRC.SearchParser;

namespace MateRatesRentACar
{
    /// <summary>
    /// Renders search box with syntax highlighting
    /// </summary>
    public class SearchBoxRenderer : TextBox<SearchBoxRenderer.State>.IContentsRenderer
    {
        private static readonly Dictionary<Token.Type, ConsoleColor> TokenColours = new Dictionary<Token.Type, ConsoleColor>
        {
            {Token.Type.And, ConsoleColor.Green},
            {Token.Type.Or, ConsoleColor.Green},
            {Token.Type.Not, ConsoleColor.Cyan},
            {Token.Type.Value, ConsoleColor.Gray},
            {Token.Type.OpenBracket, ConsoleColor.Blue},
            {Token.Type.CloseBracket, ConsoleColor.Blue},
            {Token.Type.Whitespace, ConsoleColor.Black},
            {Token.Type.Invalid, ConsoleColor.DarkRed},
            {Token.Type.NonTerminatedString, ConsoleColor.DarkRed}
        };
        
        /// <summary>
        /// Current state of the search box
        /// </summary>
        public readonly struct State
        {
            public State(string previousText)
            {
                PreviousText = previousText;
            }

            /// <summary>
            /// The text contents of the previous render
            /// </summary>
            public string PreviousText { get; }
        }

        /// <inheritdoc cref="TextBox{TRenderHookState}.IContentsRenderer.RequiresRender"/>
        public bool RequiresRender(string newText, ref State state)
        {
            var different = state.PreviousText != newText;
            state = new State(newText);
            return different;
        }

        /// <inheritdoc cref="TextBox{TRenderHookState}.IContentsRenderer.Render"/>
        public void Render(bool active, TextBox<State> textBox, State state, int startOffset)
        {
            var text = textBox.Value.ToUpperInvariant();
            var parser = new MrrcParser(text);

            var tokens = parser.Tokenise(true, false);

            var lastIndex = 0;
            foreach (var token in tokens)
            {
                var startedBefore = lastIndex <= startOffset;
                
                var source = text.Substring(lastIndex, token.Content.Length);
                lastIndex += token.Content.Length;

                if (lastIndex <= startOffset) continue;

                Console.ForegroundColor = TokenColours[token.Type];
                Console.Write(startedBefore ? source.Substring(startOffset - lastIndex + source.Length) : source);
            }
        }
    }
}