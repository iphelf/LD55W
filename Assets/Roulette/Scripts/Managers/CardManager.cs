using Roulette.Scripts.General;
using Roulette.Scripts.Models;
using Roulette.Scripts.ViewCtrls;
using UnityEngine;
using UnityEngine.Events;

namespace Roulette.Scripts.Managers
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private CardRow[] PlayerHand;

        public UnityEvent<PlayerUsesItem> onPlayerAction;
        public void EnableActionInput()
        {
            // TODO: 提示玩家输入
        }

        public void DisableActionInput()
        {
            // TODO: 禁止玩家输入
        }

        // TODO: 在玩家输入后发出事件

        public void DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
        {
            PlayerHand[(int)playerIndex-1].DrawCardFromDeck(card);
        }

        public void AppendCard(PlayerIndex playerIndex, int existingCardCount, ItemType newCard)
        {
            PlayerHand[(int)playerIndex-1].AppendCard(newCard);
        }

        public void RegretfullyDisposeLastDrawnCard(PlayerIndex playerIndex)
        {
            PlayerHand[(int)playerIndex-1].RegretfullyDisposeLastDrawnCard();
        }
    }
}