using System;
using System.Collections.Generic;
using Roulette.Scripts.Models;
using UnityEngine;

namespace Roulette.Scripts.Data
{
    [CreateAssetMenu(menuName = "Scriptable Object/Level Config", fileName = "level")]
    public class LevelConfig : ScriptableObject
    {
        public int itemCountPerRound = 1;
        public List<Tuple<ItemType, int>> ItemPool = new();
    }
}