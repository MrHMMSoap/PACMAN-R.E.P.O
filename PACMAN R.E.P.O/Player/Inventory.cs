using PACMAN_R.E.P.O.Items;
using System.Collections.Generic;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages the player's carried items, tracking weight and capacity constraints.
    /// </summary>
    public class Inventory
    {
        private readonly PlayerEntity player;

        /// <summary>Gets the list of items currently in the player's inventory.</summary>
        public List<Item> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Inventory class for the specified player.
        /// </summary>
        /// <param name="player">The player who owns this inventory.</param>
        public Inventory(PlayerEntity player)
        {
            this.player = player;
            Items = new List<Item>();
        }

        /// <summary>
        /// Attempts to add an item to the inventory.
        /// The item will only be added if it does not exceed the player's carry capacity.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the item was added successfully; false if the weight limit was exceeded.</returns>
        public bool AddItem(Item item)
        {
            // Check if adding this item would exceed carry capacity
            if (player.CarriedWeight + item.Weight > player.MaxCarryWeight)
            {
                return false;
            }

            Items.Add(item);
            player.CarriedWeight += item.Weight;
            item.IsCarried = true;

            return true;
        }

        /// <summary>
        /// Removes an item from the inventory and updates the player's carried weight.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(Item item)
        {
            if (!Items.Contains(item))
            {
                return;
            }

            Items.Remove(item);
            player.CarriedWeight -= item.Weight;

            // Ensure carried weight never goes negative
            if (player.CarriedWeight < 0f)
            {
                player.CarriedWeight = 0f;
            }

            item.IsCarried = false;
        }
    }
}
