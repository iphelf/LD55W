using Roulette.Scripts.Managers;
using Roulette.Scripts.Models;
using Roulette.Scripts.ViewCtrls;
using UnityEngine;

namespace Roulette.Scripts.SceneCtrls
{
    public class LevelCtrl : MonoBehaviour
    {
        public BombManager bombManager;
        public CardManager cardManager;
        public ActionsCtrl actionsCtrl;
        public StatusCtrl statusCtrl;
        public BuffsCtrl p1BuffsCtrl;
        public BuffsCtrl p2BuffsCtrl;

        private class Presentation : LevelPresentation
        {
        }
    }
}