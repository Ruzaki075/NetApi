using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services.Interfaces;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            var createdBooking = await _bookingService.CreateBookingAsync(createBookingDto);
            return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByUser(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserAsync(userId);
            return Ok(bookings);
        }
    }
}