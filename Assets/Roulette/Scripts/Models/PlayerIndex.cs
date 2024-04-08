namespace Roulette.Scripts.Models
{
    public enum PlayerIndex
    {
        None = 0,
        P1 = 1,
        P2 = 2,
    }

    public static class PlayerIndices
    {
        public static PlayerIndex Other(this PlayerIndex index) => index switch
        {
            PlayerIndex.P1 => PlayerIndex.P2,
            PlayerIndex.P2 => PlayerIndex.P1,
            _ => PlayerIndex.None,
        };
    }
}