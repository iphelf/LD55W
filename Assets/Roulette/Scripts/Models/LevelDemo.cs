using System;
using Roulette.Scripts.Data;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    public class LevelDemo
    {
        private readonly LevelConfig _config;
        public Player Player1 { get; }
        public Player Player2 { get; }

        public Player CurrentPlayer => Turn switch
        {
            PlayerIndex.P1 => Player1,
            PlayerIndex.P2 => Player2,
            _ => null,
        };

        public Roulette Roulette { get; set; }
        public int BulletCount => Roulette.Count;
        public PlayerIndex Turn { get; private set; } = PlayerIndex.None;
        public PlayerIndex Winner { get; private set; } = PlayerIndex.None;

        public LevelDemo(LevelConfig config)
        {
            _config = config;
            PlayerImpl player1 = new(PlayerIndex.P1, this, _config.initialHealth);
            PlayerImpl player2 = new(PlayerIndex.P2, this, _config.initialHealth);
            player1.Other = player2;
            player2.Other = player1;
            Player1 = player1;
            Player2 = player2;

            NewRound();
        }

        private void NewRound()
        {
            Roulette = new Roulette(_config.bullets);
            AssignItems(Player1);
            AssignItems(Player2);
            Turn = PlayerIndex.P1;
        }

        private void AssignItems(Player player)
        {
            int itemCount = Mathf.Min(_config.itemCountPerRound, _config.itemCapacity - player.Items.Count);
            for (int i = 0; i < _config.itemCapacity && itemCount >= 0; ++i)
            {
                if (player.Items.ContainsKey(i)) continue;
                player.Items.Add(i, _config.SampleItem());
                --itemCount;
            }
        }

        private void OnTurnOver()
        {
            if (Player1.Health <= 0 || Player2.Health <= 0)
                OnLevelOver();
            else if (Roulette.Count == 0)
                NewRound();
            else
            {
                Turn = Turn == PlayerIndex.P1 ? PlayerIndex.P2 : PlayerIndex.P1;
                if (CurrentPlayer.IsHandCuffed)
                {
                    (CurrentPlayer as PlayerImpl)?.SetHandCuffed(false);
                    OnTurnOver();
                }
            }
        }

        private void OnLevelOver()
        {
            Turn = PlayerIndex.None;
            Winner = Player1.Health <= 0 ? PlayerIndex.P2 : PlayerIndex.P1;
        }

        private class PlayerImpl : Player
        {
            private readonly PlayerIndex _index;
            private readonly LevelDemo _level;
            public PlayerImpl Other;

            public PlayerImpl(PlayerIndex index, LevelDemo level, int health)
            {
                _index = index;
                _level = level;
                Health = health;
            }

            public override void FireSelf()
            {
                if (!IsMyTurn()) return;
                int damage = _level.Roulette.Fire();
                if (damage == 0 && _level.BulletCount > 0) return; // extend this turn
                Health -= damage;
                _level.OnTurnOver();
            }

            public override void FireOther()
            {
                if (!IsMyTurn()) return;
                int damage = _level.Roulette.Fire();
                Other.Health -= damage;
                _level.OnTurnOver();
            }

            public override void UseItem(int index)
            {
                if (!IsMyTurn()) return;
                ItemType item = Items[index];
                switch (item)
                {
                    case ItemType.MagnifyingGlass:
                        CauseEffectOfMagnifyingGlass(_level.Roulette.Bullets[^1]);
                        break;
                    case ItemType.HandCuffs:
                        if (Other.IsHandCuffed) return;
                        Other.SetHandCuffed(true);
                        break;
                    default:
                        throw new NotImplementedException($"Item: {item}");
                }

                Items.Remove(index);
            }

            public void SetHandCuffed(bool isHandCuffed)
            {
                IsHandCuffed = isHandCuffed;
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