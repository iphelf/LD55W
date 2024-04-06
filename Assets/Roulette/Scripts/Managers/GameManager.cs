using Roulette.Scripts.ViewCtrls;
using UnityEngine;

namespace Roulette.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public BombManager bombManager;
        public CardManager cardManager;
        public ActionsCtrl actionsCtrl;
        public StatusCtrl statusCtrl;
        public BuffsCtrl p1BuffsCtrl;
        public BuffsCtrl p2BuffsCtrl;
    }
}