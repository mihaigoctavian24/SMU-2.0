# PRD – Sistem de Management Universitar (SMU)
## Versiune Revizuită v2.0

---

# Cuprins

1. [Introducere & Scop](#1-introducere--scop)
2. [Actori și Roluri](#2-actori-și-roluri)
3. [Arhitectură Sistem](#3-arhitectură-sistem)
4. [Schema Bazei de Date (ERD Complet)](#4-schema-bazei-de-date-erd-complet)
5. [State Machines](#5-state-machines)
6. [Module Funcționale](#6-module-funcționale)
7. [API Design & Contracts](#7-api-design--contracts)
8. [Permission Matrix Extinsă](#8-permission-matrix-extinsă)
9. [Supabase Configuration](#9-supabase-configuration)
10. [Fluxuri de Business](#10-fluxuri-de-business)
11. [UI/UX Design](#11-uiux-design)
12. [Cerințe Non-Funcționale](#12-cerințe-non-funcționale)
13. [User Stories & Acceptance Criteria](#13-user-stories--acceptance-criteria)
14. [RACI Matrix](#14-raci-matrix)
15. [Structură Proiect C#](#15-structură-proiect-c)
16. [Strategie de Testare](#16-strategie-de-testare)
17. [Deployment & DevOps](#17-deployment--devops)
18. [Roadmap & Milestones](#18-roadmap--milestones)
19. [Anexe](#19-anexe)

---

# 1. Introducere & Scop

## 1.1 Descriere Generală

Sistemul de Management Universitar (SMU) este o platformă enterprise destinată administrării complete a unei universități moderne. Soluția integrează managementul academic, administrativ și operațional într-un ecosistem digital unificat.

## 1.2 Obiective Principale

| Obiectiv | Descriere | Metric de Succes |
|----------|-----------|------------------|
| Centralizare | Unificarea tuturor datelor academice într-o singură platformă | 100% date migrare în 3 luni |
| Automatizare | Reducerea proceselor manuale cu 80% | Timp procesare cereri < 24h |
| Accesibilitate | Acces 24/7 pentru toți utilizatorii | Uptime 99.5% |
| Securitate | Protecția datelor personale conform GDPR | Zero breșe de securitate |

## 1.3 Scopul Sistemului

- Centralizarea datelor academice, administrative și operaționale
- Automatizarea proceselor universitare: studenți, profesori, facultăți, grupe, serii, note, prezențe, situații școlare
- Oferirea unei interfețe intuitive bazate pe MudBlazor
- Asigurarea securității datelor prin roluri, permisiuni și audit complet
- Generarea automată de documente și rapoarte
- Comunicare în timp real între actori

## 1.4 Out of Scope (MVP)

- Integrare cu sisteme externe de plăți
- Modul de admitere online complet
- Aplicație mobilă nativă (doar responsive web)
- Integrare cu platforme de e-learning externe (Moodle, etc.)

---

# 2. Actori și Roluri

## 2.1 Ierarhia Rolurilor

```
┌─────────────────────────────────────────────────────┐
│                      ADMIN                          │
│            (Full System Access)                     │
├─────────────────────────────────────────────────────┤
│                      RECTOR                         │
│         (University-wide Oversight)                 │
├─────────────────────────────────────────────────────┤
│                      DECAN                          │
│          (Faculty-level Management)                 │
├─────────────────────────────────────────────────────┤
│     SECRETARIAT          │         PROFESOR         │
│   (Administrative)       │        (Academic)        │
├─────────────────────────────────────────────────────┤
│                     STUDENT                         │
│              (End User / Consumer)                  │
└─────────────────────────────────────────────────────┘
```

## 2.2 Student

### Scopuri principale
- Vizualizare note, absențe, orar personal
- Înscriere la cursuri opționale/seminarii
- Completarea evaluărilor profesorilor
- Comunicare cu cadrul didactic
- Descărcare documente personale

### Scenarii de utilizare
| ID | Scenariu | Precondții | Rezultat Așteptat |
|----|----------|------------|-------------------|
| S01 | Autentificare | Cont activ | Dashboard student |
| S02 | Vizualizare catalog | Autentificat | Lista note cu filtre |
| S03 | Descărcare adeverință | Modul activ | PDF generat |
| S04 | Trimitere cerere | Autentificat | Cerere înregistrată |
| S05 | Contestare notă | Notă finalizată | Contestație creată |

### Permisiuni
- READ: Date proprii, orar, anunțuri publice
- CREATE: Cereri, contestații, evaluări profesori
- UPDATE: Profil personal (date non-academice)
- DELETE: Niciuna

## 2.3 Profesor

### Scopuri principale
- Gestionarea cursurilor și grupelor alocate
- Introducerea și modificarea notelor
- Marcarea prezențelor
- Comunicare cu studenții
- Vizualizare statistici cursuri

### Scenarii de utilizare
| ID | Scenariu | Precondții | Rezultat Așteptat |
|----|----------|------------|-------------------|
| P01 | Vizualizare cursuri | Autentificat | Lista cursuri proprii |
| P02 | Introducere note | Curs alocat | Note salvate (draft/submitted) |
| P03 | Marcare prezențe | Curs activ | Prezențe înregistrate |
| P04 | Trimitere anunț | Curs alocat | Notificare către studenți |
| P05 | Export catalog | Curs alocat | Excel/PDF generat |

### Permisiuni
- READ: Studenți din grupele proprii, cursuri proprii
- CREATE: Note, prezențe, anunțuri
- UPDATE: Note proprii (înainte de aprobare), prezențe
- DELETE: Niciuna (soft delete prin status)

## 2.4 Decan

### Scopuri principale
- Supravegherea activității academice a facultății
- Aprobare situații, note finale, exmatriculări
- Gestionare profesori din facultate
- Analiză rapoarte academice

### Scenarii de utilizare
| ID | Scenariu | Precondții | Rezultat Așteptat |
|----|----------|------------|-------------------|
| D01 | Aprobare note finale | Note submitted | Note approved/rejected |
| D02 | Vizualizare rapoarte | Autentificat | Dashboard analytics |
| D03 | Gestionare profesori | Facultate alocată | CRUD profesori |
| D04 | Aprobare exmatriculare | Cerere existentă | Student status updated |

### Permisiuni
- READ: Toate datele facultății proprii
- CREATE: Profesori, cursuri
- UPDATE: Status note, status studenți
- DELETE: Soft delete profesori/cursuri

## 2.5 Rector

### Scopuri principale
- Supervizarea întregii universități
- Vizualizare rapoarte generale cross-faculty
- Gestionarea decanilor și politicilor universitare
- Aprobare schimbări structurale majore

### Scenarii de utilizare
| ID | Scenariu | Precondții | Rezultat Așteptat |
|----|----------|------------|-------------------|
| R01 | Dashboard universitar | Autentificat | KPIs toate facultățile |
| R02 | Aprobare facultate nouă | Cerere pending | Facultate activată |
| R03 | Export rapoarte globale | Autentificat | Raport consolidat |
| R04 | Numire decan | Profesor existent | Rol actualizat |

### Permisiuni
- READ: Toate datele universității
- CREATE: Facultăți (cu aprobare)
- UPDATE: Decani, politici globale
- DELETE: Facultăți (cu aprobare), studenți (exmatriculare finală)

## 2.6 Secretariat

### Scopuri principale
- Administrarea studenților și documentelor
- Gestionarea cererilor studenților
- Emitere adeverințe, situații școlare
- Import/export date în bulk

### Scenarii de utilizare
| ID | Scenariu | Precondții | Rezultat Așteptat |
|----|----------|------------|-------------------|
| SE01 | Adăugare student | Date complete | Student creat + cont generat |
| SE02 | Modificare grupă | Student existent | Cascadare actualizări |
| SE03 | Procesare cerere | Cerere existentă | Document generat |
| SE04 | Import bulk studenți | Fișier CSV valid | Studenți creați în batch |
| SE05 | Emitere adeverință | Student activ | PDF generat + înregistrat |

### Permisiuni
- READ: Studenți și date facultate
- CREATE: Studenți, documente
- UPDATE: Date studenți, status cereri
- DELETE: Niciuna (doar soft delete)

## 2.7 Admin

### Scopuri principale
- Gestionarea utilizatorilor și rolurilor
- Configurare sistem (facultăți, serii, grupe)
- Audit și monitorizare sistem
- Backup și restore date

### Scenarii de utilizare
| ID | Scenariu | Precondții | Rezultat Așteptat |
|----|----------|------------|-------------------|
| A01 | Creare facultate | Autentificat | Facultate pending approval |
| A02 | Gestionare roluri | Autentificat | Permisiuni actualizate |
| A03 | Vizualizare audit log | Autentificat | Lista acțiuni sistem |
| A04 | Configurare sistem | Autentificat | Settings salvate |

### Permisiuni
- READ: Tot sistemul
- CREATE: Orice entitate
- UPDATE: Orice entitate
- DELETE: Orice entitate (cu audit)

---

# 3. Arhitectură Sistem

## 3.1 Diagrama Arhitecturală

```
┌─────────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                                │
├─────────────────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │              Blazor WebAssembly + MudBlazor                  │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │   │
│  │  │ Components  │  │   Services  │  │  State Management   │  │   │
│  │  │  (MudBlazor)│  │ (API Client)│  │    (Fluxor/Redux)   │  │   │
│  │  └─────────────┘  └─────────────┘  └─────────────────────┘  │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                                  │
                                  │ HTTPS
                                  ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         API LAYER                                   │
├─────────────────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │                 ASP.NET Core Web API                         │   │
│  │  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌──────────┐  │   │
│  │  │Controllers│  │ Services  │  │Validators │  │ Mappers  │  │   │
│  │  └───────────┘  └───────────┘  └───────────┘  └──────────┘  │   │
│  │  ┌───────────┐  ┌───────────┐  ┌───────────────────────────┐│   │
│  │  │Middleware │  │  Filters  │  │  Exception Handler        ││   │
│  │  │(Auth,CORS)│  │ (Audit)   │  │  (ProblemDetails)         ││   │
│  │  └───────────┘  └───────────┘  └───────────────────────────┘│   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                                  │
                                  │ Supabase Client
                                  ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      SUPABASE LAYER                                 │
├─────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌───────────┐  │
│  │    Auth     │  │  PostgreSQL │  │   Storage   │  │ Realtime  │  │
│  │   (JWT)     │  │     (DB)    │  │   (Files)   │  │ (Channels)│  │
│  └─────────────┘  └─────────────┘  └─────────────┘  └───────────┘  │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────────────┐ │
│  │    RLS      │  │  Functions  │  │        Triggers             │ │
│  │ (Policies)  │  │   (Edge)    │  │    (Audit, Cascade)         │ │
│  └─────────────┘  └─────────────┘  └─────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

## 3.2 Stack Tehnologic

| Layer | Tehnologie | Versiune | Justificare |
|-------|-----------|----------|-------------|
| Frontend | Blazor WASM | .NET 8 | SPA fără JavaScript, C# fullstack |
| UI Framework | MudBlazor | 7.x | Material Design, componente enterprise |
| State Management | Fluxor | 6.x | Redux pattern pentru Blazor |
| Backend | ASP.NET Core | 8.0 | Performance, cross-platform |
| Database | PostgreSQL | 15+ | Supabase managed, JSON support |
| Auth | Supabase Auth | Latest | JWT, OAuth2, MFA built-in |
| Storage | Supabase Storage | Latest | S3-compatible, policies |
| Realtime | Supabase Realtime | Latest | WebSocket channels |
| Caching | Redis (opțional) | 7.x | Session, response caching |
| Logging | Serilog | 3.x | Structured logging |

## 3.3 Responsabilități Supabase vs ASP.NET

### Direct din Blazor → Supabase
- Autentificare (login, signup, password reset, OAuth)
- Realtime subscriptions (note, prezențe, notificări)
- Upload fișiere în Storage
- Queries simple read-only

### Blazor → ASP.NET API → Supabase
- Business logic complex
- Validări cross-entity
- Operații batch (import/export)
- Audit logging
- Generare documente
- Calcule și agregări complexe

---

# 4. Schema Bazei de Date (ERD Complet)

## 4.1 Diagrama ERD

```
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│      users       │     │   academic_years │     │    semesters     │
├──────────────────┤     ├──────────────────┤     ├──────────────────┤
│ id (PK)          │     │ id (PK)          │     │ id (PK)          │
│ supabase_auth_id │     │ name             │     │ academic_year_id │──┐
│ email            │     │ start_date       │     │ number (1|2)     │  │
│ role             │     │ end_date         │     │ start_date       │  │
│ status           │     │ is_current       │     │ end_date         │  │
│ created_at       │     │ created_at       │     │ is_current       │  │
│ updated_at       │     └──────────────────┘     │ created_at       │  │
└──────────────────┘              │               └──────────────────┘  │
         │                        │                        │            │
         │                        └────────────────────────┼────────────┘
         │                                                 │
         ▼                                                 ▼
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│    faculties     │     │     programs     │     │     series       │
├──────────────────┤     ├──────────────────┤     ├──────────────────┤
│ id (PK)          │     │ id (PK)          │     │ id (PK)          │
│ name             │     │ faculty_id (FK)  │◄────│ program_id (FK)  │
│ code             │     │ name             │     │ academic_year_id │
│ dean_id (FK)     │◄─┐  │ code             │     │ name             │
│ status           │  │  │ duration_years   │     │ year_of_study    │
│ created_at       │  │  │ degree_type      │     │ status           │
│ updated_at       │  │  │ status           │     │ created_at       │
│ deleted_at       │  │  │ created_at       │     └──────────────────┘
└──────────────────┘  │  └──────────────────┘              │
         │            │           │                        │
         │            │           │                        ▼
         ▼            │           │               ┌──────────────────┐
┌──────────────────┐  │           │               │      groups      │
│    professors    │  │           │               ├──────────────────┤
├──────────────────┤  │           │               │ id (PK)          │
│ id (PK)          │──┘           │               │ series_id (FK)   │
│ user_id (FK)     │◄─────────────┼───────────────│ name             │
│ faculty_id (FK)  │              │               │ max_students     │
│ title            │              │               │ status           │
│ department       │              │               │ created_at       │
│ status           │              │               └──────────────────┘
│ created_at       │              │                        │
└──────────────────┘              │                        │
         │                        │                        ▼
         │                        │               ┌──────────────────┐
         │                        │               │     students     │
         ▼                        │               ├──────────────────┤
┌──────────────────┐              │               │ id (PK)          │
│     courses      │              │               │ user_id (FK)     │
├──────────────────┤              │               │ group_id (FK)    │
│ id (PK)          │              │               │ enrollment_no    │
│ program_id (FK)  │◄─────────────┘               │ first_name       │
│ name             │                              │ last_name        │
│ code             │                              │ cnp              │
│ credits          │                              │ status           │
│ course_type      │                              │ enrolled_at      │
│ year_of_study    │                              │ created_at       │
│ semester         │                              │ updated_at       │
│ status           │                              └──────────────────┘
│ created_at       │                                       │
└──────────────────┘                                       │
         │                                                 │
         ▼                                                 │
┌──────────────────┐     ┌──────────────────┐             │
│ course_instances │     │   enrollments    │             │
├──────────────────┤     ├──────────────────┤             │
│ id (PK)          │◄────│ course_inst_id   │             │
│ course_id (FK)   │     │ student_id (FK)  │◄────────────┘
│ semester_id (FK) │     │ status           │
│ professor_id(FK) │     │ enrolled_at      │
│ group_id (FK)    │     │ created_at       │
│ schedule_info    │     └──────────────────┘
│ status           │              │
│ created_at       │              │
└──────────────────┘              ▼
         │               ┌──────────────────┐     ┌──────────────────┐
         │               │      grades      │     │    attendance    │
         │               ├──────────────────┤     ├──────────────────┤
         └───────────────│ course_inst_id   │     │ course_inst_id   │◄─┐
                         │ student_id (FK)  │     │ student_id (FK)  │  │
                         │ value            │     │ date             │  │
                         │ grade_type       │     │ status           │  │
                         │ grading_period   │     │ marked_by (FK)   │  │
                         │ status           │     │ created_at       │  │
                         │ submitted_at     │     └──────────────────┘  │
                         │ approved_at      │                           │
                         │ approved_by (FK) │                           │
                         │ created_at       │                           │
                         │ updated_at       │                           │
                         └──────────────────┘                           │
                                  │                                     │
                                  ▼                                     │
                         ┌──────────────────┐                           │
                         │  grade_history   │                           │
                         ├──────────────────┤                           │
                         │ id (PK)          │                           │
                         │ grade_id (FK)    │                           │
                         │ old_value        │                           │
                         │ new_value        │                           │
                         │ changed_by (FK)  │                           │
                         │ reason           │                           │
                         │ created_at       │                           │
                         └──────────────────┘                           │
                                                                        │
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐ │
│    requests      │     │    documents     │     │  notifications   │ │
├──────────────────┤     ├──────────────────┤     ├──────────────────┤ │
│ id (PK)          │     │ id (PK)          │     │ id (PK)          │ │
│ student_id (FK)  │     │ request_id (FK)  │     │ user_id (FK)     │ │
│ type             │     │ type             │     │ title            │ │
│ status           │     │ file_path        │     │ message          │ │
│ details (JSONB)  │     │ generated_at     │     │ type             │ │
│ submitted_at     │     │ expires_at       │     │ is_read          │ │
│ processed_by(FK) │     │ created_at       │     │ related_entity   │ │
│ processed_at     │     └──────────────────┘     │ related_id       │ │
│ created_at       │                              │ created_at       │ │
└──────────────────┘                              └──────────────────┘ │
                                                                       │
┌──────────────────┐     ┌──────────────────┐                          │
│   audit_logs     │     │ grade_contests   │                          │
├──────────────────┤     ├──────────────────┤                          │
│ id (PK)          │     │ id (PK)          │                          │
│ user_id (FK)     │     │ grade_id (FK)    │                          │
│ action           │     │ student_id (FK)  │                          │
│ entity_type      │     │ reason           │                          │
│ entity_id        │     │ status           │                          │
│ old_values(JSONB)│     │ resolved_by (FK) │                          │
│ new_values(JSONB)│     │ resolution       │                          │
│ ip_address       │     │ submitted_at     │                          │
│ user_agent       │     │ resolved_at      │                          │
│ created_at       │     │ created_at       │──────────────────────────┘
└──────────────────┘     └──────────────────┘
```

## 4.2 Definiții Tabele SQL

### 4.2.1 Core Tables

```sql
-- =====================================================
-- USERS & AUTH
-- =====================================================
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    supabase_auth_id UUID UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    role VARCHAR(50) NOT NULL CHECK (role IN ('student', 'professor', 'dean', 'rector', 'secretariat', 'admin')),
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'suspended')),
    last_login_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_supabase_auth ON users(supabase_auth_id);

-- =====================================================
-- ACADEMIC STRUCTURE
-- =====================================================
CREATE TABLE academic_years (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL, -- e.g., "2024-2025"
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    is_current BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT valid_dates CHECK (end_date > start_date)
);

-- Ensure only one current academic year
CREATE UNIQUE INDEX idx_academic_years_current ON academic_years(is_current) WHERE is_current = TRUE;

CREATE TABLE semesters (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    academic_year_id UUID NOT NULL REFERENCES academic_years(id),
    number SMALLINT NOT NULL CHECK (number IN (1, 2)),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    is_current BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT valid_semester_dates CHECK (end_date > start_date),
    UNIQUE(academic_year_id, number)
);

CREATE TABLE faculties (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    code VARCHAR(20) UNIQUE NOT NULL,
    dean_id UUID REFERENCES users(id),
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'pending')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    deleted_at TIMESTAMPTZ -- soft delete
);

CREATE INDEX idx_faculties_code ON faculties(code);
CREATE INDEX idx_faculties_status ON faculties(status) WHERE deleted_at IS NULL;

CREATE TABLE programs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    faculty_id UUID NOT NULL REFERENCES faculties(id),
    name VARCHAR(255) NOT NULL,
    code VARCHAR(20) UNIQUE NOT NULL,
    duration_years SMALLINT NOT NULL CHECK (duration_years BETWEEN 1 AND 6),
    degree_type VARCHAR(50) NOT NULL CHECK (degree_type IN ('bachelor', 'master', 'doctorate')),
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE series (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    program_id UUID NOT NULL REFERENCES programs(id),
    academic_year_id UUID NOT NULL REFERENCES academic_years(id),
    name VARCHAR(50) NOT NULL, -- e.g., "A", "B", "C"
    year_of_study SMALLINT NOT NULL CHECK (year_of_study BETWEEN 1 AND 6),
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'graduated')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(program_id, academic_year_id, name, year_of_study)
);

CREATE TABLE groups (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    series_id UUID NOT NULL REFERENCES series(id),
    name VARCHAR(50) NOT NULL, -- e.g., "1", "2", "3" or "101", "102"
    max_students SMALLINT DEFAULT 30,
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(series_id, name)
);

-- =====================================================
-- PEOPLE
-- =====================================================
CREATE TABLE professors (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID UNIQUE NOT NULL REFERENCES users(id),
    faculty_id UUID NOT NULL REFERENCES faculties(id),
    title VARCHAR(50), -- e.g., "Prof. Dr.", "Conf. Dr.", "Lect. Dr."
    department VARCHAR(255),
    phone VARCHAR(20),
    office_location VARCHAR(100),
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'on_leave')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE students (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID UNIQUE NOT NULL REFERENCES users(id),
    group_id UUID NOT NULL REFERENCES groups(id),
    enrollment_number VARCHAR(50) UNIQUE NOT NULL, -- Număr matricol
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    cnp VARCHAR(13) UNIQUE, -- poate fi NULL pentru studenți străini
    birth_date DATE,
    phone VARCHAR(20),
    address TEXT,
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN (
        'pending', 'active', 'suspended', 'expelled', 'graduated', 'withdrawn'
    )),
    enrolled_at DATE NOT NULL DEFAULT CURRENT_DATE,
    graduated_at DATE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_students_enrollment ON students(enrollment_number);
CREATE INDEX idx_students_group ON students(group_id);
CREATE INDEX idx_students_status ON students(status);
CREATE INDEX idx_students_name ON students(last_name, first_name);

-- =====================================================
-- COURSES
-- =====================================================
CREATE TABLE courses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    program_id UUID NOT NULL REFERENCES programs(id),
    name VARCHAR(255) NOT NULL,
    code VARCHAR(20) NOT NULL,
    credits SMALLINT NOT NULL CHECK (credits BETWEEN 1 AND 30),
    course_type VARCHAR(50) NOT NULL CHECK (course_type IN ('mandatory', 'optional', 'elective')),
    year_of_study SMALLINT NOT NULL CHECK (year_of_study BETWEEN 1 AND 6),
    semester SMALLINT NOT NULL CHECK (semester IN (1, 2)),
    hours_course SMALLINT DEFAULT 0,
    hours_seminar SMALLINT DEFAULT 0,
    hours_lab SMALLINT DEFAULT 0,
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(program_id, code)
);

CREATE TABLE course_instances (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    course_id UUID NOT NULL REFERENCES courses(id),
    semester_id UUID NOT NULL REFERENCES semesters(id),
    professor_id UUID NOT NULL REFERENCES professors(id),
    group_id UUID NOT NULL REFERENCES groups(id),
    schedule_info JSONB, -- {"day": "Monday", "time": "08:00", "room": "A101"}
    max_students SMALLINT,
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'cancelled', 'completed')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(course_id, semester_id, group_id)
);

CREATE INDEX idx_course_instances_professor ON course_instances(professor_id);
CREATE INDEX idx_course_instances_semester ON course_instances(semester_id);

-- =====================================================
-- ENROLLMENTS & GRADES
-- =====================================================
CREATE TABLE enrollments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    course_instance_id UUID NOT NULL REFERENCES course_instances(id),
    student_id UUID NOT NULL REFERENCES students(id),
    status VARCHAR(20) DEFAULT 'enrolled' CHECK (status IN ('enrolled', 'withdrawn', 'completed', 'failed')),
    enrolled_at TIMESTAMPTZ DEFAULT NOW(),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(course_instance_id, student_id)
);

CREATE INDEX idx_enrollments_student ON enrollments(student_id);
CREATE INDEX idx_enrollments_course_instance ON enrollments(course_instance_id);

CREATE TABLE grades (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    course_instance_id UUID NOT NULL REFERENCES course_instances(id),
    student_id UUID NOT NULL REFERENCES students(id),
    value SMALLINT NOT NULL CHECK (value BETWEEN 1 AND 10),
    grade_type VARCHAR(50) NOT NULL CHECK (grade_type IN ('partial', 'final', 'exam', 'project', 'lab')),
    grading_period VARCHAR(50) NOT NULL CHECK (grading_period IN ('regular', 'restanta', 'marire')),
    weight DECIMAL(3,2) DEFAULT 1.00, -- pentru calcul medie ponderată
    status VARCHAR(20) DEFAULT 'draft' CHECK (status IN ('draft', 'submitted', 'approved', 'contested', 'final')),
    submitted_by UUID REFERENCES users(id),
    submitted_at TIMESTAMPTZ,
    approved_by UUID REFERENCES users(id),
    approved_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_grades_student ON grades(student_id);
CREATE INDEX idx_grades_course_instance ON grades(course_instance_id);
CREATE INDEX idx_grades_status ON grades(status);

-- Versionare note - păstrează istoricul modificărilor
CREATE TABLE grade_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    grade_id UUID NOT NULL REFERENCES grades(id) ON DELETE CASCADE,
    old_value SMALLINT,
    new_value SMALLINT NOT NULL,
    changed_by UUID NOT NULL REFERENCES users(id),
    reason TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_grade_history_grade ON grade_history(grade_id);

-- Contestații note
CREATE TABLE grade_contests (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    grade_id UUID NOT NULL REFERENCES grades(id),
    student_id UUID NOT NULL REFERENCES students(id),
    reason TEXT NOT NULL,
    status VARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'under_review', 'accepted', 'rejected')),
    resolution TEXT,
    resolved_by UUID REFERENCES users(id),
    submitted_at TIMESTAMPTZ DEFAULT NOW(),
    resolved_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- =====================================================
-- ATTENDANCE
-- =====================================================
CREATE TABLE attendance (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    course_instance_id UUID NOT NULL REFERENCES course_instances(id),
    student_id UUID NOT NULL REFERENCES students(id),
    date DATE NOT NULL,
    status VARCHAR(20) NOT NULL CHECK (status IN ('present', 'absent', 'excused', 'late')),
    marked_by UUID NOT NULL REFERENCES users(id),
    notes TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(course_instance_id, student_id, date)
);

CREATE INDEX idx_attendance_student ON attendance(student_id);
CREATE INDEX idx_attendance_course_date ON attendance(course_instance_id, date);

-- =====================================================
-- REQUESTS & DOCUMENTS
-- =====================================================
CREATE TABLE requests (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    student_id UUID NOT NULL REFERENCES students(id),
    type VARCHAR(50) NOT NULL CHECK (type IN (
        'certificate', 'transcript', 'enrollment_proof', 
        'group_change', 'leave_of_absence', 'withdrawal', 'other'
    )),
    status VARCHAR(20) DEFAULT 'pending' CHECK (status IN (
        'pending', 'in_progress', 'approved', 'rejected', 'completed'
    )),
    details JSONB, -- flexible data per request type
    submitted_at TIMESTAMPTZ DEFAULT NOW(),
    processed_by UUID REFERENCES users(id),
    processed_at TIMESTAMPTZ,
    notes TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_requests_student ON requests(student_id);
CREATE INDEX idx_requests_status ON requests(status);
CREATE INDEX idx_requests_type ON requests(type);

CREATE TABLE documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    request_id UUID REFERENCES requests(id),
    student_id UUID NOT NULL REFERENCES students(id),
    type VARCHAR(50) NOT NULL CHECK (type IN (
        'certificate', 'transcript', 'diploma', 'enrollment_proof', 'other'
    )),
    title VARCHAR(255) NOT NULL,
    file_path TEXT NOT NULL, -- Supabase Storage path
    file_size INTEGER,
    mime_type VARCHAR(100),
    registration_number VARCHAR(50), -- număr înregistrare document
    generated_by UUID REFERENCES users(id),
    generated_at TIMESTAMPTZ DEFAULT NOW(),
    expires_at TIMESTAMPTZ,
    is_valid BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_documents_student ON documents(student_id);
CREATE INDEX idx_documents_type ON documents(type);

-- =====================================================
-- NOTIFICATIONS
-- =====================================================
CREATE TABLE notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    type VARCHAR(50) NOT NULL CHECK (type IN (
        'info', 'warning', 'error', 'success', 'grade', 'attendance', 'request', 'announcement'
    )),
    is_read BOOLEAN DEFAULT FALSE,
    read_at TIMESTAMPTZ,
    related_entity VARCHAR(50), -- 'grade', 'request', 'attendance', etc.
    related_id UUID,
    action_url TEXT, -- deep link to related page
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_notifications_user ON notifications(user_id);
CREATE INDEX idx_notifications_unread ON notifications(user_id, is_read) WHERE is_read = FALSE;

-- =====================================================
-- AUDIT
-- =====================================================
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(id),
    action VARCHAR(50) NOT NULL, -- 'CREATE', 'UPDATE', 'DELETE', 'LOGIN', etc.
    entity_type VARCHAR(100) NOT NULL, -- 'student', 'grade', 'user', etc.
    entity_id UUID,
    old_values JSONB,
    new_values JSONB,
    ip_address INET,
    user_agent TEXT,
    session_id UUID,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_audit_user ON audit_logs(user_id);
CREATE INDEX idx_audit_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX idx_audit_created ON audit_logs(created_at);

-- Partition by month for performance (opțional pentru volume mari)
-- CREATE TABLE audit_logs_y2024m01 PARTITION OF audit_logs
--     FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');

-- =====================================================
-- SETTINGS & CONFIG
-- =====================================================
CREATE TABLE system_settings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    key VARCHAR(100) UNIQUE NOT NULL,
    value JSONB NOT NULL,
    description TEXT,
    updated_by UUID REFERENCES users(id),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Insert default settings
INSERT INTO system_settings (key, value, description) VALUES
    ('grade_approval_required', 'true', 'Whether grades require dean approval'),
    ('max_absences_percentage', '25', 'Maximum allowed absence percentage'),
    ('grade_contest_deadline_days', '7', 'Days allowed to contest a grade'),
    ('current_academic_year', 'null', 'ID of current academic year');
```

### 4.2.2 Triggers și Functions

```sql
-- =====================================================
-- TRIGGERS
-- =====================================================

-- Auto-update updated_at
CREATE OR REPLACE FUNCTION update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply to all tables with updated_at
CREATE TRIGGER tr_users_updated BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_students_updated BEFORE UPDATE ON students
    FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_professors_updated BEFORE UPDATE ON professors
    FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_grades_updated BEFORE UPDATE ON grades
    FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_faculties_updated BEFORE UPDATE ON faculties
    FOR EACH ROW EXECUTE FUNCTION update_updated_at();
CREATE TRIGGER tr_requests_updated BEFORE UPDATE ON requests
    FOR EACH ROW EXECUTE FUNCTION update_updated_at();

-- Grade history trigger
CREATE OR REPLACE FUNCTION log_grade_change()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.value IS DISTINCT FROM NEW.value THEN
        INSERT INTO grade_history (grade_id, old_value, new_value, changed_by, reason)
        VALUES (NEW.id, OLD.value, NEW.value, NEW.submitted_by, 'Grade updated');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_grade_history AFTER UPDATE ON grades
    FOR EACH ROW EXECUTE FUNCTION log_grade_change();

-- Notification on grade submission
CREATE OR REPLACE FUNCTION notify_grade_submitted()
RETURNS TRIGGER AS $$
DECLARE
    student_user_id UUID;
    course_name TEXT;
BEGIN
    IF NEW.status = 'submitted' AND OLD.status = 'draft' THEN
        SELECT u.id INTO student_user_id
        FROM students s
        JOIN users u ON s.user_id = u.id
        WHERE s.id = NEW.student_id;
        
        SELECT c.name INTO course_name
        FROM course_instances ci
        JOIN courses c ON ci.course_id = c.id
        WHERE ci.id = NEW.course_instance_id;
        
        INSERT INTO notifications (user_id, title, message, type, related_entity, related_id)
        VALUES (
            student_user_id,
            'Notă nouă înregistrată',
            'Ai primit nota ' || NEW.value || ' la ' || course_name,
            'grade',
            'grade',
            NEW.id
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_notify_grade AFTER UPDATE ON grades
    FOR EACH ROW EXECUTE FUNCTION notify_grade_submitted();

-- Audit logging function
CREATE OR REPLACE FUNCTION audit_log()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        INSERT INTO audit_logs (user_id, action, entity_type, entity_id, new_values)
        VALUES (current_setting('app.current_user_id', true)::UUID, 'CREATE', TG_TABLE_NAME, NEW.id, to_jsonb(NEW));
    ELSIF TG_OP = 'UPDATE' THEN
        INSERT INTO audit_logs (user_id, action, entity_type, entity_id, old_values, new_values)
        VALUES (current_setting('app.current_user_id', true)::UUID, 'UPDATE', TG_TABLE_NAME, NEW.id, to_jsonb(OLD), to_jsonb(NEW));
    ELSIF TG_OP = 'DELETE' THEN
        INSERT INTO audit_logs (user_id, action, entity_type, entity_id, old_values)
        VALUES (current_setting('app.current_user_id', true)::UUID, 'DELETE', TG_TABLE_NAME, OLD.id, to_jsonb(OLD));
    END IF;
    RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;

-- Apply audit to critical tables
CREATE TRIGGER tr_audit_grades AFTER INSERT OR UPDATE OR DELETE ON grades
    FOR EACH ROW EXECUTE FUNCTION audit_log();
CREATE TRIGGER tr_audit_students AFTER INSERT OR UPDATE OR DELETE ON students
    FOR EACH ROW EXECUTE FUNCTION audit_log();
CREATE TRIGGER tr_audit_users AFTER INSERT OR UPDATE OR DELETE ON users
    FOR EACH ROW EXECUTE FUNCTION audit_log();

-- =====================================================
-- VIEWS
-- =====================================================

-- Student catalog view
CREATE OR REPLACE VIEW v_student_catalog AS
SELECT 
    s.id as student_id,
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
LEFT JOIN grades gr ON s.id = gr.student_id AND ci.id = gr.course_instance_id;

-- Promovability report view
CREATE OR REPLACE VIEW v_promovability_report AS
SELECT 
    f.id as faculty_id,
    f.name as faculty_name,
    p.id as program_id,
    p.name as program_name,
    ser.year_of_study,
    sem.id as semester_id,
    ay.name as academic_year,
    sem.number as semester,
    COUNT(DISTINCT s.id) as total_students,
    COUNT(DISTINCT CASE WHEN gr.value >= 5 THEN s.id END) as passed_students,
    ROUND(
        COUNT(DISTINCT CASE WHEN gr.value >= 5 THEN s.id END)::DECIMAL / 
        NULLIF(COUNT(DISTINCT s.id), 0) * 100, 2
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
LEFT JOIN grades gr ON s.id = gr.student_id AND ci.id = gr.course_instance_id AND gr.grade_type = 'final'
WHERE s.status = 'active'
GROUP BY f.id, f.name, p.id, p.name, ser.year_of_study, sem.id, ay.name, sem.number;

-- =====================================================
-- DATABASE FUNCTIONS
-- =====================================================

-- Calculate student GPA
CREATE OR REPLACE FUNCTION calculate_student_gpa(p_student_id UUID, p_semester_id UUID DEFAULT NULL)
RETURNS DECIMAL AS $$
DECLARE
    gpa DECIMAL;
BEGIN
    SELECT 
        ROUND(
            SUM(g.value * c.credits)::DECIMAL / NULLIF(SUM(c.credits), 0),
            2
        )
    INTO gpa
    FROM grades g
    JOIN course_instances ci ON g.course_instance_id = ci.id
    JOIN courses c ON ci.course_id = c.id
    WHERE g.student_id = p_student_id
      AND g.grade_type = 'final'
      AND g.status = 'approved'
      AND (p_semester_id IS NULL OR ci.semester_id = p_semester_id);
    
    RETURN COALESCE(gpa, 0);
END;
$$ LANGUAGE plpgsql;

-- Check if student can enroll in course
CREATE OR REPLACE FUNCTION can_enroll_in_course(p_student_id UUID, p_course_instance_id UUID)
RETURNS BOOLEAN AS $$
DECLARE
    existing_enrollment INT;
    course_max_students INT;
    current_enrollments INT;
BEGIN
    -- Check for existing enrollment
    SELECT COUNT(*) INTO existing_enrollment
    FROM enrollments
    WHERE student_id = p_student_id AND course_instance_id = p_course_instance_id;
    
    IF existing_enrollment > 0 THEN
        RETURN FALSE;
    END IF;
    
    -- Check capacity
    SELECT ci.max_students, COUNT(e.id)
    INTO course_max_students, current_enrollments
    FROM course_instances ci
    LEFT JOIN enrollments e ON ci.id = e.course_instance_id AND e.status = 'enrolled'
    WHERE ci.id = p_course_instance_id
    GROUP BY ci.max_students;
    
    IF course_max_students IS NOT NULL AND current_enrollments >= course_max_students THEN
        RETURN FALSE;
    END IF;
    
    RETURN TRUE;
END;
$$ LANGUAGE plpgsql;
```

## 4.3 Reguli de Validare Date

| Câmp | Reguli | Exemplu Valid |
|------|--------|---------------|
| Email Student | `*@stud.rau.ro` | ion.popescu@stud.rau.ro |
| Email Profesor | `*@rau.ro` | maria.ionescu@rau.ro |
| CNP | 13 cifre, format valid | 1900101123456 |
| Notă | Integer 1-10 | 8 |
| Credite | Integer 1-30 | 6 |
| Număr Matricol | Format: `AN-PROG-XXXX` | 24-INFO-0001 |
| Prezență | Enum valid | present, absent, excused, late |

---

# 5. State Machines

## 5.1 Student Status

```
                    ┌────────────┐
                    │  PENDING   │
                    └─────┬──────┘
                          │ confirm enrollment
                          ▼
                    ┌────────────┐
          ┌─────────│   ACTIVE   │─────────┐
          │         └─────┬──────┘         │
          │               │                │
    suspend│         leave│           graduate
          │               │                │
          ▼               ▼                ▼
    ┌────────────┐  ┌────────────┐  ┌────────────┐
    │ SUSPENDED  │  │ WITHDRAWN  │  │ GRADUATED  │
    └─────┬──────┘  └────────────┘  └────────────┘
          │
    reactivate
          │
          ▼
    ┌────────────┐
    │   ACTIVE   │
    └─────┬──────┘
          │
     expel│
          │
          ▼
    ┌────────────┐
    │  EXPELLED  │
    └────────────┘
```

### Tranziții permise:
| From | To | Trigger | Actor |
|------|-----|---------|-------|
| pending | active | Confirmare înscriere | Secretariat |
| active | suspended | Decizie suspendare | Decan |
| active | withdrawn | Cerere retragere | Student + Secretariat |
| active | graduated | Finalizare studii | Secretariat |
| active | expelled | Decizie exmatriculare | Rector |
| suspended | active | Reactivare | Decan |

## 5.2 Grade Status

```
┌────────────┐
│   DRAFT    │ ← Profesor introduce nota
└─────┬──────┘
      │ submit
      ▼
┌────────────┐
│ SUBMITTED  │ ← Asteaptă aprobare
└─────┬──────┘
      │
      ├──────────── approve ────────────┐
      │                                 │
      ▼                                 │
┌────────────┐                          │
│ CONTESTED  │ ← Student contestă       │
└─────┬──────┘                          │
      │                                 │
      ├── accept contest ───┐           │
      │                     │           │
      │                     ▼           ▼
      │               ┌────────────────────┐
      └── reject ────►│      APPROVED      │
                      └─────────┬──────────┘
                                │ finalize
                                ▼
                      ┌────────────────────┐
                      │       FINAL        │
                      └────────────────────┘
```

### Tranziții permise:
| From | To | Trigger | Actor |
|------|-----|---------|-------|
| draft | submitted | Profesor trimite | Profesor |
| submitted | approved | Decan aprobă | Decan |
| submitted | contested | Student contestă | Student |
| contested | approved | Contestație respinsă | Decan |
| contested | draft | Contestație acceptată | Decan |
| approved | final | Finalizare sesiune | System |

## 5.3 Request Status

```
┌────────────┐
│  PENDING   │ ← Student trimite cerere
└─────┬──────┘
      │ start processing
      ▼
┌────────────┐
│IN_PROGRESS │ ← Secretariat procesează
└─────┬──────┘
      │
      ├── approve ──────────┐
      │                     │
      ├── reject ───────┐   │
      │                 │   │
      ▼                 ▼   ▼
┌────────────┐    ┌────────────┐
│  REJECTED  │    │  APPROVED  │
└────────────┘    └─────┬──────┘
                        │ generate document
                        ▼
                  ┌────────────┐
                  │ COMPLETED  │
                  └────────────┘
```

## 5.4 Faculty Status

```
┌────────────┐
│  PENDING   │ ← Admin creează
└─────┬──────┘
      │ rector approves
      ▼
┌────────────┐
│   ACTIVE   │◄────────────┐
└─────┬──────┘             │
      │                    │
      │ deactivate    reactivate
      │                    │
      ▼                    │
┌────────────┐             │
│  INACTIVE  │─────────────┘
└────────────┘
```

---

# 6. Module Funcționale

## 6.1 Modul Studenți

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| CRUD Studenți | Adăugare, modificare, vizualizare studenți | Secretariat, Admin |
| Import Bulk | Import studenți din CSV/Excel | Secretariat |
| Generare Cont | Creare automată cont la înregistrare | System |
| Vizualizare Catalog | Student vede propriile note | Student |
| Fișă Matricolă | Generare PDF situație școlară | Secretariat |
| Transfer Grupă | Mutare student între grupe | Secretariat |
| Istoric Academic | Vizualizare parcurs complet | Student, Secretariat |

### API Endpoints

```
GET    /api/v1/students                    - List students (filtrable)
GET    /api/v1/students/{id}               - Get student details
POST   /api/v1/students                    - Create student
PUT    /api/v1/students/{id}               - Update student
DELETE /api/v1/students/{id}               - Soft delete student
POST   /api/v1/students/import             - Bulk import from CSV
GET    /api/v1/students/{id}/grades        - Get student grades
GET    /api/v1/students/{id}/attendance    - Get student attendance
GET    /api/v1/students/{id}/documents     - Get student documents
POST   /api/v1/students/{id}/transfer      - Transfer to another group
GET    /api/v1/students/me                 - Get current student (self)
```

## 6.2 Modul Profesori

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| CRUD Profesori | Gestiune date profesori | Admin, Decan |
| Alocare Cursuri | Atribuire cursuri la grupe | Decan |
| Catalog Digital | Vizualizare și editare note | Profesor |
| Prezențe | Marcare și raportare prezențe | Profesor |
| Anunțuri | Trimitere notificări studenți | Profesor |
| Statistici Curs | Analytics performanță studenți | Profesor |

### API Endpoints

```
GET    /api/v1/professors                  - List professors
GET    /api/v1/professors/{id}             - Get professor details
POST   /api/v1/professors                  - Create professor
PUT    /api/v1/professors/{id}             - Update professor
GET    /api/v1/professors/{id}/courses     - Get assigned courses
GET    /api/v1/professors/me               - Get current professor (self)
GET    /api/v1/professors/me/courses       - Get my courses
```

## 6.3 Modul Note

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| Introducere Note | CRUD note pentru studenți | Profesor |
| Batch Save | Salvare multiple note simultan | Profesor |
| Workflow Aprobare | Submit → Approve flow | Profesor, Decan |
| Contestații | Sistem de contestare note | Student, Decan |
| Istoric | Versionare completă modificări | All |
| Calcul Medii | Medie ponderată automată | System |
| Export | Export catalog în Excel/PDF | Profesor, Secretariat |

### API Endpoints

```
GET    /api/v1/grades                      - List grades (filtrable)
GET    /api/v1/grades/{id}                 - Get grade details
POST   /api/v1/grades                      - Create grade
POST   /api/v1/grades/batch                - Create multiple grades
PUT    /api/v1/grades/{id}                 - Update grade
POST   /api/v1/grades/{id}/submit          - Submit for approval
POST   /api/v1/grades/{id}/approve         - Approve grade
POST   /api/v1/grades/{id}/reject          - Reject grade
GET    /api/v1/grades/{id}/history         - Get grade history
POST   /api/v1/grades/{id}/contest         - Contest grade
GET    /api/v1/grades/my                   - Get my grades (student)
GET    /api/v1/grades/course/{instanceId}  - Get grades for course instance
GET    /api/v1/grades/export/{instanceId}  - Export grades to Excel
```

## 6.4 Modul Prezențe

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| Marcare Prezență | Înregistrare prezență per ședință | Profesor |
| Motivări | Înregistrare absențe motivate | Profesor, Secretariat |
| Rapoarte | Statistici prezență per student/curs | Profesor, Decan |
| Alertă Absențe | Notificare la depășire prag | System |
| Realtime Sync | Sincronizare în timp real | System |

### API Endpoints

```
GET    /api/v1/attendance                  - List attendance records
POST   /api/v1/attendance                  - Mark attendance
POST   /api/v1/attendance/batch            - Mark attendance for group
PUT    /api/v1/attendance/{id}             - Update attendance
GET    /api/v1/attendance/my               - Get my attendance (student)
GET    /api/v1/attendance/course/{id}      - Get attendance for course
GET    /api/v1/attendance/report/{id}      - Get attendance report
```

## 6.5 Modul Cereri & Documente

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| Submisie Cereri | Student trimite cereri | Student |
| Procesare | Workflow aprobare cereri | Secretariat |
| Generare PDF | Creare automată documente | System |
| Template-uri | Șabloane documente configurabile | Admin |
| Registratură | Numerotare automată documente | System |
| Arhivare | Stocare și versionare documente | System |

### API Endpoints

```
GET    /api/v1/requests                    - List requests
GET    /api/v1/requests/{id}               - Get request details
POST   /api/v1/requests                    - Create request
PUT    /api/v1/requests/{id}               - Update request status
GET    /api/v1/requests/my                 - Get my requests (student)
POST   /api/v1/requests/{id}/process       - Process request
GET    /api/v1/documents                   - List documents
GET    /api/v1/documents/{id}              - Get document
GET    /api/v1/documents/{id}/download     - Download document
POST   /api/v1/documents/generate          - Generate document from template
```

## 6.6 Modul Notificări

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| In-App | Notificări în aplicație | All |
| Realtime | Push notifications via WebSocket | System |
| Marcare Citit | Tracking status notificări | All |
| Preferințe | Configurare tipuri notificări | All |
| Email (Future) | Notificări pe email | System |

### API Endpoints

```
GET    /api/v1/notifications               - Get my notifications
GET    /api/v1/notifications/unread        - Get unread count
PUT    /api/v1/notifications/{id}/read     - Mark as read
PUT    /api/v1/notifications/read-all      - Mark all as read
DELETE /api/v1/notifications/{id}          - Delete notification
```

## 6.7 Modul Admin

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| Gestiune Utilizatori | CRUD utilizatori și roluri | Admin |
| Structură Universitate | Facultăți, Programe, Serii, Grupe | Admin |
| Configurare Sistem | Settings globale | Admin |
| Audit Logs | Vizualizare activitate sistem | Admin |
| Backup/Restore | Management date | Admin |
| Seeding | Inițializare date test | Admin |

### API Endpoints

```
GET    /api/v1/admin/users                 - List all users
POST   /api/v1/admin/users                 - Create user
PUT    /api/v1/admin/users/{id}            - Update user
PUT    /api/v1/admin/users/{id}/role       - Change user role
GET    /api/v1/admin/audit                 - Get audit logs
GET    /api/v1/admin/settings              - Get system settings
PUT    /api/v1/admin/settings              - Update settings
POST   /api/v1/admin/seed                  - Seed test data
```

## 6.8 Modul Rapoarte

### Funcționalități

| Feature | Descriere | Actori |
|---------|-----------|--------|
| Dashboard KPIs | Metrici în timp real | Decan, Rector |
| Promovabilitate | Rate de promovare | Decan, Rector |
| Situații Școlare | Rapoarte per student/grupă | Secretariat |
| Export | PDF, Excel, CSV | All |
| Comparații | Cross-faculty/program analytics | Rector |

### API Endpoints

```
GET    /api/v1/reports/dashboard           - Get dashboard data
GET    /api/v1/reports/promovability       - Get pass rates
GET    /api/v1/reports/faculty/{id}        - Faculty report
GET    /api/v1/reports/program/{id}        - Program report
GET    /api/v1/reports/export              - Export report
```

---

# 7. API Design & Contracts

## 7.1 Design Principles

- RESTful API cu versionare (`/api/v1/...`)
- JSON pentru toate request/response bodies
- HTTP status codes standard
- HATEOAS links pentru navigare
- Paginare, filtrare, sortare consistente

## 7.2 Standard Response Format

### Success Response

```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "...": "..."
  },
  "meta": {
    "timestamp": "2024-01-15T10:30:00Z",
    "requestId": "req_xxx"
  },
  "links": {
    "self": "/api/v1/students/123",
    "grades": "/api/v1/students/123/grades"
  }
}
```

### Paginated Response

```json
{
  "success": true,
  "data": [
    { "id": "1", "..." },
    { "id": "2", "..." }
  ],
  "meta": {
    "pagination": {
      "page": 1,
      "pageSize": 20,
      "totalItems": 150,
      "totalPages": 8,
      "hasNext": true,
      "hasPrevious": false
    },
    "timestamp": "2024-01-15T10:30:00Z"
  },
  "links": {
    "self": "/api/v1/students?page=1",
    "next": "/api/v1/students?page=2",
    "last": "/api/v1/students?page=8"
  }
}
```

### Error Response

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "One or more validation errors occurred",
    "details": [
      {
        "field": "email",
        "message": "Email must be a valid @stud.rau.ro address"
      },
      {
        "field": "cnp",
        "message": "CNP must be exactly 13 digits"
      }
    ],
    "traceId": "trace_xxx"
  },
  "meta": {
    "timestamp": "2024-01-15T10:30:00Z",
    "requestId": "req_xxx"
  }
}
```

## 7.3 Error Codes

| HTTP Status | Code | Descriere |
|-------------|------|-----------|
| 400 | VALIDATION_ERROR | Erori de validare input |
| 400 | BAD_REQUEST | Request malformat |
| 401 | UNAUTHORIZED | Token invalid sau lipsă |
| 403 | FORBIDDEN | Acces interzis pentru acest rol |
| 404 | NOT_FOUND | Resursa nu există |
| 409 | CONFLICT | Conflict (ex: duplicate email) |
| 422 | UNPROCESSABLE_ENTITY | Business logic error |
| 429 | RATE_LIMITED | Prea multe request-uri |
| 500 | INTERNAL_ERROR | Eroare server |
| 503 | SERVICE_UNAVAILABLE | Serviciu indisponibil |

## 7.4 Query Parameters Standard

```
# Pagination
?page=1&pageSize=20

# Sorting
?sortBy=lastName&sortOrder=asc

# Filtering
?status=active&facultyId=uuid&search=ion

# Field Selection
?fields=id,firstName,lastName,email

# Include Relations
?include=grades,attendance

# Date Range
?fromDate=2024-01-01&toDate=2024-12-31
```

## 7.5 Authentication Headers

```
Authorization: Bearer <jwt_token>
X-Request-ID: <uuid>
X-Client-Version: 1.0.0
Accept-Language: ro-RO
```

## 7.6 Rate Limiting Headers

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640000000
```

---

# 8. Permission Matrix Extinsă

## 8.1 Modul Studenți

| Endpoint | Student | Profesor | Decan | Rector | Secretariat | Admin |
|----------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| GET /students | ❌ | ✅¹ | ✅² | ✅ | ✅² | ✅ |
| GET /students/{id} (self) | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| GET /students/{id} (other) | ❌ | ✅¹ | ✅² | ✅ | ✅² | ✅ |
| POST /students | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ |
| PUT /students/{id} | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ |
| DELETE /students/{id} | ❌ | ❌ | ✅³ | ✅ | ❌ | ✅ |
| POST /students/import | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ |
| POST /students/{id}/transfer | ❌ | ❌ | ✅ | ✅ | ✅ | ✅ |

¹ Doar studenții din grupele proprii
² Doar studenții din facultatea proprie  
³ Soft delete (exmatriculare)

## 8.2 Modul Note

| Endpoint | Student | Profesor | Decan | Rector | Secretariat | Admin |
|----------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| GET /grades/my | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| GET /grades/course/{id} | ❌ | ✅¹ | ✅² | ✅ | ✅² | ✅ |
| POST /grades | ❌ | ✅¹ | ❌ | ❌ | ❌ | ✅ |
| POST /grades/batch | ❌ | ✅¹ | ❌ | ❌ | ❌ | ✅ |
| PUT /grades/{id} | ❌ | ✅¹ | ✅³ | ❌ | ❌ | ✅ |
| DELETE /grades/{id} | ❌ | ❌ | ✅ | ✅ | ❌ | ✅ |
| POST /grades/{id}/submit | ❌ | ✅¹ | ❌ | ❌ | ❌ | ❌ |
| POST /grades/{id}/approve | ❌ | ❌ | ✅ | ❌ | ❌ | ✅ |
| POST /grades/{id}/contest | ✅⁴ | ❌ | ❌ | ❌ | ❌ | ❌ |

¹ Doar pentru cursurile proprii  
² Doar pentru cursurile din facultatea proprie  
³ Doar aprobare/respingere  
⁴ Doar pentru notele proprii, în termenul legal

## 8.3 Modul Prezențe

| Endpoint | Student | Profesor | Decan | Rector | Secretariat | Admin |
|----------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| GET /attendance/my | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| GET /attendance/course/{id} | ❌ | ✅¹ | ✅² | ✅ | ✅² | ✅ |
| POST /attendance | ❌ | ✅¹ | ❌ | ❌ | ❌ | ✅ |
| POST /attendance/batch | ❌ | ✅¹ | ❌ | ❌ | ❌ | ✅ |
| PUT /attendance/{id} | ❌ | ✅¹ | ✅² | ❌ | ✅² | ✅ |

## 8.4 Modul Cereri

| Endpoint | Student | Profesor | Decan | Rector | Secretariat | Admin |
|----------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| GET /requests | ❌ | ❌ | ✅² | ✅ | ✅² | ✅ |
| GET /requests/my | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| POST /requests | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| PUT /requests/{id}/process | ❌ | ❌ | ✅³ | ✅³ | ✅ | ✅ |

² Doar pentru facultatea proprie  
³ Doar pentru cereri speciale (exmatriculare, etc.)

## 8.5 Modul Structură (Facultăți/Programe/Grupe)

| Endpoint | Student | Profesor | Decan | Rector | Secretariat | Admin |
|----------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| GET /faculties | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| POST /faculties | ❌ | ❌ | ❌ | ❌ | ❌ | ✅¹ |
| PUT /faculties/{id} | ❌ | ❌ | ✅² | ✅ | ❌ | ✅ |
| DELETE /faculties/{id} | ❌ | ❌ | ❌ | ✅ | ❌ | ✅ |
| GET /programs | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| POST /programs | ❌ | ❌ | ✅² | ✅ | ❌ | ✅ |
| GET /groups | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| POST /groups | ❌ | ❌ | ✅² | ✅ | ✅² | ✅ |

¹ Creează în status "pending", necesită aprobare Rector  
² Doar pentru facultatea proprie

---

# 9. Supabase Configuration

## 9.1 Row Level Security (RLS) Policies

### 9.1.1 Students Table

```sql
-- Enable RLS
ALTER TABLE students ENABLE ROW LEVEL SECURITY;

-- Students can read their own data
CREATE POLICY "students_read_self" ON students
    FOR SELECT
    USING (
        user_id = auth.uid()
    );

-- Professors can read students from their groups
CREATE POLICY "professors_read_own_groups" ON students
    FOR SELECT
    USING (
        EXISTS (
            SELECT 1 FROM course_instances ci
            JOIN professors p ON ci.professor_id = p.id
            JOIN users u ON p.user_id = u.id
            WHERE u.supabase_auth_id = auth.uid()
            AND ci.group_id = students.group_id
        )
    );

-- Secretariat can read/write students from their faculty
CREATE POLICY "secretariat_manage_faculty_students" ON students
    FOR ALL
    USING (
        EXISTS (
            SELECT 1 FROM users u
            JOIN groups g ON students.group_id = g.id
            JOIN series s ON g.series_id = s.id
            JOIN programs p ON s.program_id = p.id
            JOIN faculties f ON p.faculty_id = f.id
            WHERE u.supabase_auth_id = auth.uid()
            AND u.role = 'secretariat'
            -- Assuming secretariat is linked to faculty
        )
    );

-- Admin has full access
CREATE POLICY "admin_full_access" ON students
    FOR ALL
    USING (
        EXISTS (
            SELECT 1 FROM users u
            WHERE u.supabase_auth_id = auth.uid()
            AND u.role = 'admin'
        )
    );
```

### 9.1.2 Grades Table

```sql
ALTER TABLE grades ENABLE ROW LEVEL SECURITY;

-- Students can only read their own grades
CREATE POLICY "students_read_own_grades" ON grades
    FOR SELECT
    USING (
        student_id IN (
            SELECT s.id FROM students s
            JOIN users u ON s.user_id = u.id
            WHERE u.supabase_auth_id = auth.uid()
        )
    );

-- Professors can manage grades for their courses
CREATE POLICY "professors_manage_own_grades" ON grades
    FOR ALL
    USING (
        course_instance_id IN (
            SELECT ci.id FROM course_instances ci
            JOIN professors p ON ci.professor_id = p.id
            JOIN users u ON p.user_id = u.id
            WHERE u.supabase_auth_id = auth.uid()
        )
    );

-- Deans can read/approve grades from their faculty
CREATE POLICY "deans_approve_grades" ON grades
    FOR ALL
    USING (
        EXISTS (
            SELECT 1 FROM users u
            JOIN faculties f ON f.dean_id = u.id
            JOIN programs p ON p.faculty_id = f.id
            JOIN courses c ON c.program_id = p.id
            JOIN course_instances ci ON ci.course_id = c.id
            WHERE u.supabase_auth_id = auth.uid()
            AND ci.id = grades.course_instance_id
        )
    );
```

### 9.1.3 Notifications Table

```sql
ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;

-- Users can only see their own notifications
CREATE POLICY "users_own_notifications" ON notifications
    FOR ALL
    USING (
        user_id IN (
            SELECT id FROM users
            WHERE supabase_auth_id = auth.uid()
        )
    );
```

## 9.2 Supabase Storage Policies

```sql
-- Create bucket for documents
INSERT INTO storage.buckets (id, name, public) 
VALUES ('documents', 'documents', false);

-- Students can read their own documents
CREATE POLICY "students_read_own_docs" ON storage.objects
    FOR SELECT
    USING (
        bucket_id = 'documents' AND
        (storage.foldername(name))[1] = auth.uid()::text
    );

-- Secretariat can manage all documents
CREATE POLICY "secretariat_manage_docs" ON storage.objects
    FOR ALL
    USING (
        bucket_id = 'documents' AND
        EXISTS (
            SELECT 1 FROM users u
            WHERE u.supabase_auth_id = auth.uid()
            AND u.role IN ('secretariat', 'admin')
        )
    );
```

## 9.3 Supabase Realtime Channels

```javascript
// Client-side subscription examples

// Subscribe to grade updates for a student
const gradeChannel = supabase
    .channel('grades-changes')
    .on(
        'postgres_changes',
        {
            event: '*',
            schema: 'public',
            table: 'grades',
            filter: `student_id=eq.${studentId}`
        },
        (payload) => handleGradeChange(payload)
    )
    .subscribe();

// Subscribe to notifications
const notificationChannel = supabase
    .channel('notifications')
    .on(
        'postgres_changes',
        {
            event: 'INSERT',
            schema: 'public',
            table: 'notifications',
            filter: `user_id=eq.${userId}`
        },
        (payload) => showNotification(payload.new)
    )
    .subscribe();

// Subscribe to attendance (for professors during class)
const attendanceChannel = supabase
    .channel('attendance-live')
    .on(
        'postgres_changes',
        {
            event: '*',
            schema: 'public',
            table: 'attendance',
            filter: `course_instance_id=eq.${courseInstanceId}`
        },
        (payload) => updateAttendanceUI(payload)
    )
    .subscribe();
```

## 9.4 Edge Functions (pentru logică complexă)

```typescript
// supabase/functions/calculate-gpa/index.ts
import { serve } from "https://deno.land/std@0.168.0/http/server.ts"
import { createClient } from "https://esm.sh/@supabase/supabase-js@2"

serve(async (req) => {
    const { studentId, semesterId } = await req.json();
    
    const supabase = createClient(
        Deno.env.get('SUPABASE_URL')!,
        Deno.env.get('SUPABASE_SERVICE_ROLE_KEY')!
    );
    
    const { data, error } = await supabase
        .rpc('calculate_student_gpa', { 
            p_student_id: studentId, 
            p_semester_id: semesterId 
        });
    
    if (error) {
        return new Response(JSON.stringify({ error: error.message }), {
            status: 500,
            headers: { 'Content-Type': 'application/json' }
        });
    }
    
    return new Response(JSON.stringify({ gpa: data }), {
        headers: { 'Content-Type': 'application/json' }
    });
});
```

---

# 10. Fluxuri de Business

## 10.1 Flux: Înregistrare Note

```
┌─────────────────────────────────────────────────────────────────────┐
│                    FLUX ÎNREGISTRARE NOTE                           │
└─────────────────────────────────────────────────────────────────────┘

  ┌──────────┐     ┌──────────────┐     ┌──────────────┐
  │ Profesor │────►│  Selectează  │────►│   Deschide   │
  │          │     │    Curs      │     │   Catalog    │
  └──────────┘     └──────────────┘     └──────┬───────┘
                                               │
                                               ▼
                                        ┌──────────────┐
                                        │  Introduce   │
                                        │    Note      │
                                        │ (Inline Edit)│
                                        └──────┬───────┘
                                               │
                                               ▼
                                        ┌──────────────┐
                                        │  Validare    │
                                        │  (1-10)      │
                                        └──────┬───────┘
                                               │
                          ┌────────────────────┼────────────────────┐
                          │                    │                    │
                          ▼                    ▼                    ▼
                   ┌────────────┐       ┌────────────┐       ┌────────────┐
                   │   Save     │       │   Submit   │       │   Cancel   │
                   │   Draft    │       │    All     │       │            │
                   └─────┬──────┘       └─────┬──────┘       └────────────┘
                         │                    │
                         │                    │
                         ▼                    ▼
                  ┌─────────────┐      ┌─────────────┐
                  │   Status:   │      │   Status:   │
                  │   DRAFT     │      │  SUBMITTED  │
                  └─────────────┘      └──────┬──────┘
                                              │
                                              │ [Dacă aprobare necesară]
                                              ▼
                                       ┌─────────────┐
                                       │   Decan     │
                                       │  primește   │
                                       │ notificare  │
                                       └──────┬──────┘
                                              │
                              ┌───────────────┼───────────────┐
                              │               │               │
                              ▼               ▼               ▼
                       ┌───────────┐   ┌───────────┐   ┌───────────┐
                       │  Approve  │   │  Reject   │   │  Request  │
                       │           │   │           │   │  Changes  │
                       └─────┬─────┘   └─────┬─────┘   └─────┬─────┘
                             │               │               │
                             ▼               ▼               ▼
                      ┌───────────┐   ┌───────────┐   ┌───────────┐
                      │  Status:  │   │  Status:  │   │ Notificare│
                      │ APPROVED  │   │  DRAFT    │   │ Profesor  │
                      └─────┬─────┘   └───────────┘   └───────────┘
                            │
                            │ [Notificare student]
                            ▼
                     ┌────────────┐
                     │  Student   │
                     │ vede nota  │
                     └────────────┘
```

## 10.2 Flux: Procesare Cerere Student

```
┌─────────────────────────────────────────────────────────────────────┐
│                    FLUX PROCESARE CERERE                            │
└─────────────────────────────────────────────────────────────────────┘

  ┌──────────┐     ┌──────────────┐     ┌──────────────┐
  │ Student  │────►│  Accesează   │────►│  Selectează  │
  │          │     │   Cereri     │     │  Tip Cerere  │
  └──────────┘     └──────────────┘     └──────┬───────┘
                                               │
                                               ▼
                                        ┌──────────────┐
                                        │ Completează  │
                                        │  Formular    │
                                        └──────┬───────┘
                                               │
                                               ▼
                                        ┌──────────────┐
                                        │   Validare   │
                                        │    Date      │
                                        └──────┬───────┘
                                               │
                                               ▼
                                        ┌──────────────┐
                                        │   Trimite    │
                                        │   Cerere     │
                                        └──────┬───────┘
                                               │
                                               ▼
                                       ┌───────────────┐
                                       │   Status:     │
                                       │   PENDING     │
                                       └───────┬───────┘
                                               │
                                               │ [Notificare Secretariat]
                                               ▼
                                       ┌───────────────┐
                                       │  Secretariat  │
                                       │   preia       │
                                       │   cererea     │
                                       └───────┬───────┘
                                               │
                                               ▼
                                       ┌───────────────┐
                                       │   Status:     │
                                       │ IN_PROGRESS   │
                                       └───────┬───────┘
                                               │
                               ┌───────────────┼───────────────┐
                               │               │               │
                               ▼               ▼               ▼
                        ┌───────────┐   ┌───────────┐   ┌───────────┐
                        │  Approve  │   │  Reject   │   │  Solicită │
                        │           │   │           │   │  Info     │
                        └─────┬─────┘   └─────┬─────┘   └─────┬─────┘
                              │               │               │
                              ▼               ▼               ▼
                       ┌───────────┐   ┌───────────┐   ┌───────────┐
                       │  Generare │   │  Status:  │   │ Notificare│
                       │  Document │   │ REJECTED  │   │  Student  │
                       └─────┬─────┘   └─────┬─────┘   └───────────┘
                             │               │
                             ▼               │
                      ┌───────────┐          │
                      │  Upload   │          │
                      │  Storage  │          │
                      └─────┬─────┘          │
                            │                │
                            ▼                │
                     ┌───────────┐           │
                     │  Status:  │           │
                     │ COMPLETED │           │
                     └─────┬─────┘           │
                           │                 │
                           ▼                 ▼
                    ┌─────────────────────────────┐
                    │     Notificare Student      │
                    │  (cu link document/motiv)   │
                    └─────────────────────────────┘
```

## 10.3 Flux: Autentificare

```
  ┌──────────┐     ┌──────────────┐     ┌──────────────┐
  │   User   │────►│    Login     │────►│   Supabase   │
  │          │     │    Page      │     │    Auth      │
  └──────────┘     └──────────────┘     └──────┬───────┘
                                               │
                          ┌────────────────────┼────────────────────┐
                          │                    │                    │
                          ▼                    ▼                    ▼
                   ┌────────────┐       ┌────────────┐       ┌────────────┐
                   │   Valid    │       │  Invalid   │       │    MFA     │
                   │   Creds    │       │   Creds    │       │  Required  │
                   └─────┬──────┘       └─────┬──────┘       └─────┬──────┘
                         │                    │                    │
                         ▼                    ▼                    ▼
                  ┌─────────────┐      ┌─────────────┐      ┌─────────────┐
                  │  Return JWT │      │   Show      │      │   Show      │
                  │   Token     │      │   Error     │      │  MFA Input  │
                  └──────┬──────┘      └─────────────┘      └──────┬──────┘
                         │                                         │
                         ▼                                         │
                  ┌─────────────┐                                  │
                  │ Store JWT   │◄─────────────────────────────────┘
                  │ LocalStorage│         [MFA Success]
                  └──────┬──────┘
                         │
                         ▼
                  ┌─────────────┐
                  │ Fetch User  │
                  │   Role      │
                  └──────┬──────┘
                         │
                         ▼
                  ┌─────────────┐
                  │  Redirect   │
                  │ to Dashboard│
                  │ (by Role)   │
                  └─────────────┘
```

---

# 11. UI/UX Design

## 11.1 Design System

### Paleta de Culori

| Rol | Primary | Secondary | Accent |
|-----|---------|-----------|--------|
| Student | #1976D2 (Blue) | #BBDEFB | #FF5722 |
| Profesor | #388E3C (Green) | #C8E6C9 | #FF5722 |
| Admin | #7B1FA2 (Purple) | #E1BEE7 | #FF5722 |
| Decan | #F57C00 (Orange) | #FFE0B2 | #1976D2 |
| Secretariat | #0097A7 (Teal) | #B2EBF2 | #FF5722 |

### Tipografie

- **Headers**: Roboto, 500 weight
- **Body**: Roboto, 400 weight
- **Monospace**: Roboto Mono (pentru coduri, numere matricol)

### Spacing

- Base unit: 8px
- xs: 4px, sm: 8px, md: 16px, lg: 24px, xl: 32px

## 11.2 Componente MudBlazor Utilizate

### Liste și Tabele

```razor
<MudTable Items="@students" 
          Hover="true" 
          Breakpoint="Breakpoint.Sm"
          Loading="@isLoading"
          LoadingProgressColor="Color.Primary"
          SortLabel="Sort By"
          Filter="FilterFunc">
    <ToolBarContent>
        <MudText Typo="Typo.h6">Studenți</MudText>
        <MudSpacer />
        <MudTextField @bind-Value="searchString" 
                      Placeholder="Caută..." 
                      Adornment="Adornment.Start" 
                      AdornmentIcon="@Icons.Material.Filled.Search" />
    </ToolBarContent>
    <HeaderContent>
        <MudTh><MudTableSortLabel SortBy="new Func<Student, object>(x => x.LastName)">Nume</MudTableSortLabel></MudTh>
        <MudTh>Email</MudTh>
        <MudTh>Grupă</MudTh>
        <MudTh>Status</MudTh>
        <MudTh>Acțiuni</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd DataLabel="Nume">@context.FullName</MudTd>
        <MudTd DataLabel="Email">@context.Email</MudTd>
        <MudTd DataLabel="Grupă">@context.GroupName</MudTd>
        <MudTd DataLabel="Status">
            <MudChip Color="@GetStatusColor(context.Status)" Size="Size.Small">
                @context.Status
            </MudChip>
        </MudTd>
        <MudTd>
            <MudIconButton Icon="@Icons.Material.Filled.Edit" OnClick="@(() => EditStudent(context))" />
            <MudIconButton Icon="@Icons.Material.Filled.Delete" Color="Color.Error" OnClick="@(() => DeleteStudent(context))" />
        </MudTd>
    </RowTemplate>
    <PagerContent>
        <MudTablePager />
    </PagerContent>
</MudTable>
```

### Formulare

```razor
<MudDialog>
    <DialogContent>
        <MudForm @ref="form" @bind-IsValid="@isValid">
            <MudTextField @bind-Value="student.FirstName" 
                          Label="Prenume" 
                          Required="true"
                          RequiredError="Prenumele este obligatoriu" />
            
            <MudTextField @bind-Value="student.LastName" 
                          Label="Nume" 
                          Required="true" />
            
            <MudTextField @bind-Value="student.Email" 
                          Label="Email"
                          Required="true"
                          Validation="@(new EmailAddressAttribute() { ErrorMessage = "Email invalid" })" />
            
            <MudSelect @bind-Value="student.GroupId" Label="Grupă" Required="true">
                @foreach (var group in groups)
                {
                    <MudSelectItem Value="@group.Id">@group.FullName</MudSelectItem>
                }
            </MudSelect>
            
            <MudDatePicker @bind-Date="student.EnrolledAt" Label="Data înscrierii" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Anulează</MudButton>
        <MudButton Color="Color.Primary" OnClick="Submit" Disabled="@(!isValid)">Salvează</MudButton>
    </DialogActions>
</MudDialog>
```

### Dashboard Cards

```razor
<MudGrid>
    <MudItem xs="12" sm="6" md="3">
        <MudCard>
            <MudCardContent>
                <MudText Typo="Typo.subtitle2" Color="Color.Secondary">Total Studenți</MudText>
                <MudText Typo="Typo.h3">@totalStudents</MudText>
                <MudText Typo="Typo.body2" Color="Color.Success">
                    <MudIcon Icon="@Icons.Material.Filled.TrendingUp" Size="Size.Small" />
                    +12% față de luna trecută
                </MudText>
            </MudCardContent>
        </MudCard>
    </MudItem>
    
    <MudItem xs="12" sm="6" md="3">
        <MudCard>
            <MudCardContent>
                <MudText Typo="Typo.subtitle2" Color="Color.Secondary">Rata Promovabilitate</MudText>
                <MudText Typo="Typo.h3">@passRate%</MudText>
                <MudProgressLinear Color="Color.Primary" Value="@passRate" />
            </MudCardContent>
        </MudCard>
    </MudItem>
</MudGrid>
```

## 11.3 Layout Principal

```razor
<MudLayout>
    <MudAppBar Elevation="1">
        <MudIconButton Icon="@Icons.Material.Filled.Menu" 
                       Color="Color.Inherit" 
                       Edge="Edge.Start" 
                       OnClick="@ToggleDrawer" />
        <MudText Typo="Typo.h6">Sistem Management Universitar</MudText>
        <MudSpacer />
        
        <!-- Notifications -->
        <MudBadge Content="@unreadCount" Color="Color.Error" Overlap="true" Visible="@(unreadCount > 0)">
            <MudIconButton Icon="@Icons.Material.Filled.Notifications" 
                           Color="Color.Inherit" 
                           OnClick="@OpenNotifications" />
        </MudBadge>
        
        <!-- User Menu -->
        <MudMenu Icon="@Icons.Material.Filled.Person" Color="Color.Inherit">
            <MudMenuItem Icon="@Icons.Material.Filled.AccountCircle">Profil</MudMenuItem>
            <MudMenuItem Icon="@Icons.Material.Filled.Settings">Setări</MudMenuItem>
            <MudDivider />
            <MudMenuItem Icon="@Icons.Material.Filled.Logout" OnClick="@Logout">Deconectare</MudMenuItem>
        </MudMenu>
    </MudAppBar>
    
    <MudDrawer @bind-Open="@drawerOpen" Elevation="2">
        <MudDrawerHeader>
            <MudAvatar Color="Color.Primary">@userInitials</MudAvatar>
            <MudText Typo="Typo.body1" Class="ml-2">@userName</MudText>
        </MudDrawerHeader>
        <MudDivider />
        <MudNavMenu>
            <MudNavLink Href="/dashboard" Icon="@Icons.Material.Filled.Dashboard">Dashboard</MudNavLink>
            
            @if (isStudent)
            {
                <MudNavLink Href="/catalog" Icon="@Icons.Material.Filled.Book">Catalogul meu</MudNavLink>
                <MudNavLink Href="/attendance" Icon="@Icons.Material.Filled.EventAvailable">Prezențe</MudNavLink>
                <MudNavLink Href="/requests" Icon="@Icons.Material.Filled.Description">Cereri</MudNavLink>
            }
            
            @if (isProfessor)
            {
                <MudNavLink Href="/courses" Icon="@Icons.Material.Filled.Class">Cursurile mele</MudNavLink>
                <MudNavLink Href="/grades" Icon="@Icons.Material.Filled.Grade">Note</MudNavLink>
                <MudNavLink Href="/attendance" Icon="@Icons.Material.Filled.HowToReg">Prezențe</MudNavLink>
            }
            
            @if (isAdmin || isDean)
            {
                <MudNavGroup Title="Administrare" Icon="@Icons.Material.Filled.AdminPanelSettings">
                    <MudNavLink Href="/admin/users">Utilizatori</MudNavLink>
                    <MudNavLink Href="/admin/faculties">Facultăți</MudNavLink>
                    <MudNavLink Href="/admin/reports">Rapoarte</MudNavLink>
                </MudNavGroup>
            }
        </MudNavMenu>
    </MudDrawer>
    
    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="py-4">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>
```

## 11.4 Wireframes (Descriere)

### Dashboard Student
- Header cu salut personalizat și statistici rapide
- Card-uri cu: Medie generală, Credite, Absențe, Cereri pending
- Tabel cu ultimele note primite
- Calendar cu evenimente/deadline-uri
- Notificări recente

### Dashboard Profesor
- Card-uri cu: Cursuri active, Studenți total, Note de introdus
- Lista cursurilor cu acces rapid la catalog
- Grafic rate promovabilitate pe cursuri
- Quick actions: Introducere note, Marcare prezențe

### Dashboard Admin
- KPIs universitate: Studenți activi, Profesori, Facultăți
- Grafice: Distribuție pe facultăți, Trend înmatriculări
- Audit log recent
- Alerte sistem

---

# 12. Cerințe Non-Funcționale

## 12.1 Securitate

| Cerință | Specificație | Implementare |
|---------|--------------|--------------|
| Autentificare | JWT cu refresh tokens | Supabase Auth |
| Autorizare | RBAC + RLS | Permission Matrix + Supabase RLS |
| Criptare | TLS 1.3 in transit, AES-256 at rest | Supabase managed |
| MFA | TOTP opțional | Supabase Auth MFA |
| Rate Limiting | 100 req/min per IP | ASP.NET middleware |
| CORS | Whitelist domenii | ASP.NET CORS policy |
| Audit | Log complet acțiuni critice | Triggers PostgreSQL |
| Session | Max 24h, refresh la activitate | JWT expiry |
| Password | Min 8 chars, complexity rules | Supabase Auth policies |

## 12.2 Performanță

| Metric | Target | Măsurare |
|--------|--------|----------|
| API Response Time (P95) | < 200ms | Application Insights |
| Page Load Time | < 2s | Lighthouse |
| Time to Interactive | < 3s | Lighthouse |
| Database Query Time | < 50ms | pg_stat_statements |
| Concurrent Users | 500+ | Load testing |
| Uptime | 99.5% | Monitoring |

## 12.3 Scalabilitate

- Horizontal scaling pentru API (load balancer)
- Connection pooling pentru DB (PgBouncer via Supabase)
- CDN pentru assets statice
- Lazy loading pentru module Blazor
- Pagination obligatorie (max 100 items/page)

## 12.4 Observabilitate

### Logging (Serilog)

```json
{
  "timestamp": "2024-01-15T10:30:00Z",
  "level": "Information",
  "message": "Grade submitted",
  "properties": {
    "userId": "uuid",
    "gradeId": "uuid",
    "studentId": "uuid",
    "value": 9,
    "requestId": "req_xxx",
    "traceId": "trace_xxx"
  }
}
```

### Metrics (Prometheus format)

```
# API metrics
http_requests_total{method="POST", endpoint="/api/v1/grades", status="200"} 1234
http_request_duration_seconds{method="POST", endpoint="/api/v1/grades"} 0.045

# Business metrics  
students_active_total{faculty="informatica"} 450
grades_submitted_total{semester="2024-1"} 12500
```

### Health Checks

```
GET /health/ready    - Application ready to serve traffic
GET /health/live     - Application is alive
GET /health/startup  - Startup completed
```

## 12.5 Accessibility (WCAG 2.1 AA)

- Contrast ratio minim 4.5:1
- Keyboard navigation complet
- Screen reader support
- Focus indicators vizibili
- Alt text pentru imagini
- ARIA labels pentru componente interactive

## 12.6 Localizare

- Limba principală: Română
- Format dată: DD.MM.YYYY
- Format oră: HH:mm (24h)
- Timezone: Europe/Bucharest
- Număr zecimal: virgulă (,)

---

# 13. User Stories & Acceptance Criteria

## 13.1 Epic: Gestiune Studenți

### US-001: Student vizualizează catalogul personal

**As a** student autentificat  
**I want to** vizualiza catalogul meu cu toate notele  
**So that** pot urmări progresul academic

**Acceptance Criteria:**
- [ ] Studentul vede lista tuturor cursurilor din semestrul curent
- [ ] Pentru fiecare curs se afișează: denumire, credite, tip, note
- [ ] Notele sunt afișate cu status (draft/approved/final)
- [ ] Studentul poate filtra pe semestru/an academic
- [ ] Media ponderată este calculată și afișată
- [ ] Studentul poate exporta catalogul în PDF

**Technical Notes:**
- Endpoint: GET /api/v1/grades/my
- Cache: 5 minute
- Realtime update la schimbare note

---

### US-002: Secretariat adaugă student nou

**As a** secretar  
**I want to** adăuga un student nou în sistem  
**So that** acesta să poată accesa platforma

**Acceptance Criteria:**
- [ ] Formular cu toate câmpurile obligatorii (nume, email, CNP, grupă)
- [ ] Validare email format @stud.rau.ro
- [ ] Validare CNP format și unicitate
- [ ] Generare automată număr matricol (format: AN-PROG-XXXX)
- [ ] Creare automată cont utilizator cu parolă temporară
- [ ] Trimitere email cu credențiale (sau afișare în UI pentru MVP)
- [ ] Audit log pentru acțiune

**Technical Notes:**
- Endpoint: POST /api/v1/students
- Transaction: user + student create atomic
- Trigger notification

---

### US-003: Profesor introduce note batch

**As a** profesor  
**I want to** introduce note pentru o întreagă grupă simultan  
**So that** economisesc timp la completarea catalogului

**Acceptance Criteria:**
- [ ] Tabel inline editing cu toți studenții din grupă
- [ ] Validare: note 1-10 integer
- [ ] Posibilitate de salvare draft (fără submit)
- [ ] Submit trimite toate notele către aprobare
- [ ] Feedback vizual pentru note salvate vs modificate
- [ ] Undo/Redo pentru modificări
- [ ] Confirmare înainte de submit final

**Technical Notes:**
- Endpoint: POST /api/v1/grades/batch
- Optimistic UI update
- Batch max 50 grades per request

---

## 13.2 Epic: Fluxuri Administrative

### US-010: Student trimite cerere adeverință

**As a** student  
**I want to** solicita o adeverință de student  
**So that** o pot folosi pentru diverse scopuri

**Acceptance Criteria:**
- [ ] Formular cu tipul adeverinței și scopul
- [ ] Confirmare submitere cu număr de înregistrare
- [ ] Notificare la schimbare status
- [ ] Download document când este gata
- [ ] Istoric cereri anterioare vizibil

---

### US-011: Secretariat procesează cerere

**As a** secretar  
**I want to** procesa cererile studenților  
**So that** aceștia să primească documentele solicitate

**Acceptance Criteria:**
- [ ] Listă cereri pending cu filtre (tip, dată, student)
- [ ] Detalii complete cerere la click
- [ ] Acțiuni: Approve, Reject, Request Info
- [ ] Generare automată document la aprobare
- [ ] Upload document în Storage
- [ ] Notificare automată student

---

## 13.3 Epic: Raportare

### US-020: Decan vizualizează dashboard facultate

**As a** decan  
**I want to** vedea un dashboard cu KPIs facultății  
**So that** pot lua decizii informate

**Acceptance Criteria:**
- [ ] Card-uri cu: studenți activi, rate promovabilitate, profesori
- [ ] Grafic trend promovabilitate pe ultimele 4 semestre
- [ ] Top 5 cursuri cu cele mai mici rate de promovare
- [ ] Comparație cu media universității
- [ ] Export raport PDF

---

# 14. RACI Matrix

## 14.1 Procese Academice

| Activitate | Student | Profesor | Decan | Rector | Secretariat | Admin |
|------------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| Înregistrare note | I | R | A | I | C | C |
| Aprobare note finale | I | C | R | A | I | I |
| Marcare prezențe | I | R | C | I | I | C |
| Contestare note | R | C | A | I | C | I |
| Exmatriculare | I | I | R | A | C | C |

## 14.2 Procese Administrative

| Activitate | Student | Profesor | Decan | Rector | Secretariat | Admin |
|------------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| Înregistrare student | I | I | I | I | R | A |
| Transfer grupă | C | I | A | I | R | C |
| Emitere adeverințe | C | I | I | I | R | A |
| Creare utilizator | I | I | I | I | C | R |
| Gestiune facultăți | I | I | C | A | I | R |

## 14.3 Procese Sistem

| Activitate | Student | Profesor | Decan | Rector | Secretariat | Admin |
|------------|:-------:|:--------:|:-----:|:------:|:-----------:|:-----:|
| Configurare sistem | I | I | I | A | I | R |
| Backup date | I | I | I | I | I | R |
| Monitorizare | I | I | I | A | I | R |
| Gestiune roluri | I | I | I | A | I | R |

**Legendă:**
- **R** = Responsible (execută)
- **A** = Accountable (aprobă/răspunde)
- **C** = Consulted (consultat)
- **I** = Informed (informat)

---

# 15. Structură Proiect C#

## 15.1 Solution Structure

```
UniversityManagement/
├── src/
│   ├── UniversityManagement.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── StudentsController.cs
│   │   │   ├── GradesController.cs
│   │   │   ├── AttendanceController.cs
│   │   │   ├── RequestsController.cs
│   │   │   ├── NotificationsController.cs
│   │   │   └── AdminController.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlerMiddleware.cs
│   │   │   ├── AuditMiddleware.cs
│   │   │   └── RateLimitingMiddleware.cs
│   │   ├── Filters/
│   │   │   ├── ValidateModelAttribute.cs
│   │   │   └── AuthorizeRoleAttribute.cs
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── ApplicationBuilderExtensions.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── UniversityManagement.Application/
│   │   ├── Interfaces/
│   │   │   ├── IStudentService.cs
│   │   │   ├── IGradeService.cs
│   │   │   └── ...
│   │   ├── Services/
│   │   │   ├── StudentService.cs
│   │   │   ├── GradeService.cs
│   │   │   ├── NotificationService.cs
│   │   │   └── ...
│   │   ├── DTOs/
│   │   │   ├── Requests/
│   │   │   │   ├── CreateStudentRequest.cs
│   │   │   │   ├── UpdateGradeRequest.cs
│   │   │   │   └── ...
│   │   │   ├── Responses/
│   │   │   │   ├── StudentResponse.cs
│   │   │   │   ├── GradeResponse.cs
│   │   │   │   └── ...
│   │   │   └── Common/
│   │   │       ├── PagedResponse.cs
│   │   │       └── ApiResponse.cs
│   │   ├── Validators/
│   │   │   ├── CreateStudentValidator.cs
│   │   │   └── ...
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   └── Exceptions/
│   │       ├── NotFoundException.cs
│   │       ├── ValidationException.cs
│   │       └── BusinessException.cs
│   │
│   ├── UniversityManagement.Domain/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Student.cs
│   │   │   ├── Professor.cs
│   │   │   ├── Faculty.cs
│   │   │   ├── Program.cs
│   │   │   ├── Series.cs
│   │   │   ├── Group.cs
│   │   │   ├── Course.cs
│   │   │   ├── CourseInstance.cs
│   │   │   ├── Grade.cs
│   │   │   ├── Attendance.cs
│   │   │   ├── Request.cs
│   │   │   ├── Document.cs
│   │   │   └── Notification.cs
│   │   ├── Enums/
│   │   │   ├── UserRole.cs
│   │   │   ├── StudentStatus.cs
│   │   │   ├── GradeStatus.cs
│   │   │   ├── AttendanceStatus.cs
│   │   │   └── RequestType.cs
│   │   ├── Events/
│   │   │   ├── GradeSubmittedEvent.cs
│   │   │   └── StudentCreatedEvent.cs
│   │   └── Interfaces/
│   │       ├── IEntity.cs
│   │       └── IAuditableEntity.cs
│   │
│   ├── UniversityManagement.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── SupabaseContext.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── StudentRepository.cs
│   │   │   │   ├── GradeRepository.cs
│   │   │   │   └── ...
│   │   │   └── Configurations/
│   │   │       └── EntityConfigurations.cs
│   │   ├── Services/
│   │   │   ├── SupabaseAuthService.cs
│   │   │   ├── SupabaseStorageService.cs
│   │   │   ├── SupabaseRealtimeService.cs
│   │   │   └── PdfGenerationService.cs
│   │   └── External/
│   │       └── EmailService.cs
│   │
│   ├── UniversityManagement.Shared/
│   │   ├── Constants/
│   │   │   ├── Roles.cs
│   │   │   └── ErrorCodes.cs
│   │   ├── Extensions/
│   │   │   └── StringExtensions.cs
│   │   └── Helpers/
│   │       ├── CnpValidator.cs
│   │       └── EnrollmentNumberGenerator.cs
│   │
│   └── UniversityManagement.Client/
│       ├── wwwroot/
│       ├── Pages/
│       │   ├── Auth/
│       │   │   ├── Login.razor
│       │   │   └── ForgotPassword.razor
│       │   ├── Student/
│       │   │   ├── Dashboard.razor
│       │   │   ├── Catalog.razor
│       │   │   └── Requests.razor
│       │   ├── Professor/
│       │   │   ├── Dashboard.razor
│       │   │   ├── Courses.razor
│       │   │   └── Grades.razor
│       │   ├── Admin/
│       │   │   ├── Dashboard.razor
│       │   │   ├── Users.razor
│       │   │   └── Faculties.razor
│       │   └── Shared/
│       │       ├── MainLayout.razor
│       │       └── NavMenu.razor
│       ├── Components/
│       │   ├── GradeTable.razor
│       │   ├── StudentForm.razor
│       │   ├── NotificationBell.razor
│       │   └── ConfirmDialog.razor
│       ├── Services/
│       │   ├── ApiClient.cs
│       │   ├── AuthStateProvider.cs
│       │   └── NotificationService.cs
│       ├── State/
│       │   ├── AppState.cs
│       │   └── Actions/
│       │       └── GradeActions.cs
│       ├── Program.cs
│       └── _Imports.razor
│
├── tests/
│   ├── UniversityManagement.UnitTests/
│   │   ├── Services/
│   │   │   ├── StudentServiceTests.cs
│   │   │   └── GradeServiceTests.cs
│   │   └── Validators/
│   │       └── CreateStudentValidatorTests.cs
│   │
│   ├── UniversityManagement.IntegrationTests/
│   │   ├── Controllers/
│   │   │   ├── StudentsControllerTests.cs
│   │   │   └── GradesControllerTests.cs
│   │   └── TestFixtures/
│   │       └── WebApplicationFactory.cs
│   │
│   └── UniversityManagement.E2ETests/
│       └── Playwright/
│           └── StudentFlowTests.cs
│
├── docker/
│   ├── Dockerfile.api
│   ├── Dockerfile.client
│   └── docker-compose.yml
│
├── scripts/
│   ├── seed-data.sql
│   └── migrate.sh
│
├── docs/
│   ├── api/
│   │   └── openapi.yaml
│   └── architecture/
│       └── diagrams/
│
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── cd.yml
│
├── UniversityManagement.sln
├── .gitignore
├── README.md
└── Directory.Build.props
```

## 15.2 Dependency Injection Setup

```csharp
// Program.cs - API
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Layer
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Infrastructure Layer
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();
builder.Services.AddScoped<IStorageService, SupabaseStorageService>();

// Supabase Client
builder.Services.AddSingleton<Supabase.Client>(sp =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    };
    return new Supabase.Client(
        builder.Configuration["Supabase:Url"],
        builder.Configuration["Supabase:Key"],
        options
    );
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentValidator>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Supabase:Url"];
        options.Audience = "authenticated";
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["AllowedOrigins"])
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware pipeline
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditMiddleware>();
app.MapControllers();

app.Run();
```

---

# 16. Strategie de Testare

## 16.1 Piramida de Testare

```
                    ┌───────────┐
                    │   E2E     │  5%
                    │  Tests    │
                    └─────┬─────┘
                          │
                   ┌──────┴──────┐
                   │ Integration │  25%
                   │   Tests     │
                   └──────┬──────┘
                          │
              ┌───────────┴───────────┐
              │      Unit Tests       │  70%
              │                       │
              └───────────────────────┘
```

## 16.2 Unit Tests

### Coverage Targets
- Services: 90%+
- Validators: 100%
- Helpers/Extensions: 100%
- Mappers: 80%+

### Example

```csharp
public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly StudentService _sut;

    public StudentServiceTests()
    {
        _repositoryMock = new Mock<IStudentRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new StudentService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateStudent_WithValidData_ReturnsStudentId()
    {
        // Arrange
        var request = new CreateStudentRequest
        {
            FirstName = "Ion",
            LastName = "Popescu",
            Email = "ion.popescu@stud.rau.ro"
        };
        var studentId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Student>()))
            .ReturnsAsync(studentId);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.Equal(studentId, result);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public async Task CreateStudent_WithDuplicateEmail_ThrowsException()
    {
        // Arrange
        var request = new CreateStudentRequest { Email = "existing@stud.rau.ro" };
        _repositoryMock.Setup(r => r.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => _sut.CreateAsync(request)
        );
    }
}
```

## 16.3 Integration Tests

```csharp
public class StudentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public StudentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace with test database
                services.AddScoped<IStudentRepository, InMemoryStudentRepository>();
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetStudents_AsSecretariat_ReturnsStudentsList()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", GetTestToken("secretariat"));

        // Act
        var response = await _client.GetAsync("/api/v1/students");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<List<StudentResponse>>>();
        Assert.NotNull(content);
        Assert.True(content.Success);
    }

    [Fact]
    public async Task GetStudents_AsStudent_ReturnsForbidden()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", GetTestToken("student"));

        // Act
        var response = await _client.GetAsync("/api/v1/students");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
```

## 16.4 E2E Tests (Playwright)

```csharp
[TestFixture]
public class StudentFlowTests : PageTest
{
    [Test]
    public async Task Student_CanViewGrades_AfterLogin()
    {
        // Navigate to login
        await Page.GotoAsync("https://localhost:5001/login");

        // Fill login form
        await Page.FillAsync("[data-testid=email]", "test.student@stud.rau.ro");
        await Page.FillAsync("[data-testid=password]", "TestPassword123!");
        await Page.ClickAsync("[data-testid=login-button]");

        // Wait for dashboard
        await Page.WaitForURLAsync("**/dashboard");

        // Navigate to catalog
        await Page.ClickAsync("[data-testid=nav-catalog]");
        await Page.WaitForSelectorAsync("[data-testid=grades-table]");

        // Verify grades are displayed
        var gradesCount = await Page.Locator("[data-testid=grade-row]").CountAsync();
        Assert.That(gradesCount, Is.GreaterThan(0));
    }
}
```

---

# 17. Deployment & DevOps

## 17.1 Environment Configuration

| Environment | URL | Database | Purpose |
|-------------|-----|----------|---------|
| Development | localhost:5001 | Supabase Local | Local development |
| Staging | staging.smu.rau.ro | Supabase Project (staging) | Testing before prod |
| Production | smu.rau.ro | Supabase Project (prod) | Live system |

## 17.2 CI/CD Pipeline

```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
      - name: Upload coverage
        uses: codecov/codecov-action@v3

  security:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Security scan
        run: dotnet list package --vulnerable

  deploy-staging:
    needs: [build, security]
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to staging
        run: |
          # Deploy commands
```

## 17.3 Docker Configuration

```dockerfile
# Dockerfile.api
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/UniversityManagement.API/UniversityManagement.API.csproj", "UniversityManagement.API/"]
COPY ["src/UniversityManagement.Application/UniversityManagement.Application.csproj", "UniversityManagement.Application/"]
COPY ["src/UniversityManagement.Domain/UniversityManagement.Domain.csproj", "UniversityManagement.Domain/"]
COPY ["src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj", "UniversityManagement.Infrastructure/"]
RUN dotnet restore "UniversityManagement.API/UniversityManagement.API.csproj"
COPY src/ .
WORKDIR "/src/UniversityManagement.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UniversityManagement.API.dll"]
```

```yaml
# docker-compose.yml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: docker/Dockerfile.api
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - Supabase__Url=${SUPABASE_URL}
      - Supabase__Key=${SUPABASE_KEY}
    depends_on:
      - redis

  client:
    build:
      context: .
      dockerfile: docker/Dockerfile.client
    ports:
      - "5001:80"
    depends_on:
      - api

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
```

---

# 18. Roadmap & Milestones

## 18.1 Phase 1: MVP (8 săptămâni)

### Sprint 1-2: Foundation
- [ ] Setup proiect și infrastructură
- [ ] Configurare Supabase (DB, Auth, Storage)
- [ ] Implementare autentificare și autorizare
- [ ] CRUD studenți basic
- [ ] Layout principal Blazor

### Sprint 3-4: Core Academic
- [ ] Modul Note (CRUD, batch)
- [ ] Modul Prezențe
- [ ] Dashboard student
- [ ] Dashboard profesor

### Sprint 5-6: Administrative
- [ ] Modul Cereri
- [ ] Modul Documente (generare PDF)
- [ ] Sistem Notificări in-app
- [ ] Rapoarte basic

### Sprint 7-8: Polish & Testing
- [ ] UI/UX refinement
- [ ] Integration testing
- [ ] Bug fixes
- [ ] Documentation
- [ ] Deployment staging

## 18.2 Phase 2: Enhanced Features (4 săptămâni)

- [ ] Workflow aprobare note cu Decan
- [ ] Sistem contestații
- [ ] Export Excel/PDF rapoarte
- [ ] Import bulk studenți
- [ ] Dashboard Decan/Rector
- [ ] Audit log viewer

## 18.3 Phase 3: Advanced (4 săptămâni)

- [ ] Realtime sync complet
- [ ] Modul Orar
- [ ] Email notifications
- [ ] Analytics avansat
- [ ] Mobile optimization
- [ ] Performance tuning

## 18.4 Future Considerations

- Integrare e-learning (Moodle)
- Aplicație mobilă nativă
- Chatbot support
- AI-based analytics
- Multi-tenant pentru alte universități

---

# 19. Anexe

## 19.1 Glossar

| Termen | Definiție |
|--------|-----------|
| Catalog | Lista note ale unui student/curs |
| Fișă Matricolă | Document oficial cu situația școlară |
| Număr Matricol | Identificator unic student (format: AN-PROG-XXXX) |
| Restanță | Sesiune de examinare suplimentară |
| Mărire | Examen pentru creșterea notei |
| Serie | Grupare de grupe în cadrul unui program |
| RLS | Row Level Security - politici acces Supabase |
| JWT | JSON Web Token - standard autentificare |

## 19.2 Referințe

- [MudBlazor Documentation](https://mudblazor.com/)
- [Supabase Documentation](https://supabase.com/docs)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Blazor WebAssembly](https://docs.microsoft.com/aspnet/core/blazor)

## 19.3 Change Log

| Versiune | Data | Modificări |
|----------|------|------------|
| 1.0 | 2024-01-01 | PRD inițial |
| 2.0 | 2024-01-15 | Revizuire completă: ERD, State Machines, API Design, RLS, Project Structure |

---

**Document Status:** ✅ Ready for Development  
**Last Updated:** 2024-01-15  
**Authors:** Echipa primariaTa❤️  
**Reviewers:** Prof. Andrei Luchici
