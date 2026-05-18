using System;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages fog-of-war visibility calculations.
    /// Determines which tiles are visible based on distance from the player.
    /// </summary>
    public class VisibilitySystem
    {
        /// <summary>Gets the radius (in tiles) around the player that is visible.</summary>
        public int ViewRadius { get; private set; }

        /// <summary>
        /// Initializes a new instance of the VisibilitySystem class.
        /// </summary>
        /// <param name="viewRadius">The radius of visibility around the player, in tiles.</param>
        public VisibilitySystem(int viewRadius)
        {
            ViewRadius = viewRadius;
        }

        /// <summary>
        /// Checks if a specific tile is visible to the player based on Manhattan distance.
        /// </summary>
        /// <param name="tileX">The X coordinate of the tile to check.</param>
        /// <param name="tileY">The Y coordinate of the tile to check.</param>
        /// <param name="playerTileX">The player's X tile coordinate.</param>
        /// <param name="playerTileY">The player's Y tile coordinate.</param>
        /// <returns>True if the tile is within the view radius; otherwise, false.</returns>
        public bool IsTileVisible(int tileX, int tileY, int playerTileX, int playerTileY)
        {
            return Math.Abs(tileX - playerTileX) <= ViewRadius &&
                   Math.Abs(tileY - playerTileY) <= ViewRadius;
        }
    }
}
