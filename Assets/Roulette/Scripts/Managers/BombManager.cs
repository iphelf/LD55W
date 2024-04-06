using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Managers
{
    public class BombManager : MonoBehaviour
    {
        public async Awaitable PrepareBombs()
        {
            await Dummy.PerformTask("为新的轮次准备炸弹");
        }

        public async Awaitable TakeNextBomb()
        {
            await Dummy.PerformTask("从队列中取出一个炸弹，移动到中央鉴定区");
        }

        public async Awaitable<bool> RevealBomb()
        {
            await Dummy.PerformTask("揭示鉴定区的炸弹是实弹还是虚弹");
            return Dummy.CheckCondition("鉴定区的炸弹是实弹");
        }
    }
}