namespace ToDoList.Models
{
    /// <summary>
    /// Data Transfer Object for Department information.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// The unique identifier for the role. auto-incremented primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the role. Must be unique and not null.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
