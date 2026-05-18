namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages round progression and calculates difficulty scaling.
    /// Determines extraction requirements, monster difficulty, and item value targets per round.
    /// </summary>
    public class RoundManager
    {
        /// <summary>Gets the current round number.</summary>
        public int CurrentRound { get; private set; } = 1;

        /// <summary>Gets the base extraction requirement for round 1.</summary>
        public int BaseExtractionRequirement { get; private set; } = 500;

        /// <summary>Gets the increase in extraction requirement per round.</summary>
        public int ExtractionIncreasePerRound { get; private set; } = 250;

        /// <summary>
        /// Sets the current round number.
        /// </summary>
        /// <param name="round">The round number to set (minimum of 1).</param>
        public void SetRound(int round)
        {
            if (round < 1)
            {
                CurrentRound = 1;
                return;
            }

            CurrentRound = round;
        }

        /// <summary>
        /// Advances to the next round.
        /// </summary>
        public void NextRound()
        {
            CurrentRound++;
        }

        /// <summary>
        /// Resets the round counter back to 1.
        /// </summary>
        public void ResetRounds()
        {
            CurrentRound = 1;
        }

        /// <summary>
        /// Calculates the minimum item value required to complete the current round.
        /// Increases linearly with each round.
        /// </summary>
        /// <returns>The extraction requirement for the current round.</returns>
        public int GetExtractionRequirement()
        {
            return BaseExtractionRequirement + ((CurrentRound - 1) * ExtractionIncreasePerRound);
        }

        /// <summary>
        /// Calculates the monster difficulty level based on the current round.
        /// Difficulty increases every 3 rounds.
        /// </summary>
        /// <returns>The difficulty level for spawning monsters.</returns>
        public int GetMonsterDifficultyLevel()
        {
            return 1 + ((CurrentRound - 1) / 3);
        }

        /// <summary>
        /// Calculates the extra item value margin beyond the extraction requirement.
        /// This margin decreases as rounds progress, making later rounds tighter.
        /// </summary>
        /// <returns>The extra value margin available in the current round.</returns>
        public int GetExtraItemValueMargin()
        {
            int extraValue = 200 - ((CurrentRound - 1) * 10);

            if (extraValue < 0)
            {
                extraValue = 0;
            }

            return extraValue;
        }

        /// <summary>
        /// Calculates the total item value target (extraction requirement plus extra margin).
        /// </summary>
        /// <returns>The total target value of items that should be available in the current round.</returns>
        public int GetTotalItemValueTarget()
        {
            return GetExtractionRequirement() + GetExtraItemValueMargin();
        }
    }
}
