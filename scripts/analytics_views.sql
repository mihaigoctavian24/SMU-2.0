-- =====================================================
-- ANALYTICS VIEWS & MATERIALIZED VIEWS
-- Created: November 30, 2025
-- Purpose: Support analytics dashboards for all user roles
-- =====================================================

-- =====================================================
-- MATERIALIZED VIEW: Faculty Statistics
-- Refresh: Every 6 hours (see refresh_materialized_views.sql)
-- Usage: Dean & Rector dashboards
-- =====================================================
CREATE MATERIALIZED VIEW IF NOT EXISTS mv_faculty_statistics AS
SELECT 
    f.id as faculty_id,
    f.name as faculty_name,
    f.short_name as faculty_short_name,
    COUNT(DISTINCT p.id) as total_programs,
    COUNT(DISTINCT s.id) as total_students,
    COUNT(DISTINCT prof.id) as total_professors,
    -- Pass Rate Calculation
    ROUND(
        COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(g.id), 0) * 100, 
        2
    ) as avg_pass_rate,
    -- Average Grade
    ROUND(AVG(g.value), 2) as avg_grade,
    -- Student Status Breakdown
    COUNT(CASE WHEN s.status = 'graduated' THEN 1 END) as graduated_students,
    COUNT(CASE WHEN s.status = 'active' THEN 1 END) as active_students,
    COUNT(CASE WHEN s.status = 'suspended' THEN 1 END) as suspended_students,
    COUNT(CASE WHEN s.status = 'withdrawn' THEN 1 END) as withdrawn_students,
    -- Enrollment Trends
    COUNT(CASE WHEN EXTRACT(YEAR FROM s.enrolled_at) = EXTRACT(YEAR FROM CURRENT_DATE) THEN 1 END) as current_year_enrollments,
    -- Last Updated
    NOW() as last_refreshed
FROM faculties f
LEFT JOIN programs p ON f.id = p.faculty_id
LEFT JOIN series ser ON p.id = ser.program_id
LEFT JOIN groups gr ON ser.id = gr.series_id
LEFT JOIN students s ON gr.id = s.group_id
LEFT JOIN grades g ON s.id = g.student_id AND g.grade_type = 'final' AND g.status = 'approved'
LEFT JOIN professors prof ON prof.id IN (
    SELECT DISTINCT ci.professor_id 
    FROM course_instances ci 
    JOIN courses c ON ci.course_id = c.id 
    WHERE c.program_id = p.id
)
GROUP BY f.id, f.name, f.short_name;

-- Create index for faster lookups
CREATE UNIQUE INDEX IF NOT EXISTS idx_mv_faculty_stats_faculty_id ON mv_faculty_statistics(faculty_id);

