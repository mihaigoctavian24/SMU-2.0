-- =====================================================
-- ANALYTICS PERFORMANCE INDEXES
-- Created: November 30, 2025
-- Purpose: Optimize analytics query performance
-- =====================================================

-- =====================================================
-- GRADES TABLE INDEXES
-- =====================================================

-- Index for student GPA trend queries
-- Used by: get_student_gpa_trend(), StudentAnalyticsService
CREATE INDEX IF NOT EXISTS idx_grades_student_semester 
ON grades(student_id, course_instance_id) 
WHERE grade_type = 'final' AND status = 'approved';

-- Index for grade distribution queries
CREATE INDEX IF NOT EXISTS idx_grades_value_type 
ON grades(value, grade_type) 
WHERE status = 'approved';

-- Index for submitted grades time-based queries
CREATE INDEX IF NOT EXISTS idx_grades_submitted_date 
ON grades(submitted_at) 
WHERE grade_type = 'final';

-- =====================================================
-- ATTENDANCE TABLE INDEXES
-- =====================================================

-- Index for student attendance summary
-- Used by: get_student_attendance_summary(), AttendanceSummaryDto
CREATE INDEX IF NOT EXISTS idx_attendance_student_date 
ON attendance(student_id, date, status);

-- Index for course instance attendance
CREATE INDEX IF NOT EXISTS idx_attendance_course_instance 
ON attendance(course_instance_id, status);

-- Index for attendance by date range (for dean/rector dashboards)
CREATE INDEX IF NOT EXISTS idx_attendance_date_range 
ON attendance(date, status);

-- =====================================================
-- ENROLLMENTS TABLE INDEXES
-- =====================================================

-- Index for student course enrollments
-- Used by: credit progress, course analytics
CREATE INDEX IF NOT EXISTS idx_enrollments_student_course 
ON enrollments(student_id, course_instance_id);

-- Index for course instance enrollments (for professor dashboard)
CREATE INDEX IF NOT EXISTS idx_enrollments_course_instance 
ON enrollments(course_instance_id, student_id);

-- =====================================================
-- STUDENTS TABLE INDEXES
-- =====================================================

-- Index for active students filtering
CREATE INDEX IF NOT EXISTS idx_students_status 
ON students(status) 
WHERE status = 'active';

-- Index for student group lookup (for performance views)
CREATE INDEX IF NOT EXISTS idx_students_group 
ON students(group_id, status);

-- Index for enrollment date queries
CREATE INDEX IF NOT EXISTS idx_students_enrollment_date 
ON students(enrolled_at);

-- =====================================================
-- COURSE INSTANCES TABLE INDEXES
-- =====================================================

-- Index for semester-based queries
CREATE INDEX IF NOT EXISTS idx_course_instances_semester 
ON course_instances(semester_id, course_id);

-- Index for course lookup
CREATE INDEX IF NOT EXISTS idx_course_instances_course 
ON course_instances(course_id, semester_id);

-- =====================================================
-- NOTIFICATIONS TABLE INDEXES
-- =====================================================

-- Index for user notifications lookup
CREATE INDEX IF NOT EXISTS idx_notifications_user_date 
ON notifications(user_id, created_at DESC) 
WHERE is_read = false;

-- =====================================================
-- COMMENTS
-- =====================================================

-- Add comments for documentation
COMMENT ON INDEX idx_grades_student_semester IS 'Optimizes student GPA trend and grade distribution queries';
COMMENT ON INDEX idx_attendance_student_date IS 'Optimizes student attendance summary queries';
COMMENT ON INDEX idx_enrollments_student_course IS 'Optimizes credit progress and enrollment queries';
COMMENT ON INDEX idx_students_status IS 'Optimizes active students filtering in analytics views';
COMMENT ON INDEX idx_course_instances_semester IS 'Optimizes course instance lookups by semester';

-- =====================================================
-- VERIFICATION
-- =====================================================

-- Query to verify all indexes were created
SELECT 
    schemaname,
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE indexname LIKE 'idx_%_student%' 
   OR indexname LIKE 'idx_%_attendance%'
   OR indexname LIKE 'idx_%_enrollment%'
   OR indexname LIKE 'idx_%_course_instance%'
ORDER BY tablename, indexname;

-- =====================================================
-- PERFORMANCE ANALYSIS
-- =====================================================

-- Run EXPLAIN ANALYZE on key queries to verify index usage:
-- 
-- EXPLAIN ANALYZE
-- SELECT * FROM get_student_gpa_trend('student-uuid-here');
--
-- EXPLAIN ANALYZE  
-- SELECT * FROM v_student_performance WHERE student_id = 'student-uuid-here';
--
-- Look for "Index Scan" instead of "Seq Scan" in the output
