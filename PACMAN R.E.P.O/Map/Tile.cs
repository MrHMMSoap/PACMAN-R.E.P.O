namespace PACMAN_R.E.P.O.Map
{
    /// <summary>
    /// Represents a single tile in the game map.
    /// Each tile has a type that determines its appearance and behavior.
    /// </summary>
    public class Tile
    {
        /// <summary>Gets or sets the type of this tile (Wall, Road, Item, etc.).</summary>
        public TileType Type { get; set; }

        /// <summary>
        /// Gets whether this tile can be walked through by entities.
        /// Walls are not walkable; all other tiles are walkable.
        /// </summary>
        public bool IsWalkable
        {
            get
            {
                return Type != TileType.Wall;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Tile class.
        /// </summary>
        /// <param name="type">The type of tile to create.</param>
        public Tile(TileType type)
        {
            Type = type;
        }
    }
}
