using System.Security.Cryptography;
using System.Text;

namespace PACMAN_R.E.P.O.Save
{
    /// <summary>
    /// Represents a user account with username and hashed password.
    /// Provides password verification and validation functionality.
    /// </summary>
    public class UserAccount
    {
        /// <summary>Gets the username for this account.</summary>
        public string Username { get; private set; }

        /// <summary>Gets the SHA256 hash of the user's password.</summary>
        public string PasswordHash { get; private set; }

        /// <summary>
        /// Initializes a new instance of the UserAccount class.
        /// The password is automatically hashed using SHA256.
        /// </summary>
        /// <param name="username">The username for this account.</param>
        /// <param name="password">The plaintext password (will be hashed).</param>
        public UserAccount(string username, string password)
        {
            Username = username;
            PasswordHash = HashPassword(password);
        }

        /// <summary>
        /// Verifies if the provided password matches the stored password hash.
        /// </summary>
        /// <param name="password">The plaintext password to verify.</param>
        /// <returns>True if the password matches; otherwise, false.</returns>
        public bool VerifyPassword(string password)
        {
            string hashToCheck = HashPassword(password);

            return PasswordHash == hashToCheck;
        }

        /// <summary>
        /// Checks if this account has valid username and password hash.
        /// </summary>
        /// <returns>True if both username and password hash are non-empty; otherwise, false.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(PasswordHash);
        }

        /// <summary>
        /// Hashes a password using SHA256.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <returns>A hexadecimal string representation of the password hash.</returns>
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "";
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert password to bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Compute SHA256 hash
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert hash bytes to hexadecimal string
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
