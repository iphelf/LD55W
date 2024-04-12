using System;
using System.Collections.Generic;
using Roulette.Scripts.Managers;
using Roulette.Scripts.Models;
using Roulette.Scripts.ViewCtrls;
using UnityEngine;

namespace Roulette.Scripts.SceneCtrls
{
    public class LevelCtrl : MonoBehaviour
    {
        [SerializeField] private BombManager bombManager;
        [SerializeField] private CardManager cardManager;
        [SerializeField] private ActionsCtrl actionsCtrl;
        [SerializeField] private StatusCtrl statusCtrl;
        [SerializeField] private BuffsCtrl p1BuffsCtrl;
        [SerializeField] private BuffsCtrl p2BuffsCtrl;
        [SerializeField] private BannerCtrl bannerCtrl;
        
        private Presentation _presentation;

        private async void Start()
        {
            _presentation = new Presentation(this);

            GraphicalPlayerInput playerInput = new GraphicalPlayerInput(_presentation, PlayerIndex.P1, this);
            _presentation.BindPlayerInput(PlayerIndex.P1, playerInput);
            BasicAIPlayerInput aiPlayerInput = new BasicAIPlayerInput(_presentation, PlayerIndex.P2);
            _presentation.BindPlayerInput(PlayerIndex.P2, aiPlayerInput);

            await LevelDriver.Drive(LevelManager.Current, _presentation);
        }

        private void OnLevelOver()
        {
            LevelManager.ScoreCalculation(_presentation.Info.Health(PlayerIndex.P1));
            LevelManager.CompleteLevel();
        }

        private class GraphicalPlayerInput : PlayerInput
        {
            private readonly LevelCtrl _ctrl;
            private AwaitableCompletionSource<PlayerAction> _appendingAction;

            public GraphicalPlayerInput(
                LevelPresentation presentation,
                PlayerIndex playerIndex,
                LevelCtrl ctrl)
                : base(presentation, playerIndex)
            {
                _ctrl = ctrl;
            }

            public override Awaitable<PlayerAction> ProducePlayerAction(List<ItemType> items)
            {
                _appendingAction ??= new AwaitableCompletionSource<PlayerAction>();
                StartListening();
                return _appendingAction.Awaitable;
            }

            private void StartListening()
            {
                _ctrl.actionsCtrl.onPlayerAction.AddListener(OnPlayerAction);
                _ctrl.cardManager.onPlayerAction.AddListener(OnPlayerAction);
                _ctrl.actionsCtrl.EnableActionInput();
                _ctrl.cardManager.EnableActionInput();
            }

            private void StopListening()
            {
                _ctrl.actionsCtrl.DisableActionInput();
                _ctrl.cardManager.DisableActionInput();
                _ctrl.actionsCtrl.onPlayerAction.RemoveListener(OnPlayerAction);
                _ctrl.cardManager.onPlayerAction.RemoveListener(OnPlayerAction);
            }

            private void OnPlayerAction(PlayerAction playerAction)
            {
                StopListening();
                var appendingAction = _appendingAction;
                _appendingAction = null;
                appendingAction?.SetResult(playerAction);
            }
        }
        
        private class Presentation : LevelPresentation
        {
            private readonly LevelCtrl _ctrl;

            public Presentation(LevelCtrl ctrl)
            {
                _ctrl = ctrl;
            }

            public override async Awaitable PlayCeremonyOnLevelBegin()
            {
                await _ctrl.bannerCtrl.Present($"Level {LevelManager.LevelIndex} begins");
            }

            public override async Awaitable PlayCeremonyOnRoundBegin()
            {
                await base.PlayCeremonyOnRoundBegin();
            }

            public override async Awaitable PrepareBombsForNewRound(int count, int realCount)
            {
                await _ctrl.bombManager.PrepareBombsForNewRound(count, realCount);
            }

            public override async Awaitable DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
            {
                _ctrl.cardManager.DrawCardFromDeck(playerIndex,card);
                await base.DrawCardFromDeck(playerIndex, card);
            }

            public override async Awaitable AppendCard(
                PlayerIndex playerIndex, int existingCardCount, ItemType newCard)
            {
                _ctrl.cardManager.AppendCard(playerIndex, existingCardCount,newCard);
                await base.AppendCard(playerIndex, existingCardCount, newCard);
            }

            public override async Awaitable PlayCeremonyOnTurnBegin(PlayerIndex playerIndex)
            {
                await _ctrl.bannerCtrl.Present($"{playerIndex}'s turn");
            }

            public override async Awaitable RegretfullyDisposeLastDrawnCard(PlayerIndex playerIndex)
            {
                _ctrl.cardManager.RegretfullyDisposeLastDrawnCard(playerIndex);
                await base.RegretfullyDisposeLastDrawnCard(playerIndex);
            }

            public override async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex)
            {
                await _ctrl.bombManager.TakeBombForNewTurn(playerIndex);
            }

            protected override async Awaitable ConsumeCardAndPlayEffect(PlayerIndex playerIndex, int itemIndex,
                ItemEffect itemEffect, Action onHit = null)
            {
                await base.ConsumeCardAndPlayEffect(playerIndex, itemIndex, itemEffect, onHit);
                if (playerIndex == PlayerIndex.P1
                    && itemEffect is EffectOfMagnifyingGlass effectOfMagnifyingGlass)
                    await _ctrl.bombManager.PlayMagnifyingGlassEffect(effectOfMagnifyingGlass.IsReal);
            }

            protected override async Awaitable PlayBombEffect(
                PlayerIndex instigator, PlayerIndex target, bool isReal, Action onHit)
            {
                await _ctrl.bombManager.PlayBombEffect(instigator, target, isReal, onHit);
                await _ctrl.bombManager.PlayMagnifyingGlassEffect(isReal);
            }

            public override async Awaitable PlayCeremonyOnTurnEnd(PlayerIndex playerIndex)
            {
                await base.PlayCeremonyOnTurnEnd(playerIndex);
            }

            public override async Awaitable PlayCeremonyOnRoundEnd()
            {
                await base.PlayCeremonyOnRoundEnd();
            }

            public override async Awaitable PlayCeremonyOnLevelEnd(PlayerIndex winner)
            {
                await _ctrl.bannerCtrl.Present($"Level {LevelManager.LevelIndex} completes");
                _ctrl.OnLevelOver();
            }
        }
    }
}