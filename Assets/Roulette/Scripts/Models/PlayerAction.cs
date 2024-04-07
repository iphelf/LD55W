namespace Roulette.Scripts.Models
{
    public abstract class PlayerAction
    {
    }

    public class PlayerFiresGun : PlayerAction
    {
        public readonly PlayerIndex Target;
        public PlayerFiresGun(PlayerIndex target) => Target = target;
    }

    public class PlayerUsesItem : PlayerAction
    {
        public readonly int ItemIndex;
        public PlayerUsesItem(int itemIndex) => ItemIndex = itemIndex;
    }
}