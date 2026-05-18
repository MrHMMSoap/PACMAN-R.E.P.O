namespace PACMAN_R.E.P.O.Save
{
    /// <summary>
    /// Represents a snapshot of the player's game state that can be saved and loaded.
    /// Includes round progression, player stats, upgrade levels, and map information.
    /// </summary>
    public class SaveData
    {
        /// <summary>Gets or sets the current round number.</summary>
        public int Round { get; set; } = 1;

        /// <summary>Gets or sets the player's money balance.</summary>
        public int Money { get; set; } = 0;

        /// <summary>Gets or sets the player's health points.</summary>
        public int Health { get; set; } = 100;

        /// <summary>Gets or sets the player's speed upgrade level.</summary>
        public int SpeedLevel { get; set; } = 0;

        /// <summary>Gets or sets the player's strength upgrade level.</summary>
        public int StrengthLevel { get; set; } = 0;

        /// <summary>Gets or sets the player's stamina upgrade level.</summary>
        public int StaminaLevel { get; set; } = 0;

        /// <summary>Gets or sets the player's health upgrade level.</summary>
        public int HealthLevel { get; set; } = 0;

        /// <summary>Gets or sets the filename of the current map being played.</summary>
        public string MapFile { get; set; } = "";
    }
}
