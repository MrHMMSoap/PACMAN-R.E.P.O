using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Save;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class SaveManagerTests
    {
        [TestMethod]
        public void SaveGame_ShouldStoreSaveData_ForUser()
        {
            SaveManager saveManager = new SaveManager();

            UserAccount account = new UserAccount("Axel", "password123");

            SaveData saveData = new SaveData();
            saveData.Round = 3;
            saveData.Money = 500;
            saveData.Health = 80;

            saveManager.SaveGame(account, saveData);

            bool hasSave = saveManager.HasSave(account);

            Assert.IsTrue(hasSave);
        }

        [TestMethod]
        public void LoadGame_ShouldReturnCorrectSaveData_ForUser()
        {
            SaveManager saveManager = new SaveManager();

            UserAccount account = new UserAccount("Axel", "password123");

            SaveData saveData = new SaveData();
            saveData.Round = 4;
            saveData.Money = 750;
            saveData.Health = 60;
            saveData.SpeedLevel = 2;
            saveData.StrengthLevel = 1;
            saveData.StaminaLevel = 3;
            saveData.HealthLevel = 1;
            saveData.MapFile = "round_4_map.txt";

            saveManager.SaveGame(account, saveData);

            SaveData loadedSave = saveManager.LoadGame(account);

            Assert.IsNotNull(loadedSave);
            Assert.AreEqual(4, loadedSave.Round);
            Assert.AreEqual(750, loadedSave.Money);
            Assert.AreEqual(60, loadedSave.Health);
            Assert.AreEqual(2, loadedSave.SpeedLevel);
            Assert.AreEqual(1, loadedSave.StrengthLevel);
            Assert.AreEqual(3, loadedSave.StaminaLevel);
            Assert.AreEqual(1, loadedSave.HealthLevel);
            Assert.AreEqual("round_4_map.txt", loadedSave.MapFile);
        }

        [TestMethod]
        public void LoadGame_ShouldReturnNull_WhenUserHasNoSave()
        {
            SaveManager saveManager = new SaveManager();

            UserAccount account = new UserAccount("Axel", "password123");

            SaveData loadedSave = saveManager.LoadGame(account);

            Assert.IsNull(loadedSave);
        }

        [TestMethod]
        public void SaveGame_ShouldOverwriteExistingSave_ForSameUser()
        {
            SaveManager saveManager = new SaveManager();

            UserAccount account = new UserAccount("Axel", "password123");

            SaveData firstSave = new SaveData();
            firstSave.Round = 1;
            firstSave.Money = 100;

            SaveData secondSave = new SaveData();
            secondSave.Round = 5;
            secondSave.Money = 900;

            saveManager.SaveGame(account, firstSave);
            saveManager.SaveGame(account, secondSave);

            SaveData loadedSave = saveManager.LoadGame(account);

            Assert.IsNotNull(loadedSave);
            Assert.AreEqual(5, loadedSave.Round);
            Assert.AreEqual(900, loadedSave.Money);
        }

        [TestMethod]
        public void Saves_ShouldBeSeparate_ForDifferentUsers()
        {
            SaveManager saveManager = new SaveManager();

            UserAccount accountOne = new UserAccount("Axel", "password123");
            UserAccount accountTwo = new UserAccount("PlayerTwo", "abc123");

            SaveData saveOne = new SaveData();
            saveOne.Round = 2;
            saveOne.Money = 300;

            SaveData saveTwo = new SaveData();
            saveTwo.Round = 7;
            saveTwo.Money = 1200;

            saveManager.SaveGame(accountOne, saveOne);
            saveManager.SaveGame(accountTwo, saveTwo);

            SaveData loadedSaveOne = saveManager.LoadGame(accountOne);
            SaveData loadedSaveTwo = saveManager.LoadGame(accountTwo);

            Assert.IsNotNull(loadedSaveOne);
            Assert.IsNotNull(loadedSaveTwo);

            Assert.AreEqual(2, loadedSaveOne.Round);
            Assert.AreEqual(300, loadedSaveOne.Money);

            Assert.AreEqual(7, loadedSaveTwo.Round);
            Assert.AreEqual(1200, loadedSaveTwo.Money);
        }
    }
}
