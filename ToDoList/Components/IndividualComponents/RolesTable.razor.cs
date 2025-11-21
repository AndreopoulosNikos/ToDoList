using Microsoft.AspNetCore.Components;
using System.Data;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.IndividualComponents
{
    public partial class RolesTable
    {
        /// <summary>
        /// Gets or sets the list of roles to display in the table.
        /// </summary>
        [Parameter]
        public List<Role> Roles { get; set; } = new();

        /// <summary>
        /// The connection factory used to create database connections.
        /// </summary>
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = null!;

        /// <summary>
        /// The repository used to perform CRUD operations on roles.
        /// </summary>
        [Inject]
        private IRoleRepository RoleRepository { get; set; } = null!;

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;

        /// <summary>
        /// Stores error messages to display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Performs any component initialization logic.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Navigates to the Edit Role page for the specified role ID.
        /// </summary>
        /// <param name="id">The ID of the role to edit.</param>
        private void EditRole(int id)
        {
            NavigationManager.NavigateTo($"/roles/editrole/{id}");
        }

        /// <summary>
        /// Deletes the role with the specified ID from the database
        /// and reloads the list of roles.
        /// </summary>
        /// <param name="roleId">The ID of the role to delete.</param>
        private async Task DeleteRole(int roleId)
        {
            try
            {
                var connection = ConnectionFactory.CreateConnection();
                await RoleRepository.DeleteRole(connection, roleId);
                Roles = await RoleRepository.GetAllRoles(connection);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete Role.{ex}";
            }
        }

        #region Pagination

        /// <summary>
        /// The number of roles displayed per page.
        /// </summary>
        private int _pageSize = 5;

        /// <summary>
        /// The current page number in the pagination.
        /// </summary>
        private int _currentPage = 1;

        /// <summary>
        /// Gets or sets the page size and resets to the first page if changed.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    _currentPage = 1;
                    StateHasChanged();
                }
            }
        }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int CurrentPage => _currentPage;

        /// <summary>
        /// Gets the total number of pages based on the number of roles and page size.
        /// </summary>
        public int TotalPages => Roles.Any()
            ? (int)Math.Ceiling((double)Roles.Count / PageSize)
            : 1;

        /// <summary>
        /// Gets the list of roles for the current page.
        /// </summary>
        public IEnumerable<Role> PagedRoles=>
            Roles.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        /// <summary>
        /// Returns true if the current page is the first page.
        /// </summary>
        public bool IsFirstPage => CurrentPage == 1;

        /// <summary>
        /// Returns true if the current page is the last page.
        /// </summary>
        public bool IsLastPage => CurrentPage == TotalPages;

        /// <summary>
        /// Navigates to the first page.
        /// </summary>
        public void FirstPage()
        {
            _currentPage = 1;
            StateHasChanged();
        }

        /// <summary>
        /// Navigates to the previous page, if available.
        /// </summary>
        public void PreviousPage()
        {
            if (_currentPage > 1)
                _currentPage--;

            StateHasChanged();
        }

        /// <summary>
        /// Navigates to the next page, if available.
        /// </summary>
        public void NextPage()
        {
            if (_currentPage < TotalPages)
                _currentPage++;

            StateHasChanged();
        }

        /// <summary>
        /// Navigates to the last page.
        /// </summary>
        public void LastPage()
        {
            _currentPage = TotalPages;
            StateHasChanged();
        }

        #endregion
    }
}
