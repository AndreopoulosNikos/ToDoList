using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Departments
{
    /// <summary>
    /// Component that displays and manages the list of departments.
    /// </summary>
    public partial class DepartmentsManager
    {
        /// <summary>
        /// List of all departments displayed in the UI.
        /// </summary>
        public List<Department> DepartmentsList { get; set; } = new List<Department>();

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
        /// Repository for performing CRUD operations on departments.
        /// </summary>
        [Inject]
        private IDepartmentRepository DepartmentRepository { get; set; } = null!;

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        protected NavigationManager Navigation { get; set; } = null!;

        /// <summary>
        /// Loads or refreshes the list of departments when component parameters are set.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                DepartmentsList = await DepartmentRepository.GetAllDepartments(connection);
            }
            catch (Exception ex)
            {
                // Set an error message for the UI
                ErrorMessage = $"Failed to load departments. Please try again later. {ex}";
            }
        }
    }
}
