using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Save;
using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class SQLHandlerTests
    {
        private string CreateTempDatabasePath()
        {
            string folder = Path.Combine(Path.GetTempPath(), "PACMAN_REPO_SQLTests");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = Guid.NewGuid().ToString() + ".db";

            return Path.Combine(folder, fileName);
        }

        private void SafeDelete(string filePath)
        {
            SqliteConnection.ClearAllPools();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [TestMethod]
        public void InitializeDatabase_ShouldCreateDatabaseFile()
        {
            string databasePath = CreateTempDatabasePath();

            try
            {
                SQLHandler sqlHandler = new SQLHandler(databasePath);

                sqlHandler.InitializeDatabase();

                Assert.IsTrue(File.Exists(databasePath));
            }
            finally
            {
                SafeDelete(databasePath);
            }
        }

        [TestMethod]
        public void SaveGame_ShouldCreateSaveForUser()
        {
            string databasePath = CreateTempDatabasePath();

            try
            {
                SQLHandler sqlHandler = new SQLHandler(databasePath);
                sqlHandler.InitializeDatabase();

                SaveData saveData = new SaveData();
                saveData.Round = 3;
                saveData.Money = 500;
                saveData.Health = 80;
                saveData.SpeedLevel = 1;
                saveData.StrengthLevel = 2;
                saveData.StaminaLevel = 1;
                saveData.HealthLevel = 0;
                saveData.MapFile = "round_3_map.txt";

                sqlHandler.SaveGame("Axel", saveData);

                bool hasSave = sqlHandler.HasSave("Axel");

                Assert.IsTrue(hasSave);
            }
            finally
            {
                SafeDelete(databasePath);
            }
        }

        [TestMethod]
        public void LoadGame_ShouldReturnCorrectSaveData()
        {
            string databasePath = CreateTempDatabasePath();

            try
            {
                SQLHandler sqlHandler = new SQLHandler(databasePath);
                sqlHandler.InitializeDatabase();

                SaveData saveData = new SaveData();
                saveData.Round = 4;
                saveData.Money = 750;
                saveData.Health = 60;
                saveData.SpeedLevel = 2;
                saveData.StrengthLevel = 1;
                saveData.StaminaLevel = 3;
                saveData.HealthLevel = 1;
                saveData.MapFile = "round_4_map.txt";

                sqlHandler.SaveGame("Axel", saveData);

                SaveData loadedSave = sqlHandler.LoadGame("Axel");

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
            finally
            {
                SafeDelete(databasePath);
            }
        }

        [TestMethod]
        public void LoadGame_ShouldReturnNull_WhenUserHasNoSave()
        {
            string databasePath = CreateTempDatabasePath();

            try
            {
                SQLHandler sqlHandler = new SQLHandler(databasePath);
                sqlHandler.InitializeDatabase();

                SaveData loadedSave = sqlHandler.LoadGame("MissingUser");

                Assert.IsNull(loadedSave);
            }
            finally
            {
                SafeDelete(databasePath);
            }
        }

        [TestMethod]
        public void SaveGame_ShouldOverwriteExistingSave()
        {
            string databasePath = CreateTempDatabasePath();

            try
            {
                SQLHandler sqlHandler = new SQLHandler(databasePath);
                sqlHandler.InitializeDatabase();

                SaveData firstSave = new SaveData();
                firstSave.Round = 1;
                firstSave.Money = 100;
                firstSave.Health = 100;
                firstSave.MapFile = "round_1_map.txt";

                SaveData secondSave = new SaveData();
                secondSave.Round = 5;
                secondSave.Money = 900;
                secondSave.Health = 40;
                secondSave.MapFile = "round_5_map.txt";

                sqlHandler.SaveGame("Axel", firstSave);
                sqlHandler.SaveGame("Axel", secondSave);

                SaveData loadedSave = sqlHandler.LoadGame("Axel");

                Assert.IsNotNull(loadedSave);
                Assert.AreEqual(5, loadedSave.Round);
                Assert.AreEqual(900, loadedSave.Money);
                Assert.AreEqual(40, loadedSave.Health);
                Assert.AreEqual("round_5_map.txt", loadedSave.MapFile);
            }
            finally
            {
                SafeDelete(databasePath);
            }
        }

        [TestMethod]
        public void HasSave_ShouldReturnFalse_WhenUserHasNoSave()
        {
            string databasePath = CreateTempDatabasePath();

            try
            {
                SQLHandler sqlHandler = new SQLHandler(databasePath);
                sqlHandler.InitializeDatabase();

                bool hasSave = sqlHandler.HasSave("MissingUser");

                Assert.IsFalse(hasSave);
            }
            finally
            {
                SafeDelete(databasePath);
            }
        }
    }
}