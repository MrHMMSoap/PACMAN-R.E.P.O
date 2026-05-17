using System.Collections.Generic;

namespace PACMAN_R.E.P.O.Items
{
    public class ItemManager
    {
        public List<Item> Items { get; private set; }

        public ItemManager()
        {
            Items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }

        public void ClearItems()
        {
            Items.Clear();
        }

        public int GetTotalValue()
        {
            int totalValue = 0;

            foreach (Item item in Items)
            {
                totalValue += item.Value;
            }

            return totalValue;
        }

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
