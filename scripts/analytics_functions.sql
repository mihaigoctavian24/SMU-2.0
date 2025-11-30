-- =====================================================
-- ANALYTICS DATABASE FUNCTIONS
-- Created: November 30, 2025
-- Purpose: Complex analytics queries for dashboards
-- =====================================================

-- =====================================================
-- FUNCTION: Get Student GPA Trend (by semester)
-- Usage: Student dashboard - GPA over time chart
-- =====================================================
CREATE OR REPLACE FUNCTION get_student_gpa_trend(p_student_id UUID)
RETURNS TABLE(
    semester_name VARCHAR,
    academic_year VARCHAR,
    gpa DECIMAL,
    semester_order INT,
    semester_id UUID,
    total_credits INT,
    courses_passed INT,
    courses_failed INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        'Semester ' || sem.number::TEXT as semester_name,
        ay.name as academic_year,
        ROUND(
            SUM(g.value * c.credits)::DECIMAL / NULLIF(SUM(c.credits), 0), 
            2
        ) as gpa,
        ROW_NUMBER() OVER (ORDER BY ay.start_date, sem.number)::INT as semester_order,
        sem.id as semester_id,
        SUM(c.credits)::INT as total_credits,
        COUNT(CASE WHEN g.value >= 5 THEN 1 END)::INT as courses_passed,
        COUNT(CASE WHEN g.value < 5 THEN 1 END)::INT as courses_failed
    FROM grades g
    JOIN course_instances ci ON g.course_instance_id = ci.id
    JOIN courses c ON ci.course_id = c.id
    JOIN semesters sem ON ci.semester_id = sem.id
    JOIN academic_years ay ON sem.academic_year_id = ay.id
    WHERE g.student_id = p_student_id
        AND g.grade_type = 'final'
        AND g.status = 'approved'
    GROUP BY sem.id, sem.number, ay.id, ay.name, ay.start_date
    ORDER BY ay.start_date, sem.number;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get Student Grade Distribution
-- Usage: Student dashboard - grade breakdown chart
-- =====================================================
CREATE OR REPLACE FUNCTION get_student_grade_distribution(p_student_id UUID)
RETURNS TABLE(
    grade_value INT,
    grade_count INT,
    percentage DECIMAL,
    courses TEXT[]
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        g.value as grade_value,
        COUNT(*)::INT as grade_count,
        ROUND(
            COUNT(*)::DECIMAL / 
            NULLIF((SELECT COUNT(*) FROM grades 
                    WHERE student_id = p_student_id 
                    AND grade_type = 'final' 
                    AND status = 'approved'), 0) * 100, 
            2
        ) as percentage,
        ARRAY_AGG(c.name ORDER BY c.name) as courses
    FROM grades g
    JOIN course_instances ci ON g.course_instance_id = ci.id
    JOIN courses c ON ci.course_id = c.id
    WHERE g.student_id = p_student_id
        AND g.grade_type = 'final'
        AND g.status = 'approved'
    GROUP BY g.value
    ORDER BY g.value DESC;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get Student Attendance Summary
-- Usage: Student dashboard - attendance statistics
-- =====================================================
CREATE OR REPLACE FUNCTION get_student_attendance_summary(
    p_student_id UUID,
    p_semester_id UUID DEFAULT NULL
)
RETURNS TABLE(
    total_sessions INT,
    present_count INT,
    absent_count INT,
    excused_count INT,
    late_count INT,
    attendance_rate DECIMAL,
    by_course_type JSONB
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        COUNT(*)::INT as total_sessions,
        COUNT(CASE WHEN att.status = 'present' THEN 1 END)::INT as present_count,
        COUNT(CASE WHEN att.status = 'absent' THEN 1 END)::INT as absent_count,
        COUNT(CASE WHEN att.status = 'excused' THEN 1 END)::INT as excused_count,
        COUNT(CASE WHEN att.status = 'late' THEN 1 END)::INT as late_count,
        ROUND(
            COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(*), 0) * 100, 
            2
        ) as attendance_rate,
        JSONB_OBJECT_AGG(
            att.type, 
            JSONB_BUILD_OBJECT(
                'total', COUNT(*),
                'present', COUNT(CASE WHEN att.status = 'present' THEN 1 END),
                'rate', ROUND(
                    COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
                    NULLIF(COUNT(*), 0) * 100, 
                    2
                )
            )
        ) as by_course_type
    FROM attendance att
    LEFT JOIN course_instances ci ON att.course_instance_id = ci.id
    WHERE att.student_id = p_student_id
        AND (p_semester_id IS NULL OR ci.semester_id = p_semester_id)
    GROUP BY att.type;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get Faculty Performance Metrics (time series)
-- Usage: Dean & Rector dashboards
-- =====================================================
CREATE OR REPLACE FUNCTION get_faculty_performance_metrics(
    p_faculty_id UUID,
    p_start_date DATE,
    p_end_date DATE
)
RETURNS TABLE(
    metric_date DATE,
    total_students INT,
    avg_gpa DECIMAL,
    pass_rate DECIMAL,
    attendance_rate DECIMAL,
    total_grades INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        g.submitted_at::DATE as metric_date,
        COUNT(DISTINCT s.id)::INT as total_students,
        ROUND(AVG(g.value), 2) as avg_gpa,
        ROUND(
            COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(g.id), 0) * 100, 
            2
        ) as pass_rate,
        ROUND(
            COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(att.id), 0) * 100, 
            2
        ) as attendance_rate,
        COUNT(g.id)::INT as total_grades
    FROM grades g
    JOIN students s ON g.student_id = s.id
    JOIN groups gr ON s.group_id = gr.id
    JOIN series ser ON gr.series_id = ser.id
    JOIN programs p ON ser.program_id = p.id
    LEFT JOIN course_instances ci ON g.course_instance_id = ci.id
    LEFT JOIN attendance att ON s.id = att.student_id AND ci.id = att.course_instance_id
    WHERE p.faculty_id = p_faculty_id
        AND g.submitted_at::DATE BETWEEN p_start_date AND p_end_date
        AND g.grade_type = 'final'
        AND g.status = 'approved'
    GROUP BY g.submitted_at::DATE
    ORDER BY g.submitted_at::DATE;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get Professor Course Performance
-- Usage: Professor dashboard - course comparison
-- =====================================================
CREATE OR REPLACE FUNCTION get_professor_course_performance(
    p_professor_id UUID,
    p_academic_year_id UUID DEFAULT NULL
)
RETURNS TABLE(
    course_id UUID,
    course_name VARCHAR,
    course_code VARCHAR,
    semester_number INT,
    total_students INT,
    avg_grade DECIMAL,
    pass_rate DECIMAL,
    attendance_rate DECIMAL,
    pending_grades INT,
    grade_contests INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c.id as course_id,
        c.name as course_name,
        c.code as course_code,
        sem.number as semester_number,
        COUNT(DISTINCT e.student_id)::INT as total_students,
        ROUND(AVG(g.value), 2) as avg_grade,
        ROUND(
            COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(g.id), 0) * 100, 
            2
        ) as pass_rate,
        ROUND(
            COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(att.id), 0) * 100, 
            2
        ) as attendance_rate,
        COUNT(CASE WHEN g.status = 'draft' THEN 1 END)::INT as pending_grades,
        COUNT(DISTINCT gc.id)::INT as grade_contests
    FROM course_instances ci
    JOIN courses c ON ci.course_id = c.id
    JOIN semesters sem ON ci.semester_id = sem.id
    LEFT JOIN academic_years ay ON sem.academic_year_id = ay.id
    LEFT JOIN enrollments e ON ci.id = e.course_instance_id
    LEFT JOIN grades g ON ci.id = g.course_instance_id AND e.student_id = g.student_id
    LEFT JOIN grade_contests gc ON g.id = gc.grade_id
    LEFT JOIN attendance att ON ci.id = att.course_instance_id
    WHERE ci.professor_id = p_professor_id
        AND (p_academic_year_id IS NULL OR ay.id = p_academic_year_id)
    GROUP BY c.id, c.name, c.code, sem.number
    ORDER BY sem.number, c.name;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get Program Comparison Metrics
