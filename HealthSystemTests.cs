using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Systems;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class HealthSystemTests
    {
        [TestMethod]
        public void TakeDamage_ShouldDecreasePlayerHealth()
        {
            PlayerEntity player = new PlayerEntity();
            player.Health = 100;

            HealthSystem healthSystem = new HealthSystem();

            healthSystem.TakeDamage(player, 25);

            Assert.AreEqual(75, player.Health);
        }

        [TestMethod]
        public void TakeDamage_ShouldNotLetHealthGoBelowZero()
        {
            PlayerEntity player = new PlayerEntity();
            player.Health = 50;

            HealthSystem healthSystem = new HealthSystem();

            healthSystem.TakeDamage(player, 999);

            Assert.AreEqual(0, player.Health);
        }

        [TestMethod]
        public void IsDead_ShouldReturnFalse_WhenHealthIsAboveZero()
        {
            PlayerEntity player = new PlayerEntity();
            player.Health = 10;

            HealthSystem healthSystem = new HealthSystem();

            bool isDead = healthSystem.IsDead(player);

            Assert.IsFalse(isDead);
        }

        [TestMethod]
        public void IsDead_ShouldReturnTrue_WhenHealthIsZero()
        {
            PlayerEntity player = new PlayerEntity();
            player.Health = 0;

            HealthSystem healthSystem = new HealthSystem();

            bool isDead = healthSystem.IsDead(player);

            Assert.IsTrue(isDead);
        }

        [TestMethod]
        public void Heal_ShouldIncreasePlayerHealth()
        {
            PlayerEntity player = new PlayerEntity();
            player.Health = 50;
            player.MaxHealth = 100;

            HealthSystem healthSystem = new HealthSystem();

            healthSystem.Heal(player, 25);

            Assert.AreEqual(75, player.Health);
        }

        [TestMethod]
        public void Heal_ShouldNotLetHealthGoAboveMaxHealth()
        {
            PlayerEntity player = new PlayerEntity();
            player.Health = 90;
            player.MaxHealth = 100;

            HealthSystem healthSystem = new HealthSystem();

            healthSystem.Heal(player, 50);

            Assert.AreEqual(100, player.Health);
        }
    }
}
