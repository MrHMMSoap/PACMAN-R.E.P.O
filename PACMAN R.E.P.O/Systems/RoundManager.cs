namespace PACMAN_R.E.P.O.Systems
{
    public class RoundManager
    {
        public int CurrentRound { get; private set; } = 1;

        public int BaseExtractionRequirement { get; private set; } = 500;
        public int ExtractionIncreasePerRound { get; private set; } = 250;

        public void NextRound()
        {
            CurrentRound++;
        }

        public void ResetRounds()
        {
            CurrentRound = 1;
        }

        public int GetExtractionRequirement()
        {
            return BaseExtractionRequirement + ((CurrentRound - 1) * ExtractionIncreasePerRound);
        }

        public int GetMonsterDifficultyLevel()
        {
            return 1 + ((CurrentRound - 1) / 3);
        }

        public int GetExtraItemValueMargin()
        {
            int extraValue = 200 - ((CurrentRound - 1) * 10);

            if (extraValue < 0)
            {
                extraValue = 0;
            }

            return extraValue;
        }

        public int GetTotalItemValueTarget()
        {
            return GetExtractionRequirement() + GetExtraItemValueMargin();
        }
    }
}
