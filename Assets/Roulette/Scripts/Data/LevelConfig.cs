using System;
using System.Collections.Generic;
using System.Linq;
using Roulette.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Roulette.Scripts.Data
{
    [CreateAssetMenu(menuName = "Scriptable Object/Level Config", fileName = "level")]
    public class LevelConfig : ScriptableObject
    {
        public int itemCountPerRound = 1;

        [Serializable]
        public class ItemWeight
        {
            public ItemType item;
            public float weight;
        }

        public List<ItemWeight> itemWeights = new();
        public int itemCapacity = 8;

        private float _totalWeight;

        private void OnEnable()
        {
            _totalWeight = itemWeights.Select(t => t.weight).Sum();
        }

        public ItemType SampleItem()
        {
            float sample = Random.Range(0f, _totalWeight);
            foreach (var t in itemWeights)
            {
                if (sample <= t.weight) return t.item;
                sample -= t.weight;
            }

            return itemWeights[^1].item;
        }
    }
}