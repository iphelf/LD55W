using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Managers
{
    public class CardManager : MonoBehaviour
    {
        public async Awaitable DrawCards()
        {
            await Dummy.PerformTask("玩家抽卡");
            await Awaitables.WhenAll(DrawToP1Hand(), DrawToP2Hand());
        }

        private async Awaitable DrawToP1Hand()
        {
            await Dummy.PerformTask("从牌堆抽牌，置入P1手牌区");
        }

        private async Awaitable DrawToP2Hand()
        {
            await Dummy.PerformTask("从牌堆抽牌，置入P2手牌区");
        }
    }
}