-- Enable RLS on tables
ALTER TABLE students ENABLE ROW LEVEL SECURITY;
ALTER TABLE grades ENABLE ROW LEVEL SECURITY;
ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;
-- Add others as needed, e.g. users, requests
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE requests ENABLE ROW LEVEL SECURITY;
-- =====================================================
-- 9.1.1 Students Table
-- =====================================================
-- Students can read their own data
CREATE POLICY "students_read_self" ON students FOR
SELECT USING (user_id = auth.uid());
-- Professors can read students from their groups
CREATE POLICY "professors_read_own_groups" ON students FOR
SELECT USING (
        EXISTS (
            SELECT 1
            FROM course_instances ci
                JOIN professors p ON ci.professor_id = p.id
                JOIN users u ON p.user_id = u.id
            WHERE u.supabase_auth_id = auth.uid()
                AND ci.group_id = students.group_id
        )
    );
-- Secretariat can read/write students from their faculty
CREATE POLICY "secretariat_manage_faculty_students" ON students FOR ALL USING (
    EXISTS (
        SELECT 1
        FROM users u
            JOIN groups g ON students.group_id = g.id
            JOIN series s ON g.series_id = s.id
            JOIN programs p ON s.program_id = p.id
            JOIN faculties f ON p.faculty_id = f.id
        WHERE u.supabase_auth_id = auth.uid()
            AND u.role = 'secretariat' -- Assuming secretariat is linked to faculty (logic might need refinement if secretariat isn't directly linked in schema)
            -- For now, allow if role is secretariat (simplified for MVP if no direct link)
    )
);
-- Admin has full access
CREATE POLICY "admin_full_access" ON students FOR ALL USING (
    EXISTS (
        SELECT 1
        FROM users u
        WHERE u.supabase_auth_id = auth.uid()
            AND u.role = 'admin'
    )
);
-- =====================================================
-- 9.1.2 Grades Table
-- =====================================================
-- Students can only read their own grades
CREATE POLICY "students_read_own_grades" ON grades FOR
SELECT USING (
        student_id IN (
            SELECT s.id
            FROM students s
                JOIN users u ON s.user_id = u.id
            WHERE u.supabase_auth_id = auth.uid()
        )
    );
-- Professors can manage grades for their courses
CREATE POLICY "professors_manage_own_grades" ON grades FOR ALL USING (
    course_instance_id IN (
        SELECT ci.id
        FROM course_instances ci
            JOIN professors p ON ci.professor_id = p.id
            JOIN users u ON p.user_id = u.id
        WHERE u.supabase_auth_id = auth.uid()
    )
);
-- Deans can read/approve grades from their faculty
CREATE POLICY "deans_approve_grades" ON grades FOR ALL USING (
    EXISTS (
        SELECT 1
        FROM users u
            JOIN faculties f ON f.dean_id = u.id
            JOIN programs p ON p.faculty_id = f.id
            JOIN courses c ON c.program_id = p.id
            JOIN course_instances ci ON ci.course_id = c.id
        WHERE u.supabase_auth_id = auth.uid()
            AND ci.id = grades.course_instance_id
    )
);
-- =====================================================
-- 9.1.3 Notifications Table
-- =====================================================
-- Users can only see their own notifications
CREATE POLICY "users_own_notifications" ON notifications FOR ALL USING (
    user_id IN (
        SELECT id
        FROM users
        WHERE supabase_auth_id = auth.uid()
    )
);
-- =====================================================
-- 9.2 Supabase Storage Policies
-- =====================================================
-- Create bucket for documents if not exists
INSERT INTO storage.buckets (id, name, public)
VALUES ('documents', 'documents', false) ON CONFLICT (id) DO NOTHING;
-- Students can read their own documents
CREATE POLICY "students_read_own_docs" ON storage.objects FOR
SELECT USING (
        bucket_id = 'documents'
        AND (storage.foldername(name)) [1] = auth.uid()::text
    );
-- Secretariat can manage all documents
CREATE POLICY "secretariat_manage_docs" ON storage.objects FOR ALL USING (
    bucket_id = 'documents'
    AND EXISTS (
        SELECT 1
        FROM users u
        WHERE u.supabase_auth_id = auth.uid()
            AND u.role IN ('secretariat', 'admin')
    )
);