using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

namespace PACMAN_R.E.P.O.Save
{
    public class SQLHandler
    {
        private readonly string databasePath;
        private readonly string connectionString;

        public SQLHandler(string databasePath)
        {
            this.databasePath = databasePath;
            connectionString = $"Data Source={databasePath};Pooling=False";
        }

        public void InitializeDatabase()
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            string createUsersTableQuery =
            @"
    CREATE TABLE IF NOT EXISTS Users (
        Username TEXT PRIMARY KEY,
        PasswordHash TEXT NOT NULL
    );
    ";

            using SqliteCommand createUsersCommand = new SqliteCommand(createUsersTableQuery, connection);
            createUsersCommand.ExecuteNonQuery();

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

        public bool CreateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (UserExists(username))
            {
                return false;
            }

            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

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

        public bool ValidateLogin(string username, string password)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            string query =
            @"
    SELECT PasswordHash
    FROM Users
    WHERE Username = @Username;
    ";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            object result = command.ExecuteScalar();

            if (result == null)
            {
                return false;
            }

            string savedHash = result.ToString();
            string enteredHash = HashPassword(password);

            return savedHash == enteredHash;
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "";
            }

            using SHA256 sha256 = SHA256.Create();

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            StringBuilder builder = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
        public void SaveGame(string username, SaveData saveData)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

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

        public SaveData LoadGame(string username)
        {
            using SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();

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

            if (!reader.Read())
            {
                return null;
            }

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