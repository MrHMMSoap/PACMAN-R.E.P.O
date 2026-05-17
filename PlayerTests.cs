using System;
using System.Collections.Generic;
using System.Text;

namespace PACMAN_REPO_Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PACMAN_R.E.P.O.Entities;
    using System.Numerics;

    namespace PACMAN_REPO_Tests
    {
        [TestClass]
        public class PlayerTests
        {
            [TestMethod]
            public void CalculateSpeed_ShouldDecrease_WhenCarriedWeightIncreases()
            {
                Player player = new Player();

                player.CarriedWeight = 0;
                float speedWithoutWeight = player.CalculateSpeed();

                player.CarriedWeight = 5;
                float speedWithWeight = player.CalculateSpeed();

                Assert.IsTrue(speedWithWeight < speedWithoutWeight);
            }

            [TestMethod]
            public void CalculateSpeed_ShouldNotGoBelowMinimumSpeed()
            {
                Player player = new Player();

                player.CarriedWeight = 999;
                float speed = player.CalculateSpeed();

                Assert.IsTrue(speed >= 40f);
            }

            [TestMethod]
            public void Strength_ShouldReduceWeightPenalty()
            {
                Player weakPlayer = new Player();
                weakPlayer.CarriedWeight = 10;
                weakPlayer.StrengthLevel = 0;

                Player strongPlayer = new Player();
                strongPlayer.CarriedWeight = 10;
                strongPlayer.StrengthLevel = 3;

                float weakSpeed = weakPlayer.CalculateSpeed();
                float strongSpeed = strongPlayer.CalculateSpeed();

                Assert.IsTrue(strongSpeed > weakSpeed);
            }

            [TestMethod]
            public void Player_ShouldTakeDamage_WhenHit()
            {
                Player player = new Player();
                player.Health = 100;

                player.TakeDamage(25);

                Assert.AreEqual(75, player.Health);
            }

            [TestMethod]
            public void PlayerHealth_ShouldNotGoBelowZero()
            {
                Player player = new Player();
                player.Health = 50;

                player.TakeDamage(999);

                Assert.AreEqual(0, player.Health);
            }

            [TestMethod]
            public void Player_ShouldNotCarryItem_WhenWeightExceedsMaxWeight()
            {
                Player player = new Player();
                player.CarriedWeight = 8;
                player.MaxCarryWeight = 10;

                bool canCarry = player.CanCarry(5);

                Assert.IsFalse(canCarry);
            }

            [TestMethod]
            public void Player_ShouldCarryItem_WhenWeightIsWithinMaxWeight()
            {
                Player player = new Player();
                player.CarriedWeight = 4;
                player.MaxCarryWeight = 10;

                bool canCarry = player.CanCarry(5);

                Assert.IsTrue(canCarry);
            }
        }
    }
}
