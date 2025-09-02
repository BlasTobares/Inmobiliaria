using Microsoft.Data.Sqlite;

namespace Inmobiliaria.Data
{
    public static class Database
    {
        private static string connectionString = "Data Source=Data/inmobiliaria.db";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }
    }
}
