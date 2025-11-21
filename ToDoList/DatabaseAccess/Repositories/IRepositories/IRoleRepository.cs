using System.Data;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories.IRepositories
{
    /// <summary>
    /// Defines CRUD operations for the <see cref="Role"/> entity.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Retrieves all roles from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="Role"/> objects.</returns>
        Task<List<Role>> GetAllRoles(IDbConnection connection, IDbTransaction? transaction = null);

        /// <summary>
        /// Adds a new role to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roleToAdd">The <see cref="Role"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task AddRole(IDbConnection connection, Role roleToAdd, IDbTransaction? transaction = null);

        /// <summary>
        /// Deletes the role with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roleId">The ID of the role to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task DeleteRole(IDbConnection connection, int roleId, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves a single role by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roledId">The ID of the role to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="Role"/> with the specified ID, or <c>null</c> if not found.</returns>
        Task<Role?> GetRoleById(IDbConnection connection, int roledId, IDbTransaction? transaction = null);

        /// <summary>
        /// Updates an existing role in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="role">The <see cref="Role"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task UpdateRole(IDbConnection connection, Role role, IDbTransaction? transaction = null);
    }
}
