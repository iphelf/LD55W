using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Roulette.Scripts.General;
using Roulette.Scripts.Managers;
using Roulette.Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette.Scripts.SceneCtrls
{
    public class LevelConsoleCtrl : MonoBehaviour
    {
        [SerializeField] private GameObject recordPrefab;
        [SerializeField] private Transform recordList;
        [SerializeField] private TMP_InputField inputField;

        private Presentation _presentation;
        private AwaitableCompletionSource<string> _appendingInput;
        private Predicate<string> _appendingInputValidator;

        private async void Start()
        {
            _presentation = new Presentation(this);
            await LevelDriver.Drive(LevelManager.Current, _presentation);
        }

        private void OnLevelOver()
        {
            LevelManager.CompleteLevel();
        }

        private void ShowLevelState()
        {
            string levelState =
                $"Status: P1={_presentation.Info.Health(PlayerIndex.P1)}, " +
                $"P2={_presentation.Info.Health(PlayerIndex.P2)}, " +
                $"Bullets={_presentation.Info.BulletCount}.";
            Output(levelState);
        }

        private Awaitable<string> RequireInput(Predicate<string> validator = null)
        {
            if (_appendingInput == null)
            {
                _appendingInput = new AwaitableCompletionSource<string>();
                _appendingInputValidator = validator;
            }

            return _appendingInput.Awaitable;
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
            if (input[0] == 's')
            {
                ShowLevelState();
                return true;
            }

            if (_appendingInput != null && (_appendingInputValidator == null || _appendingInputValidator(input)))
            {
                var appendingInput = _appendingInput;
                _appendingInput = null;
                _appendingInputValidator = null;
                appendingInput.SetResult(input);
                return true;
            }

            return false;
        }

        private void Output(string output)
        {
            GameObject record = Instantiate(recordPrefab, recordList);
            record.transform.SetAsFirstSibling();
            var recordText = record.GetComponent<TMP_Text>();
            recordText.text = output;
            LayoutRebuilder.ForceRebuildLayoutImmediate(record.transform as RectTransform);
        }

        private class Presentation : LevelPresentation
        {
            private readonly LevelConsoleCtrl _ctrl;

            public Presentation(LevelConsoleCtrl ctrl)
            {
                _ctrl = ctrl;
            }

            private async Awaitable Noop()
            {
                await Task.CompletedTask;
            }

            public override async Awaitable PlayCeremonyOnLevelBegin()
            {
                _ctrl.Output("# A new level begins.");
                await Noop();
            }

            public override async Awaitable PlayCeremonyOnRoundBegin()
            {
                _ctrl.Output("## A new round begins.");
                await Noop();
            }

            public override async Awaitable PrepareBombsForNewRound(int count)
            {
                _ctrl.Output($"Host: {count} bombs are in queue!");
                await Noop();
            }

            public override async Awaitable DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
            {
                _ctrl.Output($"{playerIndex} draws item {card}.");
                await Noop();
            }

            string SerializeDict(SortedDictionary<int, ItemType> dict) =>
                "{" + string.Join(", ", dict.Select(p => $"{p.Key}:{p.Value}")) + "}";

            public override async Awaitable<int> PlaceCard(PlayerIndex playerIndex,
                SortedDictionary<int, ItemType> existingCards, ItemType newCard)
            {
                _ctrl.Output($"Host: {playerIndex}, where do you want to place your new item {newCard}? " +
                             $"Note that, currently, you have {SerializeDict(existingCards)} in your hand.");
                string input = await _ctrl.RequireInput(input =>
                {
                    int index = int.Parse(input);
                    if (index >= Info.CardCapacity) return false;
                    if (existingCards.ContainsKey(index)) return false;
                    return true;
                });
                int index = int.Parse(input);
                _ctrl.Output($"{playerIndex} puts {newCard} to {index}-th item slot.");
                return index;
            }

            public override async Awaitable PlayCeremonyOnTurnBegin(PlayerIndex playerIndex)
            {
                _ctrl.Output($"### {playerIndex}'s turn begins");
                await Noop();
            }

            public override async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex, BulletQueue bulletQueue)
            {
                _ctrl.Output("Host: A new bullet has been loaded, either live or blank.");
                await Noop();
            }

            private PlayerAction ParseAction(PlayerIndex playerIndex, SortedDictionary<int, ItemType> items,
                string input)
            {
                var tokens = input.Split(" ");
                switch (tokens[0].ToLower()[0])
                {
                    case 'f': // fire
                        bool isShootingOther = tokens[1].ToLower()[0] == 'o';
                        return new PlayerFiresGun(isShootingOther ? playerIndex.Other() : playerIndex);
                    case 'u': // use
                        int itemIndex = int.Parse(tokens[1]);
                        if (!items.ContainsKey(itemIndex) || !Info.IsItemUsable(playerIndex, items[itemIndex]))
                            return null;
                        return new PlayerUsesItem(itemIndex);
                    default:
                        return null;
                }
            }

            public override async Awaitable<PlayerAction> WaitForPlayerAction(PlayerIndex playerIndex,
                SortedDictionary<int, ItemType> items)
            {
                _ctrl.Output($"Host: Speak thy wish, {playerIndex}. (Input hint is given to the right)");
                string input = await _ctrl.RequireInput(input =>
                {
                    var action = ParseAction(playerIndex, items, input);
                    return action != null;
                });
                var action = ParseAction(playerIndex, items, input);
                switch (action)
                {
                    case PlayerFiresGun playerFiresGun:
                        _ctrl.Output(playerFiresGun.Target == playerIndex
                            ? $"{playerIndex} fires himself/herself."
                            : $"{playerIndex} fires the other player.");
                        break;
                    case PlayerUsesItem playerUsesItem:
                        _ctrl.Output($"{playerIndex} uses item {items[playerUsesItem.ItemIndex]}.");
                        break;
                }

                return action;
            }

            public override async Awaitable ConsumeCardAndPlayEffect(PlayerIndex playerIndex, int itemIndex,
                ItemEffect itemEffect, Action onHit = null)
            {
                switch (itemEffect)
                {
                    case EffectOfMagnifyingGlass effectOfMagnifyingGlass:
                        _ctrl.Output($"{playerIndex} sees {effectOfMagnifyingGlass.IsReal}.");
                        break;
                    case EffectOfHandCuff:
                        onHit?.Invoke();
                        _ctrl.Output($"{playerIndex.Other()} is handcuffed by {playerIndex}.");
                        break;
                }

                _ctrl.Output($"The {itemIndex}-th item slot of {playerIndex} has been freed.");
                await Noop();
            }

            public override async Awaitable PlayBombEffect(PlayerIndex instigator, PlayerIndex target, bool isReal,
                Action onHit)
            {
                if (isReal)
                {
                    onHit();
                    _ctrl.Output($"{target} gets shot by {instigator}.");
                }
                else
                    _ctrl.Output($"{target} does not get shot by {instigator}, because the loaded bullet was blank.");

                await Noop();
            }

            public override async Awaitable PlayCeremonyOnTurnEnd(PlayerIndex playerIndex)
            {
                _ctrl.Output($"### {playerIndex}'s turn ends.");
                await Noop();
            }

            public override async Awaitable PlayCeremonyOnRoundEnd()
            {
                _ctrl.Output("## The last round ends.");
                await Noop();
            }

            public override async Awaitable PlayCeremonyOnLevelEnd(PlayerIndex winner)
            {
                _ctrl.Output($"# The level completes. And the winner is {winner}.");
                await Awaitable.WaitForSecondsAsync(2.0f);
                _ctrl.OnLevelOver();
            }
        }
    }
}