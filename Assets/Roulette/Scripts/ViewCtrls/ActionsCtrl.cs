using System;
using NUnit.Framework;
using Roulette.Scripts.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Roulette.Scripts.ViewCtrls
{
    public class ActionsCtrl : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject contentAttack;
        [SerializeField] private GameObject contentDenonate;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button denonateButton;

        private void Start()
        {
            attackButton.onClick.AddListener(OnAttackClicked);
            denonateButton.onClick.AddListener(OnDenonateClicked);
        }

        public void EnableActionInput()
        {
            content.SetActive(true);
            contentAttack.SetActive(true);
            contentDenonate.SetActive(true);
            // TODO: 提示玩家输入
            Debug.Log("按下S选择炸自己；按下O选择炸对方。");
        }

        public void DisableActionInput()
        {
            content.SetActive(false);
            contentAttack.SetActive(false);
            contentDenonate.SetActive(false);
            // TODO: 禁止玩家输入
        }

        void OnAttackClicked()
        {
            onPlayerAction.Invoke(new PlayerFiresGun(PlayerIndex.P2));
        }

        void OnDenonateClicked()
        {
            onPlayerAction.Invoke(new PlayerFiresGun(PlayerIndex.P1));
        }
        
        // TODO: 在玩家输入后发出事件
        public UnityEvent<PlayerFiresGun> onPlayerAction = new();
    }
}