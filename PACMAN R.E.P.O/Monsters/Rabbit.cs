namespace PACMAN_R.E.P.O.Monsters
{
    /// <summary>
    /// Defines the behavioral states for the Rabbit enemy.
    /// </summary>
    public enum RabbitState
    {
        /// <summary>The Rabbit moves randomly, not actively pursuing the player.</summary>
        Wander,

        /// <summary>The Rabbit actively chases the player.</summary>
        Chase
    }

    /// <summary>
    /// Represents a Rabbit enemy with wandering and chasing behavior.
    /// The Rabbit can use a wind attack with a cooldown timer.
    /// </summary>
    public class Rabbit : Monster
    {
        /// <summary>Gets the current behavioral state of this Rabbit.</summary>
        public RabbitState State { get; private set; } = RabbitState.Wander;

        /// <summary>Gets or sets the current cooldown time remaining for the wind attack (in seconds).</summary>
        public float WindCooldown { get; set; } = 0f;

        /// <summary>Gets the duration of the wind attack cooldown (in seconds).</summary>
        public float WindCooldownTime { get; private set; } = 5f;

        /// <summary>
        /// Initializes a new instance of the Rabbit class.
        /// Default speed is 80, damage is 15.
        /// </summary>
        public Rabbit()
        {
            Name = "Rabbit";
            Speed = 80f;
            Damage = 15;
        }

        /// <summary>
        /// Transitions the Rabbit to Chase state and increases speed to 100.
        /// </summary>
        public void StartChase()
        {
            State = RabbitState.Chase;
            Speed = 100f;
        }

        /// <summary>
        /// Transitions the Rabbit back to Wander state and reduces speed to 80.
        /// </summary>
        public void StopChase()
        {
            State = RabbitState.Wander;
            Speed = 80f;
        }

        /// <summary>
        /// Checks if the wind attack is off cooldown and can be used.
        /// </summary>
        /// <returns>True if the wind attack is available; otherwise, false.</returns>
        public bool CanUseWindAttack()
        {
            return WindCooldown <= 0f;
        }

        /// <summary>
        /// Uses the wind attack if available, triggering the cooldown timer.
        /// </summary>
        public void UseWindAttack()
        {
            if (!CanUseWindAttack())
            {
                return;
            }

            WindCooldown = WindCooldownTime;
        }

        /// <summary>
        /// Updates the wind attack cooldown timer.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update, in seconds.</param>
        public void UpdateCooldown(float deltaTime)
        {
            if (WindCooldown <= 0f)
            {
                WindCooldown = 0f;
                return;
            }

            WindCooldown -= deltaTime;

            if (WindCooldown < 0f)
            {
                WindCooldown = 0f;
            }
        }
    }
}
