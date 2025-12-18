using Api.Models;

namespace Api.Repositories.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
    }
}