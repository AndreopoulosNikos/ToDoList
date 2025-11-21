using System.Data;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories.IRepositories
{
    /// <summary>
    /// Defines CRUD operations for the <see cref="Department"/> entity.
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// Retrieves all departments from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="Department"/> objects.</returns>
        Task<List<Department>> GetAllDepartments(IDbConnection connection, IDbTransaction? transaction = null);

        /// <summary>
        /// Adds a new department to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="departmentToAdd">The <see cref="Department"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task AddDepartment(IDbConnection connection, Department departmentToAdd, IDbTransaction? transaction = null);

        /// <summary>
        /// Deletes the department with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="departmentId">The ID of the department to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task DeleteDepartment(IDbConnection connection, int departmentId, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves a single department by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="id">The ID of the department to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="Department"/> with the specified ID, or <c>null</c> if not found.</returns>
        Task<Department?> GetDepartmentById(IDbConnection connection, int id, IDbTransaction? transaction = null);

        /// <summary>
        /// Updates an existing department in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="department">The <see cref="Department"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task UpdateDepartment(IDbConnection connection, Department department, IDbTransaction? transaction = null);
    }
}
