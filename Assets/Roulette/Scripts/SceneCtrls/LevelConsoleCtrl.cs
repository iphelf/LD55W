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
        [SerializeField] private bool p1IsAI;
        private PlayerInput _player1Input;
        [SerializeField] private bool p2IsAI = true;
        private PlayerInput _player2Input;
        private AwaitableCompletionSource<string> _appendingInput;
        private Predicate<string> _appendingInputValidator;

        private async void Start()
        {
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

            bool isInputOk = ProcessInput(input, result => isInputOk = result);

            if (input.Length == 0 || !isInputOk)
            {
                inputField.ActivateInputField();
                return;
            }

            inputField.text = String.Empty;
            inputField.ActivateInputField();
        }

        private bool ProcessInput(string input, Action<bool> handleResult = null)
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

            handleResult?.Invoke(result);
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

        private async Awaitable OutputPlacingCard(PlayerIndex playerIndex, ItemType newCard, int index)
        {
            await OutputAsync($"{playerIndex} puts {newCard} to {index}-th item slot.");
        }

        private async Awaitable OutputAction(PlayerIndex playerIndex, SortedDictionary<int, ItemType> items,
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

            public override async Awaitable<PlayerAction> ProducePlayerAction(SortedDictionary<int, ItemType> items)
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

            string SerializeDict(SortedDictionary<int, ItemType> dict) =>
                "{" + string.Join(", ", dict.Select(p => $"{p.Key}:{p.Value}")) + "}";

            public override async Awaitable<int> PlaceCard(SortedDictionary<int, ItemType> existingCards,
                ItemType newCard)
            {
                await _ctrl.OutputAsync($"Host: {PlayerIndex}, where do you want to place your new item {newCard}? " +
                                        $"Note that, currently, you have {SerializeDict(existingCards)} in your hand.");
                string input = await _ctrl.RequireInput(input =>
                {
                    int index = int.Parse(input);
                    if (index >= Info.CardCapacity) return false;
                    if (existingCards.ContainsKey(index)) return false;
                    return true;
                });
                int index = int.Parse(input);
                await _ctrl.OutputPlacingCard(PlayerIndex, newCard, index);
                return index;
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

            public override async Awaitable<int> PlaceCard(SortedDictionary<int, ItemType> existingCards,
                ItemType newCard)
            {
                var index = await base.PlaceCard(existingCards, newCard);
                await _ctrl.OutputPlacingCard(PlayerIndex, newCard, index);
                return index;
            }

            public override async Awaitable<PlayerAction> ProducePlayerAction(SortedDictionary<int, ItemType> items)
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

            private async Awaitable Noop()
            {
                await Task.CompletedTask;
            }

            public override async Awaitable PlayCeremonyOnLevelBegin()
            {
                await _ctrl.OutputAsync("# A new level begins.");
                await Noop();
            }

            public override async Awaitable PlayCeremonyOnRoundBegin()
            {
                await _ctrl.OutputAsync("## A new round begins.");
                await Noop();
            }

            public override async Awaitable PrepareBombsForNewRound(int count)
            {
                await _ctrl.OutputAsync($"Host: {count} bombs are in queue!");
                await Noop();
            }

            public override async Awaitable DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
            {
                await _ctrl.OutputAsync($"{playerIndex} draws item {card}.");
                await Noop();
            }

            public override async Awaitable PlayCeremonyOnTurnBegin(PlayerIndex playerIndex)
            {
                await _ctrl.OutputAsync($"### {playerIndex}'s turn begins");
                await Noop();
            }

            public override async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex, BulletQueue bulletQueue)
            {
                await _ctrl.OutputAsync("Host: A new bullet has been loaded, either live or blank.");
                await Noop();
            }

            public override async Awaitable ConsumeCardAndPlayEffect(PlayerIndex playerIndex, int itemIndex,
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

                await _ctrl.OutputAsync($"The {itemIndex}-th item slot of {playerIndex} has been freed.");
                await Noop();
            }

            public override async Awaitable PlayBombEffect(PlayerIndex instigator, PlayerIndex target, bool isReal,
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

                await Noop();
            }

            public override async Awaitable PlayCeremonyOnTurnEnd(PlayerIndex playerIndex)
            {
                await _ctrl.OutputAsync($"### {playerIndex}'s turn ends.");
                await Noop();
            }

            public override async Awaitable PlayCeremonyOnRoundEnd()
            {
                await _ctrl.OutputAsync("## The last round ends.");
                await Noop();
            }

            public override async Awaitable PlayCeremonyOnLevelEnd(PlayerIndex winner)
            {
                await _ctrl.OutputAsync(
                    $"# The level completes. And the winner is {winner}. Game over will be shown in 5 seconds.");
                await Awaitable.WaitForSecondsAsync(5.0f);
                _ctrl.OnLevelOver();
            }
        }
    }
}