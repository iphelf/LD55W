using System;
using System.Collections.Generic;
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
        [SerializeField] private Button continueButton;

        private Presentation _presentation;
        [SerializeField] private bool p1IsAI;
        private PlayerInput _player1Input;
        [SerializeField] private bool p2IsAI = true;
        private PlayerInput _player2Input;

        private AwaitableCompletionSource<string> _appendingInput;
        private Predicate<string> _appendingInputValidator;
        private AwaitableCompletionSource _waitForContinue;

        private async void Start()
        {
            continueButton.gameObject.SetActive(false);
            continueButton.onClick.AddListener(() =>
            {
                _waitForContinue?.SetResult();
                // continueButton.gameObject.SetActive(false);
            });

            _presentation = new Presentation(this);

            _player1Input = p1IsAI
                ? new ConsoleAIPlayerInput(_presentation, PlayerIndex.P1, this)
                : new ConsoleHumanPlayerInput(_presentation, PlayerIndex.P1, this);
            _presentation.BindPlayerInput(PlayerIndex.P1, _player1Input);

            _player2Input = p2IsAI
                ? new ConsoleAIPlayerInput(_presentation, PlayerIndex.P2, this)
                : new ConsoleHumanPlayerInput(_presentation, PlayerIndex.P2, this);
            _presentation.BindPlayerInput(PlayerIndex.P2, _player2Input);

            await LevelDriver.Drive(LevelManager.Current, _presentation);
        }

        private void OnLevelOver()
        {
            LevelManager.ScoreCalculation(_presentation.Info.Health(PlayerIndex.P1));
            LevelManager.CompleteLevel();
        }

        private string SerializeItems(IEnumerable<ItemType> items)
        {
            return string.Join(", ", items);
        }

        private void ShowLevelState()
        {
            string p1State =
                $"HP={_presentation.Info.Health(PlayerIndex.P1)}, " +
                $"Items={{{SerializeItems(_presentation.Info.Items(PlayerIndex.P1))}}}";
            string p2State =
                $"HP={_presentation.Info.Health(PlayerIndex.P2)}, " +
                $"Items={{{SerializeItems(_presentation.Info.Items(PlayerIndex.P2))}}}";
            string levelState = $"Status: \n  P1: {p1State}\n  P2: {p2State}";
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
            bool result;
            if (input[0] == 's')
            {
                ShowLevelState();
                result = true;
            }
            else if (_appendingInput != null && (_appendingInputValidator == null || _appendingInputValidator(input)))
            {
                var appendingInput = _appendingInput;
                _appendingInput = null;
                _appendingInputValidator = null;
                appendingInput.SetResult(input);
                result = true;
            }
            else
                result = false;

            return result;
        }

        private void Output(string output)
        {
            GameObject record = Instantiate(recordPrefab, recordList);
            record.transform.SetAsFirstSibling();
            var recordText = record.GetComponent<TMP_Text>();
            recordText.text = output;
            LayoutRebuilder.ForceRebuildLayoutImmediate(record.transform as RectTransform);
        }

        private async Awaitable OutputAsync(string output)
        {
            Output(output);
            await Awaitable.WaitForSecondsAsync(1f);
        }

        private async Awaitable OutputAction(PlayerIndex playerIndex, List<ItemType> items,
            PlayerAction action)
        {
            switch (action)
            {
                case PlayerFiresGun playerFiresGun:
                    await OutputAsync(playerFiresGun.Target == playerIndex
                        ? $"{playerIndex} fires himself/herself."
                        : $"{playerIndex} fires the other player.");
                    break;
                case PlayerUsesItem playerUsesItem:
                    await OutputAsync($"{playerIndex} uses item {items[playerUsesItem.ItemIndex]}.");
                    break;
            }
        }

        private Awaitable RequireContinue()
        {
            _waitForContinue ??= new AwaitableCompletionSource();
            continueButton.gameObject.SetActive(true);
            return _waitForContinue.Awaitable;
        }

        private class ConsoleHumanPlayerInput : PlayerInput
        {
            private readonly LevelConsoleCtrl _ctrl;

            public ConsoleHumanPlayerInput(
                LevelPresentation presentation,
                PlayerIndex playerIndex,
                LevelConsoleCtrl ctrl
            ) : base(presentation, playerIndex)
            {
                _ctrl = ctrl;
            }

            private PlayerAction ParseAction(PlayerIndex playerIndex, List<ItemType> items,
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
                        if (itemIndex < 0 || itemIndex >= items.Count
                                          || !Info.IsItemUsable(playerIndex, items[itemIndex]))
                            return null;
                        return new PlayerUsesItem(itemIndex);
                    default:
                        return null;
                }
            }

            public override async Awaitable<PlayerAction> ProducePlayerAction(List<ItemType> items)
            {
                await _ctrl.OutputAsync($"Host: Speak thy wish, {PlayerIndex}. (Input hint is given to the right)");
                string input = await _ctrl.RequireInput(input =>
                {
                    var action = ParseAction(PlayerIndex, items, input);
                    return action != null;
                });
                var action = ParseAction(PlayerIndex, items, input);
                await _ctrl.OutputAction(PlayerIndex, items, action);
                return action;
            }
        }

        private class ConsoleAIPlayerInput : BasicAIPlayerInput
        {
            private readonly LevelConsoleCtrl _ctrl;

            public ConsoleAIPlayerInput(
                LevelPresentation presentation,
                PlayerIndex playerIndex,
                LevelConsoleCtrl ctrl
            ) : base(presentation, playerIndex)
            {
                _ctrl = ctrl;
            }

            public override async Awaitable<PlayerAction> ProducePlayerAction(List<ItemType> items)
            {
                await _ctrl.OutputAsync($"{PlayerIndex} thinks for a while.");
                var action = await base.ProducePlayerAction(items);
                await _ctrl.OutputAction(PlayerIndex, items, action);
                return action;
            }
        }

        private class Presentation : LevelPresentation
        {
            private readonly LevelConsoleCtrl _ctrl;

            public Presentation(LevelConsoleCtrl ctrl)
            {
                _ctrl = ctrl;
            }

            public override async Awaitable PlayCeremonyOnLevelBegin()
            {
                await _ctrl.OutputAsync("# A new level begins.");
            }

            public override async Awaitable AppendCard(PlayerIndex playerIndex, int existingCardCount, ItemType newCard)
            {
                await _ctrl.OutputAsync($"And it is added to {playerIndex}'s hand cards");
            }

            public override async Awaitable RegretfullyDisposeLastDrawnCard(PlayerIndex playerIndex)
            {
                await _ctrl.OutputAsync($"However, it is discarded since {playerIndex}'s hands are full");
            }

            public override async Awaitable PlayCeremonyOnRoundBegin()
            {
                await _ctrl.OutputAsync("## A new round begins.");
            }

            public override async Awaitable PrepareBombsForNewRound(int count, int realCount)
            {
                await _ctrl.OutputAsync($"Host: {count} bullets are in roulette! " +
                                        $"And {realCount} of them are live!");
            }

            public override async Awaitable DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
            {
                await _ctrl.OutputAsync($"{playerIndex} draws item {card}.");
            }

            public override async Awaitable PlayCeremonyOnTurnBegin(PlayerIndex playerIndex)
            {
                await _ctrl.OutputAsync($"### {playerIndex}'s turn begins");
            }

            public override async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex)
            {
                await _ctrl.OutputAsync("Host: A new bullet has been loaded, either live or blank.");
            }

            protected override async Awaitable ConsumeCardAndPlayEffect(PlayerIndex playerIndex, int itemIndex,
                ItemEffect itemEffect, Action onHit = null)
            {
                switch (itemEffect)
                {
                    case EffectOfMagnifyingGlass effectOfMagnifyingGlass:
                        await _ctrl.OutputAsync($"{playerIndex} sees {effectOfMagnifyingGlass.IsReal}.");
                        break;
                    case EffectOfHandCuff:
                        onHit?.Invoke();
                        await _ctrl.OutputAsync($"{playerIndex.Other()} is handcuffed by {playerIndex}.");
                        break;
                }
            }

            protected override async Awaitable PlayBombEffect(PlayerIndex instigator, PlayerIndex target, bool isReal,
                Action onHit)
            {
                if (isReal)
                {
                    onHit();
                    await _ctrl.OutputAsync($"{target} gets shot by {instigator}.");
                }
                else
                    await _ctrl.OutputAsync(
                        $"{target} does not get shot by {instigator}, because the loaded bullet was blank.");
            }

            public override async Awaitable PlayCeremonyOnTurnEnd(PlayerIndex playerIndex)
            {
                await _ctrl.OutputAsync($"### {playerIndex}'s turn ends.");
            }

            public override async Awaitable PlayCeremonyOnRoundEnd()
            {
                await _ctrl.OutputAsync("## The last round ends.");
            }

            public override async Awaitable PlayCeremonyOnLevelEnd(PlayerIndex winner)
            {
                await _ctrl.OutputAsync($"# The level completes. And the winner is {winner}.");
                await _ctrl.OutputAsync("Host: Now, press the `Continue` button to proceed.");
                await _ctrl.RequireContinue();
                _ctrl.OnLevelOver();
            }
        }
    }
}