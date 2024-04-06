using System.Collections.Generic;

namespace Roulette.Scripts.Models
{
    public class Roulette
    {
        public readonly List<bool> Bullets; // reversed; the first bullet to shoot is tail
        public int Count => Bullets.Count;
        public int Damage { get; private set; }

        public Roulette(IEnumerable<bool> bullets)
        {
            Bullets = new List<bool>(bullets);
            ResetDamage();
        }

        public void IncreaseDamage()
        {
            ++Damage;
        }

        private void ResetDamage()
        {
            Damage = 1;
        }

        public int Fire()
        {
            int damage = Bullets[^1] ? Damage : 0;
            Bullets.RemoveAt(Bullets.Count - 1);
            ResetDamage();
            return damage;
        }
    }
}