using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages player health, damage, healing, and death checks.
    /// </summary>
    public class HealthSystem
    {
        /// <summary>
        /// Applies damage to the player.
        /// Health cannot go below zero.
        /// </summary>
        /// <param name="player">The player who will take damage.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        public void TakeDamage(PlayerEntity player, int damage)
        {
            player.Health -= damage;

            if (player.Health < 0)
            {
                player.Health = 0;
            }
        }

        /// <summary>
        /// Heals the player by the specified amount.
        /// Health cannot exceed the player's maximum health.
        /// </summary>
        /// <param name="player">The player to heal.</param>
        /// <param name="amount">The amount of health to restore.</param>
        public void Heal(PlayerEntity player, int amount)
        {
            player.Health += amount;

            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }
        }

        /// <summary>
        /// Checks if the player is dead (health at or below zero).
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>True if the player is dead; otherwise, false.</returns>
        public bool IsDead(PlayerEntity player)
        {
            return player.Health <= 0;
        }
    }
}
