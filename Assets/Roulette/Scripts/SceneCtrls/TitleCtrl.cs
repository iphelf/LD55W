using Roulette.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette.Scripts.SceneCtrls
{
    public class TitleCtrl : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            startButton.onClick.AddListener(GameManager.StartGame);
            creditsButton.onClick.AddListener(GameManager.OpenCredits);
            quitButton.onClick.AddListener(GameManager.QuitGame);
        }
    }
}