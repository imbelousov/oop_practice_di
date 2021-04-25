using Microsoft.Data.Sqlite;

namespace Shop.Core
{
    internal class DbHelper
    {
        public static SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection("Data Source=shop.db");
            connection.Open();
            return connection;
        }
    }
}
