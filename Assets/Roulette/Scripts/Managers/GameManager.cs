using Roulette.Scripts.Data;
using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Managers
{
    /// 宏观管理游戏全流程
    public static class GameManager
    {
        private static bool _initialized;
        private static Configuration _configuration;

        #region (Anywhere)

        public static void QuitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Title

        public static void InitializeOnce(Configuration configuration)
        {
            if (_initialized) return;

            _configuration = configuration;
            LevelManager.Reset(_configuration.gameConfig.levels);

            _initialized = true;
            Debug.Log("Initialized.");
        }

        public static void StartGame()
        {
            Dummy.PerformQuickTask("开始新游戏");
        }

        public static void OpenCredits()
        {
            Dummy.PerformQuickTask("打开Credits界面");
        }

        #endregion

        #region Credits

        public static void CloseCreditsAndOpenTitle()
        {
            Dummy.PerformQuickTask("关闭Credits界面并回到标题界面");
        }

        #endregion


        #region Level

        public static void NewLevel()
        {
            Dummy.PerformQuickTask("新的关卡");
        }

        public static void OpenGameOver()
        {
            Dummy.PerformQuickTask("游戏结束");
        }

        public static void QuitLevelAndOpenTitle()
        {
            Dummy.PerformQuickTask("退出关卡并回到标题界面");
        }

        public static void QuitLevelAndRestartGame()
        {
            Dummy.PerformQuickTask("退出关卡并重新开始游戏");
        }

        #endregion

        #region Game Over

        public static void ReturnToTitle()
        {
            Dummy.PerformQuickTask("回到标题界面");
        }

        public static void RestartGame()
        {
            Dummy.PerformQuickTask("重新开始游戏");
        }

        #endregion
    }
}