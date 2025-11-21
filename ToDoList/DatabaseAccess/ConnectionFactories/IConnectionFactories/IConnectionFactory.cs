using System.Data;

namespace ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories
{
    public interface IConnectionFactory
    {
        /// <summary>
        /// Returns a new, opened database connection
        /// </summary>
        IDbConnection CreateConnection();
    }
}
