using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Components.Pages.Users;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    /// <summary>
    /// Provides API endpoints for handling user authentication, including login and logout.
    /// </summary>
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConnectionFactory _connectionFactory = null!;
        private readonly IUserRepository _userRepository = null!;
        private IRoleRepository _roleRepository = null!;


        public AuthenticationController(IConnectionFactory connectionFactory, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _connectionFactory = connectionFactory;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }


        /// <summary>
        /// Authenticates a user using their username and password.
        /// </summary>
        /// <param name="login">
        /// The login request object containing the username and password submitted by the client.
        /// </param>
        /// <returns>
        /// <see cref="OkResult"/> if authentication is successful, along with a cookie-based sign-in.
        /// <see cref="UnauthorizedObjectResult"/> if the username does not exist or the password is incorrect.
        /// </returns>
        /// <remarks>
        /// This method verifies the provided credentials against stored user data, 
        /// generates authentication claims (including role and password-change requirement),
        /// and issues a persistent authentication cookie when login is successful.
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {

            // Hardcoded admin credentials
            const string hardcodedAdminUsername = "Marina";
            const string hardcodedAdminPassword = "IzAFknAwezomePieceOfSh!t123"; 

            if (login.Username == hardcodedAdminUsername && login.Password == hardcodedAdminPassword)
            {
                var hardClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, hardcodedAdminUsername),
                    new Claim("MustChangePassword", "false"),
                    new Claim("ScreenName", "Super Admin"),
                    new Claim(ClaimTypes.Role, "Admin") 
                };

                var hardClaimsIdentity = new ClaimsIdentity(hardClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(hardClaimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    });

                return Ok();
            }


            using var connection = _connectionFactory.CreateConnection();
            var user = await _userRepository.GetUserByUsername(connection,login.Username);
            if (user == null) 
            {
                return Unauthorized("Invalid username or password");
            }

            var hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.HashedPassword, login.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid username or password");
            }

            string screenName = user.Username;
            if (!String.IsNullOrWhiteSpace(user.FirstName) && !String.IsNullOrWhiteSpace(user.LastName))
            {
                screenName = $"{user.FirstName} {user.LastName}";
            }

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.Name, user.Username),
                 new Claim("MustChangePassword", user.MustChangePassword ? "true" : "false"),
                 new Claim("ScreenName",screenName)
            };

            if (user.RoleId is not null && user.RoleId != 0) 
            {
                var role = await _roleRepository.GetRoleById(connection, Convert.ToInt32(user.RoleId));
                if (role != null) 
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

             


            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                });

            return Ok();
        }


        /// <summary>
        /// Signs the current user out by clearing their authentication cookie.
        /// </summary>
        /// <returns>
        /// Returns <see cref="OkResult"/> when the logout operation completes successfully.
        /// </returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}
