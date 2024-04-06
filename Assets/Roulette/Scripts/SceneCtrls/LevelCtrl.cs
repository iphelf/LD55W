using Roulette.Scripts.General;
using Roulette.Scripts.Managers;
using UnityEngine;

namespace Roulette.Scripts.SceneCtrls
{
    public class LevelCtrl : MonoBehaviour
    {
        [SerializeField] private GameManager game;

        private async void Start()
        {
            Dummy.PerformQuickTask("读取关卡配置");

            await Dummy.PerformTask("关卡开场仪式");

            do await NewRound();
            while (!IsLevelOver());
        }

        /// 新的轮次
        private async Awaitable NewRound()
        {
            await Dummy.PerformTask("轮次开场仪式");

            await game.bombManager.PrepareBombs();
            await game.cardManager.DrawCards();

            do await NewTurn();
            while (!IsLevelOver() && !AreBombsExhausted());
        }

        /// 新的回合
        private async Awaitable NewTurn()
        {
            await Dummy.PerformTask("回合开场仪式");

            await game.bombManager.TakeNextBomb();
            await Dummy.PerformTask("玩家使用道具");
            await Dummy.PerformTask("玩家决定对谁开火");
        }

        private bool AreBombsExhausted()
        {
            return Dummy.CheckCondition("炸弹用完了");
        }

        private bool IsLevelOver()
        {
            return Dummy.CheckCondition("关卡分出胜负了");
        }
    }
}