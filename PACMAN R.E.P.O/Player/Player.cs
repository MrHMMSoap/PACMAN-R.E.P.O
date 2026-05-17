using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACMAN_R.E.P.O.Entities
{
    public class Player
    {
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;

        public float Stamina { get; set; } = 100f;
        public float MaxStamina { get; set; } = 100f;

        public float BaseSpeed { get; set; } = 120f;

        public float CarriedWeight { get; set; } = 0f;
        public float MaxCarryWeight { get; set; } = 10f;

        public int Money { get; set; } = 0;

        public int SpeedLevel { get; set; } = 0;
        public int StrengthLevel { get; set; } = 0;
        public int StaminaLevel { get; set; } = 0;
        public int HealthLevel { get; set; } = 0;

        public float CalculateSpeed()
        {
            float strengthReduction = StrengthLevel * 0.1f;
            float weightPenalty = CarriedWeight * (1f - strengthReduction);

            float speed = BaseSpeed - weightPenalty * 8f;

            return Math.Max(speed, 40f);
        }

        public bool CanCarry(float itemWeight)
        {
            return CarriedWeight + itemWeight <= MaxCarryWeight;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health < 0)
                Health = 0;
        }
    }
}
