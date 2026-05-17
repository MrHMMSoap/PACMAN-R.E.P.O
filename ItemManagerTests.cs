using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Items;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class ItemManagerTests
    {
        [TestMethod]
        public void ItemManager_ShouldStartWithEmptyItemList()
        {
            ItemManager itemManager = new ItemManager();

            Assert.AreEqual(0, itemManager.Items.Count);
        }

        [TestMethod]
        public void AddItem_ShouldIncreaseItemCount()
        {
            ItemManager itemManager = new ItemManager();
            Item item = new Item("Golden Cup", 3f, 100);

            itemManager.AddItem(item);

            Assert.AreEqual(1, itemManager.Items.Count);
        }

        [TestMethod]
        public void AddItem_ShouldStoreCorrectItem()
        {
            ItemManager itemManager = new ItemManager();
            Item item = new Item("Golden Cup", 3f, 100);

            itemManager.AddItem(item);

            Assert.IsTrue(itemManager.Items.Contains(item));
        }

        [TestMethod]
        public void RemoveItem_ShouldDecreaseItemCount()
        {
            ItemManager itemManager = new ItemManager();
            Item item = new Item("Golden Cup", 3f, 100);

            itemManager.AddItem(item);
            itemManager.RemoveItem(item);

            Assert.AreEqual(0, itemManager.Items.Count);
        }

        [TestMethod]
        public void RemoveItem_ShouldRemoveCorrectItem()
        {
            ItemManager itemManager = new ItemManager();
            Item item = new Item("Golden Cup", 3f, 100);

            itemManager.AddItem(item);
            itemManager.RemoveItem(item);

            Assert.IsFalse(itemManager.Items.Contains(item));
        }

        [TestMethod]
        public void ClearItems_ShouldRemoveAllItems()
        {
            ItemManager itemManager = new ItemManager();

            itemManager.AddItem(new Item("Golden Cup", 3f, 100));
            itemManager.AddItem(new Item("Silver Plate", 2f, 50));

            itemManager.ClearItems();

            Assert.AreEqual(0, itemManager.Items.Count);
        }

        [TestMethod]
        public void GetTotalValue_ShouldReturnCombinedValueOfAllItems()
        {
            ItemManager itemManager = new ItemManager();

            itemManager.AddItem(new Item("Golden Cup", 3f, 100));
            itemManager.AddItem(new Item("Silver Plate", 2f, 50));

            int totalValue = itemManager.GetTotalValue();

            Assert.AreEqual(150, totalValue);
        }

        [TestMethod]
        public void GetTotalWeight_ShouldReturnCombinedWeightOfAllItems()
        {
            ItemManager itemManager = new ItemManager();

            itemManager.AddItem(new Item("Golden Cup", 3f, 100));
            itemManager.AddItem(new Item("Silver Plate", 2f, 50));

            float totalWeight = itemManager.GetTotalWeight();

            Assert.AreEqual(5f, totalWeight);
        }
    }
}
