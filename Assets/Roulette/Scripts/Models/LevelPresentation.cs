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
        private PlayerInput _placeholderP1;
        private PlayerInput _placeholderP2;

        private PlayerInput PlaceholderPlayer(PlayerIndex playerIndex)
            => playerIndex switch
            {
                PlayerIndex.P1 => _placeholderP1,
                PlayerIndex.P2 => _placeholderP2,
                _ => null,
            };

        public void InitializeByDriver(LevelInfo info)
        {
            Info = info;
            _placeholderP1 = new PlaceholderPlayerInput(info, PlayerIndex.P1);
            _placeholderP2 = new PlaceholderPlayerInput(info, PlayerIndex.P2);
        }

        public virtual async Awaitable PlayCeremonyOnLevelBegin()
            => await Dummy.PerformTask("关卡开场仪式");

        public virtual async Awaitable PlayCeremonyOnRoundBegin()
            => await Dummy.PerformTask("轮次开场仪式");

        public virtual async Awaitable PrepareBombsForNewRound(int count)
            => await Dummy.PerformTask("为新的轮次准备炸弹");

        public virtual async Awaitable DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
            => await Dummy.PerformTask("从牌库翻出一张牌");

        public virtual async Awaitable<int> PlaceCard(
            PlayerIndex playerIndex,
            SortedDictionary<int, ItemType> existingCards,
            ItemType newCard)
        {
            await Dummy.PerformTask("将翻出的牌置入手牌或丢弃");
            int position = await PlaceholderPlayer(playerIndex).PlaceCard(existingCards, newCard);
            return position;
        }

        public virtual async Awaitable PlayCeremonyOnTurnBegin(PlayerIndex playerIndex)
            => await Dummy.PerformTask("回合开场仪式");

        public virtual async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex, BulletQueue bulletQueue)
            => await Dummy.PerformTask("从队列中取出一个炸弹，移动到中央鉴定区");

        public virtual async Awaitable<PlayerAction> WaitForPlayerAction(
            PlayerIndex playerIndex, SortedDictionary<int, ItemType> items)
        {
            await Dummy.PerformTask("等待玩家行动");
            var playerAction = await PlaceholderPlayer(playerIndex).ProducePlayerAction(items);
            return playerAction;
        }

        public virtual async Awaitable ConsumeCardAndPlayEffect(
            PlayerIndex playerIndex, int itemIndex, ItemEffect itemEffect,
            Action onHit = null)
            => await Dummy.PerformTask("展示道具生效时的表现");

        public virtual async Awaitable PlayBombEffect(
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