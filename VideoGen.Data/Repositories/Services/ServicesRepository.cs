using Dapper;
using VideoGen.Data.Entities.Services;
using VideoGen.Data.Interfaces.Services;
using System.Data;

namespace VideoGen.Data.Repositories.Services
{
    public class ServicesRepository : IServicesRepository
    {
        readonly IDbConnection _dbConnection;
        public ServicesRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Service>> GetAll()
        {
            string query = @"SELECT * FROM public.""Services"" ORDER BY ""Name""";
            return await _dbConnection.QueryAsync<Service>(query);
        }

        public async Task<Service?> GetByKey(string key)
        {
            string query = @"SELECT * FROM public.""Services"" WHERE ""Key"" = @key";
            return await _dbConnection.QueryFirstOrDefaultAsync<Service>(query, new { key });
        }

        public async Task Upsert(Service service)
        {
            string query = @"INSERT INTO public.""Services"" (""Key"", ""Version"", ""Name"", ""Type"", ""GitRepo"", ""HasSchema"", ""InstalledFiles"")
                VALUES (@Key, @Version, @Name, @Type, @GitRepo, @HasSchema, @InstalledFiles)
                ON CONFLICT (""Key"") DO UPDATE SET
                    ""Version"" = @Version,
                    ""Name"" = @Name,
                    ""Type"" = @Type,
                    ""GitRepo"" = @GitRepo,
                    ""HasSchema"" = @HasSchema,
                    ""InstalledFiles"" = @InstalledFiles,
                    ""Installed"" = CURRENT_TIMESTAMP";
            await _dbConnection.ExecuteAsync(query, service);
        }

        public async Task Delete(string key)
        {
            string query = @"DELETE FROM public.""Services"" WHERE ""Key"" = @key";
            await _dbConnection.ExecuteAsync(query, new { key });
        }
    }
}
