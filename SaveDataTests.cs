using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Save;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class SaveDataTests
    {
        [TestMethod]
        public void SaveData_ShouldStoreRoundNumber()
        {
            SaveData saveData = new SaveData();

            saveData.Round = 3;

            Assert.AreEqual(3, saveData.Round);
        }

        [TestMethod]
        public void SaveData_ShouldStorePlayerMoney()
        {
            SaveData saveData = new SaveData();

            saveData.Money = 750;

            Assert.AreEqual(750, saveData.Money);
        }

        [TestMethod]
        public void SaveData_ShouldStorePlayerHealth()
        {
            SaveData saveData = new SaveData();

            saveData.Health = 80;

            Assert.AreEqual(80, saveData.Health);
        }

        [TestMethod]
        public void SaveData_ShouldStoreUpgradeLevels()
        {
            SaveData saveData = new SaveData();

            saveData.SpeedLevel = 2;
            saveData.StrengthLevel = 1;
            saveData.StaminaLevel = 3;
            saveData.HealthLevel = 4;

            Assert.AreEqual(2, saveData.SpeedLevel);
            Assert.AreEqual(1, saveData.StrengthLevel);
            Assert.AreEqual(3, saveData.StaminaLevel);
            Assert.AreEqual(4, saveData.HealthLevel);
        }

        [TestMethod]
        public void SaveData_ShouldStoreMapFileName()
        {
            SaveData saveData = new SaveData();

            saveData.MapFile = "round_3_map.txt";

            Assert.AreEqual("round_3_map.txt", saveData.MapFile);
        }
    }
}
