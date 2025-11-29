-- Enable necessary extensions
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
-- =====================================================
-- 4.2.1 Tabele Principale
-- =====================================================
-- Users (extends Supabase Auth)
CREATE TABLE users (
    id UUID PRIMARY KEY REFERENCES auth.users(id),
    supabase_auth_id UUID UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    role VARCHAR(50) NOT NULL CHECK (
        role IN (
            'student',
            'professor',
            'dean',
            'rector',
            'secretariat',
            'admin'
        )
    ),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
-- Academic Structure
CREATE TABLE academic_years (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL,
    -- ex: "2023-2024"
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    is_current BOOLEAN DEFAULT false,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE semesters (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    academic_year_id UUID REFERENCES academic_years(id),
    number INT NOT NULL CHECK (number IN (1, 2)),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE faculties (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    short_name VARCHAR(50),
    dean_id UUID REFERENCES users(id),
    -- Link to user with 'dean' role
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE programs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    faculty_id UUID REFERENCES faculties(id),
    name VARCHAR(255) NOT NULL,
    -- ex: "Informatică Economică"
    degree_level VARCHAR(50) CHECK (degree_level IN ('bachelor', 'master', 'phd')),
    duration_years INT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE series (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    program_id UUID REFERENCES programs(id),
    name VARCHAR(50) NOT NULL,
    -- ex: "Seria A"
    year_of_study INT NOT NULL,
    -- 1, 2, 3...
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE groups (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    series_id UUID REFERENCES series(id),
    name VARCHAR(50) NOT NULL,
    -- ex: "Grupa 101"
    created_at TIMESTAMPTZ DEFAULT NOW()
);
-- People
CREATE TABLE professors (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    title VARCHAR(50),
    -- ex: "Conf. Univ. Dr."
    department VARCHAR(100),
    phone VARCHAR(20),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id),
    group_id UUID REFERENCES groups(id),
    enrollment_number VARCHAR(50) UNIQUE NOT NULL,
    -- Nr. Matricol
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    cnp VARCHAR(13) UNIQUE,
    address TEXT,
    phone VARCHAR(20),
    birth_date DATE,
    status VARCHAR(50) DEFAULT 'active' CHECK (
        status IN (
            'active',
            'suspended',
            'expelled',
            'graduated',
            'withdrawn'
        )
    ),
    enrolled_at DATE DEFAULT CURRENT_DATE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
-- Curriculum
CREATE TABLE courses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    program_id UUID REFERENCES programs(id),
    name VARCHAR(255) NOT NULL,
    code VARCHAR(50) UNIQUE NOT NULL,
    credits INT NOT NULL,
    semester_number INT NOT NULL,
    -- 1-6/8
    type VARCHAR(50) CHECK (type IN ('mandatory', 'optional', 'facultative')),
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE course_instances (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    course_id UUID REFERENCES courses(id),
    semester_id UUID REFERENCES semesters(id),
    professor_id UUID REFERENCES professors(id),
    group_id UUID REFERENCES groups(id),
    -- Can be null if for entire series
    series_id UUID REFERENCES series(id),
    -- Alternative to group_id
    max_students INT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE enrollments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id),
    course_instance_id UUID REFERENCES course_instances(id),
    status VARCHAR(50) DEFAULT 'enrolled' CHECK (status IN ('enrolled', 'dropped', 'completed')),
    enrolled_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(student_id, course_instance_id)
);
-- Grading
CREATE TABLE grades (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id),
    course_instance_id UUID REFERENCES course_instances(id),
    value INT CHECK (
        value >= 1
        AND value <= 10
    ),
    grade_type VARCHAR(50) DEFAULT 'final' CHECK (
        grade_type IN ('final', 'partial', 'lab', 'project')
    ),
    grading_period VARCHAR(50) DEFAULT 'session' CHECK (
        grading_period IN ('session', 'retake', 're-retake')
    ),
    status VARCHAR(50) DEFAULT 'draft' CHECK (
        status IN (
            'draft',
            'submitted',
            'approved',
            'contested',
            'final'
        )
    ),
    submitted_by UUID REFERENCES users(id),
    -- Professor
    submitted_at TIMESTAMPTZ,
    approved_by UUID REFERENCES users(id),
    -- Dean
    approved_at TIMESTAMPTZ,
    notes TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE grade_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    grade_id UUID REFERENCES grades(id),
    old_value INT,
    new_value INT,
    changed_by UUID REFERENCES users(id),
    reason TEXT,
    changed_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE grade_contests (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    grade_id UUID REFERENCES grades(id),
    student_id UUID REFERENCES students(id),
    reason TEXT NOT NULL,
    status VARCHAR(50) DEFAULT 'pending' CHECK (status IN ('pending', 'accepted', 'rejected')),
    resolution_notes TEXT,
    resolved_by UUID REFERENCES users(id),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    resolved_at TIMESTAMPTZ
);
-- Attendance
CREATE TABLE attendance (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id),
    course_instance_id UUID REFERENCES course_instances(id),
    date DATE NOT NULL,
    week_number INT,
    type VARCHAR(50) CHECK (type IN ('course', 'seminar', 'lab')),
    status VARCHAR(50) CHECK (
        status IN ('present', 'absent', 'excused', 'late')
    ),
    notes TEXT,
    created_by UUID REFERENCES users(id),
    created_at TIMESTAMPTZ DEFAULT NOW()
);
-- Requests & Documents
CREATE TABLE requests (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID REFERENCES students(id),
    type VARCHAR(50) NOT NULL,
    -- ex: "adeverinta", "scutire", "reexaminare"
    status VARCHAR(50) DEFAULT 'pending' CHECK (
        status IN (
            'pending',
            'in_progress',
            'approved',
            'rejected',
            'completed'
        )
    ),
    details JSONB,
    -- Custom fields based on type
    submitted_at TIMESTAMPTZ DEFAULT NOW(),
    processed_by UUID REFERENCES users(id),
    processed_at TIMESTAMPTZ,
    rejection_reason TEXT,
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID REFERENCES users(id),
    request_id UUID REFERENCES requests(id),
    name VARCHAR(255) NOT NULL,
    type VARCHAR(50),
    -- "pdf", "docx"
    storage_path VARCHAR(500) NOT NULL,
    generated_at TIMESTAMPTZ DEFAULT NOW()
);
-- System
CREATE TABLE notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id),
    title VARCHAR(200) NOT NULL,
    message TEXT NOT NULL,
    type VARCHAR(50),
    -- "info", "warning", "success", "error"
    related_entity VARCHAR(50),
    -- "grade", "request"
    related_id UUID,
    is_read BOOLEAN DEFAULT false,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id),
    action VARCHAR(100) NOT NULL,
    entity_type VARCHAR(100),
    entity_id UUID,
    old_values JSONB,
    new_values JSONB,
    ip_address VARCHAR(45),
    user_agent TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE TABLE system_settings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    key VARCHAR(100) UNIQUE NOT NULL,
    value JSONB NOT NULL,
    description TEXT,
    updated_by UUID REFERENCES users(id),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);
