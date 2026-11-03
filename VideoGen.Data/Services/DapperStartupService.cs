using VideoGen.Data.Interfaces.Auth;
using VideoGen.Data.Interfaces.Services;
using VideoGen.Data.Repositories.Auth;
using VideoGen.Data.Repositories.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace VideoGen.Data.Services
{
    public static class DapperStartupService
    {
        public static void AddDapperStartupService(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IDbConnection>((sp) => new NpgsqlConnection(builder.Configuration["ConnectionStrings:Database"] ?? ""));

            builder.Services.AddTransient<IAppUserRepository, AppUserRepository>();
            builder.Services.AddTransient<IAppRoleRepository, AppRoleRepository>();
            builder.Services.AddTransient<IAppUserRolesRepository, AppUserRolesRepository>();
            builder.Services.AddTransient<IAppUserTokenRepository, AppUserTokenRepository>();
            builder.Services.AddTransient<IServicesRepository, ServicesRepository>();
        }
    }
}
