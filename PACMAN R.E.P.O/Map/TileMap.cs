using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Map
{
    public class TileMap
    {
        public Tile[,] Tiles { get; private set; }

        public int Width
        {
            get
            {
                return Tiles.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return Tiles.GetLength(1);
            }
        }

        public TileMap(int width, int height)
        {
            Tiles = new Tile[width, height];
        }

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
