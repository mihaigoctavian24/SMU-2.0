namespace UniversityManagement.Shared.DTOs.Analytics;

/// <summary>
/// Dean dashboard overview with faculty-wide metrics
/// </summary>
public class DeanDashboardDto
{
    public Guid FacultyId { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    public Guid DeanId { get; set; }
    public string DeanName { get; set; } = string.Empty;
    
    /// <summary>
    /// Faculty KPIs
    /// </summary>
    public int TotalStudents { get; set; }
    public int TotalStudentsTrend { get; set; } // Change from previous period
    public decimal OverallPassRate { get; set; }
    public decimal PassRateTrend { get; set; }
    public decimal AverageGpa { get; set; }
    public decimal GpaTrend { get; set; }
    public decimal GraduationRate { get; set; }
    public decimal GraduationRateTrend { get; set; }
    
    /// <summary>
    /// Program Comparison Data
    /// </summary>
    public List<ProgramMetricsDto> ProgramMetrics { get; set; } = new();
    
    /// <summary>
    /// At-Risk Students
    /// </summary>
    public List<AtRiskStudentDto> AtRiskStudents { get; set; } = new();
    
    /// <summary>
    /// Grade Approval Queue
    /// </summary>
    public GradeApprovalQueueDto GradeApprovalQueue { get; set; } = new();
    
    /// <summary>
    /// Enrollment Trends (last 5 years)
    /// </summary>
    public List<EnrollmentTrendDto> EnrollmentTrends { get; set; } = new();
}

/// <summary>
/// Program-level metrics for comparison
/// </summary>
public class ProgramMetricsDto
{
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public decimal AverageGpa { get; set; }
    public decimal PassRate { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal AttendanceRate { get; set; }
    public decimal GraduationRate { get; set; }
}

/// <summary>
/// At-risk student information
/// </summary>
public class AtRiskStudentDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public int YearOfStudy { get; set; }
    public decimal CurrentGpa { get; set; }
    public decimal AttendanceRate { get; set; }
    public int FailedCourses { get; set; }
    public string RiskLevel { get; set; } = "medium"; // high, medium, low
    public DateTime? LastActivityDate { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}

/// <summary>
/// Grade approval queue metrics
/// </summary>
public class GradeApprovalQueueDto
{
    public int PendingCount { get; set; }
    public int InReviewCount { get; set; }
    public int ApprovedCount { get; set; }
    public int ContestedCount { get; set; }
    public List<GradeApprovalItemDto> Items { get; set; } = new();
}

/// <summary>
/// Individual grade approval item
/// </summary>
public class GradeApprovalItemDto
{
    public Guid GradeId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string ProfessorName { get; set; } = string.Empty;
    public int GradeValue { get; set; }
    public string GradeType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public int DaysPending { get; set; }
    public bool HasContest { get; set; }
}

/// <summary>
/// Enrollment trend data point
/// </summary>
public class EnrollmentTrendDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime YearStartDate { get; set; }
    public int TotalEnrollments { get; set; }
    public int ActiveStudents { get; set; }
    public int GraduatedStudents { get; set; }
    public decimal AverageGpa { get; set; }
    public decimal PassRate { get; set; }
}
