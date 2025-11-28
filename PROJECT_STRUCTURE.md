# 📁 Structura Proiectului

## Cuprins

- [Overview](#overview)
- [Solution Structure](#solution-structure)
- [Proiecte Detaliate](#proiecte-detaliate)
- [Convenții de Denumire](#convenții-de-denumire)
- [Organizarea Fișierelor](#organizarea-fișierelor)

---

## Overview

Proiectul utilizează **Clean Architecture** cu separare clară pe layere. Fiecare layer este un proiect .NET separat pentru a impune limitele arhitecturale la compile-time.

```
┌─────────────────────────────────────────────────────────────┐
│                    UniversityManagement                      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                    src/                              │    │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │    │
│  │  │    API      │  │ Application │  │   Domain    │  │    │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  │    │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │    │
│  │  │Infrastructure│  │   Shared   │  │   Client    │  │    │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                              │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                   tests/                             │    │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │    │
│  │  │ UnitTests   │  │ Integration │  │   E2E       │  │    │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Solution Structure

```
UniversityManagement/
│
├── 📄 UniversityManagement.sln              # Solution file
├── 📄 Directory.Build.props                 # Shared MSBuild properties
├── 📄 Directory.Packages.props              # Central package management
├── 📄 .editorconfig                         # Code style rules
├── 📄 .gitignore                            # Git ignore rules
├── 📄 .gitattributes                        # Git attributes
├── 📄 README.md                             # Project overview
├── 📄 LICENSE                               # MIT License
├── 📄 CONTRIBUTING.md                       # Contribution guide
├── 📄 CHANGELOG.md                          # Version history
├── 📄 SECURITY.md                           # Security policy
│
├── 📂 src/                                  # Source code
│   ├── 📂 UniversityManagement.API/
│   ├── 📂 UniversityManagement.Application/
│   ├── 📂 UniversityManagement.Domain/
│   ├── 📂 UniversityManagement.Infrastructure/
│   ├── 📂 UniversityManagement.Shared/
│   └── 📂 UniversityManagement.Client/
│
├── 📂 tests/                                # Test projects
│   ├── 📂 UniversityManagement.UnitTests/
│   ├── 📂 UniversityManagement.IntegrationTests/
│   └── 📂 UniversityManagement.E2ETests/
│
├── 📂 docs/                                 # Documentation
│   ├── 📂 api/                              # API documentation
│   ├── 📂 architecture/                     # Architecture docs
│   ├── 📂 guides/                           # User guides
│   └── 📂 assets/                           # Images, diagrams
│
├── 📂 scripts/                              # Utility scripts
│   ├── 📄 schema.sql                        # Database schema
│   ├── 📄 seed-data.sql                     # Test data
│   ├── 📄 migrate.sh                        # Migration script
│   └── 📄 setup-local.sh                    # Local setup script
│
├── 📂 docker/                               # Docker configuration
│   ├── 📄 Dockerfile.api                    # API Dockerfile
│   ├── 📄 Dockerfile.client                 # Client Dockerfile
│   └── 📄 docker-compose.yml                # Compose file
│
└── 📂 .github/                              # GitHub configuration
    ├── 📂 workflows/                        # CI/CD pipelines
    │   ├── 📄 ci.yml
    │   └── 📄 cd.yml
    ├── 📂 ISSUE_TEMPLATE/                   # Issue templates
    └── 📄 PULL_REQUEST_TEMPLATE.md          # PR template
```

---

## Proiecte Detaliate

### 1. UniversityManagement.API

**Responsabilitate:** Entry point pentru HTTP requests, controllers, middleware.

```
UniversityManagement.API/
│
├── 📄 Program.cs                            # Application entry point
├── 📄 appsettings.json                      # Configuration
├── 📄 appsettings.Development.json          # Dev configuration
├── 📄 appsettings.Production.json           # Prod configuration
├── 📄 UniversityManagement.API.csproj       # Project file
│
├── 📂 Controllers/                          # API Controllers
│   ├── 📄 BaseController.cs                 # Base controller with common logic
│   ├── 📄 AuthController.cs                 # Authentication endpoints
│   ├── 📄 StudentsController.cs             # Student CRUD
│   ├── 📄 ProfessorsController.cs           # Professor CRUD
│   ├── 📄 GradesController.cs               # Grade management
│   ├── 📄 AttendanceController.cs           # Attendance management
│   ├── 📄 CoursesController.cs              # Course management
│   ├── 📄 RequestsController.cs             # Student requests
│   ├── 📄 DocumentsController.cs            # Document generation
│   ├── 📄 NotificationsController.cs        # Notifications
│   ├── 📄 ReportsController.cs              # Reporting endpoints
│   └── 📄 AdminController.cs                # Admin operations
│
├── 📂 Middleware/                           # Custom middleware
│   ├── 📄 ExceptionHandlerMiddleware.cs     # Global exception handler
│   ├── 📄 AuditMiddleware.cs                # Request/Response logging
│   ├── 📄 RateLimitingMiddleware.cs         # Rate limiting
│   ├── 📄 CorrelationIdMiddleware.cs        # Request correlation
│   └── 📄 PerformanceMiddleware.cs          # Performance logging
│
├── 📂 Filters/                              # Action filters
│   ├── 📄 ValidateModelAttribute.cs         # Model validation
│   ├── 📄 AuthorizeRoleAttribute.cs         # Role authorization
│   └── 📄 AuditActionAttribute.cs           # Audit specific actions
│
├── 📂 Extensions/                           # Extension methods
│   ├── 📄 ServiceCollectionExtensions.cs    # DI registration
│   ├── 📄 ApplicationBuilderExtensions.cs   # Middleware pipeline
│   ├── 📄 AuthenticationExtensions.cs       # Auth configuration
│   └── 📄 SwaggerExtensions.cs              # Swagger configuration
│
├── 📂 Configuration/                        # Strongly-typed config
│   ├── 📄 SupabaseSettings.cs
│   ├── 📄 JwtSettings.cs
│   └── 📄 CorsSettings.cs
│
└── 📂 Properties/
    └── 📄 launchSettings.json               # Debug configuration
```

### 2. UniversityManagement.Application

**Responsabilitate:** Business logic, services, validators, DTOs.

```
UniversityManagement.Application/
│
├── 📄 UniversityManagement.Application.csproj
├── 📄 DependencyInjection.cs                # Service registration
│
├── 📂 Interfaces/                           # Service interfaces
│   ├── 📄 IStudentService.cs
│   ├── 📄 IProfessorService.cs
│   ├── 📄 IGradeService.cs
│   ├── 📄 IAttendanceService.cs
│   ├── 📄 ICourseService.cs
│   ├── 📄 IRequestService.cs
│   ├── 📄 IDocumentService.cs
│   ├── 📄 INotificationService.cs
│   ├── 📄 IReportService.cs
│   └── 📄 IAuthService.cs
│
├── 📂 Services/                             # Service implementations
│   ├── 📄 StudentService.cs
│   ├── 📄 ProfessorService.cs
│   ├── 📄 GradeService.cs
│   ├── 📄 AttendanceService.cs
│   ├── 📄 CourseService.cs
│   ├── 📄 RequestService.cs
│   ├── 📄 DocumentService.cs
│   ├── 📄 NotificationService.cs
│   ├── 📄 ReportService.cs
│   └── 📄 AuthService.cs
│
├── 📂 DTOs/                                 # Data Transfer Objects
│   │
│   ├── 📂 Requests/                         # Input DTOs
│   │   ├── 📂 Students/
│   │   │   ├── 📄 CreateStudentRequest.cs
│   │   │   ├── 📄 UpdateStudentRequest.cs
│   │   │   └── 📄 TransferStudentRequest.cs
│   │   ├── 📂 Grades/
│   │   │   ├── 📄 CreateGradeRequest.cs
│   │   │   ├── 📄 BatchGradeRequest.cs
│   │   │   ├── 📄 ApproveGradeRequest.cs
│   │   │   └── 📄 ContestGradeRequest.cs
│   │   ├── 📂 Attendance/
│   │   │   ├── 📄 MarkAttendanceRequest.cs
│   │   │   └── 📄 BatchAttendanceRequest.cs
│   │   ├── 📂 Requests/
│   │   │   ├── 📄 CreateRequestRequest.cs
│   │   │   └── 📄 ProcessRequestRequest.cs
│   │   └── 📂 Auth/
│   │       ├── 📄 LoginRequest.cs
│   │       ├── 📄 RefreshTokenRequest.cs
│   │       └── 📄 ChangePasswordRequest.cs
│   │
│   ├── 📂 Responses/                        # Output DTOs
│   │   ├── 📂 Students/
│   │   │   ├── 📄 StudentResponse.cs
│   │   │   ├── 📄 StudentDetailResponse.cs
│   │   │   └── 📄 StudentListItemResponse.cs
│   │   ├── 📂 Grades/
│   │   │   ├── 📄 GradeResponse.cs
│   │   │   ├── 📄 GradeHistoryResponse.cs
│   │   │   └── 📄 CatalogResponse.cs
│   │   ├── 📂 Reports/
│   │   │   ├── 📄 DashboardResponse.cs
│   │   │   └── 📄 PromovabilityResponse.cs
│   │   └── 📂 Auth/
│   │       ├── 📄 LoginResponse.cs
│   │       └── 📄 UserInfoResponse.cs
│   │
│   └── 📂 Common/                           # Shared DTOs
│       ├── 📄 ApiResponse.cs                # Standard API response
│       ├── 📄 PagedResponse.cs              # Paginated response
│       ├── 📄 ErrorResponse.cs              # Error details
│       └── 📄 PaginationRequest.cs          # Pagination params
│
├── 📂 Validators/                           # FluentValidation validators
│   ├── 📂 Students/
│   │   ├── 📄 CreateStudentValidator.cs
│   │   └── 📄 UpdateStudentValidator.cs
│   ├── 📂 Grades/
│   │   ├── 📄 CreateGradeValidator.cs
│   │   └── 📄 BatchGradeValidator.cs
│   └── 📂 Common/
│       └── 📄 PaginationValidator.cs
│
├── 📂 Mappings/                             # AutoMapper profiles
│   ├── 📄 StudentMappingProfile.cs
│   ├── 📄 GradeMappingProfile.cs
│   ├── 📄 CourseMappingProfile.cs
│   └── 📄 MappingProfile.cs                 # Aggregated profile
│
├── 📂 Exceptions/                           # Custom exceptions
│   ├── 📄 BaseException.cs
│   ├── 📄 NotFoundException.cs
│   ├── 📄 ValidationException.cs
│   ├── 📄 BusinessException.cs
│   ├── 📄 ForbiddenException.cs
│   └── 📄 ConflictException.cs
│
└── 📂 Behaviors/                            # Pipeline behaviors
    ├── 📄 ValidationBehavior.cs             # Auto-validation
    ├── 📄 LoggingBehavior.cs                # Request logging
    └── 📄 PerformanceBehavior.cs            # Performance tracking
```

### 3. UniversityManagement.Domain

**Responsabilitate:** Entități, enums, interfețe repository, domain events.

```
UniversityManagement.Domain/
│
├── 📄 UniversityManagement.Domain.csproj
│
├── 📂 Entities/                             # Domain entities
│   │
│   ├── 📂 Identity/
│   │   └── 📄 User.cs                       # User entity
│   │
│   ├── 📂 Academic/
│   │   ├── 📄 Faculty.cs
│   │   ├── 📄 Program.cs
│   │   ├── 📄 Series.cs
│   │   ├── 📄 Group.cs
│   │   ├── 📄 Course.cs
│   │   ├── 📄 CourseInstance.cs
│   │   └── 📄 AcademicYear.cs
│   │
│   ├── 📂 People/
│   │   ├── 📄 Student.cs
│   │   └── 📄 Professor.cs
│   │
│   ├── 📂 Grading/
│   │   ├── 📄 Grade.cs
│   │   ├── 📄 GradeHistory.cs
│   │   ├── 📄 GradeContest.cs
│   │   └── 📄 Enrollment.cs
│   │
│   ├── 📂 Attendance/
│   │   └── 📄 Attendance.cs
│   │
│   ├── 📂 Administrative/
│   │   ├── 📄 Request.cs
│   │   ├── 📄 Document.cs
│   │   └── 📄 Notification.cs
│   │
│   └── 📂 System/
│       ├── 📄 AuditLog.cs
│       └── 📄 SystemSetting.cs
│
├── 📂 Enums/                                # Domain enumerations
│   ├── 📄 UserRole.cs
│   ├── 📄 StudentStatus.cs
│   ├── 📄 GradeStatus.cs
│   ├── 📄 GradeType.cs
│   ├── 📄 GradingPeriod.cs
│   ├── 📄 AttendanceStatus.cs
│   ├── 📄 RequestType.cs
│   ├── 📄 RequestStatus.cs
│   ├── 📄 DocumentType.cs
│   ├── 📄 NotificationType.cs
│   └── 📄 CourseType.cs
│
├── 📂 ValueObjects/                         # Value objects
│   ├── 📄 Email.cs
│   ├── 📄 Cnp.cs
│   ├── 📄 EnrollmentNumber.cs
│   ├── 📄 PhoneNumber.cs
│   └── 📄 DateRange.cs
│
├── 📂 Interfaces/                           # Repository interfaces
│   │
│   ├── 📂 Repositories/
│   │   ├── 📄 IRepository.cs                # Generic repository
│   │   ├── 📄 IStudentRepository.cs
│   │   ├── 📄 IProfessorRepository.cs
│   │   ├── 📄 IGradeRepository.cs
│   │   ├── 📄 IAttendanceRepository.cs
│   │   ├── 📄 ICourseRepository.cs
│   │   ├── 📄 IRequestRepository.cs
│   │   └── 📄 INotificationRepository.cs
│   │
│   └── 📂 Common/
│       ├── 📄 IEntity.cs                    # Base entity interface
│       ├── 📄 IAuditableEntity.cs           # Auditable entity
│       ├── 📄 ISoftDeletable.cs             # Soft delete support
│       └── 📄 IUnitOfWork.cs                # Unit of work
│
├── 📂 Events/                               # Domain events
│   ├── 📄 IDomainEvent.cs                   # Event interface
│   ├── 📄 StudentCreatedEvent.cs
│   ├── 📄 GradeSubmittedEvent.cs
│   ├── 📄 GradeApprovedEvent.cs
│   ├── 📄 RequestCreatedEvent.cs
│   └── 📄 AttendanceMarkedEvent.cs
│
├── 📂 Specifications/                       # Query specifications
│   ├── 📄 ISpecification.cs
│   ├── 📄 StudentsByGroupSpec.cs
│   ├── 📄 GradesByStatusSpec.cs
│   └── 📄 PendingRequestsSpec.cs
│
└── 📂 Common/                               # Shared domain logic
    ├── 📄 BaseEntity.cs                     # Base entity class
    ├── 📄 AuditableEntity.cs                # Auditable base
    └── 📄 Result.cs                         # Result pattern
```

### 4. UniversityManagement.Infrastructure

**Responsabilitate:** Implementări repository, servicii externe, Supabase integration.

```
UniversityManagement.Infrastructure/
│
├── 📄 UniversityManagement.Infrastructure.csproj
├── 📄 DependencyInjection.cs
│
├── 📂 Persistence/                          # Data access
│   │
│   ├── 📄 SupabaseContext.cs                # Supabase client wrapper
│   ├── 📄 UnitOfWork.cs                     # Unit of work impl
│   │
│   ├── 📂 Repositories/                     # Repository implementations
│   │   ├── 📄 BaseRepository.cs             # Generic repository
│   │   ├── 📄 StudentRepository.cs
│   │   ├── 📄 ProfessorRepository.cs
│   │   ├── 📄 GradeRepository.cs
│   │   ├── 📄 AttendanceRepository.cs
│   │   ├── 📄 CourseRepository.cs
│   │   ├── 📄 RequestRepository.cs
│   │   └── 📄 NotificationRepository.cs
│   │
│   ├── 📂 Configurations/                   # Entity configurations
│   │   └── 📄 EntityMappings.cs
│   │
│   └── 📂 Extensions/
│       └── 📄 QueryExtensions.cs            # Query helpers
│
├── 📂 Services/                             # External services
│   │
│   ├── 📂 Supabase/
│   │   ├── 📄 SupabaseAuthService.cs        # Auth operations
│   │   ├── 📄 SupabaseStorageService.cs     # File storage
│   │   └── 📄 SupabaseRealtimeService.cs    # Realtime subscriptions
│   │
│   ├── 📂 Documents/
│   │   ├── 📄 IPdfGenerator.cs
│   │   ├── 📄 PdfGeneratorService.cs        # PDF generation
│   │   └── 📂 Templates/
│   │       ├── 📄 CertificateTemplate.cs
│   │       └── 📄 TranscriptTemplate.cs
│   │
│   ├── 📂 Email/
│   │   ├── 📄 IEmailService.cs
│   │   ├── 📄 EmailService.cs               # Email sending
│   │   └── 📂 Templates/
│   │       ├── 📄 WelcomeEmail.html
│   │       └── 📄 GradeNotification.html
│   │
│   └── 📂 Caching/
│       ├── 📄 ICacheService.cs
│       ├── 📄 RedisCacheService.cs
│       └── 📄 InMemoryCacheService.cs
│
├── 📂 Logging/                              # Logging infrastructure
│   ├── 📄 SerilogConfiguration.cs
│   └── 📄 ApplicationInsightsConfiguration.cs
│
└── 📂 BackgroundJobs/                       # Background tasks
    ├── 📄 IBackgroundJobService.cs
    ├── 📄 NotificationCleanupJob.cs
    └── 📄 AuditArchiveJob.cs
```

### 5. UniversityManagement.Shared

**Responsabilitate:** Constante, helpers, extensii folosite în toate proiectele.

```
UniversityManagement.Shared/
│
├── 📄 UniversityManagement.Shared.csproj
│
├── 📂 Constants/                            # Application constants
│   ├── 📄 Roles.cs                          # Role names
│   ├── 📄 ErrorCodes.cs                     # Error code constants
│   ├── 📄 CacheKeys.cs                      # Cache key patterns
│   ├── 📄 ValidationMessages.cs             # Validation messages
│   └── 📄 RegexPatterns.cs                  # Regex patterns
│
├── 📂 Extensions/                           # Extension methods
│   ├── 📄 StringExtensions.cs
│   ├── 📄 DateTimeExtensions.cs
│   ├── 📄 EnumExtensions.cs
│   ├── 📄 CollectionExtensions.cs
│   └── 📄 ClaimsPrincipalExtensions.cs
│
├── 📂 Helpers/                              # Utility classes
│   ├── 📄 CnpValidator.cs                   # CNP validation
│   ├── 📄 EmailValidator.cs                 # Email validation
│   ├── 📄 EnrollmentNumberGenerator.cs      # Nr. matricol generator
│   ├── 📄 SlugGenerator.cs                  # URL slug generator
│   └── 📄 PasswordGenerator.cs              # Password generator
│
├── 📂 Guards/                               # Guard clauses
│   ├── 📄 Guard.cs
│   └── 📄 GuardExtensions.cs
│
└── 📂 Models/                               # Shared models
    ├── 📄 PagedList.cs                      # Paged list
    └── 📄 SortingOptions.cs                 # Sorting options
```

### 6. UniversityManagement.Client

**Responsabilitate:** Blazor WebAssembly frontend.

```
UniversityManagement.Client/
│
├── 📄 UniversityManagement.Client.csproj
├── 📄 Program.cs                            # Blazor entry point
├── 📄 _Imports.razor                        # Global imports
├── 📄 App.razor                             # Root component
├── 📄 MainLayout.razor                      # Main layout
│
├── 📂 wwwroot/                              # Static files
│   ├── 📄 index.html                        # Host page
│   ├── 📂 css/
│   │   └── 📄 app.css                       # Custom styles
│   ├── 📂 images/
│   │   └── 📄 logo.png
│   └── 📂 js/
│       └── 📄 interop.js                    # JS interop
│
├── 📂 Pages/                                # Page components
│   │
│   ├── 📂 Auth/
│   │   ├── 📄 Login.razor
│   │   ├── 📄 ForgotPassword.razor
│   │   └── 📄 ResetPassword.razor
│   │
│   ├── 📂 Student/
│   │   ├── 📄 Dashboard.razor
│   │   ├── 📄 Catalog.razor
│   │   ├── 📄 Attendance.razor
│   │   ├── 📄 Requests.razor
│   │   ├── 📄 Schedule.razor
│   │   └── 📄 Profile.razor
│   │
│   ├── 📂 Professor/
│   │   ├── 📄 Dashboard.razor
│   │   ├── 📄 Courses.razor
│   │   ├── 📄 CourseDetail.razor
│   │   ├── 📄 Grades.razor
│   │   └── 📄 Attendance.razor
│   │
│   ├── 📂 Secretariat/
│   │   ├── 📄 Dashboard.razor
│   │   ├── 📄 Students.razor
│   │   ├── 📄 StudentDetail.razor
│   │   ├── 📄 Requests.razor
│   │   └── 📄 Documents.razor
│   │
│   ├── 📂 Dean/
│   │   ├── 📄 Dashboard.razor
│   │   ├── 📄 Approvals.razor
│   │   ├── 📄 Reports.razor
│   │   └── 📄 Faculty.razor
│   │
│   ├── 📂 Admin/
│   │   ├── 📄 Dashboard.razor
│   │   ├── 📄 Users.razor
│   │   ├── 📄 Faculties.razor
│   │   ├── 📄 Programs.razor
│   │   ├── 📄 Settings.razor
│   │   └── 📄 AuditLogs.razor
│   │
│   └── 📂 Shared/
│       ├── 📄 NotFound.razor
│       └── 📄 Error.razor
│
├── 📂 Components/                           # Reusable components
│   │
│   ├── 📂 Layout/
│   │   ├── 📄 MainLayout.razor
│   │   ├── 📄 NavMenu.razor
│   │   ├── 📄 AppBar.razor
│   │   └── 📄 Footer.razor
│   │
│   ├── 📂 Shared/
│   │   ├── 📄 LoadingSpinner.razor
│   │   ├── 📄 ConfirmDialog.razor
│   │   ├── 📄 ErrorBoundary.razor
│   │   ├── 📄 Breadcrumb.razor
│   │   └── 📄 EmptyState.razor
│   │
│   ├── 📂 Data/
│   │   ├── 📄 DataGrid.razor               # Generic data grid
│   │   ├── 📄 Pagination.razor
│   │   └── 📄 SearchBox.razor
│   │
│   ├── 📂 Forms/
│   │   ├── 📄 StudentForm.razor
│   │   ├── 📄 GradeForm.razor
│   │   ├── 📄 RequestForm.razor
│   │   └── 📄 FormField.razor
│   │
│   ├── 📂 Cards/
│   │   ├── 📄 StatsCard.razor
│   │   ├── 📄 StudentCard.razor
│   │   └── 📄 CourseCard.razor
│   │
│   └── 📂 Notifications/
│       ├── 📄 NotificationBell.razor
│       ├── 📄 NotificationList.razor
│       └── 📄 Toast.razor
│
├── 📂 Services/                             # Client-side services
│   ├── 📄 ApiClient.cs                      # HTTP client wrapper
│   ├── 📄 AuthService.cs                    # Auth operations
│   ├── 📄 StudentService.cs
│   ├── 📄 GradeService.cs
│   ├── 📄 NotificationService.cs
│   └── 📄 RealtimeService.cs                # Supabase realtime
│
├── 📂 State/                                # State management
│   ├── 📄 AppState.cs                       # Global state
│   ├── 📄 AuthState.cs                      # Auth state
│   │
│   ├── 📂 Features/
│   │   ├── 📂 Students/
│   │   │   ├── 📄 StudentState.cs
│   │   │   ├── 📄 StudentActions.cs
│   │   │   └── 📄 StudentReducers.cs
│   │   └── 📂 Grades/
│   │       ├── 📄 GradeState.cs
│   │       ├── 📄 GradeActions.cs
│   │       └── 📄 GradeReducers.cs
│   │
│   └── 📂 Effects/
│       ├── 📄 StudentEffects.cs
│       └── 📄 GradeEffects.cs
│
├── 📂 Auth/                                 # Authentication
│   ├── 📄 CustomAuthStateProvider.cs
│   └── 📄 AuthorizationMessageHandler.cs
│
├── 📂 Helpers/                              # Client helpers
│   ├── 📄 LocalStorageService.cs
│   └── 📄 NavigationService.cs
│
└── 📂 Models/                               # Client-specific models
    └── 📄 MenuItem.cs
```

---

## Convenții de Denumire

### Fișiere și Directoare

| Element | Convenție | Exemplu |
|---------|-----------|---------|
| Clase | PascalCase | `StudentService.cs` |
| Interfețe | I + PascalCase | `IStudentRepository.cs` |
| DTOs Request | *Request | `CreateStudentRequest.cs` |
| DTOs Response | *Response | `StudentResponse.cs` |
| Validators | *Validator | `CreateStudentValidator.cs` |
| Controllers | *Controller | `StudentsController.cs` |
| Razor Pages | PascalCase | `Dashboard.razor` |
| Componente | PascalCase | `StudentForm.razor` |

### Namespace-uri

```csharp
UniversityManagement.API.Controllers
UniversityManagement.Application.Services
UniversityManagement.Application.DTOs.Requests
UniversityManagement.Application.DTOs.Responses
UniversityManagement.Domain.Entities
UniversityManagement.Domain.Enums
UniversityManagement.Infrastructure.Persistence
UniversityManagement.Client.Pages.Student
UniversityManagement.Client.Components.Shared
```

---

## Organizarea Fișierelor

### Principii

1. **Feature-based** pentru componente UI (Pages, Components)
2. **Type-based** pentru business logic (Services, Repositories)
3. **Max 10-15 fișiere** per folder
4. **Subfoldere** pentru categorii când depășește limita

### Exemplu Organizare DTOs

```
DTOs/
├── Requests/           # Input DTOs (de la client)
│   ├── Students/       # Per domeniu
│   └── Grades/
├── Responses/          # Output DTOs (către client)
│   ├── Students/
│   └── Grades/
└── Common/             # DTOs partajate
```

---

*Ultima actualizare: Ianuarie 2024*
