using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages the shop system where players can purchase upgrades and health packs between rounds.
    /// </summary>
    public class ShopManager
    {
        /// <summary>
        /// Calculates the cost of an upgrade based on the current level.
        /// Cost increases linearly with each upgrade.
        /// </summary>
        /// <param name="currentLevel">The current upgrade level (0 for first upgrade).</param>
        /// <returns>The cost of the next upgrade.</returns>
        public int GetUpgradeCost(int currentLevel)
        {
            return 100 + currentLevel * 75;
        }

        /// <summary>
        /// Gets the cost of a health pack.
        /// </summary>
        /// <returns>The fixed cost of a health pack.</returns>
        public int GetHealthPackCost()
        {
            return 50;
        }

        /// <summary>
        /// Attempts to purchase a speed upgrade for the player.
        /// Increases base speed by 10 pixels per second.
        /// </summary>
        /// <param name="player">The player making the purchase.</param>
        /// <returns>True if the upgrade was purchased successfully; false if the player cannot afford it.</returns>
        public bool BuySpeedUpgrade(PlayerEntity player)
        {
            int cost = GetUpgradeCost(player.SpeedLevel);

            if (player.Money < cost)
            {
                return false;
            }

            player.Money -= cost;
            player.SpeedLevel++;
            player.BaseSpeed += 10f;

            return true;
        }

        /// <summary>
        /// Attempts to purchase a strength upgrade for the player.
        /// Increases carry capacity by 2 weight units and reduces movement penalty from weight.
        /// </summary>
        /// <param name="player">The player making the purchase.</param>
        /// <returns>True if the upgrade was purchased successfully; false if the player cannot afford it.</returns>
        public bool BuyStrengthUpgrade(PlayerEntity player)
        {
            int cost = GetUpgradeCost(player.StrengthLevel);

            if (player.Money < cost)
            {
                return false;
            }

            player.Money -= cost;
            player.StrengthLevel++;
            player.MaxCarryWeight += 2f;

            return true;
        }

        /// <summary>
        /// Attempts to purchase a stamina upgrade for the player.
        /// Increases maximum stamina by 20 and fully restores stamina.
        /// </summary>
        /// <param name="player">The player making the purchase.</param>
        /// <returns>True if the upgrade was purchased successfully; false if the player cannot afford it.</returns>
        public bool BuyStaminaUpgrade(PlayerEntity player)
        {
            int cost = GetUpgradeCost(player.StaminaLevel);

            if (player.Money < cost)
            {
                return false;
            }

            player.Money -= cost;
            player.StaminaLevel++;
            player.MaxStamina += 20f;
            player.Stamina = player.MaxStamina;

            return true;
        }

        /// <summary>
        /// Attempts to purchase a health upgrade for the player.
        /// Increases maximum health by 20 and fully restores health.
        /// </summary>
        /// <param name="player">The player making the purchase.</param>
        /// <returns>True if the upgrade was purchased successfully; false if the player cannot afford it.</returns>
        public bool BuyHealthUpgrade(PlayerEntity player)
        {
            int cost = GetUpgradeCost(player.HealthLevel);

            if (player.Money < cost)
            {
                return false;
            }

            player.Money -= cost;
            player.HealthLevel++;
            player.MaxHealth += 20;
            player.Health = player.MaxHealth;

            return true;
        }

        /// <summary>
        /// Attempts to purchase a health pack that restores 50 health.
        /// Cannot be purchased if the player is already at max health.
        /// </summary>
        /// <param name="player">The player making the purchase.</param>
        /// <returns>True if the health pack was purchased successfully; false if the player cannot afford it or is at max health.</returns>
        public bool BuyHealthPack(PlayerEntity player)
        {
            int cost = GetHealthPackCost();

            if (player.Money < cost)
            {
                return false;
            }

            // Cannot buy if already at full health
            if (player.Health >= player.MaxHealth)
            {
                return false;
            }

            player.Money -= cost;
            player.Health += 50;

            // Cap health at maximum
            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }

            return true;
        }
    }
}
