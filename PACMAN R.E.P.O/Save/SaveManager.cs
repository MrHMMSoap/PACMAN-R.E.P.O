using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Save
{
    public class SaveManager
    {
        private readonly Dictionary<string, SaveData> saves;

        public SaveManager()
        {
            saves = new Dictionary<string, SaveData>();
        }

        public void SaveGame(UserAccount account, SaveData saveData)
        {
            saves[account.Username] = saveData;
        }

        public SaveData LoadGame(UserAccount account)
        {
            if (!saves.ContainsKey(account.Username))
            {
                return null;
            }

            return saves[account.Username];
        }

        public bool HasSave(UserAccount account)
        {
            return saves.ContainsKey(account.Username);
        }
    }
}
