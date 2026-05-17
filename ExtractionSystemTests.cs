using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Items;
using PACMAN_R.E.P.O.Entities;
using PACMAN_R.E.P.O.Systems;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class ExtractionSystemTests
    {
        [TestMethod]
        public void ExtractItem_ShouldIncreasePlayerMoney()
        {
            Player player = new Player();
            player.Money = 0;

            Item item = new Item("Golden Cup", 5f, 100);

            ExtractionSystem extractionSystem = new ExtractionSystem();

            extractionSystem.ExtractItem(player, item);

            Assert.AreEqual(100, player.Money);
        }

        [TestMethod]
        public void ExtractItem_ShouldIncreaseExtractedValue()
        {
            Player player = new Player();
            Item item = new Item("Golden Cup", 5f, 100);

            ExtractionSystem extractionSystem = new ExtractionSystem();

            extractionSystem.ExtractItem(player, item);

            Assert.AreEqual(100, extractionSystem.ExtractedValue);
        }

        [TestMethod]
        public void ExtractItem_ShouldDecreasePlayerCarriedWeight()
        {
            Player player = new Player();
            player.CarriedWeight = 5f;

            Item item = new Item("Heavy Item", 5f, 100);

            ExtractionSystem extractionSystem = new ExtractionSystem();

            extractionSystem.ExtractItem(player, item);

            Assert.AreEqual(0f, player.CarriedWeight);
        }

        [TestMethod]
        public void ExtractionGoal_ShouldBeComplete_WhenEnoughValueIsExtracted()
        {
            ExtractionSystem extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = 500;
            extractionSystem.ExtractedValue = 400;

            extractionSystem.AddExtractedValue(100);

            Assert.IsTrue(extractionSystem.IsGoalComplete());
        }

        [TestMethod]
        public void ExtractionGoal_ShouldNotBeComplete_WhenValueIsTooLow()
        {
            ExtractionSystem extractionSystem = new ExtractionSystem();
            extractionSystem.RequiredValue = 500;
            extractionSystem.ExtractedValue = 300;

            Assert.IsFalse(extractionSystem.IsGoalComplete());
        }
    }
}
