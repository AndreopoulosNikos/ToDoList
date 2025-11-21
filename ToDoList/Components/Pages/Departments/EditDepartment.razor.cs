using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Departments
{
    /// <summary>
    /// Component for editing an existing department.
    /// </summary>
    public partial class EditDepartment
    {
        /// <summary>
        /// The ID of the department to edit. This is passed as a route parameter.
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
        /// Repository for performing CRUD operations on departments.
        /// </summary>
        [Inject]
        IDepartmentRepository DepartmentRepository { get; set; } = default!;

        /// <summary>
        /// The department being edited.
        /// </summary>
        public UpdateDepartment? departmentToUpdate { get; set; } = new();

        /// <summary>
        /// Stores any error message for display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Loads the department data from the database when the component initializes.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                var department = await DepartmentRepository.GetDepartmentById(connection, Id);

                if (department == null)
                {
                    ErrorMessage = "Department not found.";
                }

                departmentToUpdate!.Id = department!.Id;
                departmentToUpdate!.Name = department.Name;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load department.{ex}";
            }
        }

        /// <summary>
        /// Handles the form submission and updates the department in the database.
        /// </summary>
        private async Task HandleValidSubmit()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                Department department = new Department
                {
                    Id = departmentToUpdate!.Id,
                    Name = departmentToUpdate!.Name
                };

                await DepartmentRepository.UpdateDepartment(connection, department);
                NavigationManager.NavigateTo("/departments/departmentsmanager");
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
            NavigationManager.NavigateTo("/departments/departmentsmanager");
        }
    }

    public class UpdateDepartment
    {
        /// <summary>
        /// The unique identifier for the department.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the department.
        /// </summary>
        [Required(ErrorMessage = "Department name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
