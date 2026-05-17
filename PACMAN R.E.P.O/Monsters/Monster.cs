namespace PACMAN_R.E.P.O.Monsters
{
    public abstract class Monster
    {
        public string Name { get; protected set; } = "";
        public float Speed { get; protected set; }
        public int Damage { get; protected set; }
    }
}
