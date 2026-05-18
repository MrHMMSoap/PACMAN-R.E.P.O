using System.IO;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Validates that required files exist before the game starts.
    /// </summary>
    public class StartupValidator
    {
        /// <summary>
        /// Validates that the map file and database file both exist.
        /// </summary>
        /// <param name="mapFilePath">The path to the map file.</param>
        /// <param name="databaseFilePath">The path to the database file.</param>
        /// <param name="errorMessage">An error message if validation fails; empty if successful.</param>
        /// <returns>True if both files exist; otherwise, false.</returns>
        public bool Validate(string mapFilePath, string databaseFilePath, out string errorMessage)
        {
            if (!File.Exists(mapFilePath))
            {
                errorMessage = "Map file is missing.";
                return false;
            }

            if (!File.Exists(databaseFilePath))
            {
                errorMessage = "Database file is missing.";
                return false;
            }

            errorMessage = "";
            return true;
        }
    }
}
