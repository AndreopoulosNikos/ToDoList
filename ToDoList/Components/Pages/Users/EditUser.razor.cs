using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Users
{
    /// <summary>
    /// Component for editing an existing user's information, including username, name, department, and role.
    /// </summary>
    public partial class EditUser
    {
        /// <summary>
        /// The ID of the user to edit. Provided as a route parameter.
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
        /// Repository for performing CRUD operations on users.
        /// </summary>
        [Inject]
        IUserRepository UserRepository { get; set; } = default!;

        /// <summary>
        /// Repository for retrieving departments.
        /// </summary>
        [Inject]
        IDepartmentRepository DepartmentRepository { get; set; } = default!;

        /// <summary>
        /// Repository for retrieving roles.
        /// </summary>
        [Inject]
        IRoleRepository RoleRepository { get; set; } = default!;

        /// <summary>
        /// The user being edited.
        /// </summary>
        public UpdateUser editUser { get; set; } = new();

        /// <summary>
        /// List of departments for the dropdown selection.
        /// </summary>
        public List<Department> Departments { get; set; } = new();

        /// <summary>
        /// List of roles for the dropdown selection.
        /// </summary>
        public List<Role> Roles { get; set; } = new();

        /// <summary>
        /// Stores error messages to display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Loads the user data and dropdown values when the component initializes.
        /// </summary>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();

                // load dropdowns
                Departments = await DepartmentRepository.GetAllDepartments(connection);
                Roles = await RoleRepository.GetAllRoles(connection);

                // load user
                var user = await UserRepository.GetUserById(connection, Id);
                if (user == null)
                {
                    ErrorMessage = "User not found.";
                    return;
                }

                editUser.Id = user.Id;
                editUser.Username = user.Username;
                editUser.FirstName = user?.FirstName ?? string.Empty;
                editUser.LastName = user?.LastName ?? string.Empty;
                editUser.DepartmentId = user?.DepartmentId;
                editUser.RoleId = user?.RoleId;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load user. {ex.Message}";
            }
        }

        /// <summary>
        /// Handles the submission of the edit user form and updates the user in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous update operation.</returns>
        private async Task HandleValidSubmit()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();

                var user = new User
                {
                    Id = editUser.Id,
                    Username = editUser.Username,
                    FirstName = editUser.FirstName,
                    LastName = editUser.LastName,
                    DepartmentId = editUser.DepartmentId,
                    RoleId = editUser.RoleId,
                };

                await UserRepository.UpdateUser(connection, user);

                NavigationManager.NavigateTo("/users/usersmanager");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update user. {ex.Message}";
            }
        }

        /// <summary>
        /// Cancels editing and navigates back to the users manager page.
        /// </summary>
        private void Cancel()
        {
            NavigationManager.NavigateTo("/users/usersmanager");
        }
    }

    /// <summary>
    /// Model representing the data for a user being edited.
    /// </summary>
    public class UpdateUser
    {
        /// <summary>
        /// The unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The username of the user.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The first name of the user.
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// The department ID assigned to the user.
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// The role ID assigned to the user.
        /// </summary>
        public int? RoleId { get; set; }
    }
}
