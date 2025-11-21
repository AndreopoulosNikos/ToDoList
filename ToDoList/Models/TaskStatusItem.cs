namespace ToDoList.Models
{
    public class TaskStatusItem
    {
        /// <summary>
        /// The unique identifier for the task status. auto-incremented primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the task status. Must be unique and not null.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
