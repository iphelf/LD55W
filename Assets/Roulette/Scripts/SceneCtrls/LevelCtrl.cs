using System;
using System.Collections.Generic;
using Roulette.Scripts.General;
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

        private async void Start()
        {
            Presentation presentation = new(this);

            GraphicalPlayerInput playerInput = new GraphicalPlayerInput(presentation, PlayerIndex.P1, this);
            presentation.BindPlayerInput(PlayerIndex.P1, playerInput);
            BasicAIPlayerInput aiPlayerInput = new BasicAIPlayerInput(presentation, PlayerIndex.P2);
            presentation.BindPlayerInput(PlayerIndex.P2, aiPlayerInput);

            await LevelDriver.Drive(LevelManager.Current, presentation);
        }

        private void OnLevelOver()
        {
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

            public override async Awaitable PrepareBombsForNewRound(int count)
            {
                await base.PrepareBombsForNewRound(count);
            }

            public override async Awaitable DrawCardFromDeck(PlayerIndex playerIndex, ItemType card)
            {
                await base.DrawCardFromDeck(playerIndex, card);
            }

            public override async Awaitable AppendCard(
                PlayerIndex playerIndex, int existingCardCount, ItemType newCard)
            {
                await base.AppendCard(playerIndex, existingCardCount, newCard);
            }

            public override async Awaitable PlayCeremonyOnTurnBegin(PlayerIndex playerIndex)
            {
                await _ctrl.bannerCtrl.Present($"{playerIndex}'s turn");
            }

            public override async Awaitable RegretfullyDisposeLastDrawnCard(PlayerIndex playerIndex)
            {
                await base.RegretfullyDisposeLastDrawnCard(playerIndex);
            }

            public override async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex, BulletQueue bulletQueue)
            {
                await base.TakeBombForNewTurn(playerIndex, bulletQueue);
            }

            protected override async Awaitable ConsumeCardAndPlayEffect(PlayerIndex playerIndex, int itemIndex,
                ItemEffect itemEffect, Action onHit = null)
            {
                await base.ConsumeCardAndPlayEffect(playerIndex, itemIndex, itemEffect, onHit);
            }

            protected override async Awaitable PlayBombEffect(PlayerIndex instigator, PlayerIndex target, bool isReal,
                Action onHit)
            {
                await base.PlayBombEffect(instigator, target, isReal, onHit);
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