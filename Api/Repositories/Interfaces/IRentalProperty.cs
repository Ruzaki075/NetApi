using Api.Models;

namespace Api.Repositories.Interfaces
{
    public interface IRentalPropertyRepository : IRepository<RentalProperty>
    {
        Task<IEnumerable<RentalProperty>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<RentalProperty>> GetPropertiesByOwnerAsync(int ownerId);
        Task<IEnumerable<RentalProperty>> GetPropertiesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<RentalProperty>> SearchPropertiesAsync(string searchTerm);
    }
}