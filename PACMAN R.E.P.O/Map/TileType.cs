namespace PACMAN_R.E.P.O.Map
{
    /// <summary>
    /// Defines the different types of tiles that can exist in the game map.
    /// </summary>
    public enum TileType
    {
        /// <summary>Solid wall that blocks movement.</summary>
        Wall,

        /// <summary>Walkable floor/corridor.</summary>
        Road,

        /// <summary>Tile containing an item to be collected.</summary>
        Item,

        /// <summary>Starting position for the player.</summary>
        Spawn,

        /// <summary>Extraction point where items can be deposited for money.</summary>
        Extraction
    }
}