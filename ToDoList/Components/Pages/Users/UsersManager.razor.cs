using Microsoft.AspNetCore.Components;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Users
{
    public partial class UsersManager
    {
        /// <summary>
        /// List of all users displayed in the UI.
        /// </summary>
        public List<User> UsersList { get; set; } = new List<User>();

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
        /// Repository for performing CRUD operations on users.
        /// </summary>
        [Inject]
        private IUserRepository UserRepository { get; set; } = null!;

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
                UsersList = await UserRepository.GetAllUsersWithAllInfo(connection);
            }
            catch (Exception ex)
            {
                // Set an error message for the UI
                ErrorMessage = $"Failed to load users. Please try again later. {ex}";
            }
        }
    }
}
