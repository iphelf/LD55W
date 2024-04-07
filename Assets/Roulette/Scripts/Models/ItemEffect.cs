using System;

namespace Roulette.Scripts.Models
{
    public class MagnifyingGlassEventArgs : EventArgs
    {
        public bool IsReal;
    }

    public abstract class ItemEffect
    {
    }

    public class EffectOfMagnifyingGlass : ItemEffect
    {
        public bool IsReal;
        public EffectOfMagnifyingGlass(bool isReal) => IsReal = isReal;
    }

    public class EffectOfHandCuff : ItemEffect
    {
    }
}