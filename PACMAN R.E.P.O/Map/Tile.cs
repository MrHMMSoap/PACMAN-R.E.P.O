namespace PACMAN_R.E.P.O.Map
{
    public class Tile
    {
        public TileType Type { get; set; }

        public bool IsWalkable
        {
            get
            {
                return Type != TileType.Wall;
            }
        }

        public Tile(TileType type)
        {
            Type = type;
        }
    }
}
