namespace ToDoList.Models
{
    /// <summary>
    /// Data Transfer Object representing a Task.
    /// </summary>
    public class TaskItem
    {

        /// <summary>
        /// Unique identifier for the task.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Brief description or title of the task.
        /// </summary>
        public string Subject { get; set; } = null!;

        /// <summary>
        /// Action required for the task.
        /// </summary>
        public string Action { get; set; } = null!;

        /// <summary>
        /// Estimated date that the task will be completed.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Date the task was completed.
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Optional additional information about the task.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Identifier of the department responsible for the task.
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// The name of the department responsible for the task.
        /// </summary>
        public string DepartmentName { get; set; } = null!;

        /// <summary>
        /// Identifier of the current status of the task.
        /// </summary>
        public int TaskStatusId { get; set; }

        /// <summary>
        /// The name of the current status of the task.
        /// </summary>
        public string TaskStatusName { get; set; } = null!;
    }
}
