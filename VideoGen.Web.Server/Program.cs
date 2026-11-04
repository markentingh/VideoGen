using System.Data;
using System.Reflection;
using VideoGen.API.Services;
using VideoGen.Auth.Services;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            origin.StartsWith("http://localhost") ||
            origin.StartsWith("https://localhost")
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

builder.Services.AddControllers()
    .AddApplicationPart(Assembly.Load("VideoGen.API"))
    .AddApplicationPart(Assembly.Load("VideoGen.Auth"))
    .AddApplicationPart(Assembly.Load("VideoGen.Plugins"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.AddApiStartupService();
builder.AddAuthService();

// let each installed service plugin register its own Dapper repositories
foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a => { try { return a.GetTypes(); } catch { return Type.EmptyTypes; } })
    .Where(t => typeof(VideoGen.Plugins.IDapperStartupService).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false }))
{
    type.GetMethod(nameof(VideoGen.Plugins.IDapperStartupService.AddDapperStartupService), BindingFlags.Public | BindingFlags.Static)
        ?.Invoke(null, new object[] { builder });
}

builder.Services.Configure<VideoGen.API.Models.EmailSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddScoped<VideoGen.API.Services.IEmailService, VideoGen.API.Services.EmailService>();

builder.Services.AddSwaggerGen(e =>
{
    e.DescribeAllParametersInCamelCase();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/server-error");
    app.UseStatusCodePagesWithReExecute("/error");
    app.UseHsts();
    app.UseHealthChecks("/healthcheck");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VideoGen API v1");
    });
}

app.UseCors();
app.UseRouting();
app.Use(async (context, next) =>
{
    const string tokenCookieName = "videogen_token";
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        if (context.Request.Cookies.TryGetValue(tokenCookieName, out var token) && !string.IsNullOrEmpty(token))
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }
    }
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

try
{
    using var scope = app.Services.CreateScope();
    var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
    using (var command = connection.CreateCommand())
    {
        command.CommandText = "SELECT public.\"ResetAllSequences\"()";
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
        command.ExecuteNonQuery();
    }
    Console.WriteLine("PostgreSQL sequences reset successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Failed to reset PostgreSQL sequences: {ex.Message}");
}

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".svg"] = "image/svg+xml";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.MapControllers();
app.MapFallbackToFile("index.html");

Console.WriteLine(
    "VideoGen Web Server {0} started.",
    typeof(Program).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion.Split("+")[0] ?? "unknown");

var urls = builder.Configuration.GetSection("Urls").Value;
if (!string.IsNullOrEmpty(urls))
{
    Console.WriteLine("\nListening on:");
    foreach (var url in urls.Split(';'))
    {
        Console.WriteLine($"  {url}");
    }
}
else
{
    Console.WriteLine("\nListening on:");
    Console.WriteLine("  http://0.0.0.0:7780");
    Console.WriteLine("  https://0.0.0.0:7781");
}

app.Run();
