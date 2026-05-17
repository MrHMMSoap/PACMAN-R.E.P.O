using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Systems;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class RoundManagerTests
    {
        [TestMethod]
        public void RoundManager_ShouldStartAtRoundOne()
        {
            RoundManager roundManager = new RoundManager();

            Assert.AreEqual(1, roundManager.CurrentRound);
        }

        [TestMethod]
        public void NextRound_ShouldIncreaseCurrentRound()
        {
            RoundManager roundManager = new RoundManager();

            roundManager.NextRound();

            Assert.AreEqual(2, roundManager.CurrentRound);
        }

        [TestMethod]
        public void ExtractionRequirement_ShouldIncrease_WhenRoundIncreases()
        {
            RoundManager roundManager = new RoundManager();

            int roundOneRequirement = roundManager.GetExtractionRequirement();

            roundManager.NextRound();

            int roundTwoRequirement = roundManager.GetExtractionRequirement();

            Assert.IsTrue(roundTwoRequirement > roundOneRequirement);
        }

        [TestMethod]
        public void MonsterDifficulty_ShouldIncrease_AfterThreeRounds()
        {
            RoundManager roundManager = new RoundManager();

            int difficultyRoundOne = roundManager.GetMonsterDifficultyLevel();

            roundManager.NextRound(); // Round 2
            roundManager.NextRound(); // Round 3
            roundManager.NextRound(); // Round 4

            int difficultyRoundFour = roundManager.GetMonsterDifficultyLevel();

            Assert.IsTrue(difficultyRoundFour > difficultyRoundOne);
        }

        [TestMethod]
        public void ResetRounds_ShouldReturnToRoundOne()
        {
            RoundManager roundManager = new RoundManager();

            roundManager.NextRound();
            roundManager.NextRound();

            roundManager.ResetRounds();

            Assert.AreEqual(1, roundManager.CurrentRound);
        }
    }
}