-- Insert default settings
INSERT INTO system_settings (key, value, description)
VALUES (
        'grade_approval_required',
        'true',
        'Whether grades require dean approval'
    ),
    (
        'max_absences_percentage',
        '25',
        'Maximum allowed absence percentage'
    ),
    (
        'grade_contest_deadline_days',
        '7',
        'Days allowed to contest a grade'
    ),
    (
        'current_academic_year',
        'null',
        'ID of current academic year'
    );
-- =====================================================
-- 4.2.2 Triggers și Functions
-- =====================================================
-- Auto-update updated_at
CREATE OR REPLACE FUNCTION update_updated_at() RETURNS TRIGGER AS $$ BEGIN NEW.updated_at = NOW();
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
-- Apply to all tables with updated_at
CREATE TRIGGER tr_users_updated BEFORE
UPDATE ON users FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_students_updated BEFORE
UPDATE ON students FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_professors_updated BEFORE
UPDATE ON professors FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_grades_updated BEFORE
UPDATE ON grades FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_faculties_updated BEFORE
UPDATE ON faculties FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_requests_updated BEFORE
UPDATE ON requests FOR EACH ROW EXECUTE FUNCTION update_updated_at();
-- Grade history trigger
CREATE OR REPLACE FUNCTION log_grade_change() RETURNS TRIGGER AS $$ BEGIN IF OLD.value IS DISTINCT
FROM NEW.value THEN
INSERT INTO grade_history (
        grade_id,
        old_value,
        new_value,
        changed_by,
        reason
    )
