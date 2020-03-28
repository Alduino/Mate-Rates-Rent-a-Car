using System;

namespace MRRC.Guacamole.Component
{
    public class Text : IComponent
    {
        public event EventHandler<char> KeyPressed;
        
        public string Contents { get; set; }
        
        public Text(string contents)
        {
            Contents = contents;
        }

        public void Render(int x, int y, bool active = true)
        {
            DrawUtil.Text(x, y, Contents);
        }

        public override string ToString() => Contents;
    }
}