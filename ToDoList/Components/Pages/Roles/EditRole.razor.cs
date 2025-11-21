using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Roles
{
    public partial class EditRole
    {
        /// <summary>
        /// The ID of the role to edit. This is passed as a route parameter.
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
        /// Repository for performing CRUD operations on roles.
        /// </summary>
        [Inject]
        IRoleRepository RoleRepository { get; set; } = default!;

        /// <summary>
        /// The role being edited.
        /// </summary>
        public UpdateRole? roleToUpdate { get; set; } = new();

        /// <summary>
        /// Stores any error message for display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Loads the role data from the database when the component initializes.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                var updatedRole = await RoleRepository.GetRoleById(connection, Id);

                if (updatedRole == null)
                {
                    ErrorMessage = "Role not found.";
                }

                roleToUpdate!.Name = updatedRole!.Name;
                roleToUpdate.Id = updatedRole.Id;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load role.{ex}";
            }
        }

        /// <summary>
        /// Handles the form submission and updates the role in the database.
        /// </summary>
        private async Task HandleValidSubmit()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                Role role = new Role
                {
                    Id = Id,
                    Name = roleToUpdate!.Name
                };
                await RoleRepository.UpdateRole(connection, role!);
                NavigationManager.NavigateTo("/roles/rolesmanager");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update department.{ex}";
            }
        }

        /// <summary>
        /// Cancels the edit and navigates back to the departments list.
        /// </summary>
        private void Cancel()
        {
            NavigationManager.NavigateTo("/roles/rolesmanager");
        }
    }

    public class UpdateRole
    {
        public int Id { get; set; }

        /// <summary>
        /// The name of the role.
        /// </summary>
        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
