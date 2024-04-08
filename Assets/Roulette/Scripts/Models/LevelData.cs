using System.Collections.Generic;
using Roulette.Scripts.General;

namespace Roulette.Scripts.Models
{
    // 关卡中的一切数据
    public record LevelData
    {
        public PlayerData Player1;
        public PlayerData Player2;
        public RoundData Round;
        public LevelStep Step;
    }

    public record PlayerData
    {
        public int Health = 1;
        public SortedDictionary<int, ItemType> Items = new();
        public bool IsHandCuffed = false;
    }

    public record RoundData
    {
        public BulletQueue BulletQueue;
        public PlayerIndex Turn = PlayerIndex.None;
    }
}