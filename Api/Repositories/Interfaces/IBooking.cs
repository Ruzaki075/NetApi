using Api.Models;

namespace Api.Repositories.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
        Task<IEnumerable<Booking>> GetBookingsByPropertyAsync(int propertyId);
        Task<IEnumerable<Booking>> GetActiveBookingsAsync();
        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime startDate, DateTime endDate);
    }
}