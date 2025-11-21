using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ToDoList.Models;

namespace ToDoList.Components.Pages
{
    /// <summary>
    /// Component for managing user login, including invoking a JavaScript API call for authentication.
    /// </summary>
    public partial class LoginManager : ComponentBase
    {
        /// <summary>
        /// The login model bound to the login form.
        /// </summary>
        private Login loginModel = new();

        /// <summary>
        /// Stores error messages related to login attempts.
        /// </summary>
        private string? loginError;

        /// <summary>
        /// JavaScript runtime for invoking JavaScript functions.
        /// </summary>
        [Inject]
        IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        /// <summary>
        /// Reference to the imported JavaScript module used for login.
        /// </summary>
        private IJSObjectReference? jsModule;

        /// <summary>
        /// Invoked after the component has rendered.
        /// Imports the JavaScript module for login handling on first render.
        /// </summary>
        /// <param name="firstRender">Indicates whether this is the first time the component is rendered.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            jsModule = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./Components/Pages/LoginManager.razor.js");
        }

        /// <summary>
        /// Handles the login form submission by calling the JavaScript login API.
        /// Updates the UI with any error messages if login fails.
        /// </summary>
        /// <returns>A task representing the asynchronous login operation.</returns>
        private async Task HandleLogin()
        {
            try
            {
                await jsModule!.InvokeVoidAsync("loginApi",
                    "/api/authentication/login",
                    loginModel);

                loginError = null;
                Navigation.NavigateTo("/", forceLoad: true);
                return;
            }
            catch
            {
                loginError = "Invalid username or password.";
            }
        }
    }
}
