using Dapper;
using System.Data;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories
{
    /// <summary>
    /// Repository that handles CRUD operations for the <see cref="Role"/> entity.
    /// Uses Dapper to perform database operations.
    /// </summary>
    public class RoleRepository : IRoleRepository
    {

        /// <summary>
        /// Retrieves all roles from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="Role"/> objects.</returns>
        /// <exception cref="Exception">Throws if an error occurs while retrieving departments.</exception>
        public async Task<List<Role>> GetAllRoles(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Roles";
                var roles = await connection.QueryAsync<Role>(sql, transaction: transaction);
                return roles.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving roles.", ex);
            }
        }

        /// <summary>
        /// Adds a new role to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roleToAdd">The <see cref="Role"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task AddRole(IDbConnection connection, Role roleToAdd, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"INSERT INTO Roles (Name) VALUES (@Name);";
                await connection.ExecuteAsync(sql, new { Name = roleToAdd.Name }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the role.", ex);
            }
        }

        /// <summary>
        /// Deletes the role with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roleId">The ID of the role to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task DeleteRole(IDbConnection connection, int roleId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM Roles WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Id = roleId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the role.", ex);
            }
        }

        /// <summary>
        /// Retrieves a single role by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roleId">The ID of the role to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="Role"/> with the specified ID, or <c>null</c> if not found.</returns>
        public async Task<Role?> GetRoleById(IDbConnection connection, int roleId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Roles WHERE Id = @Id;";
                return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = roleId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the role.", ex);
            }
        }

        /// <summary>
        /// Updates an existing role in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="role">The <see cref="Role"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task UpdateRole(IDbConnection connection, Role role, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"UPDATE Roles SET Name = @Name WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { role.Name, role.Id }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the role.", ex);
            }
        }
    }
}
