#!/bin/bash
set -e

# Create src directory
mkdir -p src

# Create Solution
dotnet new sln -n UniversityManagement

# Create Projects
dotnet new webapi -n UniversityManagement.API -o src/UniversityManagement.API
dotnet new classlib -n UniversityManagement.Application -o src/UniversityManagement.Application
dotnet new classlib -n UniversityManagement.Domain -o src/UniversityManagement.Domain
dotnet new classlib -n UniversityManagement.Infrastructure -o src/UniversityManagement.Infrastructure
dotnet new classlib -n UniversityManagement.Shared -o src/UniversityManagement.Shared
dotnet new blazorwasm -n UniversityManagement.Client -o src/UniversityManagement.Client

# Add projects to solution
dotnet sln add src/UniversityManagement.API/UniversityManagement.API.csproj
dotnet sln add src/UniversityManagement.Application/UniversityManagement.Application.csproj
dotnet sln add src/UniversityManagement.Domain/UniversityManagement.Domain.csproj
dotnet sln add src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj
dotnet sln add src/UniversityManagement.Shared/UniversityManagement.Shared.csproj
dotnet sln add src/UniversityManagement.Client/UniversityManagement.Client.csproj

# Add References
# API -> Application, Infrastructure, Shared
dotnet add src/UniversityManagement.API/UniversityManagement.API.csproj reference src/UniversityManagement.Application/UniversityManagement.Application.csproj
dotnet add src/UniversityManagement.API/UniversityManagement.API.csproj reference src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj
dotnet add src/UniversityManagement.API/UniversityManagement.API.csproj reference src/UniversityManagement.Shared/UniversityManagement.Shared.csproj

# Infrastructure -> Application, Domain, Shared
dotnet add src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj reference src/UniversityManagement.Application/UniversityManagement.Application.csproj
dotnet add src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj reference src/UniversityManagement.Domain/UniversityManagement.Domain.csproj
dotnet add src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj reference src/UniversityManagement.Shared/UniversityManagement.Shared.csproj

# Application -> Domain, Shared
dotnet add src/UniversityManagement.Application/UniversityManagement.Application.csproj reference src/UniversityManagement.Domain/UniversityManagement.Domain.csproj
dotnet add src/UniversityManagement.Application/UniversityManagement.Application.csproj reference src/UniversityManagement.Shared/UniversityManagement.Shared.csproj

# Client -> Shared
dotnet add src/UniversityManagement.Client/UniversityManagement.Client.csproj reference src/UniversityManagement.Shared/UniversityManagement.Shared.csproj

# Add Packages (Basic ones needed for Auth and Supabase)
# Infrastructure needs Supabase
dotnet add src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj package Supabase
dotnet add src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj package Microsoft.Extensions.Configuration.Abstractions
dotnet add src/UniversityManagement.Infrastructure/UniversityManagement.Infrastructure.csproj package Microsoft.Extensions.Options.ConfigurationExtensions

# API needs JWT Bearer
dotnet add src/UniversityManagement.API/UniversityManagement.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer

# Client needs MudBlazor and Fluxor
dotnet add src/UniversityManagement.Client/UniversityManagement.Client.csproj package MudBlazor
dotnet add src/UniversityManagement.Client/UniversityManagement.Client.csproj package Fluxor.Blazor.Web
dotnet add src/UniversityManagement.Client/UniversityManagement.Client.csproj package Supabase

echo "Project structure created successfully!"
