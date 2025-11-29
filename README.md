# 🎓 University Management System (SMU)

## About the Project

**University Management System (SMU)** is a comprehensive solution for digitizing university processes. The platform integrates academic, administrative, and operational management into a unified digital ecosystem.

### Key Objectives

- **Centralization** - All academic data in a single platform
- **Automation** - Reducing manual processes by 80%
- **Accessibility** - 24/7 access for all users
- **Security** - Data protection in compliance with GDPR

## 🛠️ Technology Stack

| Layer | Technology | Version |
|-------|------------|---------|
| **Frontend** | Blazor WebAssembly | .NET 10 |
| **UI Framework** | MudBlazor | 8.15.0 |
| **State Management** | Fluxor | 6.9.0 |
| **Backend** | ASP.NET Core Web API | .NET 10 |
| **Database** | PostgreSQL (Supabase) | 15+ |
| **Authentication** | Supabase Auth (JWT) | Latest |
| **Storage** | Supabase Storage | Latest |
| **Realtime** | Supabase Realtime | Latest |

## 📁 Project Structure

```
UniversityManagement/
├── 📂 src/
│   ├── 📂 UniversityManagement.API/          # ASP.NET Core Web API
│   ├── 📂 UniversityManagement.Application/  # Business Logic & Services
│   ├── 📂 UniversityManagement.Domain/       # Entities & Interfaces
│   ├── 📂 UniversityManagement.Infrastructure/ # Data Access & External Services
│   ├── 📂 UniversityManagement.Shared/       # Shared Utilities
│   └── 📂 UniversityManagement.Client/       # Blazor WASM Frontend
├── 📂 scripts/                               # DB Scripts & Utilities
├── 📂 development/                           # Development documentation (local only)
├── 📄 README.md                              # This file
├── 📄 LICENSE                                # MIT License
└── 📄 gitignore                              # Git ignore rules
```

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (v18+)
- [Docker](https://www.docker.com/) (optional)
- [Supabase](https://supabase.com/) Account

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/your-org/university-management.git
cd university-management
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Set up the database**
```bash
# Apply schema to Supabase
psql -h your-supabase-host -U postgres -d postgres -f scripts/schema.sql
```

4. **Run the application**
```bash
# Terminal 1 - API
cd src/UniversityManagement.API
dotnet run

# Terminal 2 - Client
cd src/UniversityManagement.Client
dotnet run
```

5. **Access the application**
- Client: `https://localhost:5001`
- API: `https://localhost:5000`
- Swagger: `https://localhost:5000/swagger`

## 📚 Documentation

Official documentation is currently being developed. For development-related documentation, please see the `development` folder.

## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for the full contribution guide.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [MudBlazor](https://mudblazor.com/) - UI Components
- [Supabase](https://supabase.com/) - Backend as a Service
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - WebAssembly Framework