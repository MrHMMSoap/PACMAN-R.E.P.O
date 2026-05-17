namespace PACMAN_R.E.P.O.Monsters
{
    public enum RabbitState
    {
        Wander,
        Chase
    }

    public class Rabbit : Monster
    {
        public RabbitState State { get; private set; } = RabbitState.Wander;

        public float WindCooldown { get; set; } = 0f;
        public float WindCooldownTime { get; private set; } = 5f;

        public Rabbit()
        {
            Name = "Rabbit";
            Speed = 80f;
            Damage = 15;
        }

        public void StartChase()
        {
            State = RabbitState.Chase;
            Speed = 100f;
        }

        public void StopChase()
        {
            State = RabbitState.Wander;
            Speed = 80f;
        }

        public bool CanUseWindAttack()
        {
            return WindCooldown <= 0f;
        }

        public void UseWindAttack()
        {
            if (!CanUseWindAttack())
            {
                return;
            }

            WindCooldown = WindCooldownTime;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (WindCooldown <= 0f)
            {
                WindCooldown = 0f;
                return;
            }

            WindCooldown -= deltaTime;

            if (WindCooldown < 0f)
            {
                WindCooldown = 0f;
            }
        }
    }
}
