using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    public class ShopManager
    {
        public int GetUpgradeCost(int currentLevel)
        {
            return 100 + currentLevel * 75;
        }

        public int GetHealthPackCost()
        {
            return 50;
        }

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

        public bool BuyHealthPack(PlayerEntity player)
        {
            int cost = GetHealthPackCost();

            if (player.Money < cost)
            {
                return false;
            }

            if (player.Health >= player.MaxHealth)
            {
                return false;
            }

            player.Money -= cost;
            player.Health += 50;

            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }

            return true;
        }
    }
}
