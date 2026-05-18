using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACMAN_R.E.P.O.Entities
{
    /// <summary>
    /// Represents the player character with health, stamina, carry capacity, money, and upgrade levels.
    /// </summary>
    public class Player
    {
        /// <summary>Gets or sets the player's current health points.</summary>
        public int Health { get; set; } = 100;

        /// <summary>Gets or sets the player's maximum health points.</summary>
        public int MaxHealth { get; set; } = 100;

        /// <summary>Gets or sets the player's current stamina.</summary>
        public float Stamina { get; set; } = 100f;

        /// <summary>Gets or sets the player's maximum stamina.</summary>
        public float MaxStamina { get; set; } = 100f;

        /// <summary>Gets or sets the player's base movement speed (pixels per second).</summary>
        public float BaseSpeed { get; set; } = 120f;

        /// <summary>Gets or sets the total weight of items currently carried by the player.</summary>
        public float CarriedWeight { get; set; } = 0f;

        /// <summary>Gets or sets the maximum weight the player can carry.</summary>
        public float MaxCarryWeight { get; set; } = 10f;

        /// <summary>Gets or sets the player's current money balance.</summary>
        public int Money { get; set; } = 0;

        /// <summary>Gets or sets the player's speed upgrade level.</summary>
        public int SpeedLevel { get; set; } = 0;

        /// <summary>Gets or sets the player's strength upgrade level (reduces weight penalty).</summary>
        public int StrengthLevel { get; set; } = 0;

        /// <summary>Gets or sets the player's stamina upgrade level.</summary>
        public int StaminaLevel { get; set; } = 0;

        /// <summary>Gets or sets the player's health upgrade level.</summary>
        public int HealthLevel { get; set; } = 0;

        /// <summary>
        /// Calculates the player's effective speed based on carried weight and strength.
        /// Speed is reduced by weight, but strength upgrades reduce the penalty.
        /// </summary>
        /// <returns>The effective movement speed, capped at a minimum of 40.</returns>
        public float CalculateSpeed()
        {
            // Strength reduces weight penalty by 10% per level
            float strengthReduction = StrengthLevel * 0.1f;
            float weightPenalty = CarriedWeight * (1f - strengthReduction);

            // Each unit of effective weight reduces speed by 8 pixels/second
            float speed = BaseSpeed - weightPenalty * 8f;

            return Math.Max(speed, 40f);  // Never go below 40 speed
        }

        /// <summary>
        /// Checks if the player can carry an additional item of the specified weight.
        /// </summary>
        /// <param name="itemWeight">The weight of the item to check.</param>
        /// <returns>True if the item can be carried without exceeding capacity; otherwise, false.</returns>
        public bool CanCarry(float itemWeight)
        {
            return CarriedWeight + itemWeight <= MaxCarryWeight;
        }

        /// <summary>
        /// Applies damage to the player, reducing health.
        /// Health cannot go below zero.
        /// </summary>
        /// <param name="damage">The amount of damage to apply.</param>
        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health < 0)
                Health = 0;
        }
    }
}
