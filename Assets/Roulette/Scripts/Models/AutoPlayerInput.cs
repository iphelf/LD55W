using System.Collections.Generic;
using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    public class PlaceholderPlayerInput : PlayerInput
    {
        public PlaceholderPlayerInput(LevelPresentation presentation, PlayerIndex playerIndex)
            : base(presentation, playerIndex)
        {
        }

        public override async Awaitable<PlayerAction> ProducePlayerAction(List<ItemType> items)
        {
            await Dummy.NothingButAwaitable();
            return new PlayerFiresGun(PlayerIndex.Other());
        }

        public static async Awaitable<int> PlaceCard(
            SortedDictionary<int, ItemType> existingCards, int cardCapacity)
        {
            await Dummy.NothingButAwaitable();
            for (int i = 0; i < cardCapacity; ++i)
                if (!existingCards.ContainsKey(i))
                    return i;
            return -1;
        }
    }

    public class BasicAIPlayerInput : PlayerInput
    {
        private LevelStep _stepOfLastAction;
        private PlayerAction _lastAction;
        private ItemType _lastUsedItem;
        private ItemEffect _lastItemEffect;

        public BasicAIPlayerInput(LevelPresentation presentation, PlayerIndex playerIndex)
            : base(presentation, playerIndex)
        {
        }

        private async Awaitable PretendToBeThinking()
        {
            await Awaitable.WaitForSecondsAsync(Random.Range(2.0f, 4.0f));
        }

        /**
         * AI选择策略： <br/>
         *  <br/>
         * 当持有手铐时，先使用手铐 <br/>
         *  <br/>
         * 1. 没有放大镜的情况下： <br/>
         *  实弹数/总弹数 >= 50%，选择攻击玩家 <br/>
         *  实弹数/总弹数 &lt; 50%，选择攻击自己 <br/>
         *  <br/>
         * 2. 有放大镜的情况下： <br/>
         *  实弹数/总弹数 >= 75%，不使用放大镜，选择攻击玩家 <br/>
         *  实弹数/总弹数 &lt; 75%，选择使用放大镜，如果为真攻击玩家，如果为假攻击自己 <br/>
         */
        public override async Awaitable<PlayerAction> ProducePlayerAction(List<ItemType> items)
        {
            await PretendToBeThinking();

            int indexOfHandcuffs = -1;
            int indexOfMagnifyingGlass = -1;
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i] == ItemType.HandCuffs && indexOfHandcuffs == -1)
                    indexOfHandcuffs = i;
                if (items[i] == ItemType.MagnifyingGlass && indexOfMagnifyingGlass == -1)
                    indexOfMagnifyingGlass = i;
            }

            PlayerAction action;

            // 如果同回合内 存在上一手，且上一手用了放大镜
            if (_stepOfLastAction == Info.LevelStep
                && _lastAction is PlayerUsesItem
                && _lastUsedItem == ItemType.MagnifyingGlass
                && _lastItemEffect is EffectOfMagnifyingGlass effectOfMagnifyingGlass)
                // 如果为真攻击玩家，如果为假攻击自己
                action = new PlayerFiresGun(
                    effectOfMagnifyingGlass.IsReal ? PlayerIndex.Other() : PlayerIndex);

            // 当持有手铐时，先使用手铐
            else if (indexOfHandcuffs != -1 && Info.IsItemUsable(PlayerIndex, ItemType.HandCuffs))
                action = new PlayerUsesItem(indexOfHandcuffs);

            else
            {
                float ratioOfReal = 1.0f * Info.CountRealBullets() / Info.BulletCount;

                // 1. 没有放大镜的情况下：
                if (indexOfMagnifyingGlass == -1)
                    //  实弹数/总弹数 >= 50%，选择攻击玩家
                    //  实弹数/总弹数 < 50%，选择攻击自己
                    action = new PlayerFiresGun(
                        ratioOfReal > 0.5 ? PlayerIndex.Other() : PlayerIndex);

                // 2. 有放大镜的情况下：
                else
                    //  实弹数/总弹数 >= 75%，不使用放大镜，选择攻击玩家
                    //  实弹数/总弹数 < 75%，选择使用放大镜，如果为真攻击玩家，如果为假攻击自己
                    action = ratioOfReal >= 0.75
                        ? new PlayerFiresGun(PlayerIndex.Other())
                        : new PlayerUsesItem(indexOfMagnifyingGlass);
            }

            _stepOfLastAction = Info.LevelStep.Copy();
            _lastAction = action;
            if (action is PlayerUsesItem playerUsesItem)
                _lastUsedItem = Info.Item(PlayerIndex, playerUsesItem.ItemIndex);
            else
                _lastUsedItem = ItemType.None;
            _lastItemEffect = null;
            return action;
        }

        public override void AcknowledgeItemEffect(ItemEffect effect)
        {
            _lastItemEffect = effect;
        }
    }
}