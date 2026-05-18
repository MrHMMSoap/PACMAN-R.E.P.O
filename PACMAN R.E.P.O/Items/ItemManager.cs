using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Items
{
    /// <summary>
    /// Manages a collection of items.
    /// Provides methods for adding, removing, and querying items.
    /// Currently not actively used in the main game (Inventory class used instead).
    /// </summary>
    public class ItemManager
    {
        /// <summary>Gets the list of items managed by this ItemManager.</summary>
        public List<Item> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ItemManager class with an empty item list.
        /// </summary>
        public ItemManager()
        {
            Items = new List<Item>();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void ClearItems()
        {
            Items.Clear();
        }

        /// <summary>
        /// Calculates the total monetary value of all items in the collection.
        /// </summary>
        /// <returns>The sum of all item values.</returns>
        public int GetTotalValue()
        {
            int totalValue = 0;

            foreach (Item item in Items)
            {
                totalValue += item.Value;
            }

            return totalValue;
        }

        /// <summary>
        /// Calculates the total weight of all items in the collection.
        /// </summary>
        /// <returns>The sum of all item weights.</returns>
        public float GetTotalWeight()
        {
            float totalWeight = 0f;

            foreach (Item item in Items)
            {
                totalWeight += item.Weight;
            }

            return totalWeight;
        }
    }
}
