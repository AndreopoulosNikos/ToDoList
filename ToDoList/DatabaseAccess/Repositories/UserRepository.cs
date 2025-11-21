using Dapper;
using System.Data;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories
{
    /// <summary>
    /// Repository that handles CRUD operations for the <see cref="User"/> entity.
    /// Uses Dapper to perform database operations.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="User"/> objects.</returns>
        /// <exception cref="Exception">Throws if an error occurs while retrieving users.</exception>
        public async Task<List<User>> GetAllUsers(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Users";
                var users = await connection.QueryAsync<User>(sql, transaction: transaction);
                return users.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }

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
        public async Task<List<User>> GetAllUsersWithAllInfo(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT u.Id, u.Username, u.FirstName, u.LastName, d.Name AS DepartmentName, r.Name AS RoleName, u.MustChangePassword " +
                          "FROM Users u " +
                          "LEFT JOIN Departments d ON u.DepartmentId = d.Id " +
                          "LEFT JOIN Roles r ON u.RoleId = r.Id;";

                var users = await connection.QueryAsync<User>(sql, transaction: transaction);
                return users.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving users with info.", ex);
            }
        }

        /// <summary>
        /// Retrieves a single user by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="User"/> with the specified ID, or <c>null</c> if not found.</returns>
        public async Task<User?> GetUserById(IDbConnection connection, int userId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE Id = @Id;";
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = userId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
        }

        /// <summary>
        /// Retrieves a single user by its Username.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="User"/> with the specified username, or <c>null</c> if not found.</returns>
        public async Task<User?> GetUserByUsername(IDbConnection connection, string username, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE Username = @Username;";
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
        }



        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="userToAdd">The <see cref="User"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task AddUser(IDbConnection connection, User userToAdd, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"INSERT INTO Users (Username,FirstName,LastName,DepartmentId,RoleId,HashedPassword,MustChangePassword) 
                VALUES (@Username,@FirstName,@LastName,@DepartmentId,@RoleId,@HashedPassword,@MustChangePassword);";

                var parameters = new
                {
                    Username = userToAdd.Username,
                    FirstName = userToAdd.FirstName,
                    LastName = userToAdd.LastName,
                    DepartmentId = userToAdd.DepartmentId,
                    RoleId = userToAdd.RoleId,
                    HashedPassword = userToAdd.HashedPassword,
                    MustChangePassword = userToAdd.MustChangePassword
                };

                await connection.ExecuteAsync(sql, parameters, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the user.", ex);
            }
        }


        /// <summary>
        /// Deletes the user with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task DeleteUser(IDbConnection connection, int userId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM Users WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Id = userId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }


        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="user">The <see cref="User"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task UpdateUser(IDbConnection connection, User user, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"
                UPDATE Users
                SET Username = @Username,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    DepartmentId = @DepartmentId,
                    RoleId = @RoleId
                WHERE Id = @Id;";

                var parameters = new
                {
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DepartmentId = user.DepartmentId,
                    RoleId = user.RoleId,
                    Id = user.Id
                };

                await connection.ExecuteAsync(sql, parameters, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }

        /// <summary>
        /// Updates the password of an existing user in the database.
        /// </summary>
        /// <param name="connection">The database connection used to execute the update. </param>
        /// <param name="newPassword">The new password value (hashed) to set for the user. </param>
        /// <param name="userId"> The unique identifier of the user whose password will be updated. </param>
        /// <param name="mustchangePassword"> Flag indicating if the user must change the password in his next login. </param>
        /// <param name="transaction"> An optional database transaction to associate with the operation. </param>
        public async Task UpdateUserPassword(IDbConnection connection, string newPassword,int userId, bool mustChangePassword,IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"
                UPDATE Users
                SET HashedPassword = @HashedPassword,
                    MustChangePassword = @MustChangePassword
                WHERE Id = @Id;";

                var parameters = new
                {
                    HashedPassword = newPassword,
                    MustChangePassword = mustChangePassword,
                    Id = userId
                };

                await connection.ExecuteAsync(sql, parameters, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user's password.", ex);
            }
        }
    }
}
