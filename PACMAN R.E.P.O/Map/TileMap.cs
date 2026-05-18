using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Map
{
    /// <summary>
    /// Represents a 2D grid-based tile map.
    /// Provides methods for querying tiles and searching for specific tile types.
    /// </summary>
    public class TileMap
    {
        /// <summary>Gets the 2D array of tiles that make up this map.</summary>
        public Tile[,] Tiles { get; private set; }

        /// <summary>
        /// Gets the width of the map (number of tiles horizontally).
        /// </summary>
        public int Width
        {
            get
            {
                return Tiles.GetLength(0);
            }
        }

        /// <summary>
        /// Gets the height of the map (number of tiles vertically).
        /// </summary>
        public int Height
        {
            get
            {
                return Tiles.GetLength(1);
            }
        }

        /// <summary>
        /// Initializes a new instance of the TileMap class with the specified dimensions.
        /// </summary>
        /// <param name="width">Width of the map in tiles.</param>
        /// <param name="height">Height of the map in tiles.</param>
        public TileMap(int width, int height)
        {
            Tiles = new Tile[width, height];
        }

        /// <summary>
        /// Counts how many tiles of a specific type exist in the map.
        /// </summary>
        /// <param name="type">The tile type to count.</param>
        /// <returns>The number of tiles of the specified type.</returns>
        public int CountTiles(TileType type)
        {
            int count = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].Type == type)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Finds and returns the first spawn tile in the map.
        /// </summary>
        /// <returns>The first spawn tile found, or null if no spawn tile exists.</returns>
        public Tile GetSpawnTile()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].Type == TileType.Spawn)
                    {
                        return Tiles[x, y];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all tiles of a specific type in the map.
        /// </summary>
        /// <param name="type">The tile type to search for.</param>
        /// <returns>A list of all tiles matching the specified type.</returns>
        public List<Tile> GetTilesOfType(TileType type)
        {
            List<Tile> tiles = new List<Tile>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].Type == type)
                    {
                        tiles.Add(Tiles[x, y]);
                    }
                }
            }

            return tiles;
        }
    }
}