VALUES (
        NEW.id,
        OLD.value,
        NEW.value,
        NEW.submitted_by,
        'Grade updated'
    );
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER tr_grade_history
AFTER
UPDATE ON grades FOR EACH ROW EXECUTE FUNCTION log_grade_change();
-- Notification on grade submission
CREATE OR REPLACE FUNCTION notify_grade_submitted() RETURNS TRIGGER AS $$
DECLARE student_user_id UUID;
course_name TEXT;
BEGIN IF NEW.status = 'submitted'
AND OLD.status = 'draft' THEN
SELECT u.id INTO student_user_id
FROM students s
    JOIN users u ON s.user_id = u.id
WHERE s.id = NEW.student_id;
SELECT c.name INTO course_name
FROM course_instances ci
    JOIN courses c ON ci.course_id = c.id
WHERE ci.id = NEW.course_instance_id;
INSERT INTO notifications (
        user_id,
        title,
        message,
        type,
        related_entity,
        related_id
    )
VALUES (
        student_user_id,
        'Notă nouă înregistrată',
        'Ai primit nota ' || NEW.value || ' la ' || course_name,
        'grade',
        'grade',
        'grade',
        -- Fixed: removed extra arg, wait, PRD had 6 args in INSERT but 6 values. 
        -- PRD: VALUES (student_user_id, 'Notă...', 'Ai primit...', 'grade', 'grade', NEW.id)
        -- My code above: VALUES (student_user_id, 'Notă...', 'Ai primit...', 'grade', 'grade', NEW.id)
        -- Wait, I see 'grade' twice in my pasted code above? 
        -- PRD line 888: 'grade',
        -- PRD line 889: NEW.id
        -- Let me check the columns: user_id, title, message, type, related_entity, related_id.
        -- My INSERT: user_id, title, message, type, related_entity, related_id.
        -- Values: student_user_id, 'Title', 'Message', 'grade', 'grade', NEW.id.
        -- Yes, looks correct.
        NEW.id
    );
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER tr_notify_grade
AFTER
UPDATE ON grades FOR EACH ROW EXECUTE FUNCTION notify_grade_submitted();
-- Audit logging function
CREATE OR REPLACE FUNCTION audit_log() RETURNS TRIGGER AS $$ BEGIN IF TG_OP = 'INSERT' THEN
INSERT INTO audit_logs (
        user_id,
        action,
        entity_type,
        entity_id,
        new_values
    )
VALUES (
        current_setting('app.current_user_id', true)::UUID,
        'CREATE',
        TG_TABLE_NAME,
        NEW.id,
        to_jsonb(NEW)
    );
ELSIF TG_OP = 'UPDATE' THEN
INSERT INTO audit_logs (
        user_id,
        action,
        entity_type,
        entity_id,
        old_values,
        new_values
    )
VALUES (
        current_setting('app.current_user_id', true)::UUID,
        'UPDATE',
        TG_TABLE_NAME,
        NEW.id,
        to_jsonb(OLD),
        to_jsonb(NEW)
    );
ELSIF TG_OP = 'DELETE' THEN
INSERT INTO audit_logs (
        user_id,
        action,
        entity_type,
        entity_id,
        old_values
    )
VALUES (
        current_setting('app.current_user_id', true)::UUID,
        'DELETE',
        TG_TABLE_NAME,
        OLD.id,
        to_jsonb(OLD)
    );
END IF;
RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;
-- Apply audit to critical tables
CREATE TRIGGER tr_audit_grades
AFTER
INSERT
    OR
UPDATE
    OR DELETE ON grades FOR EACH ROW EXECUTE FUNCTION audit_log();
CREATE TRIGGER tr_audit_students
AFTER
INSERT
    OR
UPDATE
    OR DELETE ON students FOR EACH ROW EXECUTE FUNCTION audit_log();
CREATE TRIGGER tr_audit_users
AFTER
INSERT
    OR
UPDATE
    OR DELETE ON users FOR EACH ROW EXECUTE FUNCTION audit_log();
-- =====================================================
-- VIEWS
-- =====================================================
-- Student catalog view
CREATE OR REPLACE VIEW v_student_catalog AS
SELECT s.id as student_id,
    s.enrollment_number,
    s.first_name,
    s.last_name,
    s.status as student_status,
    g.id as group_id,
    g.name as group_name,
    ser.name as series_name,
    ser.year_of_study,
    p.name as program_name,
    f.name as faculty_name,
    c.name as course_name,
    c.code as course_code,
    c.credits,
    gr.value as grade,
    gr.grade_type,
    gr.grading_period,
    gr.status as grade_status,
    gr.submitted_at,
    gr.approved_at
