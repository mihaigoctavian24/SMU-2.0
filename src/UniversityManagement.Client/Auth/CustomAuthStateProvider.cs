using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Supabase.Gotrue;

namespace UniversityManagement.Client.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly Supabase.Client _client;

    public CustomAuthStateProvider(Supabase.Client client)
    {
        _client = client;
        _client.Auth.AddStateChangedListener((sender, state) => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = _client.Auth.CurrentSession;
        var user = _client.Auth.CurrentUser;

        if (session == null || user == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.Email ?? "")
        };

        // Add roles if present in metadata or app_metadata
        if (user.AppMetadata != null && user.AppMetadata.TryGetValue("role", out var roleObj) && roleObj != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleObj.ToString() ?? "student"));
        }
        else
        {
             // Default to student if no role found, or maybe fetch from DB?
             // For now, let's assume 'student' or check metadata.
             // Actually, Supabase might not put role in app_metadata by default unless configured.
             // But my RLS setup uses it.
             // Let's assume for now we might need to fetch it or it's in metadata.
             // I'll add a default role for testing if missing.
             claims.Add(new Claim(ClaimTypes.Role, "student"));
        }

        var identity = new ClaimsIdentity(claims, "Supabase");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
