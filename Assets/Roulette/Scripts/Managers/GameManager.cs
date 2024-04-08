using Roulette.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roulette.Scripts.Managers
{
    /// 宏观管理游戏全流程
    public static class GameManager
    {
        private static bool _initialized;
        private static Configuration _configuration;

        #region (Anywhere)

        public static void InitializeGameOnce(Configuration configuration)
        {
            if (_initialized) return;

            _configuration = configuration;
            LevelManager.Reset(_configuration.gameConfig.levels);

            _initialized = true;
            Debug.Log("Initialized.");
        }

        public static void QuitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Title

        public static void StartGame()
        {
            LevelManager.Reset(_configuration.gameConfig.levels);
            SceneManager.LoadScene(_configuration.levelScene);
        }

        public static void OpenCredits()
        {
            SceneManager.LoadScene(_configuration.creditsScene);
        }

        #endregion

        #region Credits

        public static void CloseCreditsAndOpenTitle()
        {
            SceneManager.LoadScene(_configuration.titleScene);
        }

        #endregion


        #region Level

        public static void NewLevel()
        {
            SceneManager.LoadScene(_configuration.levelScene);
        }

        public static void OpenGameOver()
        {
            SceneManager.LoadScene(_configuration.gameOverScene);
        }

        public static void QuitLevelAndOpenTitle()
        {
            SceneManager.LoadScene(_configuration.titleScene);
        }

        public static void QuitLevelAndRestartGame()
        {
            StartGame();
        }

        #endregion

        #region Game Over

        public static void ReturnToTitle()
        {
            SceneManager.LoadScene(_configuration.titleScene);
        }

        public static void RestartGame()
        {
            StartGame();
        }

        #endregion
    }
}