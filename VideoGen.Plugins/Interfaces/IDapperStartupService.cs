using Microsoft.AspNetCore.Builder;

namespace VideoGen.Plugins
{
    public interface IDapperStartupService
    {
        static abstract void AddDapperStartupService(WebApplicationBuilder builder);
    }
}
