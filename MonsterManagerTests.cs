using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Monsters;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class MonsterManagerTests
    {
        [TestMethod]
        public void MonsterManager_ShouldStartWithEmptyMonsterList()
        {
            MonsterManager monsterManager = new MonsterManager();

            Assert.AreEqual(0, monsterManager.Monsters.Count);
        }

        [TestMethod]
        public void AddMonster_ShouldIncreaseMonsterCount()
        {
            MonsterManager monsterManager = new MonsterManager();
            Wraith wraith = new Wraith();

            monsterManager.AddMonster(wraith);

            Assert.AreEqual(1, monsterManager.Monsters.Count);
        }

        [TestMethod]
        public void AddMonster_ShouldStoreCorrectMonster()
        {
            MonsterManager monsterManager = new MonsterManager();
            Wraith wraith = new Wraith();

            monsterManager.AddMonster(wraith);

            Assert.IsTrue(monsterManager.Monsters.Contains(wraith));
        }

        [TestMethod]
        public void AddMonster_ShouldStoreRabbit()
        {
            MonsterManager monsterManager = new MonsterManager();
            Rabbit rabbit = new Rabbit();

            monsterManager.AddMonster(rabbit);

            Assert.IsTrue(monsterManager.Monsters.Contains(rabbit));
        }

        [TestMethod]
        public void RemoveMonster_ShouldDecreaseMonsterCount()
        {
            MonsterManager monsterManager = new MonsterManager();
            Wraith wraith = new Wraith();

            monsterManager.AddMonster(wraith);
            monsterManager.RemoveMonster(wraith);

            Assert.AreEqual(0, monsterManager.Monsters.Count);
        }

        [TestMethod]
        public void RemoveMonster_ShouldRemoveCorrectMonster()
        {
            MonsterManager monsterManager = new MonsterManager();
            Duck duck = new Duck();

            monsterManager.AddMonster(duck);
            monsterManager.RemoveMonster(duck);

            Assert.IsFalse(monsterManager.Monsters.Contains(duck));
        }

        [TestMethod]
        public void ClearMonsters_ShouldRemoveAllMonsters()
        {
            MonsterManager monsterManager = new MonsterManager();

            monsterManager.AddMonster(new Wraith());
            monsterManager.AddMonster(new Duck());
            monsterManager.AddMonster(new Rabbit());

            monsterManager.ClearMonsters();

            Assert.AreEqual(0, monsterManager.Monsters.Count);
        }

        [TestMethod]
        public void ContainsMonster_ShouldReturnTrue_WhenMonsterExists()
        {
            MonsterManager monsterManager = new MonsterManager();
            Wraith wraith = new Wraith();

            monsterManager.AddMonster(wraith);

            bool result = monsterManager.ContainsMonster(wraith);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsMonster_ShouldReturnFalse_WhenMonsterDoesNotExist()
        {
            MonsterManager monsterManager = new MonsterManager();
            Wraith wraith = new Wraith();

            bool result = monsterManager.ContainsMonster(wraith);

            Assert.IsFalse(result);
        }
    }
}
