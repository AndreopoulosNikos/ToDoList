using Dapper;
using System.Data;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories
{
    public class FileRepository : IFileRepository
    {

        /// <summary>
        /// Retrieves all files from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="FileItem"/> objects.</returns>
        public async Task<List<FileItem>> GetAllFiles(IDbConnection connection, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Tasks";
                var files = await connection.QueryAsync<FileItem>(sql, transaction: transaction);
                return files.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving files.", ex);
            }
        }


        /// <summary>
        /// Retrieves all files whose IDs are included in the provided list.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileIds">A collection of file IDs to filter by.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>
        /// A list of <see cref="FileItem"/> objects matching the specified IDs.
        /// </returns>
        public async Task<List<FileItem>> GetFilesByIds(IDbConnection connection, IEnumerable<int> fileIds,IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Files WHERE Id IN @Ids";
                var result = await connection.QueryAsync<FileItem>(
                    sql,
                    new { Ids = fileIds },
                    transaction: transaction
                );

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving files by IDs.", ex);
            }
        }


        ///<inheritdoc/>
        public async Task<int> AddFile(IDbConnection connection, FileItem fileToAdd, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"
                    INSERT INTO Files (Filename, FilePath)
                    VALUES (@Filename, @FilePath);
                    SELECT last_insert_rowid();
                ";

                var id = await connection.ExecuteScalarAsync<int>(
                    sql,
                    new { fileToAdd.Filename, fileToAdd.FilePath },
                    transaction
                );

                return id;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting file.", ex);
            }
        }


        /// <summary>
        /// Deletes the file with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileId">The ID of the role to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task DeleteFile(IDbConnection connection, int fileId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM Files WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Id = fileId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the file.", ex);
            }
        }

        /// <summary>
        /// Deletes all files whose IDs are included in the provided list.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileIds">A collection of file IDs to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task DeleteFiles(IDbConnection connection,IEnumerable<int> fileIds,IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"DELETE FROM Files WHERE Id IN @Ids;";
                await connection.ExecuteAsync(sql, new { Ids = fileIds }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the files.", ex);
            }
        }
        /// <summary>
        /// Retrieves a single file by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileId">The ID of the file to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="FileItem"/> with the specified ID, or <c>null</c> if not found.</returns>
        public async Task<FileItem?> GetFileById(IDbConnection connection, int fileId, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = "SELECT * FROM Files WHERE Id = @Id;";
                return await connection.QueryFirstOrDefaultAsync<FileItem>(sql, new { Id = fileId }, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the file.", ex);
            }
        }


        /// <summary>
        /// Updates an existing file in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileToUpdate">The <see cref="FileItem"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public async Task UpdateFile(IDbConnection connection, FileItem fileToUpdate, IDbTransaction? transaction = null)
        {
            try
            {
                var sql = @"UPDATE Files SET Filename = @Filename, FilePath = @FilePath WHERE Id = @Id;";
                await connection.ExecuteAsync(sql, new { Filename = fileToUpdate.Filename,FilePath = fileToUpdate.FilePath,Id=fileToUpdate.Id}, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the file.", ex);
            }
        }
    }
}
