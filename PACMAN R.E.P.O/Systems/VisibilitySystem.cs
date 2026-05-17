using System;

namespace PACMAN_R.E.P.O.Systems
{
    public class VisibilitySystem
    {
        public int ViewRadius { get; private set; }

        public VisibilitySystem(int viewRadius)
        {
            ViewRadius = viewRadius;
        }

        public bool IsTileVisible(int tileX, int tileY, int playerTileX, int playerTileY)
        {
            return Math.Abs(tileX - playerTileX) <= ViewRadius &&
                   Math.Abs(tileY - playerTileY) <= ViewRadius;
        }
    }
}
