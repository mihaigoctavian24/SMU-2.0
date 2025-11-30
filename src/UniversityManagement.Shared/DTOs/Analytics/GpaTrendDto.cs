namespace UniversityManagement.Shared.DTOs.Analytics;

/// <summary>
/// GPA trend data point for time series chart
/// </summary>
public class GpaTrendDto
{
    public string SemesterName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public decimal Gpa { get; set; }
    public int SemesterOrder { get; set; }
    public Guid SemesterId { get; set; }
    public int TotalCredits { get; set; }
    public int CoursesPassed { get; set; }
    public int CoursesFailed { get; set; }
}

/// <summary>
/// Grade distribution with course details
/// </summary>
public class GradeDistributionDto
{
    public int GradeValue { get; set; }
    public int GradeCount { get; set; }
    public decimal Percentage { get; set; }
    public List<string> Courses { get; set; } = new();
}

/// <summary>
/// Attendance summary with breakdown by course type
/// </summary>
public class AttendanceSummaryDto
{
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int ExcusedCount { get; set; }
    public int LateCount { get; set; }
    public decimal AttendanceRate { get; set; }
    
    /// <summary>
    /// Breakdown by course type (course, seminar, lab)
    /// </summary>
    public Dictionary<string, CourseTypeAttendanceDto> ByCourseType { get; set; } = new();
}

/// <summary>
/// Attendance metrics for a specific course type
/// </summary>
public class CourseTypeAttendanceDto
{
    public int Total { get; set; }
    public int Present { get; set; }
    public decimal Rate { get; set; }
}
