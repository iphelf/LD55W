using TMPro;
using UnityEngine;

namespace Roulette.Scripts.ViewCtrls
{
    public class BannerCtrl : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private TMP_Text text;

        public async Awaitable Present(string title, float seconds = 2)
        {
            text.text = title;
            content.SetActive(true);
            await Awaitable.WaitForSecondsAsync(seconds);
            content.SetActive(false);
            text.text = string.Empty;
        }
    }
}