using PACMAN_R.E.P.O.Map;

namespace PACMAN_R.E.P.O.Systems
{
    public class CollisionHandler
    {
        public bool IsWalkable(Tile tile)
        {
            if (tile == null)
            {
                return false;
            }

            return tile.IsWalkable;
        }
    }
}