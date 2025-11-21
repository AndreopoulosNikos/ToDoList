using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace ToDoList.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        //[Inject]
        //private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;


        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender) 
        //    {
        //        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        //        var user = authState.User;
        //        IsAdmin = user.Identity != null && user.Identity.IsAuthenticated && user.IsInRole("Admin");
        //        StateHasChanged();
        //        return;
        //    }
          
        //}
    }
}
