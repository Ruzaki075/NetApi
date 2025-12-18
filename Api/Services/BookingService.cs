using AutoMapper;
using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : _mapper.Map<BookingResponseDto>(booking);
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            var isAvailable = await _bookingRepository.IsPropertyAvailableAsync(
                createBookingDto.PropertyId,
                createBookingDto.StartDate,
                createBookingDto.EndDate);

            if (!isAvailable)
                throw new InvalidOperationException("Property is not available for selected dates");

            var booking = _mapper.Map<Booking>(createBookingDto);
            var createdBooking = await _bookingRepository.AddAsync(booking);
            return _mapper.Map<BookingResponseDto>(createdBooking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByUserAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserAsync(userId);
            return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
        }
    }
}