using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Monsters;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class MonsterTests
    {
        [TestMethod]
        public void Wraith_ShouldStartInSlowChaseState()
        {
            Wraith wraith = new Wraith();

            Assert.AreEqual(WraithState.SlowChase, wraith.State);
        }

        [TestMethod]
        public void Wraith_ShouldEnterRageMode_WhenSeenByPlayer()
        {
            Wraith wraith = new Wraith();

            wraith.OnSeenByPlayer();

            Assert.AreEqual(WraithState.Rage, wraith.State);
        }

        [TestMethod]
        public void Wraith_ShouldSetRageTimerToTenSeconds_WhenSeenByPlayer()
        {
            Wraith wraith = new Wraith();

            wraith.OnSeenByPlayer();

            Assert.AreEqual(10f, wraith.RageTimer);
        }

        [TestMethod]
        public void Wraith_ShouldReturnToSlowChase_WhenRageTimerEnds()
        {
            Wraith wraith = new Wraith();

            wraith.OnSeenByPlayer();
            wraith.UpdateRageTimer(10f);

            Assert.AreEqual(WraithState.SlowChase, wraith.State);
        }

        [TestMethod]
        public void Duck_ShouldStartPassive()
        {
            Duck duck = new Duck();

            Assert.AreEqual(DuckState.Passive, duck.State);
        }

        [TestMethod]
        public void Duck_ShouldBecomeAngry_WhenDisturbed()
        {
            Duck duck = new Duck();

            duck.Disturb();

            Assert.AreEqual(DuckState.Angry, duck.State);
        }

        [TestMethod]
        public void Rabbit_ShouldStartInWanderState()
        {
            Rabbit rabbit = new Rabbit();

            Assert.AreEqual(RabbitState.Wander, rabbit.State);
        }

        [TestMethod]
        public void Rabbit_ShouldEnterChaseState_WhenStartingChase()
        {
            Rabbit rabbit = new Rabbit();

            rabbit.StartChase();

            Assert.AreEqual(RabbitState.Chase, rabbit.State);
        }

        [TestMethod]
        public void Rabbit_ShouldReturnToWanderState_WhenStoppingChase()
        {
            Rabbit rabbit = new Rabbit();

            rabbit.StartChase();
            rabbit.StopChase();

            Assert.AreEqual(RabbitState.Wander, rabbit.State);
        }

        [TestMethod]
        public void Rabbit_ShouldBeAbleToUseWindAttack_WhenCooldownIsZero()
        {
            Rabbit rabbit = new Rabbit();

            rabbit.WindCooldown = 0f;

            bool canUseWind = rabbit.CanUseWindAttack();

            Assert.IsTrue(canUseWind);
        }

        [TestMethod]
        public void Rabbit_ShouldNotUseWindAttack_WhenCooldownIsAboveZero()
        {
            Rabbit rabbit = new Rabbit();

            rabbit.WindCooldown = 3f;

            bool canUseWind = rabbit.CanUseWindAttack();

            Assert.IsFalse(canUseWind);
        }

        [TestMethod]
        public void Rabbit_WindAttack_ShouldSetCooldown()
        {
            Rabbit rabbit = new Rabbit();

            rabbit.WindCooldown = 0f;

            rabbit.UseWindAttack();

            Assert.IsGreaterThan(0f, rabbit.WindCooldown);
        }
    }
}
