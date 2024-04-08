using Roulette.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette.Scripts.SceneCtrls
{
    public class CreditsCtrl : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        private void Start()
        {
            closeButton.onClick.AddListener(GameManager.CloseCreditsAndOpenTitle);
        }
    }
}