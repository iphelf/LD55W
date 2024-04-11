using Roulette.Scripts.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Roulette.Scripts.ViewCtrls
{
    public class ActionsCtrl : MonoBehaviour
    {
        public void EnableActionInput()
        {
            // TODO: 提示玩家输入
            Debug.Log("按下S选择炸自己；按下O选择炸对方。");
        }

        public void DisableActionInput()
        {
            // TODO: 禁止玩家输入
        }

        private void Update()
        {
            // 功能完成后删除下面的草稿
            if (Input.GetKey(KeyCode.S))
                onPlayerAction.Invoke(new PlayerFiresGun(PlayerIndex.P1));
            if (Input.GetKey(KeyCode.O))
                onPlayerAction.Invoke(new PlayerFiresGun(PlayerIndex.P2));
        }

        // TODO: 在玩家输入后发出事件
        public UnityEvent<PlayerFiresGun> onPlayerAction = new();
    }
}