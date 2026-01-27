using SQLite;
using YourAppName.Models;

namespace YourAppName.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            // For development on Windows, we'll use a path in the project folder
            // so it's easily accessible by the SQLite Explorer extension.
#if WINDOWS && DEBUG
            var projectDir = @"c:\Users\ACER\Documents\3rd Year code\app test\test cw1\Data";
            if (!Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
            }
            _dbPath = Path.Combine(projectDir, "Journal.db");
#else
            _dbPath = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "Journal.db");
#endif
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_database != null)
                return _database;

            _database = new SQLiteAsyncConnection(_dbPath);
            await _database.CreateTableAsync<JournalEntry>();
            return _database;
        }

        public string GetDatabasePath() => _dbPath;
    }
}
