using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Map;
using PACMAN_R.E.P.O.Systems;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class CollisionHandlerTests
    {
        [TestMethod]
        public void IsWalkable_ShouldReturnFalse_WhenTileIsWall()
        {
            Tile tile = new Tile(TileType.Wall);

            CollisionHandler collisionHandler = new CollisionHandler();

            bool result = collisionHandler.IsWalkable(tile);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsWalkable_ShouldReturnTrue_WhenTileIsRoad()
        {
            Tile tile = new Tile(TileType.Road);

            CollisionHandler collisionHandler = new CollisionHandler();

            bool result = collisionHandler.IsWalkable(tile);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsWalkable_ShouldReturnTrue_WhenTileIsSpawn()
        {
            Tile tile = new Tile(TileType.Spawn);

            CollisionHandler collisionHandler = new CollisionHandler();

            bool result = collisionHandler.IsWalkable(tile);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsWalkable_ShouldReturnTrue_WhenTileIsExtraction()
        {
            Tile tile = new Tile(TileType.Extraction);

            CollisionHandler collisionHandler = new CollisionHandler();

            bool result = collisionHandler.IsWalkable(tile);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsWalkable_ShouldReturnTrue_WhenTileIsItem()
        {
            Tile tile = new Tile(TileType.Item);

            CollisionHandler collisionHandler = new CollisionHandler();

            bool result = collisionHandler.IsWalkable(tile);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsWalkable_ShouldReturnFalse_WhenTileIsNull()
        {
            CollisionHandler collisionHandler = new CollisionHandler();

            bool result = collisionHandler.IsWalkable(null);

            Assert.IsFalse(result);
        }
    }
}
