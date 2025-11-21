using Microsoft.AspNetCore.Components;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Roles
{
    public partial class RolesManager
    {
        /// <summary>
        /// List of all roles displayed in the UI.
        /// </summary>
        public List<Role> RolesList { get; set; } = new List<Role>();

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
        /// Repository for performing CRUD operations on roles.
        /// </summary>
        [Inject]
        private IRoleRepository RoleRepository { get; set; } = null!;

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        protected NavigationManager Navigation { get; set; } = null!;

        /// <summary>
        /// Loads or refreshes the list of roles when component parameters are set.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                RolesList = await RoleRepository.GetAllRoles(connection);
            }
            catch (Exception ex)
            {
                // Set an error message for the UI
                ErrorMessage = $"Failed to load roles. Please try again later. {ex}";
            }
        }
    }
}
