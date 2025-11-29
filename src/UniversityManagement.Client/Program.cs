using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Supabase;
using UniversityManagement.Client;
using UniversityManagement.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load configuration from environment variables or appsettings
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5090";
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "https://wcmliinmbntmesfzxfmx.supabase.co";
var supabaseKey = builder.Configuration["Supabase:Key"] ?? "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndjbWxpaW5tYm50bWVzZnp4Zm14Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjM2OTIyMDIsImV4cCI6MjA3OTI2ODIwMn0.XO6R5htHxN2TEnfOZddrYeqdJ_ANaItr57vDcoC2iLY";

// Auth Handler
builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

// HttpClient
builder.Services.AddHttpClient("API", client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// MudBlazor
builder.Services.AddMudServices();

// Fluxor
builder.Services.AddFluxor(options => 
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools();
});

// Supabase
builder.Services.AddScoped<Supabase.Client>(sp =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    };
    return new Supabase.Client(supabaseUrl, supabaseKey, options);
});

// Auth
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();