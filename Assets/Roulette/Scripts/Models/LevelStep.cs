namespace Roulette.Scripts.Models
{
    public record LevelStep
    {
        public int Level { get; private set; }
        public int Round { get; private set; }
        public int Turn { get; private set; }

        public LevelStep() => Clear();

        public LevelStep Copy()
        {
            var clone = new LevelStep
            {
                Level = Level,
                Round = Round,
                Turn = Turn
            };
            return clone;
        }

        public void Clear()
        {
            Level = 0;
            Round = 0;
            Turn = 0;
        }

        public void NewLevel(int? level = null)
        {
            if (level is null)
                ++Level;
            else
                Level = level.Value;
            Round = 0;
            Turn = 0;
        }

        public void NewRound()
        {
            ++Round;
            Turn = 0;
        }

        public void NewTurn()
        {
            ++Turn;
        }
    }
}