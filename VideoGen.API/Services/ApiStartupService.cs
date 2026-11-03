using VideoGen.Data.Services;
using Microsoft.AspNetCore.Builder;

namespace VideoGen.API.Services
{
    public static class ApiStartupService
    {
        public static void AddApiStartupService(this WebApplicationBuilder builder)
        {
            builder.AddDapperStartupService();
        }
    }
}
