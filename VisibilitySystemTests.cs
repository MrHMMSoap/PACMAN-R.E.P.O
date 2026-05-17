using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Systems;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class VisibilitySystemTests
    {
        [TestMethod]
        public void Tile_ShouldBeVisible_WhenInsideNineByNineArea()
        {
            VisibilitySystem visibilitySystem = new VisibilitySystem(4);

            bool visible = visibilitySystem.IsTileVisible(
                tileX: 14,
                tileY: 10,
                playerTileX: 10,
                playerTileY: 10
            );

            Assert.IsTrue(visible);
        }

        [TestMethod]
        public void Tile_ShouldNotBeVisible_WhenOutsideNineByNineArea()
        {
            VisibilitySystem visibilitySystem = new VisibilitySystem(4);

            bool visible = visibilitySystem.IsTileVisible(
                tileX: 15,
                tileY: 10,
                playerTileX: 10,
                playerTileY: 10
            );

            Assert.IsFalse(visible);
        }

        [TestMethod]
        public void PlayerTile_ShouldAlwaysBeVisible()
        {
            VisibilitySystem visibilitySystem = new VisibilitySystem(4);

            bool visible = visibilitySystem.IsTileVisible(
                tileX: 10,
                tileY: 10,
                playerTileX: 10,
                playerTileY: 10
            );

            Assert.IsTrue(visible);
        }

        [TestMethod]
        public void Tile_ShouldBeVisible_WhenOnEdgeOfVision()
        {
            VisibilitySystem visibilitySystem = new VisibilitySystem(4);

            bool visible = visibilitySystem.IsTileVisible(
                tileX: 6,
                tileY: 6,
                playerTileX: 10,
                playerTileY: 10
            );

            Assert.IsTrue(visible);
        }

        [TestMethod]
        public void Tile_ShouldNotBeVisible_WhenOutsideDiagonalVision()
        {
            VisibilitySystem visibilitySystem = new VisibilitySystem(4);

            bool visible = visibilitySystem.IsTileVisible(
                tileX: 5,
                tileY: 5,
                playerTileX: 10,
                playerTileY: 10
            );

            Assert.IsFalse(visible);
        }
    }
}
