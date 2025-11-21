using System.Data;
using ToDoList.Models;

namespace ToDoList.DatabaseAccess.Repositories.IRepositories
{
    /// <summary>
    /// Defines CRUD operations for the <see cref="FileItem"/> entity.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Retrieves all files from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>A list of all <see cref="FileItem"/> objects.</returns>
        Task<List<FileItem>> GetAllFiles(IDbConnection connection, IDbTransaction? transaction = null);


        /// <summary>
        /// Retrieves all files whose IDs are included in the provided list.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileIds">A collection of file IDs to filter by.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>
        /// A list of <see cref="FileItem"/> objects matching the specified IDs.
        /// </returns>
        Task<List<FileItem>> GetFilesByIds(IDbConnection connection, IEnumerable<int> fileIds, IDbTransaction? transaction = null);

        /// <summary>
        /// Adds a new file to the database and returns its generated ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileToAdd">The <see cref="FileItem"/> to insert.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task<int> AddFile(IDbConnection connection, FileItem fileToAdd, IDbTransaction? transaction = null);


        /// <summary>
        /// Deletes the file with the specified ID from the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileId">The ID of the role to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task DeleteFile(IDbConnection connection, int fileId, IDbTransaction? transaction = null);


        /// <summary>
        /// Deletes all files whose IDs are included in the provided list.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileIds">A collection of file IDs to delete.</param>
        /// <param name="transaction">Optional database transaction.</param>
        public Task DeleteFiles(IDbConnection connection, IEnumerable<int> fileIds, IDbTransaction? transaction = null);


        /// <summary>
        /// Retrieves a single file by its ID.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileId">The ID of the file to retrieve.</param>
        /// <param name="transaction">Optional database transaction.</param>
        /// <returns>The <see cref="FileItem"/> with the specified ID, or <c>null</c> if not found.</returns>
        Task<FileItem?> GetFileById(IDbConnection connection, int fileId, IDbTransaction? transaction = null);


        /// <summary>
        /// Updates an existing file in the database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="fileToUpdate">The <see cref="FileItem"/> object containing updated data.</param>
        /// <param name="transaction">Optional database transaction.</param>
        Task UpdateFile(IDbConnection connection, FileItem fileToUpdate, IDbTransaction? transaction = null);

    }
}
