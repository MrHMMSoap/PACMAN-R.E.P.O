namespace PACMAN_R.E.P.O.Map
{
    public class MapGenerator
    {
        public TileMap GenerateMap(int width, int height)
        {
            TileMap map = new TileMap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map.Tiles[x, y] = new Tile(TileType.Road);
                }
            }

            for (int x = 0; x < width; x++)
            {
                map.Tiles[x, 0] = new Tile(TileType.Wall);
                map.Tiles[x, height - 1] = new Tile(TileType.Wall);
            }

            for (int y = 0; y < height; y++)
            {
                map.Tiles[0, y] = new Tile(TileType.Wall);
                map.Tiles[width - 1, y] = new Tile(TileType.Wall);
            }

            map.Tiles[1, 1] = new Tile(TileType.Spawn);

            map.Tiles[width - 2, height - 2] = new Tile(TileType.Extraction);
            map.Tiles[1, height - 2] = new Tile(TileType.Extraction);
            map.Tiles[width - 2, 1] = new Tile(TileType.Extraction);

            map.Tiles[width / 2, height / 2] = new Tile(TileType.Item);

            return map;
        }
    }
}
