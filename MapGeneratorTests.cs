using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Map;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class MapGeneratorTests
    {
        [TestMethod]
        public void GenerateMap_ShouldCreateMap_WithCorrectWidthAndHeight()
        {
            MapGenerator generator = new MapGenerator();

            TileMap map = generator.GenerateMap(30, 20);

            Assert.AreEqual(30, map.Width);
            Assert.AreEqual(20, map.Height);
        }

        [TestMethod]
        public void GeneratedMap_ShouldHaveExactlyOneSpawnPoint()
        {
            MapGenerator generator = new MapGenerator();

            TileMap map = generator.GenerateMap(30, 30);

            int spawnCount = map.CountTiles(TileType.Spawn);

            Assert.AreEqual(1, spawnCount);
        }

        [TestMethod]
        public void GeneratedMap_ShouldHaveThreeExtractionPoints()
        {
            MapGenerator generator = new MapGenerator();

            TileMap map = generator.GenerateMap(30, 30);

            int extractionCount = map.CountTiles(TileType.Extraction);

            Assert.AreEqual(3, extractionCount);
        }

        [TestMethod]
        public void GeneratedMap_ShouldHaveAtLeastOneItem()
        {
            MapGenerator generator = new MapGenerator();

            TileMap map = generator.GenerateMap(30, 30);

            int itemCount = map.CountTiles(TileType.Item);

            Assert.IsTrue(itemCount > 0);
        }

        [TestMethod]
        public void SpawnPoint_ShouldBeWalkable()
        {
            MapGenerator generator = new MapGenerator();

            TileMap map = generator.GenerateMap(30, 30);

            Tile spawnTile = map.GetSpawnTile();

            Assert.IsNotNull(spawnTile);
            Assert.IsTrue(spawnTile.IsWalkable);
        }

        [TestMethod]
        public void ExtractionPoints_ShouldBeWalkable()
        {
            MapGenerator generator = new MapGenerator();

            TileMap map = generator.GenerateMap(30, 30);

            List<Tile> extractionTiles = map.GetTilesOfType(TileType.Extraction);

            Assert.AreEqual(3, extractionTiles.Count);

            foreach (Tile tile in extractionTiles)
            {
                Assert.IsTrue(tile.IsWalkable);
            }
        }
    }
}
