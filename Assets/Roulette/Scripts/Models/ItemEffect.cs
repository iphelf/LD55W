using System;

namespace Roulette.Scripts.Models
{
    public abstract class ItemEffect
    {
    }

    public class EffectOfMagnifyingGlass : ItemEffect
    {
        public readonly bool IsReal;
        public EffectOfMagnifyingGlass(bool isReal) => IsReal = isReal;
    }

    public class EffectOfHandCuff : ItemEffect
    {
    }
}