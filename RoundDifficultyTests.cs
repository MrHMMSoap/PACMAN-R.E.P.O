using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Systems;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class RoundDifficultyTests
    {
        [TestMethod]
        public void ExtractionRequirement_ShouldBeBaseValue_OnRoundOne()
        {
            RoundManager roundManager = new RoundManager();

            int requirement = roundManager.GetExtractionRequirement();

            Assert.AreEqual(500, requirement);
        }

        [TestMethod]
        public void ExtractionRequirement_ShouldIncreaseByTwoHundredFifty_PerRound()
        {
            RoundManager roundManager = new RoundManager();

            roundManager.NextRound(); // Round 2

            int requirement = roundManager.GetExtractionRequirement();

            Assert.AreEqual(750, requirement);
        }

        [TestMethod]
        public void ExtractionRequirement_ShouldBeCorrect_OnRoundFive()
        {
            RoundManager roundManager = new RoundManager();

            roundManager.NextRound(); // Round 2
            roundManager.NextRound(); // Round 3
            roundManager.NextRound(); // Round 4
            roundManager.NextRound(); // Round 5

            int requirement = roundManager.GetExtractionRequirement();

            Assert.AreEqual(1500, requirement);
        }

        [TestMethod]
        public void MonsterDifficulty_ShouldBeOne_OnRoundOne()
        {
            RoundManager roundManager = new RoundManager();

            int difficulty = roundManager.GetMonsterDifficultyLevel();

            Assert.AreEqual(1, difficulty);
        }

        [TestMethod]
        public void MonsterDifficulty_ShouldStillBeOne_OnRoundThree()
        {
            RoundManager roundManager = new RoundManager();

            roundManager.NextRound(); // Round 2
            roundManager.NextRound(); // Round 3

            int difficulty = roundManager.GetMonsterDifficultyLevel();

            Assert.AreEqual(1, difficulty);
        }

        [TestMethod]
        public void MonsterDifficulty_ShouldBeTwo_OnRoundFour()
        {
            RoundManager roundManager = new RoundManager();

            roundManager.NextRound(); // Round 2
            roundManager.NextRound(); // Round 3
            roundManager.NextRound(); // Round 4

            int difficulty = roundManager.GetMonsterDifficultyLevel();

            Assert.AreEqual(2, difficulty);
        }

        [TestMethod]
        public void MonsterDifficulty_ShouldBeThree_OnRoundSeven()
        {
            RoundManager roundManager = new RoundManager();

            roundManager.NextRound(); // Round 2
            roundManager.NextRound(); // Round 3
            roundManager.NextRound(); // Round 4
            roundManager.NextRound(); // Round 5
            roundManager.NextRound(); // Round 6
            roundManager.NextRound(); // Round 7

            int difficulty = roundManager.GetMonsterDifficultyLevel();

            Assert.AreEqual(3, difficulty);
        }
    }
}
