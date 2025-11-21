using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Users
{
    /// <summary>
    /// Component for resetting a user's password and optionally requiring a password change on next login.
    /// </summary>
    public partial class ResetPassword
    {
        /// <summary>
        /// The ID of the user whose password is being reset. Provided as a route parameter.
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
        /// The model representing the new password and options for the password reset.
        /// </summary>
        public ResetPasswordModel resetModel { get; set; } = new();

        /// <summary>
        /// Stores error messages to display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Initializes the component by setting the user ID in the reset model.
        /// </summary>
        /// <returns>A completed task representing the initialization operation.</returns>
        protected override Task OnInitializedAsync()
        {
            resetModel.UserId = Id;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the password reset operation.
        /// </summary>
        /// <returns>A task representing the asynchronous password reset operation.</returns>
        private async Task HandlePasswordReset()
        {
            if (resetModel.NewPassword != resetModel.ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            try
            {
                using var connection = ConnectionFactory.CreateConnection();

                var hasher = new PasswordHasher<User>();
                var hashedPassword = hasher.HashPassword(new User(), resetModel.NewPassword);

                await UserRepository.UpdateUserPassword(
                    connection,
                    hashedPassword,
                    resetModel.UserId,
                    resetModel.MustChangePassword
                );

                NavigationManager.NavigateTo($"/users/edituser/{resetModel.UserId}");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to reset password. {ex.Message}";
            }
        }
    }

    /// <summary>
    /// Model representing the data required to reset a user's password.
    /// </summary>
    public class ResetPasswordModel
    {
        /// <summary>
        /// The ID of the user whose password is being reset.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Regular expression pattern used to validate the new password.
        /// </summary>
        private const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        /// <summary>
        /// The new password for the user.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(PasswordRegex, ErrorMessage = "Password must be at least 8 characters, include uppercase, lowercase, number, and special character")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation of the new password.
        /// </summary>
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user must change their password on next login.
        /// </summary>
        public bool MustChangePassword { get; set; }
    }
}
