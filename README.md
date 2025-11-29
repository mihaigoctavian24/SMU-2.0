<p align="center">
  <img src="docs/assets/logo.png" alt="SMU Logo" width="200"/>
</p>

<h1 align="center">🎓 Sistem de Management Universitar (SMU)</h1>

<p align="center">
  <strong>Platformă enterprise pentru administrarea completă a unei universități moderne</strong>
</p>

<p align="center">
  <a href="#-features">Features</a> •
  <a href="#-tech-stack">Tech Stack</a> •
  <a href="#-quick-start">Quick Start</a> •
  <a href="#-documentation">Documentation</a> •
  <a href="#-contributing">Contributing</a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8"/>
  <img src="https://img.shields.io/badge/Blazor-WASM-512BD4?style=for-the-badge&logo=blazor&logoColor=white" alt="Blazor"/>
  <img src="https://img.shields.io/badge/Supabase-3FCF8E?style=for-the-badge&logo=supabase&logoColor=white" alt="Supabase"/>
  <img src="https://img.shields.io/badge/MudBlazor-7B1FA2?style=for-the-badge" alt="MudBlazor"/>
</p>

<p align="center">
  <img src="https://img.shields.io/github/license/your-org/university-management?style=flat-square" alt="License"/>
  <img src="https://img.shields.io/github/stars/your-org/university-management?style=flat-square" alt="Stars"/>
  <img src="https://img.shields.io/github/issues/your-org/university-management?style=flat-square" alt="Issues"/>
  <img src="https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square" alt="PRs Welcome"/>
</p>

---

## 📋 Despre Proiect

**Sistemul de Management Universitar (SMU)** este o soluție completă pentru digitalizarea proceselor universitare. Platforma integrează managementul academic, administrativ și operațional într-un ecosistem digital unificat.

### 🎯 Obiective

- **Centralizare** - Toate datele academice într-o singură platformă
- **Automatizare** - Reducerea proceselor manuale cu 80%
- **Accesibilitate** - Acces 24/7 pentru toți utilizatorii
- **Securitate** - Protecția datelor conform GDPR

---

## ✨ Features

### Pentru Studenți
- 📊 Dashboard personalizat cu statistici academice
- 📚 Vizualizare catalog și note în timp real
- 📝 Sistem de cereri online (adeverințe, certificate)
- 🔔 Notificări instant pentru note noi
- 📅 Orar personalizat
- 📄 Descărcare documente (situații școlare, adeverințe)

### Pentru Profesori
- 📋 Gestiune cursuri și grupe
- ✏️ Introducere note (individual sau batch)
- ✅ Marcare prezențe în timp real
- 📊 Statistici și rapoarte per curs
- 📢 Sistem de anunțuri către studenți

### Pentru Secretariat
- 👥 CRUD complet studenți
- 📥 Import bulk din CSV/Excel
- 📄 Generare automată documente
- 📋 Procesare cereri studenți
- 🔍 Căutare și filtrare avansată

### Pentru Administrație (Decan/Rector)
- 📈 Dashboard-uri cu KPIs
- ✅ Workflow-uri de aprobare
- 📊 Rapoarte promovabilitate
- 🏛️ Gestiune structură universitate
- 📋 Audit trail complet

---

## 🛠️ Tech Stack

| Layer | Tehnologie | Versiune |
|-------|-----------|----------|
| **Frontend** | Blazor WebAssembly | .NET 8 |
| **UI Framework** | MudBlazor | 7.x |
| **Backend** | ASP.NET Core Web API | 8.0 |
| **Database** | PostgreSQL (Supabase) | 15+ |
| **Authentication** | Supabase Auth (JWT) | Latest |
| **Storage** | Supabase Storage | Latest |
| **Realtime** | Supabase Realtime | Latest |
| **Caching** | Redis (opțional) | 7.x |

---

## 🚀 Quick Start

