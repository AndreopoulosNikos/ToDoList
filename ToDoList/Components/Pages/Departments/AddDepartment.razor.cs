using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;

namespace ToDoList.Components.Pages.Departments
{
    /// <summary>
    /// Component for adding a new department.
    /// </summary>
    public partial class AddDepartment
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
        /// Repository for performing CRUD operations on departments.
        /// </summary>
        [Inject]
        IDepartmentRepository DepartmentRepository { get; set; } = default!;

        /// <summary>
        /// The department data entered in the form.
        /// </summary>
        private NewDepartment newDepartment = new();

        /// <summary>
        /// Stores any error message for display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Handles the form submission and adds a new department to the database.
        /// </summary>
        private async Task HandleValidSubmit()
        {
            try
            {
                var connection = ConnectionFactory.CreateConnection();
                await DepartmentRepository.AddDepartment(connection,new Models.Department { Name = newDepartment.Name });
                NavigationManager.NavigateTo("/departments/departmentsmanager");
            }
            catch (Exception ex)
            {
                // Set an error message for the UI
                ErrorMessage = $"Failed to add new Department. {ex}";
            }
        }

        /// <summary>
        /// Cancels the add operation and navigates back to the department list.
        /// </summary>
        private void Cancel()
        {
            NavigationManager.NavigateTo("/departments/departmentsmanager");
        }
    }

    /// <summary>
    /// Model representing the input data for creating a new department.
    /// </summary>
    public class NewDepartment
    {
        /// <summary>
        /// The name of the department.
        /// </summary>
        [Required(ErrorMessage = "Department name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
