using System.Collections.Generic;

namespace Roulette.Scripts.Models
{
    public class Roulette
    {
        private readonly List<bool> _bullets; // reversed; the first bullet to shoot is tail
        public int Count => _bullets.Count;
        public int Damage { get; private set; }

        public Roulette(IEnumerable<bool> bullets)
        {
            _bullets = new List<bool>(bullets);
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
            int damage = _bullets[^1] ? Damage : 0;
            _bullets.RemoveAt(_bullets.Count - 1);
            ResetDamage();
            return damage;
        }
    }
}