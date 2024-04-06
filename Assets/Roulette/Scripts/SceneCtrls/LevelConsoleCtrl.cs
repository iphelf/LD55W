using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Roulette.Scripts.SceneCtrls
{
    public class LevelConsoleCtrl : MonoBehaviour
    {
        [SerializeField] private GameObject recordPrefab;
        [SerializeField] private Transform recordList;
        [SerializeField] private TMP_InputField inputField;

        public void SubmitInput()
        {
            string input = inputField.text;
            if (input.Length == 0)
            {
                inputField.ActivateInputField();
                return;
            }

            if (!ProcessInput(input))
                return;
            inputField.text = String.Empty;
            GameObject record = Instantiate(recordPrefab, recordList);
            record.transform.SetAsFirstSibling();
            var recordText = record.GetComponent<TMP_Text>();
            recordText.text = input;
            LayoutRebuilder.ForceRebuildLayoutImmediate(record.transform as RectTransform);
            inputField.ActivateInputField();
        }

        private bool ProcessInput(string input)
        {
            // TODO: process console input
            return true;
        }
    }
}