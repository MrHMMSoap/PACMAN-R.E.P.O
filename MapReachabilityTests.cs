using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Map;
using PACMAN_R.E.P.O.Systems;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class MapReachabilityTests
    {
        [TestMethod]
        public void Spawn_ShouldBeReachable()
        {
            MapGenerator generator = new MapGenerator();
            TileMap map = generator.GenerateMap(30, 30);

            MapReachabilityChecker checker = new MapReachabilityChecker();

            bool result = checker.IsSpawnReachable(map);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AllExtractionPoints_ShouldBeReachableFromSpawn()
        {
            MapGenerator generator = new MapGenerator();
            TileMap map = generator.GenerateMap(30, 30);

            MapReachabilityChecker checker = new MapReachabilityChecker();

            bool result = checker.AreAllExtractionPointsReachable(map);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AtLeastOneItem_ShouldBeReachableFromSpawn()
        {
            MapGenerator generator = new MapGenerator();
            TileMap map = generator.GenerateMap(30, 30);

            MapReachabilityChecker checker = new MapReachabilityChecker();

            bool result = checker.IsAtLeastOneItemReachable(map);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ImportantTiles_ShouldBeReachableFromSpawn()
        {
            MapGenerator generator = new MapGenerator();
            TileMap map = generator.GenerateMap(30, 30);

            MapReachabilityChecker checker = new MapReachabilityChecker();

            bool result = checker.AreImportantTilesReachable(map);

            Assert.IsTrue(result);
        }
    }
}
