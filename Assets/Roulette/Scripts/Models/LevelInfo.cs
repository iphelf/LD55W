namespace Roulette.Scripts.Models
{
    // 获取关卡中的各种有用信息
    public abstract class LevelInfo
    {
        public abstract int CardCapacity { get; }
        public abstract bool IsItemUsable(PlayerIndex playerIndex, ItemType item);
    }
}