-- =====================================================
-- VIEW: Student Performance Metrics
-- Usage: Student dashboard, Dean at-risk monitoring
-- =====================================================
CREATE OR REPLACE VIEW v_student_performance AS
SELECT 
    s.id as student_id,
    s.first_name || ' ' || s.last_name as student_name,
    s.enrollment_number,
    s.status as student_status,
    s.enrolled_at,
    g.id as group_id,
    g.name as group_name,
    ser.id as series_id,
    ser.name as series_name,
    ser.year_of_study,
    p.id as program_id,
    p.name as program_name,
    f.id as faculty_id,
    f.name as faculty_name,
    
    -- GPA Calculation (weighted by credits)
    ROUND(
        SUM(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' 
            THEN gr.value * c.credits END)::DECIMAL / 
        NULLIF(SUM(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' 
            THEN c.credits END), 0), 
        2
    ) as current_gpa,
    
    -- Attendance Rate
    ROUND(
        COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(att.id), 0) * 100, 
        2
    ) as attendance_rate,
    
    -- Credits Progress
    SUM(CASE WHEN gr.value >= 5 AND gr.grade_type = 'final' AND gr.status = 'approved' 
        THEN c.credits ELSE 0 END) as credits_earned,
    SUM(CASE WHEN gr.grade_type = 'final' THEN c.credits ELSE 0 END) as total_credits_attempted,
    
    -- Grade Distribution Counts
    COUNT(CASE WHEN gr.value = 10 AND gr.grade_type = 'final' THEN 1 END) as grade_10_count,
    COUNT(CASE WHEN gr.value = 9 AND gr.grade_type = 'final' THEN 1 END) as grade_9_count,
    COUNT(CASE WHEN gr.value = 8 AND gr.grade_type = 'final' THEN 1 END) as grade_8_count,
    COUNT(CASE WHEN gr.value = 7 AND gr.grade_type = 'final' THEN 1 END) as grade_7_count,
    COUNT(CASE WHEN gr.value = 6 AND gr.grade_type = 'final' THEN 1 END) as grade_6_count,
    COUNT(CASE WHEN gr.value = 5 AND gr.grade_type = 'final' THEN 1 END) as grade_5_count,
    COUNT(CASE WHEN gr.value < 5 AND gr.grade_type = 'final' THEN 1 END) as grade_fail_count,
    
    -- Attendance Status Breakdown
    COUNT(CASE WHEN att.status = 'present' THEN 1 END) as present_count,
    COUNT(CASE WHEN att.status = 'absent' THEN 1 END) as absent_count,
    COUNT(CASE WHEN att.status = 'excused' THEN 1 END) as excused_count,
    COUNT(CASE WHEN att.status = 'late' THEN 1 END) as late_count,
    
    -- Risk Assessment
    CASE 
        WHEN AVG(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' THEN gr.value END) < 5 THEN 'high_risk'
        WHEN AVG(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' THEN gr.value END) < 6 THEN 'medium_risk'
        WHEN COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
             NULLIF(COUNT(att.id), 0) < 0.75 THEN 'medium_risk'
        ELSE 'on_track'
    END as risk_level,
    
    -- Course Count
    COUNT(DISTINCT e.course_instance_id) as enrolled_courses_count
    
FROM students s
JOIN groups g ON s.group_id = g.id
JOIN series ser ON g.series_id = ser.id
JOIN programs p ON ser.program_id = p.id
JOIN faculties f ON p.faculty_id = f.id
LEFT JOIN enrollments e ON s.id = e.student_id
LEFT JOIN course_instances ci ON e.course_instance_id = ci.id
LEFT JOIN courses c ON ci.course_id = c.id
LEFT JOIN grades gr ON s.id = gr.student_id AND ci.id = gr.course_instance_id
LEFT JOIN attendance att ON s.id = att.student_id AND ci.id = att.course_instance_id
WHERE s.status = 'active'
GROUP BY s.id, s.first_name, s.last_name, s.enrollment_number, s.status, s.enrolled_at,
         g.id, g.name, ser.id, ser.name, ser.year_of_study, 
         p.id, p.name, f.id, f.name;

-- =====================================================
-- VIEW: Course Analytics
-- Usage: Professor & Dean dashboards
-- =====================================================
CREATE OR REPLACE VIEW v_course_analytics AS
SELECT 
    c.id as course_id,
    c.name as course_name,
    c.code as course_code,
    c.credits,
    c.semester_number,
    c.type as course_type,
    p.id as program_id,
    p.name as program_name,
    f.id as faculty_id,
    f.name as faculty_name,
    
    -- Instance Metrics
    COUNT(DISTINCT ci.id) as total_instances,
    COUNT(DISTINCT e.student_id) as total_enrollments,
    
    -- Grade Statistics
    ROUND(AVG(g.value), 2) as avg_grade,
    ROUND(STDDEV(g.value), 2) as grade_stddev,
    MIN(g.value) as min_grade,
    MAX(g.value) as max_grade,
    
    -- Pass Rate
    ROUND(
        COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(g.id), 0) * 100, 
        2
    ) as pass_rate,
    
    -- Grade Distribution
    COUNT(CASE WHEN g.value = 10 THEN 1 END) as excellent_count,
    COUNT(CASE WHEN g.value >= 9 THEN 1 END) as very_good_count,
    COUNT(CASE WHEN g.value >= 7 THEN 1 END) as good_count,
    COUNT(CASE WHEN g.value >= 5 THEN 1 END) as pass_count,
    COUNT(CASE WHEN g.value < 5 THEN 1 END) as fail_count,
    
    -- Attendance Metrics
    ROUND(
        COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(att.id), 0) * 100, 
        2
    ) as avg_attendance_rate
    
