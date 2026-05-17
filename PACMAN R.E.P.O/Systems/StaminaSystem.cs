using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    public class StaminaSystem
    {
        public void UseStamina(PlayerEntity player, float amount)
        {
            player.Stamina -= amount;

            if (player.Stamina < 0f)
            {
                player.Stamina = 0f;
            }
        }

        public void RegenerateStamina(PlayerEntity player, float amount)
        {
            player.Stamina += amount;

            if (player.Stamina > player.MaxStamina)
            {
                player.Stamina = player.MaxStamina;
            }
        }

        public bool CanSprint(PlayerEntity player)
        {
            return player.Stamina > 0f;
        }
    }
}
