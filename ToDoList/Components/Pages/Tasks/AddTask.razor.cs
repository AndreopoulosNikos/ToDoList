using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Tasks
{
    /// <summary>
    /// A Blazor component for adding a new task with optional PDF attachments.
    /// </summary>
    public partial class AddTask
    {
        /// <summary>
        /// Model representing the task to be added.
        /// </summary>
        private TaskToAdd newTask = new();

        /// <summary>
        /// Stores any error message to display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// List of available departments for the department dropdown.
        /// </summary>
        private List<Department> Departments = new();

        /// <summary>
        /// List of available task statuses for the status dropdown.
        /// </summary>
        private List<TaskStatusItem> Statuses = new();

        /// <summary>
        /// List of files uploaded to the temporary folder.
        /// </summary>
        private List<UploadedTempFile> uploadedFiles = new();

        /// <summary>
        /// The currently selected file in the InputFile component.
        /// </summary>
        private IBrowserFile? selectedFile;

        /// <summary>
        /// Returns true if a file has been selected.
        /// </summary>
        private bool selectedFileHasValue => selectedFile != null;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = default!;
        [Inject]
        private ITaskRepository TaskRepository { get; set; } = default!;
        [Inject]
        private IDepartmentRepository DepartmentRepository { get; set; } = default!;
        [Inject]
        private ITaskStatusRepository TaskStatusRepository { get; set; } = default!;
        [Inject]
        private IFileRepository FileRepository { get; set; } = default!;
        [Inject]
        private ITaskFilesRepository TaskFilesRepository { get; set; } = default!;
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject]
        private IUserRepository UserRepository { get; set; } = default!;
        [Inject]
        private HttpClient Http { get; set; } = default!;
      

        /// <summary>
        /// Initializes the component by loading departments and task statuses.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                Departments = await DepartmentRepository.GetAllDepartments(connection);
                Statuses = await TaskStatusRepository.GetAllTaskStatuses(connection);
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                // Only assign department automatically if the user is NOT in Admin role
                if (!user.IsInRole("Admin"))
                {
                    var currentUserName = user.Identity?.Name;
                    var currentUser = await UserRepository.GetUserByUsername(connection, currentUserName!);
                    newTask.DepartmentId = currentUser?.DepartmentId;
                }
             


            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load data: {ex.Message}";
            }
        }

        /// <summary>
        /// Handles selection of a single PDF file from the InputFile component.
        /// </summary>
        /// <param name="e">Event arguments containing the selected file.</param>
        private void OnSingleFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file.ContentType != "application/pdf")
            {
                ErrorMessage = $"File '{file.Name}' is not a PDF and was ignored.";
                selectedFile = null;
                return;
            }
            selectedFile = file;
        }

        /// <summary>
        /// Uploads the currently selected file to the temporary server folder.
        /// </summary>
        private async Task UploadSelectedFile()
        {
            if (selectedFile == null) return;

            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(selectedFile.OpenReadStream(maxAllowedSize: 50_000_000)); // 50 MB
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);
                content.Add(streamContent, "file", selectedFile.Name);

                var response = await Http.PostAsync("api/file/upload-temp", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UploadedTempFile>();
                    if (result != null)
                    {
                        uploadedFiles.Add(result);
                        selectedFile = null;
                        ErrorMessage = null;
                    }
                }
                else
                {
                    ErrorMessage = $"Failed to upload '{selectedFile.Name}': {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading file '{selectedFile?.Name}': {ex.Message}";
            }
        }

        /// <summary>
        /// Removes a temporary uploaded file from the list and deletes it from disk.
        /// </summary>
        /// <param name="file">The temporary file to remove.</param>
        private void RemoveTempFile(UploadedTempFile file)
        {
            uploadedFiles.Remove(file);

            // Optionally delete the temp file immediately
            if (System.IO.File.Exists(file.TempFilePath))
            {
                try { System.IO.File.Delete(file.TempFilePath); } catch { }
            }
        }

        /// <summary>
        /// Cancels adding a task and navigates back to the task list page.
        /// </summary>
        private void Cancel() => NavigationManager.NavigateTo("/");

        /// <summary>
        /// Handles submission of the task form, saves the task and uploaded files permanently, and commits changes to the database.
        /// </summary>
        private async Task HandleValidSubmit()
        {
            using var connection = ConnectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Create the task
                TaskItem taskItemToAdd = new TaskItem
                {
                    Subject = newTask.Subject,
                    Action = newTask.Action,
                    DueDate = newTask.DueDate,
                    Notes = newTask.Notes,
                    DepartmentId = newTask.DepartmentId!.Value,
                    TaskStatusId = newTask.TaskStatusId!.Value
                };

                int taskId = await TaskRepository.AddTask(connection, taskItemToAdd, transaction);

                // Save uploaded files permanently
                string taskFolder = Path.Combine("C:\\ToDoListApp", taskId.ToString());
                if (!Directory.Exists(taskFolder)) Directory.CreateDirectory(taskFolder);

                foreach (var tempFile in uploadedFiles)
                {
                    string permanentPath = Path.Combine(taskFolder, tempFile.FileName);
                    System.IO.File.Move(tempFile.TempFilePath, permanentPath, true);

                    // Save in DB
                    var fileItem = new FileItem { Filename = tempFile.FileName, FilePath = permanentPath };
                    int fileId = await FileRepository.AddFile(connection, fileItem, transaction);

                    await TaskFilesRepository.AddTaskFile(connection, new TaskFile
                    {
                        TaskId = taskId,
                        FileId = fileId
                    }, transaction);
                }

                transaction.Commit();
                NavigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorMessage = $"Failed to add task: {ex.Message}";
            }
        }

        #region Models

        /// <summary>
        /// Represents a task being added.
        /// </summary>
        public class TaskToAdd
        {
            [Required(ErrorMessage = "Subject is required")]
            public string Subject { get; set; } = string.Empty;

            [Required(ErrorMessage = "Action is required")]
            public string Action { get; set; } = string.Empty;

            [Required(ErrorMessage = "Department is required")]
            public int? DepartmentId { get; set; }

            [Required(ErrorMessage = "Task status is required")]
            public int? TaskStatusId { get; set; }

            [Required(ErrorMessage = "Due date is required")]
            public DateTime DueDate { get; set; } = DateTime.Now;
      
            public DateTime? CompletedDate { get; set; }

            /// <summary>
            /// Optional notes for the task.
            /// </summary>
            public string? Notes { get; set; }
        }

        /// <summary>
        /// Represents a temporary uploaded file returned by the temp upload API.
        /// </summary>
        public class UploadedTempFile
        {
            /// <summary>
            /// The original file name.
            /// </summary>
            public string FileName { get; set; } = string.Empty;

            /// <summary>
            /// The temporary file path on the server.
            /// </summary>
            public string TempFilePath { get; set; } = string.Empty;

            /// <summary>
            /// The MIME content type of the file.
            /// </summary>
            public string ContentType { get; set; } = string.Empty;

            /// <summary>
            /// The size of the file in bytes.
            /// </summary>
            public long Size { get; set; }
        }

        #endregion
    }
}