FROM courses c
JOIN programs p ON c.program_id = p.id
JOIN faculties f ON p.faculty_id = f.id
LEFT JOIN course_instances ci ON c.id = ci.course_id
LEFT JOIN enrollments e ON ci.id = e.course_instance_id
LEFT JOIN grades g ON e.student_id = g.student_id 
    AND ci.id = g.course_instance_id 
    AND g.grade_type = 'final' 
    AND g.status = 'approved'
LEFT JOIN attendance att ON e.student_id = att.student_id AND ci.id = att.course_instance_id
GROUP BY c.id, c.name, c.code, c.credits, c.semester_number, c.type,
         p.id, p.name, f.id, f.name;

-- =====================================================
-- VIEW: Professor Workload & Performance
-- Usage: Professor dashboard, Dean oversight
-- =====================================================
CREATE OR REPLACE VIEW v_professor_performance AS
SELECT 
    prof.id as professor_id,
    prof.first_name || ' ' || prof.last_name as professor_name,
    prof.title,
    prof.department,
    u.email as professor_email,
    
    -- Workload Metrics
    COUNT(DISTINCT ci.id) as total_courses_teaching,
    COUNT(DISTINCT e.student_id) as total_students,
    COUNT(DISTINCT c.id) as unique_courses,
    
    -- Grading Metrics
    COUNT(CASE WHEN g.status = 'draft' THEN 1 END) as pending_grades_count,
    COUNT(CASE WHEN g.status = 'submitted' THEN 1 END) as submitted_grades_count,
    COUNT(CASE WHEN g.status = 'approved' THEN 1 END) as approved_grades_count,
    
    -- Grade Contest Metrics
    COUNT(DISTINCT gc.id) as total_grade_contests,
    COUNT(CASE WHEN gc.status = 'pending' THEN 1 END) as pending_contests,
    COUNT(CASE WHEN gc.status = 'accepted' THEN 1 END) as accepted_contests,
    COUNT(CASE WHEN gc.status = 'rejected' THEN 1 END) as rejected_contests,
    
    -- Performance Metrics
    ROUND(AVG(g.value), 2) as avg_grade_given,
    ROUND(
        COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(g.id), 0) * 100, 
        2
    ) as pass_rate,
    
    -- Attendance Tracking
    COUNT(DISTINCT att.id) as total_attendance_records,
    ROUND(
        COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(att.id), 0) * 100, 
        2
    ) as class_attendance_rate
    
FROM professors prof
JOIN users u ON prof.user_id = u.id
LEFT JOIN course_instances ci ON prof.id = ci.professor_id
LEFT JOIN courses c ON ci.course_id = c.id
LEFT JOIN enrollments e ON ci.id = e.course_instance_id
LEFT JOIN grades g ON ci.id = g.course_instance_id AND prof.user_id = g.submitted_by
LEFT JOIN grade_contests gc ON g.id = gc.grade_id
LEFT JOIN attendance att ON ci.id = att.course_instance_id AND prof.user_id = att.created_by
GROUP BY prof.id, prof.first_name, prof.last_name, prof.title, prof.department, u.email;

