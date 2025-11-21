using System.Data;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories.IRepositories
{
    /// <summary>
    /// Defines CRUD operations for the <see cref="TaskFile"/> entity.
    /// </summary>
    public interface ITaskFilesRepository
    {
        /// <summary>
        /// Retrieves all task files from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="TaskFile"/> objects.</returns>
        Task<List<TaskFile>> GetAllTaskFiles(IDbConnection connection, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves all files associated with the specified task from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskId">The ID of the task whose files should be retrieved.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>
        /// A list of <see cref="TaskFile"/> objects linked to the given task.
        /// </returns>
        Task<List<TaskFile>> GetAllTaskFilesByTaskId(IDbConnection connection, int taskId,IDbTransaction? transaction = null);

        /// <summary>
        /// Adds a new task file to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="roleToAdd">The <see cref="TaskFile"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task AddTaskFile(IDbConnection connection, TaskFile taskFileToAdd, IDbTransaction? transaction = null);

        /// <summary>
        /// Deletes the task file with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskFileId">The ID of the task file to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task DeleteTaskFile(IDbConnection connection, int taskFileId, IDbTransaction? transaction = null);

        /// <summary>
        /// Deletes all files associated with the specified task from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskId">The ID of the task whose files should be deleted.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAllTaskFilesOfATask(IDbConnection connection, int taskId, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves a single task file by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskFileId">The ID of the task file to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="TaskFile"/> with the specified ID, or <c>null</c> if not found.</returns>
        Task<TaskFile?> GetTaskFileById(IDbConnection connection, int taskFileId, IDbTransaction? transaction = null);

        /// <summary>
        /// Updates an existing role in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="role">The <see cref="TaskFile"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task UpdateTaskFile(IDbConnection connection, TaskFile taskFile, IDbTransaction? transaction = null);
    }
}
