using System;
using System.Collections.Generic;
using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    /// 关卡中各步骤、操作的呈现
    public abstract class LevelPresentation
    {
        public LevelInfo Info;
        private PlayerInput _player1Input;
        private PlayerInput _player2Input;

        private PlayerInput PlayerInput(PlayerIndex playerIndex)
            => playerIndex switch
            {
                PlayerIndex.P1 => _player1Input,
                PlayerIndex.P2 => _player2Input,
                _ => null,
            };

        public void InitializeByDriver(LevelInfo info)
        {
            Info = info;
            _player1Input ??= new PlaceholderPlayerInput(this, PlayerIndex.P2);
            _player2Input ??= new PlaceholderPlayerInput(this, PlayerIndex.P2);
        }

        public void BindPlayerInput(PlayerIndex playerIndex, PlayerInput playerInput)
        {
            switch (playerIndex)
            {
                case PlayerIndex.P1:
                    _player1Input = playerInput;
                    break;
                case PlayerIndex.P2:
                    _player2Input = playerInput;
                    break;
            }
        }

        public virtual async Awaitable PlayCeremonyOnLevelBegin()
            => await Dummy.PerformTask("关卡开场仪式");

        public virtual async Awaitable PlayCeremonyOnRoundBegin()
            => await Dummy.PerformTask("轮次开场仪式");

        public virtual async Awaitable PrepareBombsForNewRound(int count, int realCount)
            => await Dummy.PerformTask("为新的轮次准备炸弹");

        public virtual async Awaitable DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
            => await Dummy.PerformTask("从牌库翻出一张牌");

        public virtual async Awaitable AppendCard(
            PlayerIndex playerIndex, int existingCardCount, ItemType newCard)
            => await Dummy.PerformTask("将刚翻出的牌加入手牌尾部");

        public virtual async Awaitable RegretfullyDisposeLastDrawnCard(PlayerIndex playerIndex)
            => await Dummy.PerformTask("表示遗憾，并销毁刚翻出的牌");

        public virtual async Awaitable PlayCeremonyOnTurnBegin(PlayerIndex playerIndex)
            => await Dummy.PerformTask("回合开场仪式");

        public virtual async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex, BulletQueue bulletQueue)
            => await Dummy.PerformTask("从队列中取出一个炸弹，移动到中央鉴定区");

        public async Awaitable<PlayerAction> WaitForPlayerAction(
            PlayerIndex playerIndex, List<ItemType> items)
        {
            var playerAction = await PlayerInput(playerIndex).ProducePlayerAction(items);
            return playerAction;
        }

        public async Awaitable AcknowledgeItemEffect(
            PlayerIndex playerIndex, int itemIndex, ItemEffect itemEffect,
            Action onHit = null)
        {
            await ConsumeCardAndPlayEffect(playerIndex, itemIndex, itemEffect, onHit);
            PlayerInput(playerIndex).AcknowledgeItemEffect(itemEffect);
        }

        protected virtual async Awaitable ConsumeCardAndPlayEffect(
            PlayerIndex playerIndex, int itemIndex, ItemEffect itemEffect,
            Action onHit = null)
            => await Dummy.PerformTask("展示道具被使用并随后生效的表现");

        public async Awaitable AcknowledgeBombExplosion(
            PlayerIndex instigator, PlayerIndex target, bool isReal,
            Action onHit)
        {
            await PlayBombEffect(instigator, target, isReal, onHit);
            PlayerInput(instigator).AcknowledgeBombExplosion(isReal);
        }

        protected virtual async Awaitable PlayBombEffect(
            PlayerIndex instigator, PlayerIndex target, bool isReal,
            Action onHit)
        {
            await Dummy.PerformTask("展示炸弹爆炸前摇");
            onHit?.Invoke();
            Debug.Log("炸弹造成伤害");
            await Dummy.PerformTask("展示炸弹爆炸后摇");
        }

        public virtual async Awaitable PlayCeremonyOnTurnEnd(PlayerIndex playerIndex)
            => await Dummy.PerformTask("回合谢幕仪式");

        public virtual async Awaitable PlayCeremonyOnRoundEnd()
            => await Dummy.PerformTask("轮次谢幕仪式");

        public virtual async Awaitable PlayCeremonyOnLevelEnd(PlayerIndex winner)
            => await Dummy.PerformTask("关卡谢幕仪式");
    }
}