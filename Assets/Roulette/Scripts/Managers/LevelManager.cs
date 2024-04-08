using System.Collections.Generic;
using Roulette.Scripts.Data;

namespace Roulette.Scripts.Managers
{
    /// 管理多关卡流程
    public static class LevelManager
    {
        private static List<LevelConfig> _levels;
        public static int LevelIndex { get; private set; }

        public static LevelConfig Current => _levels[LevelIndex];

        public static void Reset(List<LevelConfig> levels)
        {
            _levels = levels;
            LevelIndex = 0;
        }

        public static void CompleteLevel()
        {
            GameManager.OpenGameOver();

            // ++LevelIndex;
            // if (LevelIndex >= _levels.Count)
            //     GameManager.OpenGameOver();
            // else
            //     GameManager.NewLevel();
        }
    }
}