-- Usage: Dean dashboard - radar chart data
-- =====================================================
CREATE OR REPLACE FUNCTION get_program_comparison_metrics(p_faculty_id UUID)
RETURNS TABLE(
    program_id UUID,
    program_name VARCHAR,
    total_students INT,
    avg_gpa DECIMAL,
    pass_rate DECIMAL,
    retention_rate DECIMAL,
    attendance_rate DECIMAL,
    graduation_rate DECIMAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        p.id as program_id,
        p.name as program_name,
        COUNT(DISTINCT s.id)::INT as total_students,
        ROUND(AVG(g.value), 2) as avg_gpa,
        ROUND(
            COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(g.id), 0) * 100, 
            2
        ) as pass_rate,
        ROUND(
            COUNT(CASE WHEN s.status IN ('active', 'graduated') THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(s.id), 0) * 100, 
            2
        ) as retention_rate,
        ROUND(
            COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(att.id), 0) * 100, 
            2
        ) as attendance_rate,
        ROUND(
            COUNT(CASE WHEN s.status = 'graduated' THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(s.id), 0) * 100, 
            2
        ) as graduation_rate
    FROM programs p
    JOIN series ser ON p.id = ser.program_id
    JOIN groups gr ON ser.id = gr.series_id
    LEFT JOIN students s ON gr.id = s.group_id
    LEFT JOIN enrollments e ON s.id = e.student_id
    LEFT JOIN course_instances ci ON e.course_instance_id = ci.id
    LEFT JOIN grades g ON s.id = g.student_id 
        AND ci.id = g.course_instance_id 
        AND g.grade_type = 'final' 
        AND g.status = 'approved'
    LEFT JOIN attendance att ON s.id = att.student_id AND ci.id = att.course_instance_id
    WHERE p.faculty_id = p_faculty_id
    GROUP BY p.id, p.name
    ORDER BY p.name;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get University-Wide KPIs
