using System;
using DG.Tweening;
using Roulette.Scripts.General;
using Roulette.Scripts.Models;
using Roulette.Scripts.ViewCtrls;
using TMPro;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette.Scripts.Managers
{
    public class BombManager : MonoBehaviour
    {
        [SerializeField] private BannerCtrl bannerCtrl;
        
        
        //设置移动参数
        private Vector3 _targetPosition = new Vector3(0, 0.5f);
        private float _duration = 1f;
        private Ease _easeType = Ease.Linear;
        private Transform _childTransform;
        private Vector3 _vec;
        
        

        public async Awaitable PrepareBombsForNewRound(int count, int realCount)
        {
            await bannerCtrl.Present($"Bombs: {count}\nTrue Bombs: {realCount}");
            // TODO: 变更雷池中炸弹的数量
            
            Transform queue = transform.Find("Queue");
            Transform current = transform.Find("Current");
            
            
            for (int i = 0; i < count; i++)
            {
                foreach (Transform child in queue)
                {
                    if (child.name.Equals($"Bomb_0 ({i})"))
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
            
            
        }

        public async Awaitable TakeBombForNewTurn(PlayerIndex playerIndex)
        {
            // TODO: 实现移动炸弹的平移动画
            Transform queue = transform.Find("Queue");
            Transform current = transform.Find("Current");
            foreach (Transform child in queue)
            {
                if (child.gameObject.activeSelf)
                {
                    _vec = child.transform.position;
                    _childTransform = child;
                    break;
                }
            }
            _childTransform.DOMove(_targetPosition, _duration).SetEase(_easeType);
            _childTransform.SetParent(current);
            await Dummy.PerformTask($"为{playerIndex}从队列中取出一个炸弹，移动到中央鉴定区");
        }

        public async Awaitable PlayBombEffect(
            PlayerIndex instigator, PlayerIndex target, bool isReal, Action onHit)
        {
            string action = instigator == target
                ? $"{instigator} detonates the bomb."
                : $"{instigator} attacks {target} with the bomb.";
            string result = isReal ? "And it explodes." : "And it doesn't explode.";
            await bannerCtrl.Present($"{action} {result}");
            onHit();
            // TODO: 清空鉴定区
            Transform current = transform.Find("Current");
            Transform queue = transform.Find("Queue");
            foreach (Transform child in current)
            {
                child.gameObject.SetActive(false);
                child.transform.position = _vec;
                child.SetParent(queue);
            }
        }

        public async Awaitable PlayMagnifyingGlassEffect(bool isReal)
        {
            // TODO: 在鉴定区的炸弹旁边显示文字表明其真假
             Transform isRealText = transform.Find("IsRealText");
             TextMeshPro textMeshPro= isRealText.GetComponent<TextMeshPro>();
             textMeshPro.text = isReal.ToString();
             isRealText.gameObject.SetActive(true);
             await Awaitable.WaitForSecondsAsync(1f);
             isRealText.gameObject.SetActive(false);
            
            await Dummy.PerformTask($"The bomb is {isReal}.");
        }

        
    }
}