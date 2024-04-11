using System.Collections.Generic;
using System.Security.Authentication;
using Roulette.Scripts.Data;
using Roulette.Scripts.Models;
using Roulette.Scripts.SceneCtrls;
using UnityEngine;

namespace Roulette.Scripts.Managers
{
    /// 管理多关卡流程
    public static class LevelManager
    {
        private static List<LevelConfig> _levels;
        public static int LevelIndex { get; private set; }
        private static int Score=0;
        

        public static LevelConfig Current => _levels[LevelIndex];

        public static void Reset(List<LevelConfig> levels)
        {
            _levels = levels;
            LevelIndex = 0;
        }

        public static void CompleteLevel()
        {
             ++LevelIndex;
             if (LevelIndex >= _levels.Count)
                 GameManager.OpenGameOver();
             else
                 GameManager.NewLevel();
        }
        public static void ScoreCalculation(int health)
        {
            Score += health;
            
        }
    }
}