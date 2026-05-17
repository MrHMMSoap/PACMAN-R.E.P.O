using Microsoft.VisualStudio.TestTools.UnitTesting;
using PACMAN_R.E.P.O.Save;

namespace PACMAN_REPO_Tests
{
    [TestClass]
    public class UserAccountTests
    {
        [TestMethod]
        public void UserAccount_ShouldStoreUsername()
        {
            UserAccount account = new UserAccount("Axel", "password123");

            Assert.AreEqual("Axel", account.Username);
        }

        [TestMethod]
        public void UserAccount_ShouldNotStorePasswordInPlainText()
        {
            UserAccount account = new UserAccount("Axel", "password123");

            Assert.AreNotEqual("password123", account.PasswordHash);
        }

        [TestMethod]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            UserAccount account = new UserAccount("Axel", "password123");

            bool result = account.VerifyPassword("password123");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            UserAccount account = new UserAccount("Axel", "password123");

            bool result = account.VerifyPassword("wrongPassword");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_ShouldReturnFalse_WhenUsernameIsEmpty()
        {
            UserAccount account = new UserAccount("", "password123");

            bool result = account.IsValid();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_ShouldReturnFalse_WhenPasswordHashIsEmpty()
        {
            UserAccount account = new UserAccount("Axel", "");

            bool result = account.IsValid();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_ShouldReturnTrue_WhenUsernameAndPasswordExist()
        {
            UserAccount account = new UserAccount("Axel", "password123");

            bool result = account.IsValid();

            Assert.IsTrue(result);
        }
    }
}
