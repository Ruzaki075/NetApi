using Api.Models;

namespace Api.Repositories.Interfaces
{
    public interface IRentalPropertyRepository : IRepository<RentalProperty>
    {
        Task<IEnumerable<RentalProperty>> GetPropertiesByOwnerAsync(int ownerId);
    }
}