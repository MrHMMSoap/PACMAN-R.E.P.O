namespace PACMAN_R.E.P.O.Monsters
{
    public enum DuckState
    {
        Passive,
        Angry
    }

    public class Duck : Monster
    {
        public DuckState State { get; private set; } = DuckState.Passive;

        public Duck()
        {
            Name = "Duck";
            Speed = 60f;
            Damage = 20;
        }

        public void Disturb()
        {
            State = DuckState.Angry;
            Speed = 90f;
        }
    }
}
