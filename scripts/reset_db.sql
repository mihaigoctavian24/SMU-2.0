-- Drop all tables to reset schema
DROP TABLE IF EXISTS audit_logs,
notifications,
documents,
requests,
attendance,
grade_contests,
grade_history,
grades,
enrollments,
course_instances,
courses,
students,
professors,
groups,
series,
programs,
faculties,
semesters,
academic_years,
system_settings,
profiles,
-- Old table
users -- New table
CASCADE;
-- Drop types if they exist (to avoid conflicts with new schema)
DROP TYPE IF EXISTS user_role CASCADE;
DROP TYPE IF EXISTS student_status CASCADE;
DROP TYPE IF EXISTS grade_status CASCADE;
DROP TYPE IF EXISTS attendance_status CASCADE;
DROP TYPE IF EXISTS request_type CASCADE;