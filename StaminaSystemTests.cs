using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Systems;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class StaminaSystemTests
    {
        [TestMethod]
        public void UseStamina_ShouldDecreasePlayerStamina()
        {
            PlayerEntity player = new PlayerEntity();
            player.Stamina = 100f;

            StaminaSystem staminaSystem = new StaminaSystem();

            staminaSystem.UseStamina(player, 25f);

            Assert.AreEqual(75f, player.Stamina);
        }

        [TestMethod]
        public void UseStamina_ShouldNotLetStaminaGoBelowZero()
        {
            PlayerEntity player = new PlayerEntity();
            player.Stamina = 10f;

            StaminaSystem staminaSystem = new StaminaSystem();

            staminaSystem.UseStamina(player, 999f);

            Assert.AreEqual(0f, player.Stamina);
        }

        [TestMethod]
        public void RegenerateStamina_ShouldIncreasePlayerStamina()
        {
            PlayerEntity player = new PlayerEntity();
            player.Stamina = 50f;
            player.MaxStamina = 100f;

            StaminaSystem staminaSystem = new StaminaSystem();

            staminaSystem.RegenerateStamina(player, 25f);

            Assert.AreEqual(75f, player.Stamina);
        }

        [TestMethod]
        public void RegenerateStamina_ShouldNotGoAboveMaxStamina()
        {
            PlayerEntity player = new PlayerEntity();
            player.Stamina = 90f;
            player.MaxStamina = 100f;

            StaminaSystem staminaSystem = new StaminaSystem();

            staminaSystem.RegenerateStamina(player, 50f);

            Assert.AreEqual(100f, player.Stamina);
        }

        [TestMethod]
        public void CanSprint_ShouldReturnTrue_WhenPlayerHasStamina()
        {
            PlayerEntity player = new PlayerEntity();
            player.Stamina = 10f;

            StaminaSystem staminaSystem = new StaminaSystem();

            bool canSprint = staminaSystem.CanSprint(player);

            Assert.IsTrue(canSprint);
        }

        [TestMethod]
        public void CanSprint_ShouldReturnFalse_WhenPlayerHasNoStamina()
        {
            PlayerEntity player = new PlayerEntity();
            player.Stamina = 0f;

            StaminaSystem staminaSystem = new StaminaSystem();

            bool canSprint = staminaSystem.CanSprint(player);

            Assert.IsFalse(canSprint);
        }
    }
}
