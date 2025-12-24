using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services.Interfaces;

namespace Api.Controllers
{
    /// <summary>
    /// Контроллер для управления бронированиями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        /// <summary>
        /// Конструктор контроллера бронирований
        /// </summary>
        /// <param name="bookingService">Сервис для работы с бронированиями</param>
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Получает список всех бронирований
        /// </summary>
        /// <returns>Список бронирований</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingResponseDto>))]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        /// <summary>
        /// Получает бронирование по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Данные бронирования</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = $"Бронирование с ID {id} не найдено" });
            }
            return Ok(booking);
        }

        /// <summary>
        /// Создает новое бронирование
        /// </summary>
        /// <param name="createBookingDto">Данные для создания бронирования</param>
        /// <returns>Созданное бронирование</returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookingResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                
                return BadRequest(new 
                { 
                    message = "Ошибка валидации данных",
                    errors = errors
                });
            }

            var createdBooking = await _bookingService.CreateBookingAsync(createBookingDto);
            return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
        }

        /// <summary>
        /// Получает список бронирований по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список бронирований пользователя</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingResponseDto>))]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByUser(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserAsync(userId);
            return Ok(bookings);
        }
    }
}