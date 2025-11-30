namespace UniversityManagement.Shared.DTOs.Analytics;

/// <summary>
/// Professor dashboard overview with workload and performance metrics
/// </summary>
public class ProfessorDashboardDto
{
    public Guid ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Workload KPIs
    /// </summary>
    public int TotalCoursesTeaching { get; set; }
    public int TotalStudents { get; set; }
    public int PendingGrades { get; set; }
    public decimal AverageClassPerformance { get; set; }
    
    /// <summary>
    /// Course Performance List
    /// </summary>
    public List<CoursePerformanceDto> CoursePerformances { get; set; } = new();
    
    /// <summary>
    /// Pending Tasks
    /// </summary>
    public PendingTasksDto PendingTasks { get; set; } = new();
    
    /// <summary>
    /// Grade Distribution Summary
    /// </summary>
    public Dictionary<int, int> OverallGradeDistribution { get; set; } = new();
}

/// <summary>
/// Course performance metrics for professor
/// </summary>
public class CoursePerformanceDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int SemesterNumber { get; set; }
    public int TotalStudents { get; set; }
    public decimal AverageGrade { get; set; }
    public decimal PassRate { get; set; }
    public decimal AttendanceRate { get; set; }
    public int PendingGrades { get; set; }
    public int GradeContests { get; set; }
}

/// <summary>
/// Pending tasks summary
/// </summary>
public class PendingTasksDto
{
    public int GradesToSubmit { get; set; }
    public int ContestsToReview { get; set; }
    public int AttendanceToMark { get; set; }
    public List<PendingTaskItemDto> Items { get; set; } = new();
}

/// <summary>
/// Individual pending task
/// </summary>
public class PendingTaskItemDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Priority { get; set; } = "normal";
}