-- Usage: Rector dashboard
-- =====================================================
CREATE OR REPLACE FUNCTION get_university_kpis(
    p_academic_year_id UUID DEFAULT NULL
)
RETURNS TABLE(
    total_students INT,
    total_faculties INT,
    total_programs INT,
    total_professors INT,
    overall_avg_gpa DECIMAL,
    overall_pass_rate DECIMAL,
    overall_retention_rate DECIMAL,
    overall_graduation_rate DECIMAL,
    overall_attendance_rate DECIMAL,
    active_students INT,
    graduated_students INT,
    pending_grade_approvals INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        COUNT(DISTINCT s.id)::INT as total_students,
        COUNT(DISTINCT f.id)::INT as total_faculties,
        COUNT(DISTINCT p.id)::INT as total_programs,
        COUNT(DISTINCT prof.id)::INT as total_professors,
        ROUND(AVG(g.value), 2) as overall_avg_gpa,
        ROUND(
            COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(g.id), 0) * 100, 
            2
        ) as overall_pass_rate,
        ROUND(
            COUNT(CASE WHEN s.status IN ('active', 'graduated') THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(s.id), 0) * 100, 
            2
        ) as overall_retention_rate,
        ROUND(
            COUNT(CASE WHEN s.status = 'graduated' THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(s.id), 0) * 100, 
            2
        ) as overall_graduation_rate,
        ROUND(
            COUNT(CASE WHEN att.status = 'present' THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(att.id), 0) * 100, 
            2
        ) as overall_attendance_rate,
        COUNT(CASE WHEN s.status = 'active' THEN 1 END)::INT as active_students,
        COUNT(CASE WHEN s.status = 'graduated' THEN 1 END)::INT as graduated_students,
        COUNT(CASE WHEN gr.status = 'submitted' THEN 1 END)::INT as pending_grade_approvals
    FROM faculties f
    LEFT JOIN programs p ON f.id = p.faculty_id
    LEFT JOIN series ser ON p.id = ser.program_id
    LEFT JOIN groups grp ON ser.id = grp.series_id
    LEFT JOIN students s ON grp.id = s.group_id
    LEFT JOIN enrollments e ON s.id = e.student_id
    LEFT JOIN course_instances ci ON e.course_instance_id = ci.id
    LEFT JOIN semesters sem ON ci.semester_id = sem.id
    LEFT JOIN grades gr ON s.id = gr.student_id 
        AND ci.id = gr.course_instance_id 
        AND gr.grade_type = 'final'
    LEFT JOIN grades g ON s.id = g.student_id 
        AND g.grade_type = 'final' 
        AND g.status = 'approved'
    LEFT JOIN attendance att ON s.id = att.student_id AND ci.id = att.course_instance_id
    LEFT JOIN professors prof ON ci.professor_id = prof.id
    WHERE p_academic_year_id IS NULL OR sem.academic_year_id = p_academic_year_id;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get Faculty Enrollment Trends (5 years)
