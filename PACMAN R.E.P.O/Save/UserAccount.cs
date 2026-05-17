using System.Security.Cryptography;
using System.Text;

namespace PACMAN_R.E.P.O.Save
{
    public class UserAccount
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }

        public UserAccount(string username, string password)
        {
            Username = username;
            PasswordHash = HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            string hashToCheck = HashPassword(password);

            return PasswordHash == hashToCheck;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(PasswordHash);
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "";
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                StringBuilder builder = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
