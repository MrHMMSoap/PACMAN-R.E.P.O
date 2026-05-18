using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Monsters
{
    /// <summary>
    /// Manages a collection of monsters in the game.
    /// Provides methods for adding, removing, and querying monsters.
    /// </summary>
    public class MonsterManager
    {
        /// <summary>Gets the list of active monsters.</summary>
        public List<Monster> Monsters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MonsterManager class.
        /// </summary>
        public MonsterManager()
        {
            Monsters = new List<Monster>();
        }

        /// <summary>
        /// Adds a monster to the manager.
        /// </summary>
        /// <param name="monster">The monster to add.</param>
        public void AddMonster(Monster monster)
        {
            Monsters.Add(monster);
        }

        /// <summary>
        /// Removes a monster from the manager.
        /// </summary>
        /// <param name="monster">The monster to remove.</param>
        public void RemoveMonster(Monster monster)
        {
            Monsters.Remove(monster);
        }

        /// <summary>
        /// Removes all monsters from the manager.
        /// </summary>
        public void ClearMonsters()
        {
            Monsters.Clear();
        }

        /// <summary>
        /// Checks if a specific monster is currently managed.
        /// </summary>
        /// <param name="monster">The monster to check for.</param>
        /// <returns>True if the monster is in the list; otherwise, false.</returns>
        public bool ContainsMonster(Monster monster)
        {
            return Monsters.Contains(monster);
        }
    }
}
