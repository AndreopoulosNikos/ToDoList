using System.Data;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories.IRepositories
{
    public interface ITaskRepository
    {
        /// <summary>
        /// Retrieves all task items from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="TaskItem"/> objects.</returns>
        Task<List<TaskItem>> GetAllTasks(IDbConnection connection, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves all tasks from the database along with their associated department and status information.
        /// Each task object will include the <c>DepartmentName</c> and <c>TaskStatusName</c> properties, if available.
        /// </summary>
        /// <param name="connection"> The database connection to use for the query. </param> 
        /// <param name="transaction"> An optional database transaction to associate with the query. </param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The result contains a list of <see cref="TaskItem"/> objects with full task, departments, and task status information.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if an error occurs while retrieving the task from the database.
        /// </exception>
        Task<List<TaskItem>> GetAllTasksWithInfo(IDbConnection connection, IDbTransaction? transaction = null);

        /// <summary>
        /// Adds a new task to the database and returns the generated ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskToAdd">The <see cref="TaskItem"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task<int> AddTask(IDbConnection connection, TaskItem taskToAdd, IDbTransaction? transaction = null);

        /// <summary>
        /// Deletes the task with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task DeleteTask(IDbConnection connection, int taskId, IDbTransaction? transaction = null);

        /// <summary>
        /// Retrieves a single task by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="User"/> with the specified ID, or <c>null</c> if not found.</returns>
        Task<TaskItem?> GetTaskById(IDbConnection connection, int taskId, IDbTransaction? transaction = null);

        /// <summary>
        /// Updates an existing task in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="task">The <see cref="TaskItem"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task UpdateTask(IDbConnection connection, TaskItem task, IDbTransaction? transaction = null);
    }
}
