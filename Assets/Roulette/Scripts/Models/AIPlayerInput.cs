using System.Collections.Generic;
using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    public class PlaceholderPlayerInput : PlayerInput
    {
        public PlaceholderPlayerInput(LevelInfo info, PlayerIndex playerIndex) : base(info, playerIndex)
        {
        }

        public override async Awaitable<PlayerAction> ProducePlayerAction(SortedDictionary<int, ItemType> items)
        {
            await Dummy.NothingButAwaitable();
            return new PlayerFiresGun(PlayerIndex.Other());
        }

        public override async Awaitable<int> PlaceCard(
            SortedDictionary<int, ItemType> existingCards, ItemType newCard)
        {
            await Dummy.NothingButAwaitable();
            for (int i = 0; i < Info.CardCapacity; ++i)
                if (!existingCards.ContainsKey(i))
                    return i;
            return -1;
        }
    }
}