using Roulette.Scripts.General;
using UnityEngine;

namespace Roulette.Scripts.Data
{
    /// 运行时真正启用的配置
    [CreateAssetMenu(menuName = "Scriptable Object/Configuration", fileName = "configuration")]
    public class Configuration : ScriptableObject
    {
        public GameConfig gameConfig;

        [Header("Scenes"), Scene] public string titleScene = "Title";
        [Scene] public string creditsScene = "Credits";
        [Scene] public string levelScene = "Level";
        [Scene] public string gameOverScene = "GameOver";
    }
}