using VideoGen.Plugins.Services.OpenAI.Entities;

namespace VideoGen.Plugins.Services.OpenAI.Interfaces
{
    public interface IOpenAIPlatformRepository
    {
        Task<IEnumerable<OpenAIPlatform>> GetAll();
        Task<int> Add(OpenAIPlatform platform);
        Task Update(OpenAIPlatform platform);
        Task Delete(int id);
    }
}
