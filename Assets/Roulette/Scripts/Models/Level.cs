using System;
using Roulette.Scripts.Data;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    public class Level
    {
        private LevelConfig _config;
        public Player Player1 { get; }
        public Player Player2 { get; }

        public Player CurrentPlayer => Turn switch
        {
            PlayerIndex.P1 => Player1,
            PlayerIndex.P2 => Player2,
            _ => null,
        };

        private Roulette Roulette { get; set; }
        public int BulletCount => Roulette.Count;
        public PlayerIndex Turn { get; private set; } = PlayerIndex.None;
        public PlayerIndex Winner { get; private set; } = PlayerIndex.None;

        public Level(LevelConfig config)
        {
            _config = config;
            PlayerImpl player1 = new(PlayerIndex.P1, this, 2);
            PlayerImpl player2 = new(PlayerIndex.P2, this, 2);
            player1.Other = player2;
            player2.Other = player1;
            Player1 = player1;
            Player2 = player2;

            NewRound();
        }

        private void NewRound()
        {
            Roulette = new Roulette(new[] { false, false, true });
            Turn = PlayerIndex.P1;
        }

        private void OnTurnOver()
        {
            if (Player1.Health <= 0 || Player2.Health <= 0)
                OnLevelOver();
            else if (Roulette.Count == 0)
                NewRound();
            else
                Turn = Turn == PlayerIndex.P1 ? PlayerIndex.P2 : PlayerIndex.P1;
        }

        private void OnLevelOver()
        {
            Turn = PlayerIndex.None;
            Winner = Player1.Health <= 0 ? PlayerIndex.P2 : PlayerIndex.P1;
        }

        private class PlayerImpl : Player
        {
            private readonly PlayerIndex _index;
            private readonly Level _level;
            private int _health;
            public override int Health => _health;
            public PlayerImpl Other;

            public PlayerImpl(PlayerIndex index, Level level, int health)
            {
                _index = index;
                _level = level;
                _health = health;
            }

            public override void FireSelf()
            {
                if (!IsMyTurn()) return;
                int damage = _level.Roulette.Fire();
                if (damage == 0 && _level.BulletCount > 0) return; // extend this turn
                _health -= damage;
                _level.OnTurnOver();
            }

            public override void FireOther()
            {
                if (!IsMyTurn()) return;
                int damage = _level.Roulette.Fire();
                Other._health -= damage;
                _level.OnTurnOver();
            }

            public override void UseItem(int index)
            {
                if (!IsMyTurn()) return;
            }

            private bool IsMyTurn()
            {
                if (_level.Turn == _index) return true;
                Debug.LogWarning($"Current turn ({_level.Turn}) is not my turn ({_index})");
                return false;
            }
        }
    }
}