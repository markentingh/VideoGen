using Dapper;
using VideoGen.Plugins.Services.OpenAI.Entities;
using VideoGen.Plugins.Services.OpenAI.Interfaces;
using System.Data;

namespace VideoGen.Plugins.Services.OpenAI.Repositories
{
    public class OpenAIPlatformRepository : IOpenAIPlatformRepository
    {
        readonly IDbConnection _dbConnection;
        public OpenAIPlatformRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<OpenAIPlatform>> GetAll()
        {
            string query = @"SELECT * FROM public.""OpenAI"" ORDER BY ""Id""";
            return await _dbConnection.QueryAsync<OpenAIPlatform>(query);
        }

        public async Task<int> Add(OpenAIPlatform platform)
        {
            string query = @"INSERT INTO public.""OpenAI"" (""Name"", ""Endpoint"", ""Model"", ""ApiKey"")
                VALUES (@Name, @Endpoint, @Model, @ApiKey) RETURNING ""Id""";
            return await _dbConnection.ExecuteScalarAsync<int>(query, platform);
        }

        public async Task Update(OpenAIPlatform platform)
        {
            string query = @"UPDATE public.""OpenAI"" SET
                ""Name"" = @Name,
                ""Endpoint"" = @Endpoint,
                ""Model"" = @Model,
                ""ApiKey"" = @ApiKey
                WHERE ""Id"" = @Id";
            await _dbConnection.ExecuteAsync(query, platform);
        }

        public async Task Delete(int id)
        {
            string query = @"DELETE FROM public.""OpenAI"" WHERE ""Id"" = @id";
            await _dbConnection.ExecuteAsync(query, new { id });
        }
    }
}
