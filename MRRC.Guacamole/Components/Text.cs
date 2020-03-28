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

        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            DrawUtil.Text(x, y, Contents);
        }

        public override string ToString() => Contents;
    }
}