using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Users
{
    /// <summary>
    /// Component for adding a new user to the system, including assigning roles and departments.
    /// </summary>
    public partial class AddUser
    {
        /// <summary>
        /// The user being added.
        /// </summary>
        private NewUser newUser = new();

        /// <summary>
        /// Stores error messages to display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// The list of available departments for selection.
        /// </summary>
        private List<Department> Departments = new();

        /// <summary>
        /// The list of available roles for selection.
        /// </summary>
        private List<Role> Roles = new();

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// Connection factory used to create database connections.
        /// </summary>
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = default!;

        /// <summary>
        /// Repository for performing CRUD operations on users.
        /// </summary>
        [Inject]
        private IUserRepository UserRepository { get; set; } = default!;

        /// <summary>
        /// Repository for retrieving departments.
        /// </summary>
        [Inject]
        private IDepartmentRepository DepartmentRepository { get; set; } = default!;

        /// <summary>
        /// Repository for retrieving roles.
        /// </summary>
        [Inject]
        private IRoleRepository RoleRepository { get; set; } = default!;

        /// <summary>
        /// Regular expression pattern for password validation.
        /// </summary>
        private const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$";

        /// <summary>
        /// Loads the list of departments and roles when the component initializes.
        /// </summary>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                Departments = await DepartmentRepository.GetAllDepartments(connection);
                Roles = await RoleRepository.GetAllRoles(connection);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load roles or departments: {ex.Message}";
            }
        }

        /// <summary>
        /// Handles the submission of the Add User form.
        /// </summary>
        /// <returns>A task representing the asynchronous add operation.</returns>
        private async Task HandleValidSubmit()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();

                // Create a hasher
                var hasher = new PasswordHasher<User>();
                var hashedPassword = hasher.HashPassword(new User(), newUser.Password);

                await UserRepository.AddUser(connection, new User
                {
                    Username = newUser.Username,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    DepartmentId = newUser.DepartmentId,
                    RoleId = newUser.RoleId,
                    HashedPassword = hashedPassword,
                    MustChangePassword = newUser.MustChangePassword
                });

                NavigationManager.NavigateTo("/users/usersmanager");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to add user: {ex.Message}";
            }
        }

        /// <summary>
        /// Cancels the add user operation and navigates back to the users manager page.
        /// </summary>
        private void Cancel()
        {
            NavigationManager.NavigateTo("/users/usersmanager");
        }

        /// <summary>
        /// Model representing a new user to be added.
        /// </summary>
        public class NewUser
        {
            /// <summary>
            /// The username of the new user.
            /// </summary>
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; } = string.Empty;

            /// <summary>
            /// The first name of the new user.
            /// </summary>
            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; } = string.Empty;

            /// <summary>
            /// The last name of the new user.
            /// </summary>
            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; } = string.Empty;

            /// <summary>
            /// The ID of the department to assign to the new user.
            /// </summary>
            [Required(ErrorMessage = "Department is required")]
            public int? DepartmentId { get; set; } = null;

            /// <summary>
            /// The ID of the role to assign to the new user.
            /// </summary>
            public int? RoleId { get; set; } = null;

            /// <summary>
            /// The password for the new user.
            /// </summary>
            [Required(ErrorMessage = "Password is required")]
            [RegularExpression(PasswordRegex, ErrorMessage = "Password must be at least 8 characters, include uppercase, lowercase, number, and special character")]
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// The confirmation of the password.
            /// </summary>
            [Required(ErrorMessage = "Confirm password is required")]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; } = string.Empty;

            /// <summary>
            /// Indicates whether the user must change their password on next login.
            /// </summary>
            public bool MustChangePassword { get; set; }
        }
    }
}
