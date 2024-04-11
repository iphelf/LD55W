using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Roulette.Scripts.Data;
using Roulette.Scripts.Managers;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    /// 推动关卡流程的状态机
    public class LevelDriver
    {
        public static async Awaitable Drive(LevelConfig config, LevelPresentation presentation)
        {
            LevelDriver driver = new(config, presentation);
            await driver.NewLevel();
        }

        private readonly LevelConfig _config;
        private readonly LevelPresentation _presentation;
        private readonly LevelData _data;

        private LevelDriver(LevelConfig config, LevelPresentation presentation)
        {
            _config = config;
            _presentation = presentation;
            _data = new LevelData();

            _presentation.InitializeByDriver(new LevelInfoImpl(this));
        }

        private async Awaitable NewLevel()
        {
            _data.Player1 = new PlayerData { Health = _config.initialHealth };
            _data.Player2 = new PlayerData { Health = _config.initialHealth };
            _data.Step = new LevelStep();
            _data.Step.NewLevel(LevelManager.LevelIndex + 1);

            await _presentation.PlayCeremonyOnLevelBegin();

            do await NewRound();
            while (!IsLevelOver());

            await _presentation.PlayCeremonyOnLevelEnd(
                _data.Player2.Health <= 0 ? PlayerIndex.P1 : PlayerIndex.P2);
        }

        private static readonly PlayerIndex[] PlayerIndices = { PlayerIndex.P1, PlayerIndex.P2 };

        private PlayerData Player(PlayerIndex index) => index switch
        {
            PlayerIndex.P1 => _data.Player1,
            PlayerIndex.P2 => _data.Player2,
            _ => null,
        };

        private async Awaitable NewRound()
        {
            _data.Round = new RoundData();
            _data.Step.NewRound();

            await _presentation.PlayCeremonyOnRoundBegin();

            _data.Round.BulletQueue = new BulletQueue(_config.bullets);
            await _presentation.PrepareBombsForNewRound(
                _data.Round.BulletQueue.Count,
                _data.Round.BulletQueue.CountRealBullets()
            );

            var overflowHappened = new Dictionary<PlayerIndex, bool>
            {
                [PlayerIndex.P1] = false,
                [PlayerIndex.P2] = false,
            };
            for (int i = 0; i < _config.itemCountPerRound; ++i)
            {
                foreach (var playerIndex in PlayerIndices)
                {
                    if (overflowHappened[playerIndex]) continue;

                    ItemType card = _config.SampleItem();
                    await _presentation.DrawCardFromDeck(playerIndex, card);
                    var player = Player(playerIndex);
                    if (player.Items.Count >= _config.itemCapacity)
                    {
                        await _presentation.RegretfullyDisposeLastDrawnCard(playerIndex);
                        overflowHappened[playerIndex] = true;
                    }
                    else
                    {
                        await _presentation.AppendCard(playerIndex, player.Items.Count, card);
                        player.Items.Add(card);
                    }
                }
            }

            _data.Round.Turn = PlayerIndex.P1;
            do await NewTurn(_data.Round.Turn);
            while (!IsLevelOver() && !AreBombsExhausted());

            await _presentation.PlayCeremonyOnRoundEnd();
        }

        private async Awaitable NewTurn(PlayerIndex playerIndex)
        {
            _data.Step.NewTurn();

            await _presentation.PlayCeremonyOnTurnBegin(playerIndex);

            await _presentation.TakeBombForNewTurn(playerIndex, _data.Round.BulletQueue);

            bool turnFinished = false;
            while (!turnFinished)
            {
                turnFinished = await NewActionAndEndTurn(playerIndex);
                if (IsLevelOver()) return;
            }

            await _presentation.PlayCeremonyOnTurnEnd(playerIndex);
        }

        private async Awaitable<bool> NewActionAndEndTurn(PlayerIndex playerIndex)
        {
            var playerAction = await _presentation.WaitForPlayerAction(playerIndex, Player(playerIndex).Items);
            switch (playerAction)
            {
                case PlayerFiresGun playerFiresGun:
                    await HandleFiringGun(playerIndex, playerFiresGun);
                    return true;
                case PlayerUsesItem playerUsesItem:
                    await HandleUsingItem(playerIndex, playerUsesItem);
                    return false;
                default:
                    throw new NotImplementedException($"Action: {playerAction}");
            }
        }

        private async Awaitable HandleFiringGun(PlayerIndex playerIndex, PlayerFiresGun playerFiresGun)
        {
            bool isReal = _data.Round.BulletQueue.Peek;
            _data.Round.BulletQueue.Pop();
            await _presentation.AcknowledgeBombExplosion(playerIndex, playerFiresGun.Target, isReal, () =>
            {
                if (isReal)
                    --Player(playerFiresGun.Target).Health;
            });
            if (playerIndex != playerFiresGun.Target || isReal)
                _data.Round.Turn = playerIndex.Other();
        }

        private async Awaitable HandleUsingItem(PlayerIndex playerIndex, PlayerUsesItem playerUsesItem)
        {
            var player = Player(playerIndex);
            ItemType item = player.Items[playerUsesItem.ItemIndex];
            // LevelInfo.IsItemUsable is assumed
            ItemEffect effect;
            Action onHit = null;
            switch (item)
            {
                case ItemType.MagnifyingGlass:
                    effect = new EffectOfMagnifyingGlass(_data.Round.BulletQueue.Peek);
                    break;
                case ItemType.HandCuffs:
                    effect = new EffectOfHandCuff();
                    onHit = () => { Player(playerIndex.Other()).IsHandCuffed = true; };
                    break;
                default:
                    throw new NotImplementedException($"ItemType: {item}");
            }

            player.Items.RemoveAt(playerUsesItem.ItemIndex);
            await _presentation.AcknowledgeItemEffect(
                playerIndex, playerUsesItem.ItemIndex, effect, onHit);
        }

        private bool AreBombsExhausted()
        {
            return _data.Round.BulletQueue.Count == 0;
        }

        private bool IsLevelOver()
        {
            return _data.Player1.Health <= 0 || _data.Player2.Health <= 0;
        }

        private class LevelInfoImpl : LevelInfo
        {
            public override int Health(PlayerIndex playerIndex) => _driver.Player(playerIndex).Health;
            public override int CardCapacity => _driver._config.itemCapacity;

            public override ItemType Item(PlayerIndex playerIndex, int itemIndex)
            {
                var player = _driver.Player(playerIndex);
                if (0 <= itemIndex && itemIndex < player.Items.Count)
                    return player.Items[itemIndex];
                return ItemType.None;
            }

            public override ReadOnlyCollection<ItemType> Items(PlayerIndex playerIndex)
            {
                return _driver.Player(playerIndex).Items.AsReadOnly();
            }

            public override bool IsItemUsable(PlayerIndex playerIndex, ItemType item)
            {
                return item switch
                {
                    ItemType.MagnifyingGlass => true,
                    ItemType.HandCuffs => !_driver.Player(playerIndex.Other()).IsHandCuffed,
                    _ => false,
                };
            }

            public override int BulletCount => _driver._data.Round?.BulletQueue?.Count ?? 0;
            public override int CountRealBullets() => _driver._data.Round?.BulletQueue?.CountRealBullets() ?? 0;
            public override LevelStep LevelStep => _driver._data.Step;

            private readonly LevelDriver _driver;

            public LevelInfoImpl(LevelDriver driver)
            {
                _driver = driver;
            }
        }
    }
}