using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;

namespace ToDoList.Components.Pages.Roles
{
    /// <summary>
    /// Component for adding a new role.
    /// </summary>
    public partial class AddRole
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
        /// Repository for performing CRUD operations on roles.
        /// </summary>
        [Inject]
        IRoleRepository RoleRepository { get; set; } = default!;

        /// <summary>
        /// The role data entered in the form.
        /// </summary>
        private NewRole newRole = new();

        /// <summary>
        /// Stores any error message for display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Handles the form submission and adds a new role to the database.
        /// </summary>
        private async Task HandleValidSubmit()
        {
            try
            {
                var connection = ConnectionFactory.CreateConnection();
                await RoleRepository.AddRole(connection,new Models.Role { Name = newRole.Name });
                NavigationManager.NavigateTo("/roles/rolesmanager");
            }
            catch (Exception ex)
            {
                // Set an error message for the UI
                ErrorMessage = $"Failed to add new Role. {ex}";
            }
        }

        /// <summary>
        /// Cancels the add operation and navigates back to the department list.
        /// </summary>
        private void Cancel()
        {
            NavigationManager.NavigateTo("/roles/rolesmanager");
        }
    }

    /// <summary>
    /// Model representing the input data for creating a new role.
    /// </summary>
    public class NewRole
    {
        /// <summary>
        /// The name of the role.
        /// </summary>
        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
