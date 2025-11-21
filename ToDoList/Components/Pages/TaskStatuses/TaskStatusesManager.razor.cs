using Microsoft.AspNetCore.Components;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.TaskStatuses
{

    /// <summary>
    /// Component that displays and manages the list of task statuses.
    /// </summary>
    public partial class TaskStatusesManager
    {
        /// <summary>
        /// List of all task statuses displayed in the UI.
        /// </summary>
        public List<TaskStatusItem> TaskStatusesList { get; set; } = new List<TaskStatusItem>();

        /// <summary>
        /// Stores any error message for display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Factory used to create database connections.
        /// </summary>
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = null!;

        /// <summary>
        /// Repository for performing CRUD operations on task statuses.
        /// </summary>
        [Inject]
        private ITaskStatusRepository TaskStatusRepository { get; set; } = null!;

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        protected NavigationManager Navigation { get; set; } = null!;


        /// <summary>
        /// Loads or refreshes the list of task statuses when component parameters are set.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                TaskStatusesList = await TaskStatusRepository.GetAllTaskStatuses(connection);
            }
            catch (Exception ex)
            {
                // Set an error message for the UI
                ErrorMessage = $"Failed to load task statuses. Please try again later. {ex}";
            }
        }
    }
}
