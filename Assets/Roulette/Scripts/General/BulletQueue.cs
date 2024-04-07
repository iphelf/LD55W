using System.Collections.Generic;

namespace Roulette.Scripts.General
{
    public class BulletQueue
    {
        private readonly List<bool> _list;

        public BulletQueue(IEnumerable<bool> items)
        {
            _list = new List<bool>(items);
            _list.Reverse();
        }

        public bool Peek => _list[^1];

        public void Pop()
        {
            _list.RemoveAt(_list.Count - 1);
        }

        public int Count => _list.Count;
    }
}