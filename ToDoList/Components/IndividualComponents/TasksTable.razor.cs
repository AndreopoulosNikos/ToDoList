using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.IndividualComponents
{
    public partial class TasksTable
    {
        /// <summary>
        /// Gets or sets the list of tasks to display in the table.
        /// </summary>
        [Parameter]
        public List<TaskItem> Tasks { get; set; } = new();

        [Parameter]
        public EventCallback<int> OnTaskDeleted { get; set; }

        [CascadingParameter]
        public CultureInfo BrowserCulture { get; set; } = CultureInfo.InvariantCulture;


        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;

        /// <summary>
        /// The connection factory used to create database connections.
        /// </summary>
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = default!;

        /// <summary>
        /// The repository used to perform CRUD operations on tasks.
        /// </summary>
        [Inject]
        private ITaskRepository TaskRepository { get; set; } = default!;

        /// <summary>
        /// The repository used to manage files attached to tasks.
        /// </summary>
        [Inject]
        private IFileRepository FileRepository { get; set; } = default!;

        /// <summary>
        /// The repository used to manage the TaskFiles relationships.
        /// </summary>
        [Inject]
        private ITaskFilesRepository TaskFilesRepository { get; set; } = default!;

        /// <summary>
        /// Stores error messages to display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Deletes the task with the specified ID, including all associated files and TaskFiles entries.
        /// Updates the UI after deletion.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        private async Task DeleteTask(int taskId)
        {
            using var connection = ConnectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var taskFiles = await TaskFilesRepository.GetAllTaskFilesByTaskId(connection, taskId);
                await TaskFilesRepository.DeleteAllTaskFilesOfATask(connection, taskId, transaction);

                var taskFileIds = taskFiles.Select(tf => tf.FileId).ToList();
                var filesOfTask = await FileRepository.GetFilesByIds(connection, taskFileIds);

                foreach (var file in filesOfTask)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                }

                await FileRepository.DeleteFiles(connection, taskFileIds, transaction);
                await TaskRepository.DeleteTask(connection, taskId, transaction);

                transaction.Commit();

                // Notify parent component
                if (OnTaskDeleted.HasDelegate)
                {
                    await OnTaskDeleted.InvokeAsync(taskId);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorMessage = $"Failed to delete task: {ex.Message}";
            }
        }

        /// <summary>
        /// Navigates to the Edit Task page for the specified task ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to edit.</param>
        private void EdiTask(int taskId)
        {
            NavigationManager.NavigateTo($"/tasks/edittask/{taskId}");
        }
        #region Pagination

        /// <summary>
        /// The number of tasks displayed per page.
        /// </summary>
        private int _pageSize = 5;

        /// <summary>
        /// The current page number in the pagination.
        /// </summary>
        private int _currentPage = 1;

        /// <summary>
        /// Gets or sets the page size. Resets to the first page if changed.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    _currentPage = 1;
                    StateHasChanged();
                }
            }
        }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int CurrentPage => _currentPage;

        /// <summary>
        /// Gets the total number of pages based on the number of tasks and page size.
        /// </summary>
        public int TotalPages => Tasks.Any()
            ? (int)Math.Ceiling((double)Tasks.Count / PageSize)
            : 1;

        /// <summary>
        /// Gets the list of tasks for the current page.
        /// </summary>
        public IEnumerable<TaskItem> PagedTasks =>
            Tasks.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        /// <summary>
        /// Returns true if the current page is the first page.
        /// </summary>
        public bool IsFirstPage => CurrentPage == 1;

        /// <summary>
        /// Returns true if the current page is the last page.
        /// </summary>
        public bool IsLastPage => CurrentPage == TotalPages;

        /// <summary>
        /// Navigates to the first page.
        /// </summary>
        public void FirstPage()
        {
            _currentPage = 1;
            StateHasChanged();
        }

        /// <summary>
        /// Navigates to the previous page, if available.
        /// </summary>
        public void PreviousPage()
        {
            if (_currentPage > 1)
                _currentPage--;

            StateHasChanged();
        }

        /// <summary>
        /// Navigates to the next page, if available.
        /// </summary>
        public void NextPage()
        {
            if (_currentPage < TotalPages)
                _currentPage++;

            StateHasChanged();
        }

        /// <summary>
        /// Navigates to the last page.
        /// </summary>
        public void LastPage()
        {
            _currentPage = TotalPages;
            StateHasChanged();
        }

        #endregion
    }
}
