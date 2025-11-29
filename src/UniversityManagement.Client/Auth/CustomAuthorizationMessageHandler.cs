using System.Net.Http.Headers;
using Supabase;
using Microsoft.Extensions.DependencyInjection;

namespace UniversityManagement.Client.Auth;

public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public CustomAuthorizationMessageHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var client = _serviceProvider.GetRequiredService<Supabase.Client>();
        var session = client.Auth.CurrentSession;
        if (session != null && !string.IsNullOrEmpty(session.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
