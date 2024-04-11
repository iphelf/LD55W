using System;
using Roulette.Scripts.General;
using Roulette.Scripts.Models;
using Roulette.Scripts.ViewCtrls;
using UnityEngine;

namespace Roulette.Scripts.Managers
{
    public class BombManager : MonoBehaviour
    {
        [SerializeField] private BannerCtrl bannerCtrl;

        public async Awaitable PrepareBombsForNewRound(int count, int realCount)
        {
            await bannerCtrl.Present($"Bombs: {count}\nTrue Bombs: {realCount}");
            // TODO: 变更雷池中炸弹的数量
        }

        public async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex)
        {
            // TODO: 实现移动炸弹的平移动画
            await Dummy.PerformTask($"为{playerIndex}从队列中取出一个炸弹，移动到中央鉴定区");
        }

        public async Awaitable PlayBombEffect(
            PlayerIndex instigator, PlayerIndex target, bool isReal, Action onHit)
        {
            string action = instigator == target
                ? $"{instigator} detonates the bomb."
                : $"{instigator} attacks {target} with the bomb.";
            string result = isReal ? "And it explodes." : "And it doesn't explode.";
            await bannerCtrl.Present($"{action} {result}");
            onHit();
            // TODO: 清空鉴定区
        }

        public async Awaitable PlayMagnifyingGlassEffect(bool isReal)
        {
            // TODO: 在鉴定区的炸弹旁边显示文字表明其真假
            await Dummy.PerformTask($"The bomb is {isReal}.");
        }
    }
}