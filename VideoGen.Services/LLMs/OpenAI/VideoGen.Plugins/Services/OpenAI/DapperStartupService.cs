using VideoGen.Plugins.Services.OpenAI.Interfaces;
using VideoGen.Plugins.Services.OpenAI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace VideoGen.Plugins.Services.OpenAI
{
    public class OpenAIDapperStartupService : IDapperStartupService
    {
        public static void AddDapperStartupService(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IOpenAIPlatformRepository, OpenAIPlatformRepository>();
        }
    }
}
