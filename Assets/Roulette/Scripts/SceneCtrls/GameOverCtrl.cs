using Roulette.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette.Scripts.SceneCtrls
{
    public class GameOverCtrl : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI score;

        void Start()
        {
            restartButton.onClick.AddListener(GameManager.RestartGame);
            menuButton.onClick.AddListener(GameManager.ReturnToTitle);
            quitButton.onClick.AddListener(GameManager.QuitGame);
            score.text = $"Score:{LevelManager._score}";
        }
        
    }
}