using Microsoft.AspNetCore.Components;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;
using ToDoList.Models;

namespace ToDoList.Components.IndividualComponents
{
    /// <summary>
    /// A form component for filtering and searching tasks.
    /// </summary>
    public partial class SearchForm : ComponentBase
    {
        /// <summary>
        /// The full list of tasks to filter.
        /// </summary>
        [Parameter]
        public List<TaskItem> Tasks { get; set; } = new();

        /// <summary>
        /// List of departments used in the department filter dropdown.
        /// </summary>
        [Parameter]
        public List<Department> Departments { get; set; } = new();

        /// <summary>
        /// List of task statuses used in the status filter dropdown.
        /// </summary>
        [Parameter]
        public List<TaskStatusItem> TaskStatuses { get; set; } = new();

        /// <summary>
        /// Event callback invoked when the filtered list of tasks changes.
        /// </summary>
        [Parameter]
        public EventCallback<List<TaskItem>> OnFiltered { get; set; }

        /// <summary>
        /// Factory used to create database connections.
        /// </summary>
        [Inject]
        private IConnectionFactory ConnectionFactory { get; set; } = null!;

        /// <summary>
        /// Repository for performing CRUD operations on departments.
        /// </summary>
        [Inject]
        private IDepartmentRepository DepartmentRepository { get; set; } = null!;

        /// <summary>
        /// Repository for performing CRUD operations on task statuses.
        /// </summary>
        [Inject]
        private ITaskStatusRepository TaskStatusRepository { get; set; } = null!;

        // Search fields
        private string SubjectSearch { get; set; } = "";
        private int? DepartmentId { get; set; }
        private int? TaskStatusId { get; set; }
        private DateTime? DueDateFrom { get; set; }
        private DateTime? DueDateTo { get; set; }
        private DateTime? CompletedDateFrom { get; set; }
        private DateTime? CompletedDateTo { get; set; }
        private bool ShowDateFilters { get; set; } = false;

        /// <summary>
        /// Loads the departments and task statuses when component parameters are set.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                using var connection = ConnectionFactory.CreateConnection();
                Departments = await DepartmentRepository.GetAllDepartments(connection);
                TaskStatuses = await TaskStatusRepository.GetAllTaskStatuses(connection);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while loading filter data. {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Toggles the visibility of date range filters.
        /// </summary>
        private void ToggleDateFilters()
        {
            ShowDateFilters = !ShowDateFilters;

            if (!ShowDateFilters)
            {
                DueDateFrom = null;
                DueDateTo = null;
                CompletedDateFrom = null;
                CompletedDateTo = null;
            }
        }

        /// <summary>
        /// Applies the current filter criteria to the task list and invokes <see cref="OnFiltered"/>.
        /// </summary>
        private async Task ApplyFilter()
        {
            var filtered = Tasks.AsEnumerable();

            // Subject text search
            if (!string.IsNullOrWhiteSpace(SubjectSearch))
            {
                filtered = filtered.Where(t =>
                    t.Subject.Contains(SubjectSearch, StringComparison.OrdinalIgnoreCase));
            }

            // Department filter
            if (DepartmentId.HasValue)
            {
                filtered = filtered.Where(t => t.DepartmentId == DepartmentId.Value);
            }

            // Status filter
            if (TaskStatusId.HasValue)
            {
                filtered = filtered.Where(t => t.TaskStatusId == TaskStatusId.Value);
            }

            // Date filters
            if (ShowDateFilters)
            {
                if (DueDateFrom.HasValue)
                    filtered = filtered.Where(t => t.DueDate.Date >= DueDateFrom.Value.Date);

                if (DueDateTo.HasValue)
                    filtered = filtered.Where(t => t.DueDate.Date <= DueDateTo.Value.Date);

                if (CompletedDateFrom.HasValue)
                    filtered = filtered.Where(t => t.CompletedDate.HasValue &&
                                                   t.CompletedDate.Value.Date >= CompletedDateFrom.Value.Date);

                if (CompletedDateTo.HasValue)
                    filtered = filtered.Where(t => t.CompletedDate.HasValue &&
                                                   t.CompletedDate.Value.Date <= CompletedDateTo.Value.Date);
            }

            await OnFiltered.InvokeAsync(filtered.ToList());
        }

        /// <summary>
        /// Resets all filter fields and invokes <see cref="OnFiltered"/> with the full task list.
        /// </summary>
        private async Task Reset()
        {
            SubjectSearch = "";
            DepartmentId = null;
            TaskStatusId = null;
            DueDateFrom = null;
            DueDateTo = null;
            CompletedDateFrom = null;
            CompletedDateTo = null;

            await OnFiltered.InvokeAsync(Tasks);
        }
    }
}
