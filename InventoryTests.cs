using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Entities;
using PACMAN_R.E.P.O.Items;
using PACMAN_R.E.P.O.Systems;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void AddItem_ShouldReturnTrue_WhenPlayerCanCarryItem()
        {
            PlayerEntity player = new PlayerEntity();
            player.CarriedWeight = 0f;
            player.MaxCarryWeight = 10f;

            Inventory inventory = new Inventory(player);
            Item item = new Item("Small Item", 3f, 50);

            bool added = inventory.AddItem(item);

            Assert.IsTrue(added);
        }

        [TestMethod]
        public void AddItem_ShouldReturnFalse_WhenItemIsTooHeavy()
        {
            PlayerEntity player = new PlayerEntity();
            player.CarriedWeight = 9f;
            player.MaxCarryWeight = 10f;

            Inventory inventory = new Inventory(player);
            Item item = new Item("Heavy Item", 5f, 100);

            bool added = inventory.AddItem(item);

            Assert.IsFalse(added);
        }

        [TestMethod]
        public void AddItem_ShouldIncreasePlayerCarriedWeight()
        {
            PlayerEntity player = new PlayerEntity();
            player.CarriedWeight = 0f;
            player.MaxCarryWeight = 10f;

            Inventory inventory = new Inventory(player);
            Item item = new Item("Small Item", 3f, 50);

            inventory.AddItem(item);

            Assert.AreEqual(3f, player.CarriedWeight);
        }

        [TestMethod]
        public void AddItem_ShouldMarkItemAsCarried()
        {
            PlayerEntity player = new PlayerEntity();
            player.CarriedWeight = 0f;
            player.MaxCarryWeight = 10f;

            Inventory inventory = new Inventory(player);
            Item item = new Item("Small Item", 3f, 50);

            inventory.AddItem(item);

            Assert.IsTrue(item.IsCarried);
        }

        [TestMethod]
        public void RemoveItem_ShouldDecreasePlayerCarriedWeight()
        {
            PlayerEntity player = new PlayerEntity();
            player.CarriedWeight = 0f;
            player.MaxCarryWeight = 10f;

            Inventory inventory = new Inventory(player);
            Item item = new Item("Small Item", 3f, 50);

            inventory.AddItem(item);
            inventory.RemoveItem(item);

            Assert.AreEqual(0f, player.CarriedWeight);
        }

        [TestMethod]
        public void RemoveItem_ShouldMarkItemAsNotCarried()
        {
            PlayerEntity player = new PlayerEntity();
            player.CarriedWeight = 0f;
            player.MaxCarryWeight = 10f;

            Inventory inventory = new Inventory(player);
            Item item = new Item("Small Item", 3f, 50);

            inventory.AddItem(item);
            inventory.RemoveItem(item);

            Assert.IsFalse(item.IsCarried);
        }

        [TestMethod]
        public void Inventory_ShouldStoreAddedItem()
        {
            PlayerEntity player = new PlayerEntity();
            player.MaxCarryWeight = 10f;

            Inventory inventory = new Inventory(player);
            Item item = new Item("Golden Cup", 2f, 100);

            inventory.AddItem(item);

            Assert.IsTrue(inventory.Items.Contains(item));
        }
    }
}
