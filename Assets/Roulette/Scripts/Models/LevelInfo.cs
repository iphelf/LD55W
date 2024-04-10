using System;
using System.Collections.ObjectModel;

namespace Roulette.Scripts.Models
{
    // 获取关卡中的各种有用信息
    public abstract class LevelInfo
    {
        public abstract int Health(PlayerIndex playerIndex);
        public abstract int CardCapacity { get; }
        public abstract ItemType Item(PlayerIndex playerIndex, int itemIndex);
        public abstract ReadOnlyCollection<ItemType> Items(PlayerIndex playerIndex);
        public abstract bool IsItemUsable(PlayerIndex playerIndex, ItemType item);
        public abstract int BulletCount { get; }
        public abstract int CountRealBullets();
        public abstract LevelStep LevelStep { get; }
    }
}