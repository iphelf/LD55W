using Roulette.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roulette.Scripts.Managers
{
    /// 宏观管理游戏全流程
    public static class GameManager
    {
        private static bool _initialized;
        private static GameConfig _gameConfig;

        #region (Anywhere)

        public static void InitializeGameOnce(Configuration configuration, AudioSource audioSource)
        {
            if (_initialized) return;

            _gameConfig = configuration.gameConfig;
            LevelManager.Reset(_gameConfig.levels);
            AudioManager.Initialize(_gameConfig.audioConfig, audioSource);

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
            LevelManager.Reset(_gameConfig.levels);
            SceneManager.LoadScene(_gameConfig.levelScene);
        }

        public static void OpenCredits()
        {
            SceneManager.LoadScene(_gameConfig.creditsScene);
        }

        #endregion

        #region Credits

        public static void CloseCreditsAndOpenTitle()
        {
            SceneManager.LoadScene(_gameConfig.titleScene);
        }

        #endregion


        #region Level

        public static void NewLevel()
        {
            SceneManager.LoadScene(_gameConfig.levelScene);
        }

        public static void OpenGameOver()
        {
            SceneManager.LoadScene(_gameConfig.gameOverScene);
        }

        public static void QuitLevelAndOpenTitle()
        {
            SceneManager.LoadScene(_gameConfig.titleScene);
        }

        public static void QuitLevelAndRestartGame()
        {
            StartGame();
        }

        #endregion

        #region Game Over

        public static void ReturnToTitle()
        {
            SceneManager.LoadScene(_gameConfig.titleScene);
        }

        public static void RestartGame()
        {
            StartGame();
        }

        #endregion
    }
}