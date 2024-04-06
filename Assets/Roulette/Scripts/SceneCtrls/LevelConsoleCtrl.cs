using System;
using System.Collections.Generic;
using System.Linq;
using Roulette.Scripts.Data;
using Roulette.Scripts.Models;
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
        [SerializeField] private LevelConfig levelConfig;

        private Level _level;
        private int _levelStep;

        private void Start()
        {
            _level = new Level(levelConfig);
            _levelStep = 0;
            ShowLevelState();
        }

        private void ShowLevelState()
        {
            string infoLevel = $"Level Step={_levelStep}, Turn={_level.Turn}, Bullets={_level.BulletCount}";

            string SerializeDict(SortedDictionary<int, ItemType> dict) =>
                "{" + string.Join(',', dict.Select(p => $"{p.Key}:{p.Value}")) + "}";

            string infoP1 =
                $"{PlayerIndex.P1}: HP={_level.Player1.Health}, Items={SerializeDict(_level.Player1.Items)}";
            string infoP2 =
                $"{PlayerIndex.P2}: HP={_level.Player2.Health}, Items={SerializeDict(_level.Player2.Items)}";
            string info = string.Join("\n", infoLevel, infoP1, infoP2);
            Output(info);
        }

        public void SubmitInput()
        {
            string input = inputField.text;
            if (input.Length == 0 || !ProcessInput(input))
            {
                inputField.ActivateInputField();
                return;
            }

            inputField.text = String.Empty;
            inputField.ActivateInputField();
        }

        private bool ProcessInput(string input)
        {
            var tokens = input.Split(" ");
            switch (tokens[0].ToLower()[0])
            {
                case 'f': // fire
                    if (tokens[1].ToLower()[0] == 'o')
                    {
                        Output($"{_level.Turn} fires the other player.");
                        _level.CurrentPlayer.FireOther();
                    }
                    else
                    {
                        Output($"{_level.Turn} fires himself/herself.");
                        _level.CurrentPlayer.FireSelf();
                    }

                    break;
                case 'u': // use
                    int item = int.Parse(tokens[2]);
                    Output($"{_level.Turn} uses item");
                    _level.CurrentPlayer.UseItem(item);
                    break;
                case 'n': // new
                    _level = new Level(levelConfig);
                    _levelStep = -1;
                    break;
                default:
                    return false;
            }

            ++_levelStep;
            ShowLevelState();

            return true;
        }

        private void Output(string output)
        {
            GameObject record = Instantiate(recordPrefab, recordList);
            record.transform.SetAsFirstSibling();
            var recordText = record.GetComponent<TMP_Text>();
            recordText.text = output;
            LayoutRebuilder.ForceRebuildLayoutImmediate(record.transform as RectTransform);
        }
    }
}