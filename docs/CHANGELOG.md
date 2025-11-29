# Changelog

Toate modificările notabile ale acestui proiect vor fi documentate în acest fișier.

Formatul este bazat pe [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
și acest proiect aderă la [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Integrare Supabase Realtime pentru notificări instant
- Export rapoarte în format PDF

### Changed
- Îmbunătățire performanță încărcare liste studenți

### Fixed
- Corectare validare CNP pentru studenți străini

---

## [1.0.0] - 2024-XX-XX

### Added

#### Core Features
- **Modul Studenți**
  - CRUD complet pentru studenți
  - Import bulk din CSV
  - Generare automată număr matricol
  - Vizualizare catalog personal

- **Modul Profesori**
  - Gestiune profesori
  - Alocare cursuri la grupe
  - Dashboard profesor

- **Modul Note**
  - Introducere note (individual și batch)
  - Workflow aprobare note (Profesor → Decan)
  - Istoric modificări note
  - Sistem contestații

- **Modul Prezențe**
  - Marcare prezențe în timp real
  - Rapoarte absențe
  - Notificări la depășire prag

- **Modul Cereri & Documente**
  - Sistem cereri online
  - Generare automată adeverințe
  - Stocare documente în Supabase Storage

- **Modul Notificări**
  - Notificări in-app
  - Realtime via Supabase
  - Istoric notificări

- **Modul Admin**
  - Gestiune utilizatori și roluri
  - Configurare structură universitate
  - Audit log viewer

#### Infrastructure
- Autentificare JWT via Supabase Auth
- Row Level Security (RLS) pentru toate tabelele
- API versionată (v1)
- Logging structurat cu Serilog
- Health checks

#### UI/UX
- Design responsive cu MudBlazor
- Dark mode support
- Dashboards personalizate per rol
- Loading states și skeleton screens

### Security
- Implementare RBAC complet
- Rate limiting
- Input validation
- CORS configuration
- Security headers

---

## [0.1.0] - 2024-XX-XX (Alpha)

### Added
- Setup inițial proiect
- Structură Clean Architecture
- Configurare Supabase
- Autentificare de bază
- CRUD studenți (MVP)
- Layout principal Blazor

---

## Tipuri de Modificări

- **Added** - pentru funcționalități noi
- **Changed** - pentru modificări în funcționalitățile existente
- **Deprecated** - pentru funcționalități care vor fi eliminate în viitor
- **Removed** - pentru funcționalități eliminate
- **Fixed** - pentru orice bug fixes
- **Security** - în caz de vulnerabilități

[Unreleased]: https://github.com/your-org/university-management/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/your-org/university-management/compare/v0.1.0...v1.0.0
[0.1.0]: https://github.com/your-org/university-management/releases/tag/v0.1.0