-- =====================================================
-- VIEW: Program Enrollment Trends
-- Usage: Dean & Rector dashboards
-- =====================================================
CREATE OR REPLACE VIEW v_program_enrollment_trends AS
SELECT 
    p.id as program_id,
    p.name as program_name,
    p.degree_level,
    f.id as faculty_id,
    f.name as faculty_name,
    ay.id as academic_year_id,
    ay.name as academic_year,
    ser.year_of_study,
    
    -- Enrollment Counts
    COUNT(DISTINCT s.id) as total_students,
    COUNT(CASE WHEN s.status = 'active' THEN 1 END) as active_students,
    COUNT(CASE WHEN s.status = 'graduated' THEN 1 END) as graduated_students,
    COUNT(CASE WHEN s.status = 'suspended' THEN 1 END) as suspended_students,
    COUNT(CASE WHEN s.status = 'withdrawn' THEN 1 END) as withdrawn_students,
    
    -- Performance Metrics
    ROUND(AVG(g.value), 2) as avg_gpa,
    ROUND(
        COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(g.id), 0) * 100, 
        2
    ) as pass_rate,
    
    -- Retention Rate
    ROUND(
        COUNT(CASE WHEN s.status IN ('active', 'graduated') THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(s.id), 0) * 100, 
        2
    ) as retention_rate
    
FROM programs p
JOIN faculties f ON p.faculty_id = f.id
JOIN series ser ON p.id = ser.program_id
JOIN groups gr ON ser.id = gr.series_id
LEFT JOIN students s ON gr.id = s.group_id
LEFT JOIN enrollments e ON s.id = e.student_id
LEFT JOIN course_instances ci ON e.course_instance_id = ci.id
LEFT JOIN semesters sem ON ci.semester_id = sem.id
LEFT JOIN academic_years ay ON sem.academic_year_id = ay.id
LEFT JOIN grades g ON s.id = g.student_id 
    AND ci.id = g.course_instance_id 
    AND g.grade_type = 'final' 
    AND g.status = 'approved'
GROUP BY p.id, p.name, p.degree_level, f.id, f.name, 
         ay.id, ay.name, ser.year_of_study;

