using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages player stamina consumption and regeneration.
    /// </summary>
    public class StaminaSystem
    {
        /// <summary>
        /// Consumes stamina from the player.
        /// Stamina cannot go below zero.
        /// </summary>
        /// <param name="player">The player whose stamina will be consumed.</param>
        /// <param name="amount">The amount of stamina to consume.</param>
        public void UseStamina(PlayerEntity player, float amount)
        {
            player.Stamina -= amount;

            if (player.Stamina < 0f)
            {
                player.Stamina = 0f;
            }
        }

        /// <summary>
        /// Regenerates stamina for the player.
        /// Stamina cannot exceed the player's maximum stamina.
        /// </summary>
        /// <param name="player">The player whose stamina will be regenerated.</param>
        /// <param name="amount">The amount of stamina to regenerate.</param>
        public void RegenerateStamina(PlayerEntity player, float amount)
        {
            player.Stamina += amount;

            if (player.Stamina > player.MaxStamina)
            {
                player.Stamina = player.MaxStamina;
            }
        }

        /// <summary>
        /// Checks if the player has enough stamina to sprint.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>True if the player has any stamina remaining; otherwise, false.</returns>
        public bool CanSprint(PlayerEntity player)
        {
            return player.Stamina > 0f;
        }
    }
}
