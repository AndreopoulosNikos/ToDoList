using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using ToDoList.Components;
using ToDoList.DatabaseAccess.ConnectionFactories;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;

namespace ToDoList
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddRazorComponents().AddInteractiveServerComponents();
            builder.Services.AddControllers();
            builder.Services.AddAuthorizationCore(); // Needed for Blazor auth
            builder.Services.AddHttpContextAccessor();

            // Dependency Injections for Database Access
            var dbPath = Path.Combine(AppContext.BaseDirectory, "DatabaseAccess", "Database.db");
            builder.Services.AddSingleton<IConnectionFactory>(sp => new SQLiteConnectionFactory($"Data Source={dbPath}"));
            builder.Services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddSingleton<IRoleRepository, RoleRepository>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<ITaskStatusRepository, TaskStatusRepository>();
            builder.Services.AddSingleton<ITaskRepository, TaskRepository>();
            builder.Services.AddSingleton<IFileRepository, FileRepository>();
            builder.Services.AddSingleton<ITaskFilesRepository, TaskFilesRepository>();



            //Depenedency Injection for HttpClient (Maybe made it a named client later but dont really need it)
            builder.Services.AddScoped(sp =>
            {
                var navigation = sp.GetRequiredService<NavigationManager>();
                return new HttpClient { BaseAddress = new Uri(navigation.BaseUri) };
            });

            // Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/loginmanager";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = true;
                });

            var app = builder.Build();

            // Middleware order is critical
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();
            app.MapControllers();
            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
