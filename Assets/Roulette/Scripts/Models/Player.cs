using System.Collections.Generic;

namespace Roulette.Scripts.Models
{
    public enum PlayerIndex
    {
        None = 0,
        P1 = 1,
        P2 = 2,
    }

    public abstract class Player
    {
        public abstract int Health { get; }
        public abstract void FireSelf();
        public abstract void FireOther();
        public SortedDictionary<int, ItemType> Items = new();
        public abstract void UseItem(int index);
    }
}