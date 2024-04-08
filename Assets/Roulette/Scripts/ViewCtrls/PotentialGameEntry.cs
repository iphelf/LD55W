using Roulette.Scripts.Data;
using Roulette.Scripts.Managers;
using UnityEngine;

namespace Roulette.Scripts.ViewCtrls
{
    /// 在进入游戏前完成初始化
    public class PotentialGameEntry : MonoBehaviour
    {
        [SerializeField] private Configuration configuration;

        private void Awake()
        {
            GameManager.InitializeGameOnce(configuration);
        }
    }
}