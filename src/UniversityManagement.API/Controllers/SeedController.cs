using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;
using Supabase;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public SeedController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        var url = _configuration["Supabase:Url"];
        var serviceKey = _configuration["Supabase:ServiceKey"];
        var anonKey = _configuration["Supabase:Key"];

        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };
        
        // Client for DB operations (Service Key - Bypasses RLS)
        var _dbClient = new Client(url, serviceKey, options);
        await _dbClient.InitializeAsync();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceKey);
        httpClient.DefaultRequestHeaders.Add("apikey", serviceKey);

        var users = new List<(string Email, string Password, UserRole Role, string FirstName, string LastName)>
        {
            ("mihai.g.octavian24@stud.rau.ro", "F32e891_!!_Admin", UserRole.Admin, "Mihai", "Octavian"),
            ("student.test@stud.rau.ro", "F32e891_!!_Admin", UserRole.Student, "Test", "Student"),
            ("professor.test@stud.rau.ro", "F32e891_!!_Admin", UserRole.Professor, "Test", "Professor"),
            ("secretary.test@stud.rau.ro", "F32e891_!!_Admin", UserRole.Secretariat, "Test", "Secretary")
        };

        var results = new List<string>();

        // 0. Fetch existing users to avoid 422
        var existingUsers = new Dictionary<string, string>(); // Email -> Id
        try
        {
            var listResponse = await httpClient.GetAsync($"{url}/auth/v1/admin/users");
            if (listResponse.IsSuccessStatusCode)
            {
                var listContent = await listResponse.Content.ReadAsStringAsync();
                var listJson = Newtonsoft.Json.Linq.JObject.Parse(listContent);
                var usersArray = listJson["users"] as Newtonsoft.Json.Linq.JArray;
                if (usersArray != null)
                {
                    foreach (var u in usersArray)
                    {
                        var email = u["email"]?.ToString();
                        var id = u["id"]?.ToString();
                        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(id))
                        {
                            existingUsers[email] = id;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            results.Add($"Failed to list users: {ex.Message}");
        }

        foreach (var u in users)
        {
            try
            {
                Guid userId;
                if (existingUsers.TryGetValue(u.Email, out var existingId))
                {
                    userId = Guid.Parse(existingId);
                    results.Add($"User already exists. UserId: {userId}");
                }
                else
                {
                    // 1. Create Auth User via Admin API
                    var payload = new
                    {
                        email = u.Email,
                        password = u.Password,
                        email_confirm = true
                    };
                    
                    var response = await httpClient.PostAsJsonAsync($"{url}/auth/v1/admin/users", payload);
                    var content = await response.Content.ReadAsStringAsync();
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        results.Add($"Failed to create auth user for {u.Email}: {content}");
                        continue;
                    }
                    
                    var json = Newtonsoft.Json.Linq.JObject.Parse(content);
                    var userIdStr = json["id"]?.ToString() ?? json["user"]?["id"]?.ToString();

                    if (string.IsNullOrEmpty(userIdStr))
                    {
                        results.Add($"Could not parse User ID for {u.Email}");
                        continue;
                    }

                    userId = Guid.Parse(userIdStr);
                    results.Add($"Created User. UserId: {userId}");
                }

                // 2. Create Public User
                var user = new User
                {
                    Id = userId,
                    SupabaseAuthId = userId,
                    Email = u.Email,
                    Role = u.Role
                };

                await _dbClient.From<User>().Upsert(user);
                results.Add($"User upserted: {u.Email}");

                // 3. Create Specific Role Entity (Student/Professor)
                if (u.Role == UserRole.Student)
                {
                    results.Add($"Creating student for {u.Email}");
                    var student = new Student
                    {
                        UserId = userId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        // Email is in User entity
                        EnrollmentNumber = $"AN-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                        Status = StudentStatus.Active,
                        Cnp = "1234567890123", // Dummy
                        Address = "Test Address",
                        Phone = "0700000000"
                    };
                    await _dbClient.From<Student>().Insert(student);
                }
                else if (u.Role == UserRole.Professor)
                {
                     var professor = new Professor
                     {
                         UserId = userId,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         Title = "Dr.",
                         Department = "Computer Science",
                         Phone = "0700000000"
                     };
                     await _dbClient.From<Professor>().Insert(professor);
                }

                results.Add($"Created {u.Role}: {u.Email}");
            }
            catch (Exception ex)
            {
                results.Add($"Error creating {u.Email}: {ex.Message}");
            }
        }

        return Ok(results);
    }
}
