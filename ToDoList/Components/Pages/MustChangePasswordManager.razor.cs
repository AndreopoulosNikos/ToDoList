using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;
using static ToDoList.Components.Pages.Users.AddUser;

namespace ToDoList.Components.Pages
{
    /// <summary>
    /// Component for managing required password changes for users who must change their password on next login.
    /// </summary>
    public partial class MustChangePasswordManager
    {
        /// <summary>
        /// The model for the password change form.
        /// </summary>
        private MustChangePassword mustChangePasswordModel = new();

        /// <summary>
        /// Stores error messages related to the password change process.
        /// </summary>
        private string? mustChangePasswordError;

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        /// <summary>
        /// Authentication state provider used to get the current logged-in user.
        /// </summary>
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

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
        /// Handles the submission of the password change form.
        /// Updates the user's password in the database and redirects to the login page.
        /// </summary>
        /// <returns>A task representing the asynchronous password change operation.</returns>
        private async Task HandlePasswordChange()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity == null || !user.Identity.IsAuthenticated)
                {
                    Navigation.NavigateTo("/loginmanager", forceLoad: true);
                    return;
                }

                string username = user.Identity.Name!;
                using var connection = ConnectionFactory.CreateConnection();
                var existingUser = await UserRepository.GetUserByUsername(connection, username);
                if (existingUser == null)
                {
                    Navigation.NavigateTo("/loginmanager", forceLoad: true);
                    return;
                }

                var hasher = new PasswordHasher<User>();
                var hashedPassword = hasher.HashPassword(new User(), mustChangePasswordModel.NewPassword);

                await UserRepository.UpdateUserPassword(connection, hashedPassword, existingUser.Id, false);
                Navigation.NavigateTo("/loginmanager", forceLoad: true);
                return;

            }
            catch
            {
                mustChangePasswordError = "Invalid username or password.";
            }
        }

        /// <summary>
        /// Model representing the required data for a user to change their password.
        /// </summary>
        public class MustChangePassword
        {
            /// <summary>
            /// Regular expression used to validate the new password.
            /// </summary>
            private const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$";

            /// <summary>
            /// The new password for the user.
            /// </summary>
            [Required(ErrorMessage = "New Password is required.")]
            [RegularExpression(PasswordRegex,
            ErrorMessage = "Password must be at least 8 characters, with at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
            public string NewPassword { get; set; } = string.Empty;

            /// <summary>
            /// Confirmation of the new password.
            /// </summary>
            [Required(ErrorMessage = "Confirm Password is required.")]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
