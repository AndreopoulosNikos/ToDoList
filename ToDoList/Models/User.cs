namespace ToDoList.Models
{
    /// <summary>
    /// Data Transfer Object for User information.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique identifier for the user. auto-incremented primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The username. Must be unique and not null.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The first name of the user. This field is optional.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// The last name of the user. This field is optional.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// The department ID the user belongs to. Nullable foreign key. Refers to Department.Id.
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// The name of the Department the user belongs too.
        /// </summary>
        /// <remarks>
        /// Retrieved via join with Department table based on DepartmentId.
        /// </remarks>
        public string? DepartmentName { get; set; }

        /// <summary>
        /// The role ID assigned to the user. Nullable foreign key. Refers to Role.Id.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// The name of the Role of the User.
        /// </summary>
        /// <remarks>
        /// Retrieved via join with Role table based on RoleId.
        /// </remarks>
        public string? RoleName { get; set; }

        /// <summary>
        /// The hashed password for the user. Required field.
        /// </summary>
        public string HashedPassword { get; set; } = string.Empty;

        /// <summary>
        /// A value indicating whether the user must change their password on next login.
        /// </summary>
        public bool MustChangePassword { get; set; }
    }
}
