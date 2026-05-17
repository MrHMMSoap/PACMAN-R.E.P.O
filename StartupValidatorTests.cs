using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Systems;
using System;
using System.IO;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class StartupValidatorTests
    {
        private string CreateUniqueFilePath(string extension)
        {
            string folder = Path.Combine(Path.GetTempPath(), "PACMAN_REPO_TestFiles");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(folder, fileName);
        }

        private string CreateTempMapFile()
        {
            string filePath = CreateUniqueFilePath(".txt");

            string[] mapLines =
            {
                "##########",
                "#S.......#",
                "#........#",
                "#..I.....#",
                "#........#",
                "#....E...#",
                "#........#",
                "#E......E#",
                "#........#",
                "##########"
            };

            File.WriteAllLines(filePath, mapLines);

            return filePath;
        }

        private string CreateTempDatabaseFile()
        {
            string filePath = CreateUniqueFilePath(".db");

            File.WriteAllText(filePath, "test database");

            return filePath;
        }

        private string GetMissingTempFilePath(string extension)
        {
            string filePath = CreateUniqueFilePath(extension);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return filePath;
        }

        private void SafeDelete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [TestMethod]
        public void Validate_ShouldReturnTrue_WhenRequiredFilesExist()
        {
            string mapFile = CreateTempMapFile();
            string databaseFile = CreateTempDatabaseFile();

            try
            {
                StartupValidator validator = new StartupValidator();

                bool result = validator.Validate(mapFile, databaseFile, out string errorMessage);

                Assert.IsTrue(result, errorMessage);
                Assert.AreEqual("", errorMessage);
            }
            finally
            {
                SafeDelete(mapFile);
                SafeDelete(databaseFile);
            }
        }

        [TestMethod]
        public void Validate_ShouldReturnFalse_WhenMapFileIsMissing()
        {
            string mapFile = GetMissingTempFilePath(".txt");
            string databaseFile = CreateTempDatabaseFile();

            try
            {
                StartupValidator validator = new StartupValidator();

                bool result = validator.Validate(mapFile, databaseFile, out string errorMessage);

                Assert.IsFalse(result);
                Assert.AreEqual("Map file is missing.", errorMessage);
            }
            finally
            {
                SafeDelete(databaseFile);
            }
        }

        [TestMethod]
        public void Validate_ShouldReturnFalse_WhenDatabaseFileIsMissing()
        {
            string mapFile = CreateTempMapFile();
            string databaseFile = GetMissingTempFilePath(".db");

            try
            {
                StartupValidator validator = new StartupValidator();

                bool result = validator.Validate(mapFile, databaseFile, out string errorMessage);

                Assert.IsFalse(result);
                Assert.AreEqual("Database file is missing.", errorMessage);
            }
            finally
            {
                SafeDelete(mapFile);
            }
        }

        [TestMethod]
        public void Validate_ShouldReturnFalse_WhenBothFilesAreMissing()
        {
            string mapFile = GetMissingTempFilePath(".txt");
            string databaseFile = GetMissingTempFilePath(".db");

            StartupValidator validator = new StartupValidator();

            bool result = validator.Validate(mapFile, databaseFile, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Map file is missing.", errorMessage);
        }
    }
}
