using Dapper;
using System.Data;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories
{
    public class TaskFilesRepository : ITaskFilesRepository
    {
        /// <inheritdoc />
        public async Task<List<TaskFile>> GetAllTaskFiles(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM TaskFiles";
                var files = await connection.QueryAsync<TaskFile>(sql, transaction);
                return files.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all task files.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<List<TaskFile>> GetAllTaskFilesByTaskId(IDbConnection connection, int taskId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM TaskFiles WHERE TaskId = @TaskId";
                var files = await connection.QueryAsync<TaskFile>(sql,new {TaskId = taskId} ,transaction);
                return files.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all task files.", ex);
            }
        }

        /// <inheritdoc />
        public async Task AddTaskFile(IDbConnection connection, TaskFile taskFileToAdd, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"INSERT INTO TaskFiles (TaskId,FileId) VALUES (@TaskId,@FileId);";
                await connection.ExecuteAsync(sql, new { TaskId = taskFileToAdd.TaskId, FileId =taskFileToAdd.FileId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the task file.", ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteTaskFile(IDbConnection connection, int taskFileId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM TaskFiles WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Id = taskFileId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the task file.", ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAllTaskFilesOfATask(IDbConnection connection, int taskId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM TaskFiles WHERE TaskId = @TaskId;";
                await connection.ExecuteAsync(sql, new { TaskId = taskId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the task files of a task.", ex);
            }
        }

        /// <inheritdoc />
        public Task<TaskFile?> GetTaskFileById(IDbConnection connection, int taskFileId, IDbTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task UpdateTaskFile(IDbConnection connection, TaskFile taskFile, IDbTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }
    }
}
