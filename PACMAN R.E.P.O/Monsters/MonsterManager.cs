using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Monsters
{
    public class MonsterManager
    {
        public List<Monster> Monsters { get; private set; }

        public MonsterManager()
        {
            Monsters = new List<Monster>();
        }

        public void AddMonster(Monster monster)
        {
            Monsters.Add(monster);
        }

        public void RemoveMonster(Monster monster)
        {
            Monsters.Remove(monster);
        }

        public void ClearMonsters()
        {
            Monsters.Clear();
        }

        public bool ContainsMonster(Monster monster)
        {
            return Monsters.Contains(monster);
        }
    }
}
