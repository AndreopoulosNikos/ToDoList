using Microsoft.AspNetCore.Components;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.IndividualComponents
{
    public partial class UsersTable
    {
        /// <summary>
        /// Gets or sets the list of Users to display in the table.
        /// </summary>
        [Parameter]
        public List<User> Users { get; set; } = new();

        /// <summary>
        /// The connection factory used to create database connections.
        /// </summary>
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = null!;

        /// <summary>
        /// The repository used to perform CRUD operations on Users.
        /// </summary>
        [Inject]
        private IUserRepository UsersRepository { get; set; } = null!;

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
        /// Navigates to the Edit User page for the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to edit.</param>
        private void EditUser(int userId)
        {
            NavigationManager.NavigateTo($"users/edituser/{userId}");
        }

        /// <summary>
        /// Deletes the user with the specified ID from the database
        /// and reloads the list of users.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        private async Task DeleteUser(int userId)
        {
            try
            {
                var connection = ConnectionFactory.CreateConnection();
                await UsersRepository.DeleteUser(connection, userId);
                Users = await UsersRepository.GetAllUsersWithAllInfo(connection);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete User.{ex}";
            }
        }

        #region Pagination

        /// <summary>
        /// The number of users displayed per page.
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
        /// Gets the total number of pages based on the number of users and page size.
        /// </summary>
        public int TotalPages => Users.Any()
            ? (int)Math.Ceiling((double)Users.Count / PageSize)
            : 1;

        /// <summary>
        /// Gets the list of users for the current page.
        /// </summary>
        public IEnumerable<User> PagedUsers =>
            Users.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

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
