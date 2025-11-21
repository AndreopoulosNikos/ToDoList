using Dapper;
using System.Data;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories
{
    /// <summary>
    /// Repository that handles CRUD operations for the <see cref="TaskStatus"/> entity.
    /// Uses Dapper to perform database operations.
    /// </summary>
    public class TaskStatusRepository : ITaskStatusRepository
    {

        /// <summary>
        /// Retrieves all task statuses from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="TaskStatusItem"/> objects.</returns>
        public async Task<List<TaskStatusItem>> GetAllTaskStatuses(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM TaskStatuses";
                var departments = await connection.QueryAsync<TaskStatusItem>(sql, transaction: transaction);
                return departments.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving task statuses.", ex);
            }
        }


        /// <summary>
        /// Adds a new task status to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="departmentToAdd">The <see cref="TaskStatusItem"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task AddTaskStatus(IDbConnection connection, TaskStatusItem taskStatusToAdd, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"INSERT INTO TaskStatuses (Name) VALUES (@Name);";
                await connection.ExecuteAsync(sql, new { Name = taskStatusToAdd.Name }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the task status.", ex);
            }
        }

        /// <summary>
        /// Deletes the task status with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskStatusId">The ID of the status to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task DeleteTaskStatus(IDbConnection connection, int taskStatusId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM TaskStatuses WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Id = taskStatusId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the task status.", ex);
            }
        }

        /// <summary>
        /// Retrieves a single task status by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskStatusId">The ID of the task status to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="TaskStatusItem"/> with the specified ID, or <c>null</c> if not found.</returns>
        public async Task<TaskStatusItem?> GetTaskStatusById(IDbConnection connection, int taskStatusId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM TaskStatuses WHERE Id = @Id;";
                return await connection.QueryFirstOrDefaultAsync<TaskStatusItem>(sql, new { Id = taskStatusId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the task status.", ex);
            }
        }

        /// <summary>
        /// Updates an existing task status in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskStatus">The <see cref="TaskStatusItem"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task UpdateTaskStatus(IDbConnection connection, TaskStatusItem taskStatus, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"UPDATE TaskStatuses SET Name = @Name WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { taskStatus.Name, taskStatus.Id }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the task status.", ex);
            }
        }

    }
}