-- Usage: Rector dashboard - historical trends
-- =====================================================
CREATE OR REPLACE FUNCTION get_enrollment_trends(
    p_faculty_id UUID DEFAULT NULL,
    p_years_back INT DEFAULT 5
)
RETURNS TABLE(
    academic_year VARCHAR,
    year_start_date DATE,
    faculty_id UUID,
    faculty_name VARCHAR,
    total_enrollments INT,
    active_students INT,
    graduated_students INT,
    avg_gpa DECIMAL,
    pass_rate DECIMAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        ay.name as academic_year,
        ay.start_date as year_start_date,
        f.id as faculty_id,
        f.name as faculty_name,
        COUNT(DISTINCT s.id)::INT as total_enrollments,
        COUNT(CASE WHEN s.status = 'active' THEN 1 END)::INT as active_students,
        COUNT(CASE WHEN s.status = 'graduated' THEN 1 END)::INT as graduated_students,
        ROUND(AVG(g.value), 2) as avg_gpa,
        ROUND(
            COUNT(CASE WHEN g.value >= 5 THEN 1 END)::DECIMAL / 
            NULLIF(COUNT(g.id), 0) * 100, 
            2
        ) as pass_rate
    FROM academic_years ay
    LEFT JOIN semesters sem ON ay.id = sem.academic_year_id
    LEFT JOIN course_instances ci ON sem.id = ci.semester_id
    LEFT JOIN courses c ON ci.course_id = c.id
    LEFT JOIN programs p ON c.program_id = p.id
    LEFT JOIN faculties f ON p.faculty_id = f.id
    LEFT JOIN series ser ON p.id = ser.program_id
    LEFT JOIN groups gr ON ser.id = gr.series_id
    LEFT JOIN students s ON gr.id = s.group_id
    LEFT JOIN grades g ON s.id = g.student_id 
        AND ci.id = g.course_instance_id 
        AND g.grade_type = 'final' 
        AND g.status = 'approved'
    WHERE ay.start_date >= CURRENT_DATE - INTERVAL '1 year' * p_years_back
        AND (p_faculty_id IS NULL OR f.id = p_faculty_id)
    GROUP BY ay.id, ay.name, ay.start_date, f.id, f.name
    ORDER BY ay.start_date DESC, f.name;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get At-Risk Students by Faculty
-- Usage: Dean dashboard - intervention list
-- =====================================================
CREATE OR REPLACE FUNCTION get_at_risk_students_by_faculty(
    p_faculty_id UUID,
    p_risk_level VARCHAR DEFAULT 'all'
)
RETURNS TABLE(
    student_id UUID,
    student_name VARCHAR,
    enrollment_number VARCHAR,
    program_name VARCHAR,
    year_of_study INT,
    current_gpa DECIMAL,
    attendance_rate DECIMAL,
    failed_courses INT,
    risk_level VARCHAR,
    last_activity_date DATE
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        vrs.student_id,
        vrs.student_name,
        vrs.enrollment_number,
        vrs.program_name,
        vrs.year_of_study,
        vrs.current_gpa,
        vrs.attendance_rate,
        vrs.failed_courses_count as failed_courses,
        vrs.risk_level,
        GREATEST(vrs.last_grade_date, vrs.last_attendance_date) as last_activity_date
    FROM v_at_risk_students vrs
    WHERE vrs.faculty_name = (SELECT name FROM faculties WHERE id = p_faculty_id)
        AND (p_risk_level = 'all' OR vrs.risk_level = p_risk_level)
    ORDER BY 
        CASE vrs.risk_level
            WHEN 'high' THEN 1
            WHEN 'medium' THEN 2
            ELSE 3
        END,
        vrs.current_gpa ASC NULLS LAST;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- FUNCTION: Get Course Grade Distribution
-- Usage: Professor dashboard - analyze grade patterns
-- =====================================================
CREATE OR REPLACE FUNCTION get_course_grade_distribution(
    p_course_instance_id UUID
)
RETURNS TABLE(
    grade_value INT,
    student_count INT,
    percentage DECIMAL,
    cumulative_percentage DECIMAL
) AS $$
BEGIN
    RETURN QUERY
    WITH grade_counts AS (
        SELECT 
            g.value,
            COUNT(*)::INT as count
        FROM grades g
        WHERE g.course_instance_id = p_course_instance_id
            AND g.grade_type = 'final'
            AND g.status = 'approved'
        GROUP BY g.value
    ),
    total_count AS (
        SELECT SUM(count)::INT as total FROM grade_counts
    )
    SELECT 
        gc.value as grade_value,
        gc.count as student_count,
        ROUND(gc.count::DECIMAL / tc.total * 100, 2) as percentage,
        ROUND(
            SUM(gc.count) OVER (ORDER BY gc.value DESC)::DECIMAL / tc.total * 100, 
            2
        ) as cumulative_percentage
    FROM grade_counts gc
    CROSS JOIN total_count tc
    ORDER BY gc.value DESC;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- GRANT PERMISSIONS
-- =====================================================
GRANT EXECUTE ON FUNCTION get_student_gpa_trend(UUID) TO authenticated;
GRANT EXECUTE ON FUNCTION get_student_grade_distribution(UUID) TO authenticated;
GRANT EXECUTE ON FUNCTION get_student_attendance_summary(UUID, UUID) TO authenticated;
GRANT EXECUTE ON FUNCTION get_faculty_performance_metrics(UUID, DATE, DATE) TO authenticated;
GRANT EXECUTE ON FUNCTION get_professor_course_performance(UUID, UUID) TO authenticated;
GRANT EXECUTE ON FUNCTION get_program_comparison_metrics(UUID) TO authenticated;
GRANT EXECUTE ON FUNCTION get_university_kpis(UUID) TO authenticated;
GRANT EXECUTE ON FUNCTION get_enrollment_trends(UUID, INT) TO authenticated;
GRANT EXECUTE ON FUNCTION get_at_risk_students_by_faculty(UUID, VARCHAR) TO authenticated;
GRANT EXECUTE ON FUNCTION get_course_grade_distribution(UUID) TO authenticated;

-- =====================================================
-- COMMENTS FOR DOCUMENTATION
-- =====================================================
COMMENT ON FUNCTION get_student_gpa_trend(UUID) IS 'Returns GPA trend over semesters for a student';
COMMENT ON FUNCTION get_student_grade_distribution(UUID) IS 'Returns grade distribution breakdown for a student';
COMMENT ON FUNCTION get_student_attendance_summary(UUID, UUID) IS 'Returns attendance summary with optional semester filter';
COMMENT ON FUNCTION get_faculty_performance_metrics(UUID, DATE, DATE) IS 'Returns faculty performance time series data';
COMMENT ON FUNCTION get_professor_course_performance(UUID, UUID) IS 'Returns course-level performance metrics for a professor';
COMMENT ON FUNCTION get_program_comparison_metrics(UUID) IS 'Returns comparison metrics across programs in a faculty';
COMMENT ON FUNCTION get_university_kpis(UUID) IS 'Returns university-wide KPI metrics';
COMMENT ON FUNCTION get_enrollment_trends(UUID, INT) IS 'Returns enrollment trends over specified years';
COMMENT ON FUNCTION get_at_risk_students_by_faculty(UUID, VARCHAR) IS 'Returns at-risk students filtered by risk level';
COMMENT ON FUNCTION get_course_grade_distribution(UUID) IS 'Returns grade distribution for a specific course instance';
