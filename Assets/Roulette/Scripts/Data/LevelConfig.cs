using System;
using System.Collections.Generic;
using System.Linq;
using Roulette.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Roulette.Scripts.Data
{
    /// 单个关卡的配置
    [CreateAssetMenu(menuName = "Scriptable Object/Level Config", fileName = "level")]
    public class LevelConfig : ScriptableObject
    {
        public int initialHealth = 2;

        public List<bool> bullets = new(new[] { false, true, false });

        public int itemCountPerRound = 1;

        [Serializable]
        public struct ItemWeight
        {
            public ItemType item;
            public float weight;
        }

        public List<ItemWeight> itemWeights = new(new[]
        {
            new ItemWeight { item = ItemType.MagnifyingGlass, weight = 1.0f }
        });

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