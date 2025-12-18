using Api.DTOs;

namespace Api.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByUserAsync(int userId);
    }
}