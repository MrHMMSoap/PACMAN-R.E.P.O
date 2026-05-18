using System;
using System.IO;

namespace PACMAN_R.E.P.O.Map
{
    /// <summary>
    /// Handles loading and saving tile maps to/from text files.
    /// Maps are stored as character-based representations where each character
    /// corresponds to a specific tile type.
    /// </summary>
    public class MapFileHandler
    {
        /// <summary>
        /// Saves a tile map to a text file.
        /// </summary>
        /// <param name="map">The map to save.</param>
        /// <param name="filePath">The file path where the map should be saved.</param>
        /// <remarks>
        /// Output format: one character per tile, one row per line.
        /// Tile types are encoded as: Wall = '#', Road = '.', Item = 'I', Spawn = 'S', Extraction = 'E'
        /// </remarks>
        public void SaveMap(TileMap map, string filePath)
        {
            string[] lines = new string[map.Height];

            // Convert each row of tiles to a string
            for (int y = 0; y < map.Height; y++)
            {
                char[] chars = new char[map.Width];

                for (int x = 0; x < map.Width; x++)
                {
                    chars[x] = TileTypeToChar(map.Tiles[x, y].Type);
                }

                lines[y] = new string(chars);
            }

            // Write all lines to file
            File.WriteAllLines(filePath, lines);
        }

        /// <summary>
        /// Loads a tile map from a text file.
        /// </summary>
        /// <param name="filePath">Path to the map file.</param>
        /// <returns>A TileMap loaded from the file.</returns>
        /// <remarks>
        /// Expected file format:
        /// - Each line represents a row of tiles
        /// - Characters map to tile types: '#' = Wall, '.' = Road, 'I' = Item, 'S' = Spawn, 'E' = Extraction
        /// - Supports jagged rows (rows with different lengths)
        /// </remarks>
        public TileMap LoadMap(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            int height = lines.Length;

            // Find the widest line to determine map width
            int width = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > width)
                {
                    width = lines[i].Length;
                }
            }

            TileMap map = new TileMap(width, height);

            // Parse each character into a tile type
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char character;

                    // If this position is beyond the current line's length, treat as wall
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

        /// <summary>
        /// Converts a tile type to its character representation.
        /// </summary>
        /// <param name="type">The tile type to convert.</param>
        /// <returns>A character representing the tile type.</returns>
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

        /// <summary>
        /// Converts a character to its corresponding tile type.
        /// </summary>
        /// <param name="character">The character to convert.</param>
        /// <returns>The tile type represented by the character.</returns>
        /// <exception cref="Exception">Thrown when an unknown map character is encountered.</exception>
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
