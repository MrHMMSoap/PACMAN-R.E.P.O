using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Save
{
    /// <summary>
    /// Manages save data for player accounts in memory.
    /// Provides methods to save, load, and check for existing save files.
    /// </summary>
    /// <remarks>
    /// This is an in-memory save manager; save data is not persisted to disk.
    /// The game also uses SQLHandler for persistent database saves.
    /// </remarks>
    public class SaveManager
    {
        private readonly Dictionary<string, SaveData> saves;

        /// <summary>
        /// Initializes a new instance of the SaveManager class.
        /// </summary>
        public SaveManager()
        {
            saves = new Dictionary<string, SaveData>();
        }

        /// <summary>
        /// Saves the current game state for a user account.
        /// If a save already exists for this account, it will be overwritten.
        /// </summary>
        /// <param name="account">The user account to save for.</param>
        /// <param name="saveData">The save data to store.</param>
        public void SaveGame(UserAccount account, SaveData saveData)
        {
            saves[account.Username] = saveData;
        }

        /// <summary>
        /// Loads the saved game state for a user account.
        /// </summary>
        /// <param name="account">The user account to load for.</param>
        /// <returns>The saved game data, or null if no save exists for this account.</returns>
        public SaveData LoadGame(UserAccount account)
        {
            if (!saves.ContainsKey(account.Username))
            {
                return null;
            }

            return saves[account.Username];
        }

        /// <summary>
        /// Checks if a save file exists for the specified user account.
        /// </summary>
        /// <param name="account">The user account to check.</param>
        /// <returns>True if a save exists for this account; otherwise, false.</returns>
        public bool HasSave(UserAccount account)
        {
            return saves.ContainsKey(account.Username);
        }
    }
}
