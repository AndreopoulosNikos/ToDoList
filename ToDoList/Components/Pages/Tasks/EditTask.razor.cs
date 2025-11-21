using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages.Tasks
{
    /// <summary>
    /// A Blazor component for editing an existing task, including updating task details,
    /// managing existing file attachments, and handling new PDF uploads.
    /// </summary>
    public partial class EditTaskBase : ComponentBase
    {
        /// <summary>
        /// Gets or sets the ID of the task to edit.
        /// </summary>
        [Parameter]
        public int TaskId { get; set; }

        /// <summary>
        /// Database connection factory.
        /// </summary>
        [Inject]
        protected IConnectionFactory ConnectionFactory { get; set; } = default!;

        /// <summary>
        /// Repository for task CRUD operations.
        /// </summary>
        [Inject]
        protected ITaskRepository TaskRepository { get; set; } = default!;

        /// <summary>
        /// Repository for managing files.
        /// </summary>
        [Inject]
        protected IFileRepository FileRepository { get; set; } = default!;

        /// <summary>
        /// Repository for managing task-file relationships.
        /// </summary>
        [Inject]
        protected ITaskFilesRepository TaskFilesRepository { get; set; } = default!;

        /// <summary>
        /// Repository for retrieving departments.
        /// </summary>
        [Inject]
        protected IDepartmentRepository DepartmentRepository { get; set; } = default!;

        /// <summary>
        /// Repository for retrieving task statuses.
        /// </summary>
        [Inject]
        protected ITaskStatusRepository TaskStatusRepository { get; set; } = default!;

        /// <summary>
        /// Navigation manager for page navigation.
        /// </summary>
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// HTTP client for API requests.
        /// </summary>
        [Inject]
        protected HttpClient Http { get; set; } = default!;

        /// <summary>
        /// Authentication state provider to check user roles.
        /// </summary>
        [Inject]
        protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        /// <summary>
        /// Repository for retrieving user information.
        /// </summary>
        [Inject]
        protected IUserRepository UserRepository { get; set; } = default!;

        /// <summary>
        /// Model representing the task being edited.
        /// </summary>
        protected AddTask.TaskToAdd editTask = new();

        /// <summary>
        /// List of available departments for selection.
        /// </summary>
        protected List<Department> Departments = new();

        /// <summary>
        /// List of available task statuses for selection.
        /// </summary>
        protected List<TaskStatusItem> Statuses = new();

        /// <summary>
        /// List of existing files attached to the task.
        /// </summary>
        protected List<FileItem> ExistingFiles = new();

        /// <summary>
        /// List of newly uploaded temporary files to attach.
        /// </summary>
        protected List<AddTask.UploadedTempFile> uploadedFiles = new();

        /// <summary>
        /// Currently selected file in the InputFile component.
        /// </summary>
        protected IBrowserFile? selectedFile;

        /// <summary>
        /// List of files removed by the user.
        /// </summary>
        protected List<FileItem> removedFiles = new();

        /// <summary>
        /// Returns true if a file has been selected.
        /// </summary>
        protected bool selectedFileHasValue => selectedFile != null;

        /// <summary>
        /// Error message to display in the UI.
        /// </summary>
        protected string? ErrorMessage { get; set; }

        /// <summary>
        /// Indicates if the current user has admin privileges.
        /// </summary>
        protected bool IsAdmin { get; set; } = false;


        /// <summary>
        /// Indicates if the current user is in the department that the task is assigned to.
        /// </summary
        protected bool IsInSameDepartment { get; set; } = false;

        /// <summary>
        /// Loads task data, departments, statuses, and existing files.
        /// Checks the user's role to pre-fill department for non-admins.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            using var connection = ConnectionFactory.CreateConnection();

            // Load task
            var task = await TaskRepository.GetTaskById(connection, TaskId);
            editTask.Subject = task!.Subject;
            editTask.Action = task.Action;
            editTask.DueDate = task.DueDate;
            editTask.Notes = task.Notes;
            editTask.TaskStatusId = task.TaskStatusId;
            editTask.DepartmentId = task.DepartmentId;
            editTask.CompletedDate = task.CompletedDate;

            // Load departments and statuses
            Departments = await DepartmentRepository.GetAllDepartments(connection);
            Statuses = await TaskStatusRepository.GetAllTaskStatuses(connection);

            // Load existing files
            var taskFiles = await TaskFilesRepository.GetAllTaskFilesByTaskId(connection, TaskId);
            var taskFilesId = taskFiles.Select(tf => tf.FileId).ToList();
            ExistingFiles = await FileRepository.GetFilesByIds(connection, taskFilesId);

            // Check admin
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            IsAdmin = authState.User.IsInRole("Admin");

            var username = authState.User.Identity?.Name;
            var user = await UserRepository.GetUserByUsername(connection, username!);

            // Non-admin default department
            if (!IsAdmin && editTask.DepartmentId == 0)
            {    
                editTask.DepartmentId = user?.DepartmentId ?? 0;
            }

            if (editTask.DepartmentId !=0 && editTask.DepartmentId == user?.DepartmentId) 
            {
                IsInSameDepartment = true;
            }
        }

        /// <summary>
        /// Removes an existing file from the task and tracks it for deletion.
        /// </summary>
        /// <param name="file">The file to remove.</param>
        protected void RemoveExistingFile(FileItem file)
        {
            ExistingFiles.Remove(file);
            removedFiles.Add(file);
        }

        /// <summary>
        /// Handles selection of a PDF file from the InputFile component.
        /// </summary>
        /// <param name="e">The file change event arguments.</param>
        protected void OnSingleFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file.ContentType != "application/pdf")
            {
                ErrorMessage = $"File '{file.Name}' is not a PDF.";
                selectedFile = null;
                return;
            }
            selectedFile = file;
        }

        /// <summary>
        /// Uploads the selected PDF file to a temporary folder on the server.
        /// </summary>
        protected async Task UploadSelectedFile()
        {
            if (selectedFile == null) return;

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(selectedFile.OpenReadStream(maxAllowedSize: 50_000_000));
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);
            content.Add(streamContent, "file", selectedFile.Name);

            var response = await Http.PostAsync("api/file/upload-temp", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AddTask.UploadedTempFile>();
                if (result != null)
                {
                    uploadedFiles.Add(result);
                    selectedFile = null;
                }
            }
            else
            {
                ErrorMessage = $"Failed to upload '{selectedFile.Name}': {response.ReasonPhrase}";
            }
        }

        /// <summary>
        /// Removes a temporary uploaded file and deletes it from disk.
        /// </summary>
        /// <param name="file">The temporary file to remove.</param>
        protected void RemoveTempFile(AddTask.UploadedTempFile file)
        {
            uploadedFiles.Remove(file);
            if (File.Exists(file.TempFilePath))
            {
                try { File.Delete(file.TempFilePath); } catch { }
            }
        }

        /// <summary>
        /// Cancels editing and navigates back to the main page.
        /// </summary>
        protected void Cancel() => NavigationManager.NavigateTo("/");

        /// <summary>
        /// Handles form submission, updates the task, deletes removed files,
        /// and moves newly uploaded files to permanent storage.
        /// </summary>
        protected async Task HandleValidSubmit()
        {
            using var connection = ConnectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                TaskItem taskItemToUpdate = new TaskItem
                {
                    Id = TaskId,
                    Subject = editTask.Subject,
                    Action = editTask.Action,
                    DueDate = editTask.DueDate,
                    Notes = editTask.Notes,
                    TaskStatusId = editTask.TaskStatusId!.Value,
                    DepartmentId = editTask.DepartmentId!.Value,
                    CompletedDate = editTask.CompletedDate
                };

                // Update task
                await TaskRepository.UpdateTask(connection, taskItemToUpdate, transaction);

                // Delete files user removed
                foreach (var removedFile in removedFiles)
                {
                    //I should make a freaking method in the repository for this fucking thing but i am sooo fking lazy rn
                    var taskFilesOfTask = await TaskFilesRepository.GetAllTaskFilesByTaskId(connection, TaskId, transaction);
                    var tasKFileToDelete = taskFilesOfTask.FirstOrDefault(tf => tf.FileId == removedFile.Id)?.Id;
                    if (tasKFileToDelete == null || tasKFileToDelete == 0)
                    { 
                        continue;
                    }

                    await TaskFilesRepository.DeleteTaskFile(connection, Convert.ToInt32(tasKFileToDelete), transaction);
                    await FileRepository.DeleteFile(connection, Convert.ToInt32(tasKFileToDelete), transaction);

                    if (File.Exists(removedFile.FilePath))
                    {
                        try { File.Delete(removedFile.FilePath); } catch { }
                    }
                }

                // Save new uploaded files
                string taskFolder = Path.Combine("C:\\ToDoListApp", TaskId.ToString());
                if (!Directory.Exists(taskFolder)) Directory.CreateDirectory(taskFolder);

                foreach (var tempFile in uploadedFiles)
                {
                    string permanentPath = Path.Combine(taskFolder, tempFile.FileName);
                    File.Move(tempFile.TempFilePath, permanentPath, true);

                    var fileItem = new FileItem { Filename = tempFile.FileName, FilePath = permanentPath };
                    int fileId = await FileRepository.AddFile(connection, fileItem, transaction);

                    await TaskFilesRepository.AddTaskFile(connection, new TaskFile
                    {
                        TaskId = TaskId,
                        FileId = fileId
                    }, transaction);
                }

                transaction.Commit();
                NavigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorMessage = $"Failed to save task: {ex.Message}";
            }
        }
    }
}
