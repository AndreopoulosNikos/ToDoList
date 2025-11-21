using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;

namespace ToDoList.Components.Pages.TaskStatuses
{
    /// <summary>
    /// Component for adding a new task status.
    /// </summary>
    public partial class AddTaskStatus
    {
        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// Factory used to create database connections.
        /// </summary>
        [Inject]
        IConnectionFactory ConnectionFactory { get; set; } = default!;

        /// <summary>
        /// Repository for performing CRUD operations on task statuses.
        /// </summary>
        [Inject]
        ITaskStatusRepository TaskStatusRepository { get; set; } = default!;

        /// <summary>
        /// The task status data entered in the form.
        /// </summary>
        private NewTaskStatus newTaskStatus = new();

        /// <summary>
        /// Stores any error message for display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Handles the form submission and adds a new task status to the database.
        /// </summary>
        private async Task HandleValidSubmit()
        {
            try
            {
                var connection = ConnectionFactory.CreateConnection();
                await TaskStatusRepository.AddTaskStatus(connection, new Models.TaskStatusItem { Name = newTaskStatus.Name });
                NavigationManager.NavigateTo("/taskstatuses/taskstatusesmanager");
            }
            catch (Exception ex)
            {
                // Set an error message for the UI
                ErrorMessage = $"Failed to add new Task Status. {ex}";
            }
        }

        /// <summary>
        /// Cancels the add operation and navigates back to the task statuses list.
        /// </summary>
        private void Cancel()
        {
            NavigationManager.NavigateTo("/taskstatuses/taskstatusesmanager");
        }
    }

    /// <summary>
    /// Model representing the input data for creating a new task status.
    /// </summary>
    public class NewTaskStatus
    {
        /// <summary>
        /// The name of the department.
        /// </summary>
        [Required(ErrorMessage = "Task Status name is required")]
        public string Name { get; set; } = string.Empty;
    }

}
