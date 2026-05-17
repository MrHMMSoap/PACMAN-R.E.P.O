using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Map;
using System.IO;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class MapFileHandlerTests
    {
        [TestMethod]
        public void SaveMap_ShouldCreateFile()
        {
            MapGenerator generator = new MapGenerator();
            TileMap map = generator.GenerateMap(20, 20);

            MapFileHandler fileHandler = new MapFileHandler();

            string filePath = "test_map_save.txt";

            fileHandler.SaveMap(map, filePath);

            Assert.IsTrue(File.Exists(filePath));

            File.Delete(filePath);
        }

        [TestMethod]
        public void LoadMap_ShouldReturnMap_WithCorrectWidthAndHeight()
        {
            MapGenerator generator = new MapGenerator();
            TileMap originalMap = generator.GenerateMap(20, 15);

            MapFileHandler fileHandler = new MapFileHandler();

            string filePath = "test_map_load.txt";

            fileHandler.SaveMap(originalMap, filePath);

            TileMap loadedMap = fileHandler.LoadMap(filePath);

            Assert.AreEqual(20, loadedMap.Width);
            Assert.AreEqual(15, loadedMap.Height);

            File.Delete(filePath);
        }

        [TestMethod]
        public void LoadMap_ShouldPreserveSpawnPoint()
        {
            MapGenerator generator = new MapGenerator();
            TileMap originalMap = generator.GenerateMap(20, 20);

            MapFileHandler fileHandler = new MapFileHandler();

            string filePath = "test_map_spawn.txt";

            fileHandler.SaveMap(originalMap, filePath);

            TileMap loadedMap = fileHandler.LoadMap(filePath);

            Assert.AreEqual(1, loadedMap.CountTiles(TileType.Spawn));

            File.Delete(filePath);
        }

        [TestMethod]
        public void LoadMap_ShouldPreserveExtractionPoints()
        {
            MapGenerator generator = new MapGenerator();
            TileMap originalMap = generator.GenerateMap(20, 20);

            MapFileHandler fileHandler = new MapFileHandler();

            string filePath = "test_map_extractions.txt";

            fileHandler.SaveMap(originalMap, filePath);

            TileMap loadedMap = fileHandler.LoadMap(filePath);

            Assert.AreEqual(3, loadedMap.CountTiles(TileType.Extraction));

            File.Delete(filePath);
        }

        [TestMethod]
        public void LoadMap_ShouldPreserveItems()
        {
            MapGenerator generator = new MapGenerator();
            TileMap originalMap = generator.GenerateMap(20, 20);

            MapFileHandler fileHandler = new MapFileHandler();

            string filePath = "test_map_items.txt";

            fileHandler.SaveMap(originalMap, filePath);

            TileMap loadedMap = fileHandler.LoadMap(filePath);

            Assert.IsTrue(loadedMap.CountTiles(TileType.Item) > 0);

            File.Delete(filePath);
        }
    }
}
