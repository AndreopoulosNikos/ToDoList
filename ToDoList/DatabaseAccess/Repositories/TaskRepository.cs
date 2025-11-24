using Dapper;
using Microsoft.VisualBasic;
using System.Data;
using System.Threading.Tasks;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;


namespace ToDoList.DatabaseAccess.Repositories
{
    /// <summary>
    /// Repository that handles CRUD operations for the <see cref="TaskItem"/> entity.
    /// Uses Dapper to perform database operations.
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        /// <summary>
        /// Retrieves all task items from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="TaskItem"/> objects.</returns>
        public async Task<List<TaskItem>> GetAllTasks(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Tasks ORDER BY Id DESC;";
                var tasks = await connection.QueryAsync<TaskItem>(sql, transaction: transaction);
                return tasks.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving tasks.", ex);
            }
        }


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
        public async Task<List<TaskItem>> GetAllTasksWithInfo(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT t.Id, t.Subject, t.Action, t.DueDate,t.CompletedDate,t.Notes,t.DepartmentId,TaskStatusId, d.Name AS DepartmentName, ts.Name AS TaskStatusName " +
                          "FROM Tasks t " +
                          "LEFT JOIN Departments d ON t.DepartmentId = d.Id " +
                          "LEFT JOIN TaskStatuses ts ON t.TaskStatusId = ts.Id " +
                          "ORDER BY t.Id DESC;";

                var tasks = await connection.QueryAsync<TaskItem>(sql, transaction: transaction);
                return tasks.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving task statuses with info.", ex);
            }
        }


        /// <summary>
        /// Retrieves a single task by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="User"/> with the specified ID, or <c>null</c> if not found.</returns>
        public async Task<TaskItem?> GetTaskById(IDbConnection connection, int taskId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Tasks WHERE Id = @Id;";
                return await connection.QueryFirstOrDefaultAsync<TaskItem>(sql, new { Id = taskId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the task.", ex);
            }
        }

        /// <summary>
        /// Adds a new task to the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskToAdd">The <see cref="TaskItem"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task<int> AddTask(IDbConnection connection, TaskItem taskToAdd, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"INSERT INTO Tasks (Subject,Action,DueDate,CompletedDate,Notes,DepartmentId,TaskStatusId) 
                VALUES (@Subject,@Action,@DueDate,@CompletedDate,@Notes,@DepartmentId,@TaskStatusId);
                SELECT last_insert_rowid();";

                var parameters = new
                {
                    Subject = taskToAdd.Subject,
                    Action = taskToAdd.Action,
                    DueDate = taskToAdd.DueDate,
                    CompletedDate = taskToAdd.CompletedDate,
                    Notes = taskToAdd.Notes,
                    DepartmentId = taskToAdd.DepartmentId,
                    TaskStatusId = taskToAdd.TaskStatusId,

                };
               
                int id = await connection.ExecuteScalarAsync<int>(sql, parameters, transaction);
                return id;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the task.", ex);
            }
        }

        /// <summary>
        /// Deletes the task with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task DeleteTask(IDbConnection connection, int taskId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM Tasks WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Id = taskId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the task.", ex);
            }
        }


        /// <summary>
        /// Updates an existing task in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="task">The <see cref="TaskItem"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task UpdateTask(IDbConnection connection, TaskItem task, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"
                UPDATE Tasks
                SET Subject = @Subject,
                    Action = @Action,
                    DueDate = @DueDate,
                    CompletedDate = @CompletedDate,
                    Notes = @Notes,
                    DepartmentId = @DepartmentId,   
                    TaskStatusId = @TaskStatusId
                WHERE Id = @Id;";

                var parameters = new
                {
                    Subject = task.Subject,
                    Action = task.Action,
                    DueDate = task.DueDate,
                    CompletedDate = task.CompletedDate,
                    Notes = task.Notes,
                    DepartmentId = task.DepartmentId,
                    TaskStatusId = task.TaskStatusId,
                    Id = task.Id
                };

                await connection.ExecuteAsync(sql, parameters, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the task.", ex);
            }
        }
    }
}
