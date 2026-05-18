using PACMAN_R.E.P.O.Map;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Handles collision detection and walkability checks.
    /// </summary>
    public class CollisionHandler
    {
        /// <summary>
        /// Checks if a tile is walkable (not null and not a wall).
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if the tile exists and can be walked on; otherwise, false.</returns>
        public bool IsWalkable(Tile tile)
        {
            if (tile == null)
            {
                return false;
            }

            return tile.IsWalkable;
        }
    }
}