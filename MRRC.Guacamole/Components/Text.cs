using System;

namespace MRRC.Guacamole.Components
{
    public class Text : Component
    {
        private string _contents;
        public string Contents
        {
            get => _contents;
            set
            {
                _contents = value;
                TriggerRender();
            }
        }

        public Text(string contents)
        {
            Contents = contents;
        }

        public override void Render(int x, int y, bool active = true)
        {
            DrawUtil.Text(x, y, Contents);
        }

        public override string ToString() => Contents;
    }
}