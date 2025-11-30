using ClosedXML.Excel;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Analytics;

namespace UniversityManagement.Infrastructure.Services;

/// <summary>
/// Professional Excel export service using ClosedXML
/// </summary>
public class ExcelExportService
{
    public byte[] ExportStudentDashboard(StudentDashboardDto dashboard, List<GpaTrendDto> gpaTrend)
    {
        using var workbook = new XLWorkbook();
        
        // Sheet 1: Summary
        var summarySheet = workbook.Worksheets.Add("Summary");
        CreateStudentSummarySheet(summarySheet, dashboard);
        
        // Sheet 2: GPA Trend
        var gpaTrendSheet = workbook.Worksheets.Add("GPA Trend");
        CreateGpaTrendSheet(gpaTrendSheet, gpaTrend);
        
        // Sheet 3: Grade Distribution
        var gradesSheet = workbook.Worksheets.Add("Grade Distribution");
        CreateGradeDistributionSheet(gradesSheet, dashboard.GradeDistribution);
        
        // Sheet 4: Attendance
        var attendanceSheet = workbook.Worksheets.Add("Attendance");
        CreateAttendanceSheet(attendanceSheet, dashboard.AttendanceBreakdown);
        
        // Save to memory stream
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    
    private void CreateStudentSummarySheet(IXLWorksheet sheet, StudentDashboardDto dashboard)
    {
        // Title
        sheet.Cell("A1").Value = "Student Academic Report";
        sheet.Cell("A1").Style.Font.Bold = true;
        sheet.Cell("A1").Style.Font.FontSize = 16;
        sheet.Range("A1:B1").Merge();
        
        // Student Information
        sheet.Cell("A3").Value = "Student Information";
        sheet.Cell("A3").Style.Font.Bold = true;
        sheet.Cell("A3").Style.Fill.BackgroundColor = XLColor.LightBlue;
        
        sheet.Cell("A4").Value = "Name:";
        sheet.Cell("B4").Value = dashboard.StudentName;
        
        sheet.Cell("A5").Value = "Enrollment Number:";
        sheet.Cell("B5").Value = dashboard.EnrollmentNumber;
        
        sheet.Cell("A6").Value = "Program:";
        sheet.Cell("B6").Value = dashboard.ProgramName;
        
        sheet.Cell("A7").Value = "Faculty:";
        sheet.Cell("B7").Value = dashboard.FacultyName;
        
        sheet.Cell("A8").Value = "Year of Study:";
        sheet.Cell("B8").Value = dashboard.YearOfStudy;
        
        // Academic Performance
        sheet.Cell("A10").Value = "Academic Performance";
        sheet.Cell("A10").Style.Font.Bold = true;
        sheet.Cell("A10").Style.Fill.BackgroundColor = XLColor.LightGreen;
        
        sheet.Cell("A11").Value = "Current GPA:";
        sheet.Cell("B11").Value = dashboard.CurrentGpa;
        sheet.Cell("B11").Style.NumberFormat.Format = "0.00";
        
        sheet.Cell("A12").Value = "Attendance Rate:";
        sheet.Cell("B12").Value = dashboard.AttendanceRate / 100;
        sheet.Cell("B12").Style.NumberFormat.Format = "0.0%";
        
        sheet.Cell("A13").Value = "Credits Earned:";
        sheet.Cell("B13").Value = $"{dashboard.CreditsEarned} / {dashboard.TotalCreditsRequired}";
        
        sheet.Cell("A14").Value = "Risk Level:";
        sheet.Cell("B14").Value = dashboard.RiskLevel;
        
        // Apply risk level color
        var riskCell = sheet.Cell("B14");
        if (dashboard.RiskLevel == "high_risk")
            riskCell.Style.Font.FontColor = XLColor.Red;
        else if (dashboard.RiskLevel == "medium_risk")
            riskCell.Style.Font.FontColor = XLColor.Orange;
        else
            riskCell.Style.Font.FontColor = XLColor.Green;
        
        // Auto-fit columns
        sheet.Columns().AdjustToContents();
    }
    
    private void CreateGpaTrendSheet(IXLWorksheet sheet, List<GpaTrendDto> gpaTrend)
    {
        // Headers
        sheet.Cell("A1").Value = "Semester";
        sheet.Cell("B1").Value = "Academic Year";
        sheet.Cell("C1").Value = "GPA";
        sheet.Cell("D1").Value = "Total Credits";
        sheet.Cell("E1").Value = "Courses Passed";
        sheet.Cell("F1").Value = "Courses Failed";
        
        // Style headers
        var headerRange = sheet.Range("A1:F1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        
        // Data
        int row = 2;
        foreach (var trend in gpaTrend)
        {
            sheet.Cell(row, 1).Value = trend.SemesterName;
            sheet.Cell(row, 2).Value = trend.AcademicYear;
            sheet.Cell(row, 3).Value = trend.Gpa;
            sheet.Cell(row, 3).Style.NumberFormat.Format = "0.00";
            sheet.Cell(row, 4).Value = trend.TotalCredits;
            sheet.Cell(row, 5).Value = trend.CoursesPassed;
            sheet.Cell(row, 6).Value = trend.CoursesFailed;
            
            // Add border
            sheet.Range(row, 1, row, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            row++;
        }
        
        // Create chart (if data exists)
        if (gpaTrend.Any())
        {
            var chart = sheet.Range($"A1:C{row - 1}").CreateChart();
            chart.SetChartType(XLChartType.Line);
            chart.SetTitle("GPA Trend Over Time");
        }
        
        sheet.Columns().AdjustToContents();
    }
    
    private void CreateGradeDistributionSheet(IXLWorksheet sheet, GradeDistributionSummaryDto distribution)
    {
        // Headers
        sheet.Cell("A1").Value = "Grade";
        sheet.Cell("B1").Value = "Count";
        sheet.Cell("C1").Value = "Percentage";
        
        var headerRange = sheet.Range("A1:C1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
        
        // Data
        var grades = new[]
        {
            ("10", distribution.Grade10Count),
            ("9", distribution.Grade9Count),
            ("8", distribution.Grade8Count),
            ("7", distribution.Grade7Count),
            ("6", distribution.Grade6Count),
            ("5", distribution.Grade5Count),
            ("< 5", distribution.GradeFailCount)
        };
        
        int totalGrades = grades.Sum(g => g.Item2);
        int row = 2;
        
        foreach (var (grade, count) in grades)
        {
            sheet.Cell(row, 1).Value = grade;
            sheet.Cell(row, 2).Value = count;
            
            if (totalGrades > 0)
            {
                sheet.Cell(row, 3).Value = (double)count / totalGrades;
                sheet.Cell(row, 3).Style.NumberFormat.Format = "0.0%";
            }
            
            row++;
        }
        
        sheet.Columns().AdjustToContents();
    }
    
    private void CreateAttendanceSheet(IXLWorksheet sheet, AttendanceBreakdownDto attendance)
    {
        // Headers
        sheet.Cell("A1").Value = "Status";
        sheet.Cell("B1").Value = "Count";
        sheet.Cell("C1").Value = "Percentage";
        
        var headerRange = sheet.Range("A1:C1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;
        
        // Data
        var attendanceData = new[]
        {
            ("Present", attendance.PresentCount, attendance.PresentPercentage),
            ("Absent", attendance.AbsentCount, attendance.AbsentPercentage),
            ("Excused", attendance.ExcusedCount, attendance.ExcusedPercentage),
            ("Late", attendance.LateCount, attendance.LatePercentage)
        };
        
        int row = 2;
        foreach (var (status, count, percentage) in attendanceData)
        {
            sheet.Cell(row, 1).Value = status;
            sheet.Cell(row, 2).Value = count;
            sheet.Cell(row, 3).Value = percentage / 100;
            sheet.Cell(row, 3).Style.NumberFormat.Format = "0.0%";
            
            row++;
        }
        
        // Total row
        sheet.Cell(row, 1).Value = "TOTAL";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        sheet.Cell(row, 2).Value = attendance.TotalSessions;
        sheet.Cell(row, 2).Style.Font.Bold = true;
        
        sheet.Columns().AdjustToContents();
    }
    
    public byte[] ExportProfessorDashboard(ProfessorDashboardDto dashboard)
    {
        using var workbook = new XLWorkbook();
        
        // Summary sheet
        var summarySheet = workbook.Worksheets.Add("Summary");
        summarySheet.Cell("A1").Value = "Professor Performance Report";
        summarySheet.Cell("A1").Style.Font.Bold = true;
        summarySheet.Cell("A1").Style.Font.FontSize = 16;
        
        summarySheet.Cell("A3").Value = "Professor:";
        summarySheet.Cell("B3").Value = $"{dashboard.Title} {dashboard.ProfessorName}";
        
        summarySheet.Cell("A4").Value = "Courses Teaching:";
        summarySheet.Cell("B4").Value = dashboard.TotalCoursesTeaching;
        
        summarySheet.Cell("A5").Value = "Total Students:";
        summarySheet.Cell("B5").Value = dashboard.TotalStudents;
        
        summarySheet.Cell("A6").Value = "Pending Grades:";
        summarySheet.Cell("B6").Value = dashboard.PendingGrades;
        
        summarySheet.Cell("A7").Value = "Average Class Performance:";
        summarySheet.Cell("B7").Value = dashboard.AverageClassPerformance;
        summarySheet.Cell("B7").Style.NumberFormat.Format = "0.00";
        
        // Course Performance sheet
        var coursesSheet = workbook.Worksheets.Add("Course Performance");
        coursesSheet.Cell("A1").Value = "Course Name";
        coursesSheet.Cell("B1").Value = "Course Code";
        coursesSheet.Cell("C1").Value = "Students";
        coursesSheet.Cell("D1").Value = "Avg Grade";
        coursesSheet.Cell("E1").Value = "Pass Rate";
        coursesSheet.Cell("F1").Value = "Attendance Rate";
        
        coursesSheet.Range("A1:F1").Style.Font.Bold = true;
        coursesSheet.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.LightBlue;
        
        int row = 2;
        foreach (var course in dashboard.CoursePerformances)
        {
            coursesSheet.Cell(row, 1).Value = course.CourseName;
            coursesSheet.Cell(row, 2).Value = course.CourseCode;
            coursesSheet.Cell(row, 3).Value = course.TotalStudents;
            coursesSheet.Cell(row, 4).Value = course.AverageGrade;
            coursesSheet.Cell(row, 4).Style.NumberFormat.Format = "0.00";
            coursesSheet.Cell(row, 5).Value = course.PassRate / 100;
            coursesSheet.Cell(row, 5).Style.NumberFormat.Format = "0.0%";
            coursesSheet.Cell(row, 6).Value = course.AttendanceRate / 100;
            coursesSheet.Cell(row, 6).Style.NumberFormat.Format = "0.0%";
            row++;
        }
        
        summarySheet.Columns().AdjustToContents();
        coursesSheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
