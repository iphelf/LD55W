﻿using Roulette.Scripts.Data;
using Roulette.Scripts.Models;

namespace Roulette.Scripts.Managers
{
    public class LevelManager
    {
        public static LevelDemo Current => _instance._current;
        public static void InitializeOnce(LevelConfig config) => _instance ??= new LevelManager(config);

        public static bool SwitchToNext()
        {
            return _instance.SwitchToNextLevel();
        }

        private static LevelManager _instance;

        private LevelDemo _current;
        private readonly LevelConfig _config;
        private int _levelCount = 2;

        private LevelManager(LevelConfig config)
        {
            _config = config;
            _current = new LevelDemo(config);
        }

        private bool SwitchToNextLevel()
        {
            --_levelCount;
            if (_levelCount <= 0)
            {
                _current = null;
                return false;
            }
            else
            {
                _current = new LevelDemo(_config);
                return true;
            }
        }
    }
}