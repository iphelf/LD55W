using System.Collections.Generic;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    public abstract class PlayerInput
    {
        protected readonly LevelInfo Info;
        protected readonly PlayerIndex PlayerIndex;

        protected PlayerInput(LevelInfo info, PlayerIndex playerIndex)
        {
            Info = info;
            PlayerIndex = playerIndex;
        }

        public abstract Awaitable<PlayerAction> ProducePlayerAction(
            SortedDictionary<int, ItemType> items);

        public abstract Awaitable<int> PlaceCard(
            SortedDictionary<int, ItemType> existingCards, ItemType newCard);
    }
}