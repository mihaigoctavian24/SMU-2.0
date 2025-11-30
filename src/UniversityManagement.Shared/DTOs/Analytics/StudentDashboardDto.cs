namespace UniversityManagement.Shared.DTOs.Analytics;

/// <summary>
/// Student dashboard overview with KPIs and metrics
/// </summary>
public class StudentDashboardDto
{
    /// <summary>
    /// Student basic information
    /// </summary>
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public int YearOfStudy { get; set; }
    
    /// <summary>
    /// KPI Metrics
    /// </summary>
    public decimal CurrentGpa { get; set; }
    public decimal? PreviousGpa { get; set; }
    public decimal GpaTrend { get; set; } // Percentage change
    
    public decimal AttendanceRate { get; set; }
    public decimal? PreviousAttendanceRate { get; set; }
    
    public int CreditsEarned { get; set; }
    public int TotalCreditsAttempted { get; set; }
    public int TotalCreditsRequired { get; set; }
    public decimal CreditProgress { get; set; } // Percentage
    
    public int? ClassRank { get; set; }
    public int? TotalStudentsInClass { get; set; }
    public decimal? ClassPercentile { get; set; }
    
    /// <summary>
    /// Risk Assessment
    /// </summary>
    public string RiskLevel { get; set; } = "on_track"; // on_track, medium_risk, high_risk
    
    /// <summary>
    /// Counts
    /// </summary>
    public int EnrolledCoursesCount { get; set; }
    public int PassedCoursesCount { get; set; }
    public int FailedCoursesCount { get; set; }
    
    /// <summary>
    /// Attendance Breakdown
    /// </summary>
    public AttendanceBreakdownDto AttendanceBreakdown { get; set; } = new();
    
    /// <summary>
    /// Grade Distribution Summary
    /// </summary>
    public GradeDistributionSummaryDto GradeDistribution { get; set; } = new();
    
    /// <summary>
    /// Recent Activity
    /// </summary>
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    
    /// <summary>
    /// Upcoming Deadlines
    /// </summary>
    public List<UpcomingDeadlineDto> UpcomingDeadlines { get; set; } = new();
}

/// <summary>
/// Attendance breakdown by status
/// </summary>
public class AttendanceBreakdownDto
{
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int ExcusedCount { get; set; }
    public int LateCount { get; set; }
    public int TotalSessions { get; set; }
    
    public decimal PresentPercentage { get; set; }
    public decimal AbsentPercentage { get; set; }
    public decimal ExcusedPercentage { get; set; }
    public decimal LatePercentage { get; set; }
}

/// <summary>
/// Grade distribution summary
/// </summary>
public class GradeDistributionSummaryDto
{
    public int Grade10Count { get; set; }
    public int Grade9Count { get; set; }
    public int Grade8Count { get; set; }
    public int Grade7Count { get; set; }
    public int Grade6Count { get; set; }
    public int Grade5Count { get; set; }
    public int GradeFailCount { get; set; }
}

/// <summary>
/// Recent activity item
/// </summary>
public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // "grade", "attendance", "request"
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

/// <summary>
/// Upcoming deadline item
/// </summary>
public class UpcomingDeadlineDto
{
    public string Type { get; set; } = string.Empty; // "exam", "assignment", "request"
    public string Title { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int DaysRemaining { get; set; }
    public string Priority { get; set; } = "normal"; // "low", "normal", "high"
}
