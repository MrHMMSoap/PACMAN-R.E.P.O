namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Defines the possible game states that control the main game loop flow.
    /// </summary>
    public enum GameState
    {
        /// <summary>Player is entering credentials to log in or create an account.</summary>
        Login,

        /// <summary>Main menu where the player can start a new game or load a saved game.</summary>
        MainMenu,

        /// <summary>Tutorial screen showing game legend and controls.</summary>
        Tutorial,

        /// <summary>Active gameplay mode where the player explores and collects items.</summary>
        Playing,

        /// <summary>Game is paused (currently not actively used).</summary>
        Paused,

        /// <summary>Player is in the shop purchasing upgrades between rounds.</summary>
        Shop,

        /// <summary>Player has died and sees the game over screen.</summary>
        GameOver
    }
}
