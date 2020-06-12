using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace MRRC.Guacamole.Components
{
    public class Table<T> : Component
    {
        private readonly PropertyInfo[] _keys = typeof(T).GetProperties();
        
        public ObservableCollection<T> Items { get; }

        public Table(IEnumerable<T> items)
        {
            Items = new ObservableCollection<T>(items);
            Items.CollectionChanged += TriggerRender;
        }
        
        public Table() : this(new T[0]) {}
        
        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            var maxWidths = _keys.Select(key => 
                Items.Max(it => key.GetValue(it)?.ToString().Length ?? 4));

            var separatorBar = string.Join(" ", maxWidths.Select(w => '─'.Repeat(w + 2)));
            
            DrawUtil.Text(x, y, $"┌{separatorBar.Replace(' ', '┬')}┐");
            DrawUtil.Text(x, y + 1, 
                $"│{string.Join("│", _keys.Select(k => $" {k.Name} "))}│");
            DrawUtil.Text(x, y + 2, $"├{separatorBar.Replace(' ', '┼')}┤");
            
            DrawUtil.Lines(
                x, y + 3,
                Items.Select(item => string.Join("│",
                    _keys.Select(key => " " + (key.GetValue(item)?.ToString() ?? "null") + " ")
                    )
                )
            );
            
            DrawUtil.Text(x, y + _keys.Length + 3, $"└{separatorBar.Replace(' ', '┴')}┘");
        }
    }
}