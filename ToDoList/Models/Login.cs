using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    /// <summary>
    /// Represents the data required to authenticate a user through the login form.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// The username entered by the user.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password entered by the user.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Flag indicating whether the user must change their password upon next login.
        /// </summary>
        public bool MustChangePassord { get; set; }
    }
}
