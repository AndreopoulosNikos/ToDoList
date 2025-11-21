namespace ToDoList.Models
{
    public class TaskFile
    {
        /// <summary>
        /// The unique identifier for the role. auto-incremented primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The id of the task. References Tasks.Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// The id of the file. References Files.Id
        /// </summary>
        public int FileId { get; set; }
    }
}
