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
    public class DepartmentRepository : IDepartmentRepository
    {
        /// <summary>
        /// Retrieves all departments from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="Department"/> objects.</returns>
        /// <exception cref="Exception">Throws if an error occurs while retrieving departments.</exception>
        public async Task<List<Department>> GetAllDepartments(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Departments";
                var departments = await connection.QueryAsync<Department>(sql, transaction: transaction);
                return departments.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving departments.", ex);
            }
        }

        /// <summary>
        /// Adds a new department to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="departmentToAdd">The <see cref="Department"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Throws if an error occurs while inserting the department.</exception>
        public async Task AddDepartment(IDbConnection connection, Department departmentToAdd, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"INSERT INTO Departments (Name) VALUES (@Name);";
                await connection.ExecuteAsync(sql, new { Name = departmentToAdd.Name }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the department.", ex);
            }
        }

        /// <summary>
        /// Deletes the department with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="departmentId">The ID of the department to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Throws if an error occurs while deleting the department.</exception>
        public async Task DeleteDepartment(IDbConnection connection, int departmentId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM Departments WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Id = departmentId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the department.", ex);
            }
        }

        /// <summary>
        /// Retrieves a single department by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roleId">The ID of the department to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="Department"/> with the specified ID, or <c>null</c> if not found.</returns>
        /// <exception cref="Exception">Throws if an error occurs while retrieving the department.</exception>
        public async Task<Department?> GetDepartmentById(IDbConnection connection, int roleId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Departments WHERE Id = @Id;";
                return await connection.QueryFirstOrDefaultAsync<Department>(sql, new { Id = roleId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the department.", ex);
            }
        }

        /// <summary>
        /// Updates an existing department in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="department">The <see cref="Department"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Throws if an error occurs while updating the department.</exception>
        public async Task UpdateDepartment(IDbConnection connection, Department department, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"UPDATE Departments SET Name = @Name WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { department.Name, department.Id }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the department.", ex);
            }
        }
    }
}
