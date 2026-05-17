using System;
using System.IO;

namespace PACMAN_R.E.P.O.Map
{
    public class MapFileHandler
    {
        public void SaveMap(TileMap map, string filePath)
        {
            string[] lines = new string[map.Height];

            for (int y = 0; y < map.Height; y++)
            {
                char[] chars = new char[map.Width];

                for (int x = 0; x < map.Width; x++)
                {
                    chars[x] = TileTypeToChar(map.Tiles[x, y].Type);
                }

                lines[y] = new string(chars);
            }

            File.WriteAllLines(filePath, lines);
        }

        public TileMap LoadMap(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            int height = lines.Length;

            int width = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > width)
                {
                    width = lines[i].Length;
                }
            }

            TileMap map = new TileMap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char character;

                    if (x >= lines[y].Length)
                    {
                        character = '#';
                    }
                    else
                    {
                        character = lines[y][x];
                    }

                    TileType type = CharToTileType(character);
                    map.Tiles[x, y] = new Tile(type);
                }
            }

            return map;
        }

        private char TileTypeToChar(TileType type)
        {
            switch (type)
            {
                case TileType.Wall:
                    return '#';

                case TileType.Road:
                    return '.';

                case TileType.Item:
                    return 'I';

                case TileType.Spawn:
                    return 'S';

                case TileType.Extraction:
                    return 'E';

                default:
                    return '?';
            }
        }

        private TileType CharToTileType(char character)
        {
            switch (character)
            {
                case '#':
                    return TileType.Wall;

                case '.':
                    return TileType.Road;

                case 'I':
                    return TileType.Item;

                case 'S':
                    return TileType.Spawn;

                case 'E':
                    return TileType.Extraction;

                default:
                    throw new Exception("Unknown map character: " + character);
            }
        }
    }
}
