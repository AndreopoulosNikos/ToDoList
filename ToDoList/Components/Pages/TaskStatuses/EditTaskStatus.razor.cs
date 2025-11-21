using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.TaskStatuses
{
    public partial class EditTaskStatus
    {
        /// <summary>
        /// The ID of the task status to edit. This is passed as a route parameter.
        /// </summary>
        [Parameter]
        public int Id { get; set; }

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// Connection factory used to create database connections.
        /// </summary>
        [Inject]
        IConnectionFactory ConnectionFactory { get; set; } = default!;

        /// <summary>
        /// Repository for performing CRUD operations on task statuses.
        /// </summary>
        [Inject]
        ITaskStatusRepository TaskStatusRepository { get; set; } = default!;

        /// <summary>
        /// The task status being edited.
        /// </summary>
        public UpdateTaskStatus? taskStatusToUpdate { get; set; } = new();

        /// <summary>
        /// Stores any error message for display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Loads the task status data from the database when the component initializes.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                var taskStatus = await TaskStatusRepository.GetTaskStatusById(connection, Id);

                if (taskStatus == null)
                {
                    ErrorMessage = "Task Status not found.";
                }

                taskStatusToUpdate!.Id = taskStatus!.Id;
                taskStatusToUpdate!.Name = taskStatus.Name;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load task status.{ex}";
            }
        }

        /// <summary>
        /// Handles the form submission and updates the task status in the database.
        /// </summary>
        private async Task HandleValidSubmit()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                TaskStatusItem taskStatus = new TaskStatusItem
                {
                    Id = taskStatusToUpdate!.Id,
                    Name = taskStatusToUpdate!.Name
                };

                await TaskStatusRepository.UpdateTaskStatus(connection, taskStatus);
                NavigationManager.NavigateTo("/taskstatuses/taskstatusesmanager");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update department.{ex}";
            }
        }

        /// <summary>
        /// Cancels the edit and navigates back to the task status list.
        /// </summary>
        private void Cancel()
        {
            NavigationManager.NavigateTo("/taskstatuses/taskstatusesmanager");
        }
    }

    public class UpdateTaskStatus
    {
        /// <summary>
        /// The unique identifier for the task status.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the task status.
        /// </summary>
        [Required(ErrorMessage = "Task Status name is required")]
        public string Name { get; set; } = string.Empty;
    }

}
