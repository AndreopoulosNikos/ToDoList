using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Reflection;
using ToDoList.Components.IndividualComponents;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.Pages
{
    /// <summary>
    /// Manages the list of tasks, including filtering and deletion.
    /// </summary>
    public partial class TasksManager
    {
        /// <summary>
        /// The full list of tasks loaded from the database.
        /// </summary>
        private List<TaskItem> TaskList = new List<TaskItem>();

        /// <summary>
        /// Stores any error message to display in the UI.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// The filtered list of tasks based on the search criteria.
        /// </summary>
        private List<TaskItem> FilteredTasks = new List<TaskItem>();

        /// <summary>
        /// Factory used to create database connections.
        /// </summary>
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = null!;


        /// <summary>
        /// JavaScript runtime instance used to invoke JavaScript functions from .NET.
        /// </summary>
        [Inject]
        IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Holds a reference to the imported JavaScript module used by this layout.
        /// </summary>
        private IJSObjectReference? _jsModule;

        /// <summary>
        /// Repository used to perform CRUD operations on tasks.
        /// </summary>
        [Inject]
        private ITaskRepository TaskRepository { get; set; } = null!;

        /// <summary>
        /// Navigation manager used to navigate between pages.
        /// </summary>
        [Inject]
        protected NavigationManager Navigation { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _jsModule = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./Components/Pages/TasksManager.razor.js");
            }
        }

        /// <summary>
        /// Loads or refreshes the list of tasks when component parameters are set.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                TaskList = await TaskRepository.GetAllTasksWithInfo(connection);
                FilteredTasks = TaskList;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load tasks. Please try again later. {ex}";
            }
        }

        /// <summary>
        /// Handles the deletion of a task by removing it from the task list
        /// and reapplying the current filters.
        /// </summary>
        /// <param name="deletedTaskId">The ID of the deleted task.</param>
        private async Task HandleTaskDeleted(int deletedTaskId)
        {
            var taskToRemove = TaskList.FirstOrDefault(t => t.Id == deletedTaskId);
            if (taskToRemove != null)
                TaskList.Remove(taskToRemove);

            await InvokeAsync(() => OnFiltered(TaskList));
        }

        /// <summary>
        /// Updates the filtered list of tasks and refreshes the UI.
        /// This method is called by the SearchForm component.
        /// </summary>
        /// <param name="filtered">The filtered list of tasks.</param>
        private void OnFiltered(List<TaskItem> filtered)
        {
            FilteredTasks = filtered;
            StateHasChanged();
        }


        /// <summary>
        /// Exports the task items to excel
        /// </summary>
        /// <returns></returns>
        private async Task ExportToExcel()
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tasks");

            // HEADER
            worksheet.Cell(1, 1).Value = "#";
            worksheet.Cell(1, 2).Value = "Subject";
            worksheet.Cell(1, 3).Value = "Action";
            worksheet.Cell(1, 4).Value = "Department";
            worksheet.Cell(1, 5).Value = "Status";
            worksheet.Cell(1, 6).Value = "Due Date";
            worksheet.Cell(1, 7).Value = "Completed Date";
            worksheet.Cell(1, 8).Value = "Notes";

            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontName = "Arial";
            headerRange.Style.Font.FontSize = 16;
            headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

            // DATA ROWS
            int row = 2;
            foreach (var task in FilteredTasks)
            {
                worksheet.Cell(row, 1).Value = task.Id;
                worksheet.Cell(row, 2).Value = task.Subject;
                worksheet.Cell(row, 3).Value = task.Action;
                worksheet.Cell(row, 4).Value = task.DepartmentName;
                worksheet.Cell(row, 5).Value = task.TaskStatusName;
                worksheet.Cell(row, 6).Value = task.DueDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 7).Value = task.CompletedDate?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 8).Value = task.Notes ?? "";

                var dataRange = worksheet.Range(row, 1, row, 8);

                dataRange.Style.Font.FontName = "Arial";
                dataRange.Style.Font.FontSize = 12;
                dataRange.Style.Fill.BackgroundColor = XLColor.White;
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Wrap long text (like Notes)
                dataRange.Style.Alignment.WrapText = true;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            // Export to browser
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Tasks_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var bytes = stream.ToArray();
            
            if (_jsModule != null) 
            {
                await _jsModule.InvokeVoidAsync(
                "downloadFileFromBytes",
                fileName,
                Convert.ToBase64String(bytes)
            );
            }
        }
            

    }
}
