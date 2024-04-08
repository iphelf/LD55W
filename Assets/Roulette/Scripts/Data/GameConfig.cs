using System.Collections.Generic;
using UnityEngine;

namespace Roulette.Scripts.Data
{
    /// 游戏流程的配置
    [CreateAssetMenu(menuName = "Scriptable Object/Game Config", fileName = "game")]
    public class GameConfig : ScriptableObject
    {
        public List<LevelConfig> levels = new();
    }
}