using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace MRRC.Guacamole.Components
{
    public class Table<T> : Component
    {
        private readonly PropertyInfo[] _keys = typeof(T).GetProperties();
        private bool _disableCollectionRenderTrigger;

        public event EventHandler PreRender;
        
        public ObservableCollection<T> Items { get; }
        
        public string Title { get; }

        public Table(string title, IEnumerable<T> items)
        {
            Title = title;
            Items = new ObservableCollection<T>(items);
            Items.CollectionChanged += (sender, args) =>
            {
                if (_disableCollectionRenderTrigger) return;
                TriggerRender(sender, args);
            };
        }
        
        public Table(string title) : this(title, new T[0]) {}
        
        protected override void Draw(int x, int y, bool active, ApplicationState state)
        {
            _disableCollectionRenderTrigger = true;
            PreRender?.Invoke(this, EventArgs.Empty);
            _disableCollectionRenderTrigger = false;
            
            var maxWidths = _keys.Select(key => 
                Items.Select(it => key.GetValue(it)?.ToString().Length ?? 4)
                    .Append(key.Name.Length)
                    .Max());

            var separatorBar = string.Join(" ", maxWidths.Select(w => '─'.Repeat(w + 2)));
            
            DrawUtil.Text(x, y, $"┌{separatorBar.Replace(' ', '┬')}┐");
            DrawUtil.Text(x, y + 1, 
                $"│{string.Join("│", _keys.Select(k => $" {k.Name} "))}│");
            DrawUtil.Text(x, y + 2, $"├{separatorBar.Replace(' ', '┼')}┤");

            DrawUtil.Lines(
                x, y + 3,
                Items.Select(item => 
                    "│" +
                    string.Join("│",
                    _keys.Select(key => " " + (key.GetValue(item)?.ToString() ?? "null") + " ")
                    ) +
                    "│"
                )
            );
            
            DrawUtil.Text(x, y + _keys.Length + 3, $"└{separatorBar.Replace(' ', '┴')}┘");
        }

        public override string ToString() => Title;
    }
}