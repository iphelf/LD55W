using UnityEngine;

namespace Roulette.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BombManager bombManager;
        [SerializeField] private CardManager cardManager;
        [SerializeField] private ActionsCtrl actionsCtrl;
        [SerializeField] private StatusCtrl statusCtrl;
        [SerializeField] private BuffsCtrl p1BuffsCtrl;
        [SerializeField] private BuffsCtrl p2BuffsCtrl;
    }
}