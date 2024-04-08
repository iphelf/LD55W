using Roulette.Scripts.Managers;
using UnityEngine;

namespace Roulette.Scripts.SceneCtrls
{
    public class TitleCtrl : MonoBehaviour
    {
        private void Start() => GameManager.OnGameLaunchedOnce();

        public void StartGame() => GameManager.StartGame();

        public void OpenCredits() => GameManager.OpenCredits();

        public void QuitGame() => GameManager.QuitGame();
    }
}