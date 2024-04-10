using System.Collections.Generic;
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

        public abstract Awaitable<PlayerAction> ProducePlayerAction(List<ItemType> items);

        /// 仅AI可能需要override
        public virtual void AcknowledgeBombExplosion(bool exploded)
        {
        }

        /// 仅AI可能需要override
        public virtual void AcknowledgeItemEffect(ItemEffect effect)
        {
        }
    }
}