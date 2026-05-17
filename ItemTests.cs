using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Items;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        public void Item_ShouldHavePositiveWeight()
        {
            Item item = new Item("Test Item", 5f, 100);

            Assert.IsGreaterThan(0f, item.Weight);
        }

        [TestMethod]
        public void Item_ShouldHavePositiveValue()
        {
            Item item = new Item("Test Item", 5f, 100);

            Assert.IsGreaterThan(0, item.Value);
        }

        [TestMethod]
        public void Item_ShouldNotBeCarried_WhenCreated()
        {
            Item item = new Item("Test Item", 5f, 100);

            Assert.IsFalse(item.IsCarried);
        }

        [TestMethod]
        public void Item_ShouldStoreCorrectNameWeightAndValue()
        {
            Item item = new Item("Golden Cup", 4f, 150);

            Assert.AreEqual("Golden Cup", item.Name);
            Assert.AreEqual(4f, item.Weight);
            Assert.AreEqual(150, item.Value);
        }
    }
}
