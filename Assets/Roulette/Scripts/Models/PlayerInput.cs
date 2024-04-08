﻿using System.Collections.Generic;
using UnityEngine;

namespace Roulette.Scripts.Models
{
    public abstract class PlayerInput
    {
        private readonly LevelPresentation _presentation;
        protected LevelInfo Info => _presentation.Info;
        protected readonly PlayerIndex PlayerIndex;

        protected PlayerInput(LevelPresentation presentation, PlayerIndex playerIndex)
        {
            _presentation = presentation;
            PlayerIndex = playerIndex;
        }

        public abstract Awaitable<PlayerAction> ProducePlayerAction(
            SortedDictionary<int, ItemType> items);

        public virtual void AcknowledgeBombExplosion(bool exploded)
        {
        }

        public virtual void AcknowledgeItemEffect(ItemEffect effect)
        {
        }

        public abstract Awaitable<int> PlaceCard(
            SortedDictionary<int, ItemType> existingCards, ItemType newCard);
    }
}