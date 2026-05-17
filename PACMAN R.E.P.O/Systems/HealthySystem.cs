using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    public class HealthSystem
    {
        public void TakeDamage(PlayerEntity player, int damage)
        {
            player.Health -= damage;

            if (player.Health < 0)
            {
                player.Health = 0;
            }
        }

        public void Heal(PlayerEntity player, int amount)
        {
            player.Health += amount;

            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }
        }

        public bool IsDead(PlayerEntity player)
        {
            return player.Health <= 0;
        }
    }
}
