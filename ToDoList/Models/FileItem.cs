namespace ToDoList.Models
{
    public class FileItem
    {
        /// <summary>
        /// The unique identifier for the file. auto-incremented primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the file. Not null.
        /// </summary>
        public string Filename { get; set; } = string.Empty;


        /// <summary>
        /// The full directory where the file is located (including the filename). Not null.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
    }
}
