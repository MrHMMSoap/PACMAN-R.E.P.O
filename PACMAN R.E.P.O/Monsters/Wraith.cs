namespace PACMAN_R.E.P.O.Monsters
{
    public enum WraithState
    {
        SlowChase,
        Rage
    }

    public class Wraith : Monster
    {
        public WraithState State { get; private set; } = WraithState.SlowChase;
        public float RageTimer { get; private set; } = 0f;

        public Wraith()
        {
            Name = "Wraith";
            Speed = 40f;
            Damage = 25;
        }

        public void OnSeenByPlayer()
        {
            State = WraithState.Rage;
            RageTimer = 10f;
            Speed = 100f;
        }

        public void UpdateRageTimer(float deltaTime)
        {
            if (State != WraithState.Rage)
            {
                return;
            }

            RageTimer -= deltaTime;

            if (RageTimer <= 0f)
            {
                RageTimer = 0f;
                State = WraithState.SlowChase;
                Speed = 40f;
            }
        }
    }
}
