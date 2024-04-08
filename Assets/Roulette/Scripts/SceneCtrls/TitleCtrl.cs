using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.SceneCtrls
{
    public class TitleCtrl : MonoBehaviour
    {
        public void StartGame()
        {
            Dummy.PerformQuickTask("开始新游戏");
        }

        public void OpenCredits()
        {
            Dummy.PerformQuickTask("打开Credits界面");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}