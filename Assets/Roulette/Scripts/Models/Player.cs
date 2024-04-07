using System;
using System.Collections.Generic;

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

    public abstract class Player
    {
        public int Health { get; protected set; }
        public abstract void FireSelf();
        public abstract void FireOther();

        #region Items

        public readonly SortedDictionary<int, ItemType> Items = new();
        public abstract void UseItem(int index);

        public event EventHandler<MagnifyingGlassEventArgs> EffectOfMagnifyingGlass;

        protected void CauseEffectOfMagnifyingGlass(bool isReal)
        {
            EffectOfMagnifyingGlass?.Invoke(this, new MagnifyingGlassEventArgs() { IsReal = isReal });
        }

        public bool IsHandCuffed { get; protected set; }

        #endregion
    }
}