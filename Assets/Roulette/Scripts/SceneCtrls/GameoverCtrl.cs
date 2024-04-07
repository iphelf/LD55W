using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette.Scripts.SceneCtrls
{
    public class GameoverCtrl : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button quitButton;

        void Start()
        {
            restartButton.onClick.AddListener(RestartGame);
            menuButton.onClick.AddListener(ReturnToMenu);
            quitButton.onClick.AddListener(QuitGame);
        }
        void RestartGame()
        { 
            Debug.Log("重新开始游戏");
            Application.Quit();
        }
        void ReturnToMenu()
        { 
            Debug.Log("返回主菜单");
            Application.Quit();
        }
        void QuitGame()
        { 
            Debug.Log("退出游戏");
            Application.Quit();
        }
    }
}