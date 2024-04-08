using UnityEngine;

namespace Roulette.Scripts.Data
{
    /// 运行时真正启用的配置
    [CreateAssetMenu(menuName = "Scriptable Object/Configuration", fileName = "configuration")]
    public class Configuration : ScriptableObject
    {
        public GameConfig gameConfig;
    }
}