FROM students s
    JOIN groups g ON s.group_id = g.id
    JOIN series ser ON g.series_id = ser.id
    JOIN programs p ON ser.program_id = p.id
    JOIN faculties f ON p.faculty_id = f.id
    LEFT JOIN enrollments e ON s.id = e.student_id
    LEFT JOIN course_instances ci ON e.course_instance_id = ci.id
    LEFT JOIN courses c ON ci.course_id = c.id
    LEFT JOIN grades gr ON s.id = gr.student_id
    AND ci.id = gr.course_instance_id;
-- Promovability report view
CREATE OR REPLACE VIEW v_promovability_report AS
SELECT f.id as faculty_id,
    f.name as faculty_name,
    p.id as program_id,
    p.name as program_name,
    ser.year_of_study,
    sem.id as semester_id,
    ay.name as academic_year,
    sem.number as semester,
    COUNT(DISTINCT s.id) as total_students,
    COUNT(
        DISTINCT CASE
            WHEN gr.value >= 5 THEN s.id
        END
    ) as passed_students,
    ROUND(
        COUNT(
            DISTINCT CASE
                WHEN gr.value >= 5 THEN s.id
            END
        )::DECIMAL / NULLIF(COUNT(DISTINCT s.id), 0) * 100,
        2
    ) as pass_rate
FROM faculties f
    JOIN programs p ON f.id = p.faculty_id
    JOIN series ser ON p.id = ser.program_id
    JOIN groups g ON ser.id = g.series_id
    JOIN students s ON g.id = s.group_id
    JOIN enrollments e ON s.id = e.student_id
    JOIN course_instances ci ON e.course_instance_id = ci.id
    JOIN semesters sem ON ci.semester_id = sem.id
    JOIN academic_years ay ON sem.academic_year_id = ay.id
    LEFT JOIN grades gr ON s.id = gr.student_id
    AND ci.id = gr.course_instance_id
    AND gr.grade_type = 'final'
WHERE s.status = 'active'
GROUP BY f.id,
    f.name,
    p.id,
    p.name,
    ser.year_of_study,
    sem.id,
    ay.name,
    sem.number;
-- =====================================================
-- DATABASE FUNCTIONS
-- =====================================================
-- Calculate student GPA
CREATE OR REPLACE FUNCTION calculate_student_gpa(
        p_student_id UUID,
        p_semester_id UUID DEFAULT NULL
    ) RETURNS DECIMAL AS $$
DECLARE gpa DECIMAL;
BEGIN
SELECT ROUND(
        SUM(g.value * c.credits)::DECIMAL / NULLIF(SUM(c.credits), 0),
        2
    ) INTO gpa
FROM grades g
    JOIN course_instances ci ON g.course_instance_id = ci.id
    JOIN courses c ON ci.course_id = c.id
WHERE g.student_id = p_student_id
    AND g.grade_type = 'final'
    AND g.status = 'approved'
    AND (
        p_semester_id IS NULL
        OR ci.semester_id = p_semester_id
    );
RETURN COALESCE(gpa, 0);
END;
$$ LANGUAGE plpgsql;
-- Check if student can enroll in course
CREATE OR REPLACE FUNCTION can_enroll_in_course(p_student_id UUID, p_course_instance_id UUID) RETURNS BOOLEAN AS $$
DECLARE existing_enrollment INT;
course_max_students INT;
current_enrollments INT;
BEGIN -- Check for existing enrollment
SELECT COUNT(*) INTO existing_enrollment
FROM enrollments
WHERE student_id = p_student_id
    AND course_instance_id = p_course_instance_id;
IF existing_enrollment > 0 THEN RETURN FALSE;
END IF;
-- Check capacity
SELECT ci.max_students,
    COUNT(e.id) INTO course_max_students,
    current_enrollments
FROM course_instances ci
    LEFT JOIN enrollments e ON ci.id = e.course_instance_id
    AND e.status = 'enrolled'
WHERE ci.id = p_course_instance_id
GROUP BY ci.max_students;
IF course_max_students IS NOT NULL
AND current_enrollments >= course_max_students THEN RETURN FALSE;
END IF;
RETURN TRUE;
END;
$$ LANGUAGE plpgsql;