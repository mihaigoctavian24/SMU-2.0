using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Validators;
using UniversityManagement.Infrastructure.Repositories;
using UniversityManagement.Infrastructure.Services;
using UniversityManagement.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Add global exception filter
    options.Filters.Add<GlobalExceptionFilter>();
    // Add validation filter
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Supabase Client
builder.Services.AddScoped<Supabase.Client>(sp =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    };
    return new Supabase.Client(
        builder.Configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is required"),
        builder.Configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is required"),
        options
    );
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// Services
builder.Services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();

// Validators
builder.Services.AddScoped<IValidator<CreateStudentRequest>, CreateStudentValidator>();
builder.Services.AddScoped<IValidator<SignInRequest>, SignInRequestValidator>();
builder.Services.AddScoped<IValidator<SignUpRequest>, SignUpRequestValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Supabase:Url"];
        options.Audience = "authenticated";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{builder.Configuration["Supabase:Url"]}/auth/v1",
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Supabase:ServiceKey"] ?? ""))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(';') ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();