﻿using Roulette.Scripts.General;
using Roulette.Scripts.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Roulette.Scripts.Managers
{
    public class CardManager : MonoBehaviour
    {
        public void EnableActionInput()
        {
            // TODO: 提示玩家输入
        }

        public void DisableActionInput()
        {
            // TODO: 禁止玩家输入
        }

        // TODO: 在玩家输入后发出事件
        public UnityEvent<PlayerUsesItem> onPlayerAction;

        public async Awaitable DrawCards()
        {
            await Dummy.PerformTask("玩家抽卡");
            await Awaitables.WhenAll(DrawToP1Hand(), DrawToP2Hand());
        }

        private async Awaitable DrawToP1Hand()
        {
            await Dummy.PerformTask("从牌堆抽牌，置入P1手牌区");
        }

        private async Awaitable DrawToP2Hand()
        {
            await Dummy.PerformTask("从牌堆抽牌，置入P2手牌区");
        }
    }
}