using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Systems;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class ShopManagerTests
    {
        [TestMethod]
        public void BuySpeedUpgrade_ShouldIncreaseSpeedLevel_WhenPlayerHasEnoughMoney()
        {
            PlayerEntity player = new PlayerEntity();
            player.Money = 500;
            player.SpeedLevel = 0;
            player.BaseSpeed = 120f;

            ShopManager shopManager = new ShopManager();

            bool bought = shopManager.BuySpeedUpgrade(player);

            Assert.IsTrue(bought);
            Assert.AreEqual(1, player.SpeedLevel);
            Assert.AreEqual(130f, player.BaseSpeed);
            Assert.AreEqual(400, player.Money);
        }

        [TestMethod]
        public void BuySpeedUpgrade_ShouldFail_WhenPlayerDoesNotHaveEnoughMoney()
        {
            PlayerEntity player = new PlayerEntity();
            player.Money = 0;
            player.SpeedLevel = 0;
            player.BaseSpeed = 120f;

            ShopManager shopManager = new ShopManager();

            bool bought = shopManager.BuySpeedUpgrade(player);

            Assert.IsFalse(bought);
            Assert.AreEqual(0, player.SpeedLevel);
            Assert.AreEqual(120f, player.BaseSpeed);
            Assert.AreEqual(0, player.Money);
        }

        [TestMethod]
        public void BuyStrengthUpgrade_ShouldIncreaseStrengthLevel_WhenPlayerHasEnoughMoney()
        {
            PlayerEntity player = new PlayerEntity();
            player.Money = 500;
            player.StrengthLevel = 0;
            player.MaxCarryWeight = 10f;

            ShopManager shopManager = new ShopManager();

            bool bought = shopManager.BuyStrengthUpgrade(player);

            Assert.IsTrue(bought);
            Assert.AreEqual(1, player.StrengthLevel);
            Assert.AreEqual(12f, player.MaxCarryWeight);
            Assert.AreEqual(400, player.Money);
        }

        [TestMethod]
        public void BuyStrengthUpgrade_ShouldFail_WhenPlayerDoesNotHaveEnoughMoney()
        {
            PlayerEntity player = new PlayerEntity();
            player.Money = 0;
            player.StrengthLevel = 0;
            player.MaxCarryWeight = 10f;

            ShopManager shopManager = new ShopManager();

            bool bought = shopManager.BuyStrengthUpgrade(player);

            Assert.IsFalse(bought);
            Assert.AreEqual(0, player.StrengthLevel);
            Assert.AreEqual(10f, player.MaxCarryWeight);
            Assert.AreEqual(0, player.Money);
        }

        [TestMethod]
        public void UpgradeCost_ShouldIncrease_WhenLevelIncreases()
        {
            ShopManager shopManager = new ShopManager();

            int level0Cost = shopManager.GetUpgradeCost(0);
            int level3Cost = shopManager.GetUpgradeCost(3);

            Assert.IsGreaterThan(level0Cost, level3Cost);
        }
    }
}
