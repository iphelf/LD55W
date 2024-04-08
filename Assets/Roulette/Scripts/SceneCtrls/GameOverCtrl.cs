using Roulette.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette.Scripts.SceneCtrls
{
    public class GameOverCtrl : MonoBehaviour
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
            GameManager.RestartGame();
        }

        void ReturnToMenu()
        {
            GameManager.ReturnToTitle();
        }

        void QuitGame()
        {
            GameManager.QuitGame();
        }
    }
}