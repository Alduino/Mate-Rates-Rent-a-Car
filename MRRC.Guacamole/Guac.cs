using System;

namespace MRRC.Guacamole
{
    public class Guac
    {
        public Component Root { get; }

        private void Render()
        {
            Root.Render(0, 0);
        }
        
        public Guac(Component root)
        {
            Root = root;

            root.MustRender += delegate
            {
                Render();
            };

            Render();
        }

        /// <summary>
        /// Blocking function that should be called in a while loop.
        /// </summary>
        public void HandleEventLoop()
        {
            var character = Console.ReadKey();
            Root.HandleKeyPress(character);
        }
    }
}