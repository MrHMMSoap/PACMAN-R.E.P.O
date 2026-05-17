using PACMAN_R.E.P.O.Map;
using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Systems
{
    public class MapReachabilityChecker
    {
        public bool IsSpawnReachable(TileMap map)
        {
            Tile spawnTile = map.GetSpawnTile();

            return spawnTile != null && spawnTile.IsWalkable;
        }

        public bool AreAllExtractionPointsReachable(TileMap map)
        {
            bool[,] reachable = GetReachableTilesFromSpawn(map);

            List<TilePosition> extractionPositions = GetPositionsOfType(map, TileType.Extraction);

            if (extractionPositions.Count == 0)
            {
                return false;
            }

            foreach (TilePosition position in extractionPositions)
            {
                if (!reachable[position.X, position.Y])
                {
                    return false;
                }
            }

            return true;
        }

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

        public bool AreImportantTilesReachable(TileMap map)
        {
            return IsSpawnReachable(map) &&
                   AreAllExtractionPointsReachable(map) &&
                   IsAtLeastOneItemReachable(map);
        }

        private bool[,] GetReachableTilesFromSpawn(TileMap map)
        {
            bool[,] visited = new bool[map.Width, map.Height];

            TilePosition spawnPosition = GetFirstPositionOfType(map, TileType.Spawn);

            if (spawnPosition == null)
            {
                return visited;
            }

            Queue<TilePosition> queue = new Queue<TilePosition>();
            queue.Enqueue(spawnPosition);
            visited[spawnPosition.X, spawnPosition.Y] = true;

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (queue.Count > 0)
            {
                TilePosition current = queue.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    int nextX = current.X + dx[i];
                    int nextY = current.Y + dy[i];

                    if (nextX < 0 || nextX >= map.Width || nextY < 0 || nextY >= map.Height)
                    {
                        continue;
                    }

                    if (visited[nextX, nextY])
                    {
                        continue;
                    }

                    if (!map.Tiles[nextX, nextY].IsWalkable)
                    {
                        continue;
                    }

                    visited[nextX, nextY] = true;
                    queue.Enqueue(new TilePosition(nextX, nextY));
                }
            }

            return visited;
        }

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

    public class TilePosition
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public TilePosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
