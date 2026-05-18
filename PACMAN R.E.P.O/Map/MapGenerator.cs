namespace PACMAN_R.E.P.O.Map
{
    /// <summary>
    /// Generates procedural tile maps for the game.
    /// Creates a simple rectangular map with walls around the border,
    /// spawn point, extraction points, and an item in the center.
    /// </summary>
    public class MapGenerator
    {
        /// <summary>
        /// Generates a new tile map with the specified dimensions.
        /// The map includes border walls, a spawn point at (1,1),
        /// extraction points at three corners, and an item at the center.
        /// </summary>
        /// <param name="width">Width of the map in tiles.</param>
        /// <param name="height">Height of the map in tiles.</param>
        /// <returns>A newly generated TileMap.</returns>
        public TileMap GenerateMap(int width, int height)
        {
            TileMap map = new TileMap(width, height);

            // Fill entire map with road tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map.Tiles[x, y] = new Tile(TileType.Road);
                }
            }

            // Create border walls (top and bottom edges)
            for (int x = 0; x < width; x++)
            {
                map.Tiles[x, 0] = new Tile(TileType.Wall);
                map.Tiles[x, height - 1] = new Tile(TileType.Wall);
            }

            // Create border walls (left and right edges)
            for (int y = 0; y < height; y++)
            {
                map.Tiles[0, y] = new Tile(TileType.Wall);
                map.Tiles[width - 1, y] = new Tile(TileType.Wall);
            }

            // Place spawn point at top-left corner (inside border)
            map.Tiles[1, 1] = new Tile(TileType.Spawn);

            // Place extraction points at three corners (inside border)
            map.Tiles[width - 2, height - 2] = new Tile(TileType.Extraction);  // Bottom-right
            map.Tiles[1, height - 2] = new Tile(TileType.Extraction);          // Bottom-left
            map.Tiles[width - 2, 1] = new Tile(TileType.Extraction);           // Top-right

            // Place an item in the center of the map
            map.Tiles[width / 2, height / 2] = new Tile(TileType.Item);

            return map;
        }
    }
}
