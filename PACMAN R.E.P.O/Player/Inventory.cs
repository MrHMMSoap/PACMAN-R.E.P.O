using PACMAN_R.E.P.O.Items;
using System.Collections.Generic;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    public class Inventory
    {
        private readonly PlayerEntity player;

        public List<Item> Items { get; private set; }

        public Inventory(PlayerEntity player)
        {
            this.player = player;
            Items = new List<Item>();
        }

        public bool AddItem(Item item)
        {
            if (player.CarriedWeight + item.Weight > player.MaxCarryWeight)
            {
                return false;
            }

            Items.Add(item);
            player.CarriedWeight += item.Weight;
            item.IsCarried = true;

            return true;
        }

        public void RemoveItem(Item item)
        {
            if (!Items.Contains(item))
            {
                return;
            }

            Items.Remove(item);
            player.CarriedWeight -= item.Weight;

            if (player.CarriedWeight < 0f)
            {
                player.CarriedWeight = 0f;
            }

            item.IsCarried = false;
        }
    }
}
