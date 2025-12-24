using Api.DTOs;

namespace Api.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingResponseDto?> UpdateBookingAsync(int id, UpdateBookingDto updateBookingDto);
        Task<bool> CancelBookingAsync(int id);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByUserAsync(int userId);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByPropertyAsync(int propertyId);
    }
}