### Cerințe

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18+)
- [Docker](https://www.docker.com/) (opțional)
- Cont [Supabase](https://supabase.com/)

### Instalare

1. **Clonează repository-ul**
```bash
git clone https://github.com/your-org/university-management.git
cd university-management
```

2. **Configurează variabilele de mediu**
```bash
cp .env.example .env
# Editează .env cu credențialele tale Supabase
```

3. **Restaurează dependențele**
```bash
dotnet restore
```

4. **Rulează migrările**
```bash
# Aplică schema în Supabase
psql -h your-supabase-host -U postgres -d postgres -f scripts/schema.sql
psql -h your-supabase-host -U postgres -d postgres -f scripts/seed-data.sql
```

5. **Pornește aplicația**
```bash
# Terminal 1 - API
cd src/UniversityManagement.API
dotnet run

# Terminal 2 - Client
cd src/UniversityManagement.Client
dotnet run
```

6. **Accesează aplicația**
- Client: `https://localhost:5001`
- API: `https://localhost:5000`
- Swagger: `https://localhost:5000/swagger`

### Cu Docker

```bash
docker-compose up -d
```

---

## 📁 Structura Proiectului

```
UniversityManagement/
├── 📂 src/
│   ├── 📂 UniversityManagement.API/          # ASP.NET Core Web API
│   ├── 📂 UniversityManagement.Application/  # Business Logic & Services
│   ├── 📂 UniversityManagement.Domain/       # Entities & Interfaces
│   ├── 📂 UniversityManagement.Infrastructure/ # Data Access & External Services
│   ├── 📂 UniversityManagement.Shared/       # Shared Utilities
│   └── 📂 UniversityManagement.Client/       # Blazor WASM Frontend
├── 📂 tests/
│   ├── 📂 UnitTests/
│   ├── 📂 IntegrationTests/
│   └── 📂 E2ETests/
├── 📂 docs/                                   # Documentation
├── 📂 scripts/                                # DB Scripts & Utilities
└── 📂 docker/                                 # Docker Configuration
```

👉 Vezi [PROJECT_STRUCTURE.md](docs/PROJECT_STRUCTURE.md) pentru detalii complete.

---

## 📚 Documentation

This folder contains all the official documentation for the University Management System project.

## Documentation Files

- [ARCHITECTURE.md](ARCHITECTURE.md) - System architecture documentation
- [CHANGELOG.md](CHANGELOG.md) - Version history and changes
- [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) - Code of conduct for contributors
- [LICENSE](LICENSE) - Project license information
- [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) - Detailed project structure
- [SECURITY.md](SECURITY.md) - Security policies and procedures

## Templates and Configuration

- [PULL_REQUEST_TEMPLATE.md](PULL_REQUEST_TEMPLATE.md) - Template for pull requests
- [bug_report.md](bug_report.md) - Template for bug reports
- [feature_request.md](feature_request.md) - Template for feature requests
- [ci.yml](ci.yml) - CI/CD configuration
- [editorconfig](editorconfig) - Editor configuration
- [env.example](env.example) - Example environment configuration

## Development Documentation

Development-related documentation is stored in the `development` folder which is excluded from version control. This includes:
- Implementation plans
- Progress tracking
- Analysis summaries
- Requirements documents

For development documentation, please refer to the `development` folder in the project root.

```
├── 📂 src/
│   ├── 📂 UniversityManagement.API/          # ASP.NET Core Web API
│   ├── 📂 UniversityManagement.Application/  # Business Logic & Services
│   ├── 📂 UniversityManagement.Domain/       # Entities & Interfaces
│   ├── 📂 UniversityManagement.Infrastructure/ # Data Access & External Services
│   ├── 📂 UniversityManagement.Shared/       # Shared Utilities
│   └── 📂 UniversityManagement.Client/       # Blazor WASM Frontend
├── 📂 tests/
│   ├── 📂 UnitTests/
│   ├── 📂 IntegrationTests/
│   └── 📂 E2ETests/
├── 📂 docs/                                   # Documentation
├── 📂 scripts/                                # DB Scripts & Utilities
└── 📂 docker/                                 # Docker Configuration
```

👉 Vezi [PROJECT_STRUCTURE.md](docs/PROJECT_STRUCTURE.md) pentru detalii complete.

---

## 📚 Documentation

| Document | Descriere |
|----------|-----------|
| [📐 ARCHITECTURE.md](docs/ARCHITECTURE.md) | Arhitectura sistemului |
| [📁 PROJECT_STRUCTURE.md](docs/PROJECT_STRUCTURE.md) | Structura detaliată a proiectului |
| [🔌 API.md](docs/API.md) | Documentație API endpoints |
| [🗄️ DATABASE.md](docs/DATABASE.md) | Schema bazei de date |
| [🚀 DEPLOYMENT.md](docs/DEPLOYMENT.md) | Ghid de deployment |
| [🧪 TESTING.md](docs/TESTING.md) | Strategie de testare |

For development documentation (implementation plans, progress tracking, etc.), please see the `development` folder which contains:
- [📋 IMPLEMENTATION_PLAN.md](development/IMPLEMENTATION_PLAN.md) - Plan detaliat de implementare
- [📊 PROGRESS_TRACKING.md](development/PROGRESS_TRACKING.md) - Urmărirea progresului implementării
- [📝 ANALYSIS_SUMMARY.md](development/ANALYSIS_SUMMARY.md) - Rezumatul analizei sistemului
- [📖 IMPLEMENTATION_GUIDE.md](development/IMPLEMENTATION_GUIDE.md) - Ghid pentru utilizarea documentației de implementare

---

## 🔐 Roluri și Permisiuni

| Rol | Descriere |
|-----|-----------|
| **Student** | Vizualizare note, prezențe, cereri |
| **Profesor** | Gestiune cursuri, note, prezențe |
| **Secretariat** | Administrare studenți, documente |
| **Decan** | Supervizare facultate, aprobări |
| **Rector** | Supervizare universitate |
| **Admin** | Acces complet sistem |

---

## 🧪 Testare

```bash
# Toate testele
dotnet test

# Cu coverage
dotnet test --collect:"XPlat Code Coverage"

# Doar unit tests
dotnet test --filter "Category=Unit"

# Doar integration tests
dotnet test --filter "Category=Integration"
```

---

## 📊 Roadmap

- [x] **Phase 1** - MVP (Studenți, Note, Prezențe)
- [ ] **Phase 2** - Features extinse (Aprobări, Contestații)
- [ ] **Phase 3** - Advanced (Realtime, Analytics)
- [ ] **Phase 4** - Mobile App

Vezi [ROADMAP.md](docs/ROADMAP.md) pentru detalii.

---

## 🤝 Contributing

Contribuțiile sunt binevenite! Vezi [CONTRIBUTING.md](CONTRIBUTING.md) pentru ghidul complet.

1. Fork repository-ul
2. Creează branch-ul tău (`git checkout -b feature/AmazingFeature`)
3. Commit modificările (`git commit -m 'Add some AmazingFeature'`)
4. Push la branch (`git push origin feature/AmazingFeature`)
5. Deschide un Pull Request

---

## 👥 Echipa

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/username1">
        <img src="https://github.com/username1.png" width="100px;" alt=""/>
        <br /><sub><b>Octavian Mihai</b></sub>
      </a>
      <br />
      <sub>Lead Developer & Technical Architect</sub>
    </td>
    <td align="center">
      <a href="https://github.com/username2">
        <img src="https://github.com/username2.png" width="100px;" alt=""/>
        <br /><sub><b>Bianca-Maria Abbasi</b></sub>
      </a>
      <br />
      <sub>Frontend Developer & UI/UX Designer</sub>
    </td>
  </tr>
</table>

**Coordonator:** Prof. Andrei Luchici  
**Curs:** Web Application Programming  
**Universitate:** Romanian-American University

---

## 📝 License

Acest proiect este licențiat sub [MIT License](LICENSE).

---

## 🙏 Acknowledgments

- [MudBlazor](https://mudblazor.com/) - UI Components
- [Supabase](https://supabase.com/) - Backend as a Service
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - WebAssembly Framework

---

<p align="center">
  Made with ❤️ by Echipa SMU @ Romanian-American University
</p>

<p align="center">
  <a href="#-sistem-de-management-universitar-smu">⬆️ Back to Top</a>
</p>
