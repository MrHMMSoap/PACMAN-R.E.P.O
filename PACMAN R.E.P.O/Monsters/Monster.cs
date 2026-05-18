namespace PACMAN_R.E.P.O.Monsters
{
    /// <summary>
    /// Abstract base class for all monsters/enemies in the game.
    /// Defines common properties like name, speed, and damage.
    /// </summary>
    public abstract class Monster
    {
        /// <summary>Gets or sets the display name of this monster.</summary>
        public string Name { get; protected set; } = "";

        /// <summary>Gets or sets the movement speed of this monster (pixels per second).</summary>
        public float Speed { get; protected set; }

        /// <summary>Gets or sets the damage this monster deals to the player on collision.</summary>
        public int Damage { get; protected set; }
    }
}
