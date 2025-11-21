using System.Data;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories.IRepositories
{
    /// <summary>
    /// Defines CRUD operations for the <see cref="TaskStatusItem"/> entity.
    /// </summary>
    public interface ITaskStatusRepository
    {
        /// <summary>
        /// Retrieves all task statuses from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="TaskStatusItem"/> objects.</returns>
        Task<List<TaskStatusItem>> GetAllTaskStatuses(IDbConnection connection, IDbTransaction? transaction = null);

        /// <summary>
        /// Adds a new task status to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="departmentToAdd">The <see cref="TaskStatusItem"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task AddTaskStatus(IDbConnection connection, TaskStatusItem taskStatusToAdd, IDbTransaction? transaction = null);

        /// <summary>
        /// Deletes the task status with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskStatusId">The ID of the status to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task DeleteTaskStatus(IDbConnection connection, int taskStatusId, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves a single task status by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskStatusId">The ID of the task status to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="TaskStatusItem"/> with the specified ID, or <c>null</c> if not found.</returns>
        Task<TaskStatusItem?> GetTaskStatusById(IDbConnection connection, int taskStatusId, IDbTransaction? transaction = null);

        /// <summary>
        /// Updates an existing task status in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskStatus">The <see cref="TaskStatusItem"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task UpdateTaskStatus(IDbConnection connection, TaskStatusItem taskStatus, IDbTransaction? transaction = null);
    }
}