-- =====================================================
-- VIEW: At-Risk Students
-- Usage: Dean dashboard, academic intervention
-- =====================================================
CREATE OR REPLACE VIEW v_at_risk_students AS
SELECT 
    s.id as student_id,
    s.enrollment_number,
    s.first_name || ' ' || s.last_name as student_name,
    s.phone as student_phone,
    u.email as student_email,
    g.name as group_name,
    ser.year_of_study,
    p.name as program_name,
    f.name as faculty_name,
    
    -- Performance Indicators
    ROUND(
        SUM(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' 
            THEN gr.value * c.credits END)::DECIMAL / 
        NULLIF(SUM(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' 
            THEN c.credits END), 0), 
        2
    ) as current_gpa,
    
    ROUND(
        COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
        NULLIF(COUNT(att.id), 0) * 100, 
        2
    ) as attendance_rate,
    
    COUNT(CASE WHEN gr.value < 5 AND gr.grade_type = 'final' THEN 1 END) as failed_courses_count,
    COUNT(CASE WHEN att.status = 'absent' THEN 1 END) as total_absences,
    
    -- Risk Level
    CASE 
        WHEN AVG(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' THEN gr.value END) < 5 
            OR COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / NULLIF(COUNT(att.id), 0) < 0.60 
            THEN 'high'
        WHEN AVG(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' THEN gr.value END) < 6 
            OR COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / NULLIF(COUNT(att.id), 0) < 0.75 
            THEN 'medium'
        ELSE 'low'
    END as risk_level,
    
    -- Last Activity
    MAX(gr.submitted_at) as last_grade_date,
    MAX(att.date) as last_attendance_date
    
FROM students s
JOIN users u ON s.user_id = u.id
JOIN groups g ON s.group_id = g.id
JOIN series ser ON g.series_id = ser.id
JOIN programs p ON ser.program_id = p.id
JOIN faculties f ON p.faculty_id = f.id
LEFT JOIN enrollments e ON s.id = e.student_id
LEFT JOIN course_instances ci ON e.course_instance_id = ci.id
LEFT JOIN courses c ON ci.course_id = c.id
LEFT JOIN grades gr ON s.id = gr.student_id AND ci.id = gr.course_instance_id
LEFT JOIN attendance att ON s.id = att.student_id AND ci.id = att.course_instance_id
WHERE s.status = 'active'
GROUP BY s.id, s.enrollment_number, s.first_name, s.last_name, s.phone, u.email,
         g.name, ser.year_of_study, p.name, f.name
HAVING 
    AVG(CASE WHEN gr.grade_type = 'final' AND gr.status = 'approved' THEN gr.value END) < 6
    OR COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / NULLIF(COUNT(att.id), 0) < 0.75
    OR COUNT(CASE WHEN gr.value < 5 AND gr.grade_type = 'final' THEN 1 END) >= 2;

-- =====================================================
-- VIEW: Grade Approval Queue
-- Usage: Dean dashboard for grade approval workflow
-- =====================================================
CREATE OR REPLACE VIEW v_grade_approval_queue AS
SELECT 
    g.id as grade_id,
    g.value as grade_value,
    g.grade_type,
    g.grading_period,
    g.status as grade_status,
    g.submitted_at,
    g.notes,
    s.enrollment_number,
    s.first_name || ' ' || s.last_name as student_name,
    c.name as course_name,
    c.code as course_code,
    prof.first_name || ' ' || prof.last_name as professor_name,
    p.name as program_name,
    f.id as faculty_id,
    f.name as faculty_name,
    ay.name as academic_year,
    sem.number as semester_number,
    
    -- Contest Information
    COUNT(gc.id) as contest_count,
    MAX(gc.status) as latest_contest_status,
    
    -- Time Metrics
    EXTRACT(DAY FROM NOW() - g.submitted_at) as days_pending
    
FROM grades g
JOIN students s ON g.student_id = s.id
JOIN course_instances ci ON g.course_instance_id = ci.id
JOIN courses c ON ci.course_id = c.id
JOIN professors prof ON ci.professor_id = prof.id
JOIN semesters sem ON ci.semester_id = sem.id
JOIN academic_years ay ON sem.academic_year_id = ay.id
JOIN series ser ON ci.series_id = ser.id OR s.group_id IN (
    SELECT id FROM groups WHERE series_id = ci.series_id
)
JOIN programs p ON ser.program_id = p.id
JOIN faculties f ON p.faculty_id = f.id
LEFT JOIN grade_contests gc ON g.id = gc.grade_id
WHERE g.status IN ('submitted', 'contested')
GROUP BY g.id, g.value, g.grade_type, g.grading_period, g.status, g.submitted_at, g.notes,
         s.enrollment_number, s.first_name, s.last_name,
         c.name, c.code, prof.first_name, prof.last_name,
         p.name, f.id, f.name, ay.name, sem.number
ORDER BY g.submitted_at DESC;

-- =====================================================
-- GRANT PERMISSIONS
-- =====================================================
-- Allow authenticated users to read analytics views
GRANT SELECT ON v_student_performance TO authenticated;
GRANT SELECT ON v_course_analytics TO authenticated;
GRANT SELECT ON v_professor_performance TO authenticated;
GRANT SELECT ON v_program_enrollment_trends TO authenticated;
GRANT SELECT ON v_at_risk_students TO authenticated;
GRANT SELECT ON v_grade_approval_queue TO authenticated;
GRANT SELECT ON mv_faculty_statistics TO authenticated;

-- =====================================================
-- COMMENTS FOR DOCUMENTATION
-- =====================================================
COMMENT ON MATERIALIZED VIEW mv_faculty_statistics IS 'Aggregated faculty statistics, refreshed every 6 hours';
COMMENT ON VIEW v_student_performance IS 'Individual student performance metrics including GPA, attendance, and risk level';
COMMENT ON VIEW v_course_analytics IS 'Course-level analytics including enrollment, grades, and pass rates';
COMMENT ON VIEW v_professor_performance IS 'Professor workload and performance metrics';
COMMENT ON VIEW v_program_enrollment_trends IS 'Program enrollment trends over academic years';
COMMENT ON VIEW v_at_risk_students IS 'Students identified as at-risk based on GPA and attendance';
COMMENT ON VIEW v_grade_approval_queue IS 'Grades pending dean approval with workflow metrics';
