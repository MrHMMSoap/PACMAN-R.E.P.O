using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class PlayerStatsTests
    {
        [TestMethod]
        public void Player_ShouldStartWithDefaultHealth()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(100, player.Health);
        }

        [TestMethod]
        public void Player_ShouldStartWithDefaultMaxHealth()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(100, player.MaxHealth);
        }

        [TestMethod]
        public void Player_ShouldStartWithDefaultStamina()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(100f, player.Stamina);
        }

        [TestMethod]
        public void Player_ShouldStartWithDefaultMaxStamina()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(100f, player.MaxStamina);
        }

        [TestMethod]
        public void Player_ShouldStartWithDefaultBaseSpeed()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(120f, player.BaseSpeed);
        }

        [TestMethod]
        public void Player_ShouldStartWithDefaultMaxCarryWeight()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(10f, player.MaxCarryWeight);
        }

        [TestMethod]
        public void Player_ShouldStartWithZeroMoney()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(0, player.Money);
        }

        [TestMethod]
        public void Player_ShouldStartWithZeroUpgradeLevels()
        {
            PlayerEntity player = new PlayerEntity();

            Assert.AreEqual(0, player.SpeedLevel);
            Assert.AreEqual(0, player.StrengthLevel);
            Assert.AreEqual(0, player.StaminaLevel);
            Assert.AreEqual(0, player.HealthLevel);
        }
    }
}
