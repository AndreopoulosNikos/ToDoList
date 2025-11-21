using System.Data;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories.IRepositories
{
    /// <summary>
    /// Defines CRUD operations for the <see cref="User"/> entity.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="User"/> objects.</returns>
        Task<List<User>> GetAllUsers(IDbConnection connection, IDbTransaction? transaction = null);


        /// <summary>
        /// Retrieves all users from the database along with their associated department and role information.
        /// Each user object will include the <c>DepartmentName</c> and <c>RoleName</c> properties, if available.
        /// </summary>
        /// <param name="connection"> The database connection to use for the query. </param> 
        /// <param name="transaction"> An optional database transaction to associate with the query. </param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The result contains a list of <see cref="User"/> objects with full user, department, and role information.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if an error occurs while retrieving the users from the database.
        /// </exception>
        public Task<List<User>> GetAllUsersWithAllInfo(IDbConnection connection, IDbTransaction? transaction = null);


        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="userToAdd">The <see cref="User"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task AddUser(IDbConnection connection, User userToAdd, IDbTransaction? transaction = null);

        /// <summary>
        /// Deletes the user with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task DeleteUser(IDbConnection connection, int userId, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves a single user by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="User"/> with the specified ID, or <c>null</c> if not found.</returns>
        Task<User?> GetUserById(IDbConnection connection, int userId, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves a single user by its Username.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="username">The name of the user to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="User"/> with the specified Username, or <c>null</c> if not found.</returns>
        Task<User?> GetUserByUsername(IDbConnection connection, string username, IDbTransaction? transaction = null);

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="user">The <see cref="User"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task UpdateUser(IDbConnection connection, User user, IDbTransaction? transaction = null);

        /// <summary>
        /// Updates the password of an existing user in the database.
        /// </summary>
        /// <param name="connection">The database connection used to execute the update. </param>
        /// <param name="newPassword">The new password value (hashed) to set for the user. </param>
        /// <param name="userId"> The unique identifier of the user whose password will be updated. </param>
        /// <param name="mustchangePassword"> Flag indicating if the user must change the password in his next login. </param>
        /// <param name="transaction"> An optional database transaction to associate with the operation. </param>
        Task UpdateUserPassword(IDbConnection connection, string newPassword, int userId, bool mustchangePassword, IDbTransaction? transaction = null);
    }
}
