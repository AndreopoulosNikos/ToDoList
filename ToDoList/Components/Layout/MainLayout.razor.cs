using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Globalization;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;

namespace ToDoList.Components.Layout
{
    /// <summary>
    /// Represents the main layout component of the application, providing
    /// shared structure and handling logout interactions via JavaScript interop.
    /// </summary>
    public partial class MainLayout : LayoutComponentBase
    {
        private string? ScreenName { get; set; }
        /// <summary>
        /// JavaScript runtime instance used to invoke JavaScript functions from .NET.
        /// </summary>
        [Inject]
        IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Provides utilities for working with navigation and URIs within the application.
        /// </summary>
        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        /// <summary>
        /// Holds a reference to the imported JavaScript module used by this layout.
        /// </summary>
        private IJSObjectReference? jsModule;

        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = null!;

        public CultureInfo BrowserCulture = CultureInfo.InvariantCulture;

        [Inject]
        private IUserRepository UserRepository { get; set; } = null!;

        /// <summary>
        /// Invoked after the component has finished rendering.  
        /// On first render, loads the associated JavaScript module.
        /// </summary>
        /// <param name="firstRender">
        /// A boolean that indicates whether this is the first time the component is rendering.
        /// </param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                jsModule = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./Components/Layout/MainLayout.razor.js");

                // Get browser locale via JS
                var cultureName = await JS.InvokeAsync<string>("eval", "navigator.language");
                BrowserCulture = new CultureInfo(cultureName);
                StateHasChanged(); // re-render to apply culture
            }
        }

        /// <summary>
        /// Called by the framework when component parameters are set or updated. <br/>
        /// Retrieves the current authentication state and checks whether the user <br/>
        /// is required to change their password. If so, and the user is not already <br/>
        /// on the change-password page, redirects them to the appropriate route.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                ScreenName = user.FindFirst("ScreenName")?.Value
                                   ?? user.Identity?.Name; // fallback to username

                var mustChange = user.FindFirst("MustChangePassword")?.Value == "true";

                if (mustChange && !Navigation.Uri.Contains("mustchangepassword"))
                {
                    Navigation.NavigateTo("/mustchangepasswordmanager", forceLoad: true);
                }
            }
         
        }

        /// <summary>
        /// Logs out the currently authenticated user by calling the logout API via JavaScript
        /// and redirects the user to the login page.
        /// </summary>
        private async Task Logout()
        {
            await jsModule!.InvokeVoidAsync("logoutApi", "/api/authentication/logout");
            Navigation.NavigateTo("/loginmanager", forceLoad: true);
        }
    }
}
