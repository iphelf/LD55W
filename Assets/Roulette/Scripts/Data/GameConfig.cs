using System.Collections.Generic;
using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Data
{
    /// 游戏流程的配置
    [CreateAssetMenu(menuName = "Scriptable Object/Game Config", fileName = "game")]
    public class GameConfig : ScriptableObject
    {
        public List<LevelConfig> levels = new();

        [Header("Scenes"), Scene] public string titleScene = "Title";
        [Scene] public string creditsScene = "Credits";
        [Scene] public string levelScene = "Level";
        [Scene] public string gameOverScene = "GameOver";

        [Space] public AudioConfig audioConfig;
    }
}