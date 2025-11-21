using System.Data;
using System.Data.SQLite;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;

namespace ToDoList.DatabaseAccess.ConnectionFactories
{
    public class SQLiteConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public SQLiteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            var connection = new SQLiteConnection(_connectionString);
            connection.Open(); // already opened
            return connection;
        }
    }
}
