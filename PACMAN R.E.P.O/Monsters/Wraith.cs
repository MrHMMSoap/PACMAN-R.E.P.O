namespace PACMAN_R.E.P.O.Monsters
{
    /// <summary>
    /// Defines the behavioral states for the Wraith enemy.
    /// </summary>
    public enum WraithState
    {
        /// <summary>The Wraith moves slowly, not yet enraged.</summary>
        SlowChase,

        /// <summary>The Wraith has been seen by the player and moves at high speed for a limited time.</summary>
        Rage
    }

    /// <summary>
    /// Represents a Wraith enemy that chases the player.
    /// The Wraith enters a "Rage" state when seen, moving much faster for a short duration.
    /// </summary>
    public class Wraith : Monster
    {
        /// <summary>Gets the current behavioral state of this Wraith.</summary>
        public WraithState State { get; private set; } = WraithState.SlowChase;

        /// <summary>Gets the remaining time (in seconds) before the Wraith exits Rage state.</summary>
        public float RageTimer { get; private set; } = 0f;

        /// <summary>
        /// Initializes a new instance of the Wraith class.
        /// Default speed is 40, damage is 25.
        /// </summary>
        public Wraith()
        {
            Name = "Wraith";
            Speed = 40f;
            Damage = 25;
        }

        /// <summary>
        /// Triggers the Wraith's Rage state when the player sees it.
        /// In Rage, speed increases to 100 for 10 seconds.
        /// </summary>
        public void OnSeenByPlayer()
        {
            State = WraithState.Rage;
            RageTimer = 10f;
            Speed = 100f;
        }

        /// <summary>
        /// Updates the Rage timer and transitions back to SlowChase when the timer expires.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update, in seconds.</param>
        public void UpdateRageTimer(float deltaTime)
        {
            if (State != WraithState.Rage)
            {
                return;
            }

            RageTimer -= deltaTime;

            if (RageTimer <= 0f)
            {
                RageTimer = 0f;
                State = WraithState.SlowChase;
                Speed = 40f;
            }
        }
    }
}
