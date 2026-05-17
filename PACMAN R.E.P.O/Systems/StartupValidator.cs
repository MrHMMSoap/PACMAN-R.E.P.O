using System.IO;

namespace PACMAN_R.E.P.O.Systems
{
    public class StartupValidator
    {
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
