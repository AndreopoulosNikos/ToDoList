namespace ToDoList.Models
{
    /// <summary>
    /// Data Transfer Object for Department information.
    /// </summary>
    public class Department
    {
        /// <summary>
        /// The unique identifier for the department. auto-incremented primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the department. Must be unique and not null.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
