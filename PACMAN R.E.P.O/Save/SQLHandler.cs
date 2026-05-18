using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

namespace PACMAN_R.E.P.O.Save
{
    /// <summary>
    /// Handles all SQLite database operations for user accounts and save files.
    /// Manages user authentication, save data persistence, and database initialization.
    /// </summary>
    public class SQLHandler
    {
        private readonly string databasePath;
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the SQLHandler class.
        /// </summary>
        /// <param name="databasePath">The file path to the SQLite database.</param>
        public SQLHandler(string databasePath)
        {
            this.databasePath = databasePath;
            connectionString = $"Data Source={databasePath};Pooling=False";
        }

        /// <summary>
        /// Initializes the database by creating the Users and SaveFiles tables if they don't exist.
        /// </summary>
        /// <remarks>
        /// Creates two tables:
        /// - Users: Stores usernames and password hashes.
        /// - SaveFiles: Stores game progress data for each user.
        /// </remarks>
        public void InitializeDatabase()
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            // Create Users table for authentication
            string createUsersTableQuery =
            @"
    CREATE TABLE IF NOT EXISTS Users (
        Username TEXT PRIMARY KEY,
        PasswordHash TEXT NOT NULL
    );
    ";

            using SqliteCommand createUsersCommand = new SqliteCommand(createUsersTableQuery, connection);
            createUsersCommand.ExecuteNonQuery();

            // Create SaveFiles table for game progress
            string createSaveFilesTableQuery =
            @"
    CREATE TABLE IF NOT EXISTS SaveFiles (
        Username TEXT PRIMARY KEY,
        Round INTEGER NOT NULL,
        Money INTEGER NOT NULL,
        Health INTEGER NOT NULL,
        SpeedLevel INTEGER NOT NULL,
        StrengthLevel INTEGER NOT NULL,
        StaminaLevel INTEGER NOT NULL,
        HealthLevel INTEGER NOT NULL,
        MapFile TEXT NOT NULL,
        FOREIGN KEY(Username) REFERENCES Users(Username)
    );
    ";

            using SqliteCommand createSaveCommand = new SqliteCommand(createSaveFilesTableQuery, connection);
            createSaveCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if a user with the specified username exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        public bool UserExists(string username)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            string query =
            @"
    SELECT COUNT(*)
    FROM Users
    WHERE Username = @Username;
    ";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            long count = (long)command.ExecuteScalar();

            return count > 0;
        }

        /// <summary>
        /// Creates a new user account in the database.
        /// </summary>
        /// <param name="username">The username for the new account.</param>
        /// <param name="password">The plaintext password (will be hashed before storing).</param>
        /// <returns>True if the account was created successfully; false if validation fails or the username already exists.</returns>
        public bool CreateUser(string username, string password)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            // Check for duplicate username
            if (UserExists(username))
            {
                return false;
            }

            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            // Insert new user with hashed password
            string query =
            @"
    INSERT INTO Users (Username, PasswordHash)
    VALUES (@Username, @PasswordHash);
    ";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));

            command.ExecuteNonQuery();

            return true;
        }

        /// <summary>
        /// Validates a user's login credentials by comparing password hashes.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The plaintext password to verify.</param>
        /// <returns>True if the credentials are valid; false if the user doesn't exist or the password is incorrect.</returns>
        public bool ValidateLogin(string username, string password)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            // Retrieve stored password hash for the user
            string query =
            @"
    SELECT PasswordHash
    FROM Users
    WHERE Username = @Username;
    ";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            object result = command.ExecuteScalar();

            // User not found
            if (result == null)
            {
                return false;
            }

            // Compare hashes
            string savedHash = result.ToString();
            string enteredHash = HashPassword(password);

            return savedHash == enteredHash;
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

            using SHA256 sha256 = SHA256.Create();

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
        /// <summary>
        /// Saves game progress to the database for a specific user.
        /// If a save already exists for this user, it will be updated.
        /// </summary>
        /// <param name="username">The username whose progress should be saved.</param>
        /// <param name="saveData">The save data containing game state.</param>
        public void SaveGame(string username, SaveData saveData)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            // Use INSERT with ON CONFLICT to either create or update the save
            string query =
            @"
            INSERT INTO SaveFiles 
            (
                Username,
                Round,
                Money,
                Health,
                SpeedLevel,
                StrengthLevel,
                StaminaLevel,
                HealthLevel,
                MapFile
            )
            VALUES
            (
                @Username,
                @Round,
                @Money,
                @Health,
                @SpeedLevel,
                @StrengthLevel,
                @StaminaLevel,
                @HealthLevel,
                @MapFile
            )
            ON CONFLICT(Username) DO UPDATE SET
                Round = excluded.Round,
                Money = excluded.Money,
                Health = excluded.Health,
                SpeedLevel = excluded.SpeedLevel,
                StrengthLevel = excluded.StrengthLevel,
                StaminaLevel = excluded.StaminaLevel,
                HealthLevel = excluded.HealthLevel,
                MapFile = excluded.MapFile;
            ";

            using SqliteCommand command = new SqliteCommand(query, connection);

            // Bind all parameters from the save data
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Round", saveData.Round);
            command.Parameters.AddWithValue("@Money", saveData.Money);
            command.Parameters.AddWithValue("@Health", saveData.Health);
            command.Parameters.AddWithValue("@SpeedLevel", saveData.SpeedLevel);
            command.Parameters.AddWithValue("@StrengthLevel", saveData.StrengthLevel);
            command.Parameters.AddWithValue("@StaminaLevel", saveData.StaminaLevel);
            command.Parameters.AddWithValue("@HealthLevel", saveData.HealthLevel);
            command.Parameters.AddWithValue("@MapFile", saveData.MapFile ?? "");

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Loads saved game progress from the database for a specific user.
        /// </summary>
        /// <param name="username">The username whose progress should be loaded.</param>
        /// <returns>The saved game data, or null if no save exists for this user.</returns>
        public SaveData LoadGame(string username)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            // Query all save data fields
            string query =
            @"
            SELECT 
                Round,
                Money,
                Health,
                SpeedLevel,
                StrengthLevel,
                StaminaLevel,
                HealthLevel,
                MapFile
            FROM SaveFiles
            WHERE Username = @Username;
            ";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using SqliteDataReader reader = command.ExecuteReader();

            // No save found
            if (!reader.Read())
            {
                return null;
            }

            // Reconstruct SaveData from database row
            SaveData saveData = new SaveData();

            saveData.Round = reader.GetInt32(0);
            saveData.Money = reader.GetInt32(1);
            saveData.Health = reader.GetInt32(2);
            saveData.SpeedLevel = reader.GetInt32(3);
            saveData.StrengthLevel = reader.GetInt32(4);
            saveData.StaminaLevel = reader.GetInt32(5);
            saveData.HealthLevel = reader.GetInt32(6);
            saveData.MapFile = reader.GetString(7);

            return saveData;
        }

        /// <summary>
        /// Checks if a save file exists in the database for the specified user.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if a save exists for this user; otherwise, false.</returns>
        public bool HasSave(string username)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            string query =
            @"
            SELECT COUNT(*)
            FROM SaveFiles
            WHERE Username = @Username;
            ";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            long count = (long)command.ExecuteScalar();

            return count > 0;
        }
    }
}