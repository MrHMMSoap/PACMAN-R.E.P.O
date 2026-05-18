using PACMAN_R.E.P.O.Map;
using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Validates map reachability using breadth-first search.
    /// Ensures that spawn, extraction points, and items are properly connected and accessible.
    /// </summary>
    public class MapReachabilityChecker
    {
        /// <summary>
        /// Checks if the spawn tile exists and is walkable.
        /// </summary>
        /// <param name="map">The map to validate.</param>
        /// <returns>True if the spawn tile is valid and walkable; otherwise, false.</returns>
        public bool IsSpawnReachable(TileMap map)
        {
            Tile spawnTile = map.GetSpawnTile();

            return spawnTile != null && spawnTile.IsWalkable;
        }

        /// <summary>
        /// Checks if all extraction points can be reached from the spawn point.
        /// </summary>
        /// <param name="map">The map to validate.</param>
        /// <returns>True if all extraction points are reachable; false if any are isolated or if no extraction points exist.</returns>
        public bool AreAllExtractionPointsReachable(TileMap map)
        {
            // Get all tiles reachable from spawn using BFS
            bool[,] reachable = GetReachableTilesFromSpawn(map);

            List<TilePosition> extractionPositions = GetPositionsOfType(map, TileType.Extraction);

            // No extraction points means invalid map
            if (extractionPositions.Count == 0)
            {
                return false;
            }

            // Check that every extraction point is reachable
            foreach (TilePosition position in extractionPositions)
            {
                if (!reachable[position.X, position.Y])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if at least one item tile can be reached from the spawn point.
        /// </summary>
        /// <param name="map">The map to validate.</param>
        /// <returns>True if at least one item is reachable; otherwise, false.</returns>
        public bool IsAtLeastOneItemReachable(TileMap map)
        {
            bool[,] reachable = GetReachableTilesFromSpawn(map);

            List<TilePosition> itemPositions = GetPositionsOfType(map, TileType.Item);

            foreach (TilePosition position in itemPositions)
            {
                if (reachable[position.X, position.Y])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates that all important tile types (spawn, extraction points, and at least one item) are reachable.
        /// </summary>
        /// <param name="map">The map to validate.</param>
        /// <returns>True if the map is fully playable; otherwise, false.</returns>
        public bool AreImportantTilesReachable(TileMap map)
        {
            return IsSpawnReachable(map) &&
                   AreAllExtractionPointsReachable(map) &&
                   IsAtLeastOneItemReachable(map);
        }

        /// <summary>
        /// Performs a breadth-first search from the spawn point to determine all reachable tiles.
        /// </summary>
        /// <param name="map">The map to analyze.</param>
        /// <returns>A 2D boolean array where true indicates a tile is reachable from spawn.</returns>
        private bool[,] GetReachableTilesFromSpawn(TileMap map)
        {
            bool[,] visited = new bool[map.Width, map.Height];

            TilePosition spawnPosition = GetFirstPositionOfType(map, TileType.Spawn);

            // No spawn = no tiles reachable
            if (spawnPosition == null)
            {
                return visited;
            }

            // BFS initialization
            Queue<TilePosition> queue = new Queue<TilePosition>();
            queue.Enqueue(spawnPosition);
            visited[spawnPosition.X, spawnPosition.Y] = true;

            // Four-directional movement
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            // BFS exploration
            while (queue.Count > 0)
            {
                TilePosition current = queue.Dequeue();

                // Check all four neighbors
                for (int i = 0; i < 4; i++)
                {
                    int nextX = current.X + dx[i];
                    int nextY = current.Y + dy[i];

                    // Bounds check
                    if (nextX < 0 || nextX >= map.Width || nextY < 0 || nextY >= map.Height)
                    {
                        continue;
                    }

                    // Skip already visited tiles
                    if (visited[nextX, nextY])
                    {
                        continue;
                    }

                    // Skip walls
                    if (!map.Tiles[nextX, nextY].IsWalkable)
                    {
                        continue;
                    }

                    // Mark as visited and enqueue
                    visited[nextX, nextY] = true;
                    queue.Enqueue(new TilePosition(nextX, nextY));
                }
            }

            return visited;
        }

        /// <summary>
        /// Finds the first tile of the specified type in the map.
        /// </summary>
        /// <param name="map">The map to search.</param>
        /// <param name="type">The tile type to find.</param>
        /// <returns>The position of the first matching tile, or null if none exists.</returns>
        private TilePosition GetFirstPositionOfType(TileMap map, TileType type)
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.Tiles[x, y].Type == type)
                    {
                        return new TilePosition(x, y);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all tiles of the specified type in the map.
        /// </summary>
        /// <param name="map">The map to search.</param>
        /// <param name="type">The tile type to find.</param>
        /// <returns>A list of positions for all matching tiles.</returns>
        private List<TilePosition> GetPositionsOfType(TileMap map, TileType type)
        {
            List<TilePosition> positions = new List<TilePosition>();

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.Tiles[x, y].Type == type)
                    {
                        positions.Add(new TilePosition(x, y));
                    }
                }
            }

            return positions;
        }
    }

    /// <summary>
    /// Represents a 2D tile coordinate.
    /// </summary>
    public class TilePosition
    {
        /// <summary>Gets the X coordinate (horizontal).</summary>
        public int X { get; private set; }

        /// <summary>Gets the Y coordinate (vertical).</summary>
        public int Y { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TilePosition class.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public TilePosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
