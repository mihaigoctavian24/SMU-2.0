namespace UniversityManagement.Shared.DTOs.Analytics;

/// <summary>
/// Rector dashboard with university-wide overview
/// </summary>
public class RectorDashboardDto
{
    public Guid RectorId { get; set; }
    public string RectorName { get; set; } = string.Empty;
    
    /// <summary>
    /// University-Wide KPIs
    /// </summary>
    public UniversityKpisDto UniversityKpis { get; set; } = new();
    
    /// <summary>
    /// Faculty Comparison Data
    /// </summary>
    public List<FacultyComparisonDto> FacultyComparisons { get; set; } = new();
    
    /// <summary>
    /// Historical Performance (5 years)
    /// </summary>
    public List<HistoricalPerformanceDto> HistoricalPerformance { get; set; } = new();
    
    /// <summary>
    /// Strategic KPIs
    /// </summary>
    public StrategicKpisDto StrategicKpis { get; set; } = new();
}

/// <summary>
/// University-wide KPI metrics
/// </summary>
public class UniversityKpisDto
{
    public int TotalStudents { get; set; }
    public int TotalStudentsTrend { get; set; }
    
    public int TotalFaculties { get; set; }
    public int TotalPrograms { get; set; }
    public int TotalProfessors { get; set; }
    
    public decimal OverallAverageGpa { get; set; }
    public decimal GpaTrend { get; set; }
    
    public decimal OverallPassRate { get; set; }
    public decimal PassRateTrend { get; set; }
    
    public decimal OverallRetentionRate { get; set; }
    public decimal RetentionRateTrend { get; set; }
    
    public decimal OverallGraduationRate { get; set; }
    public decimal GraduationRateTrend { get; set; }
    
    public decimal OverallAttendanceRate { get; set; }
    
    public int ActiveStudents { get; set; }
    public int GraduatedStudents { get; set; }
    public int PendingGradeApprovals { get; set; }
}

/// <summary>
/// Faculty comparison metrics
/// </summary>
public class FacultyComparisonDto
{
    public Guid FacultyId { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    public string FacultyShortName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int TotalPrograms { get; set; }
    public int TotalProfessors { get; set; }
    public decimal AverageGpa { get; set; }
    public decimal PassRate { get; set; }
    public decimal GraduationRate { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal AttendanceRate { get; set; }
}

/// <summary>
/// Historical performance data point
/// </summary>
public class HistoricalPerformanceDto
{
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime YearStartDate { get; set; }
    public int TotalEnrollments { get; set; }
    public decimal AverageGpa { get; set; }
    public decimal PassRate { get; set; }
    public decimal GraduationRate { get; set; }
    public decimal RetentionRate { get; set; }
}

/// <summary>
/// Strategic KPIs for rector decision-making
/// </summary>
public class StrategicKpisDto
{
    /// <summary>
    /// Enrollment target progress (percentage of target achieved)
    /// </summary>
    public decimal EnrollmentTargetProgress { get; set; }
    public int EnrollmentTarget { get; set; }
    public int CurrentEnrollment { get; set; }
    
    /// <summary>
    /// Quality score (composite metric 0-10)
    /// </summary>
    public decimal QualityScore { get; set; }
    
    /// <summary>
    /// Student satisfaction (if surveys available)
    /// </summary>
    public decimal? StudentSatisfactionScore { get; set; }
    
    /// <summary>
    /// Faculty efficiency metrics
    /// </summary>
    public decimal AverageStudentsPerProfessor { get; set; }
    public decimal AverageClassSize { get; set; }
}
