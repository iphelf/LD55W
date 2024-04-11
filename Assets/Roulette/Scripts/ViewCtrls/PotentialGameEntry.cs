using Roulette.Scripts.Data;
using Roulette.Scripts.Managers;
using UnityEngine;

namespace Roulette.Scripts.ViewCtrls
{
    /// 在进入游戏前完成初始化
    public class PotentialGameEntry : MonoBehaviour
    {
        [SerializeField] private Configuration configuration;
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");

            if (objs.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            GameManager.InitializeGameOnce(configuration, audioSource);
        }
    }
}