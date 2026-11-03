using VideoGen.Data.Entities.Services;

namespace VideoGen.Data.Interfaces.Services
{
    public interface IServicesRepository
    {
        Task<IEnumerable<Service>> GetAll();
        Task<Service?> GetByKey(string key);
        Task Upsert(Service service);
    }
}
