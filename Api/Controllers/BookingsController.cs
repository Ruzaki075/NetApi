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
        /// <param name="tenantId">ID арендатора (можно передать через query параметр, если не указан в теле запроса)</param>
        /// <returns>Созданное бронирование</returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookingResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(
            [FromBody] CreateBookingDto createBookingDto,
            [FromQuery] int? tenantId = null)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                
                // Если есть ошибки на уровне модели (например, от DateRangeAttribute)
                var modelErrors = errors.Where(e => !string.IsNullOrEmpty(e)).ToList();
                
                return BadRequest(new 
                { 
                    message = "Ошибка валидации данных",
                    errors = modelErrors
                });
            }

            // Если tenantId не указан в теле запроса, используем из query параметра
            if (!createBookingDto.TenantId.HasValue && tenantId.HasValue)
            {
                createBookingDto.TenantId = tenantId.Value;
            }

            // Проверка что tenantId указан хотя бы одним способом
            if (!createBookingDto.TenantId.HasValue || createBookingDto.TenantId.Value <= 0)
            {
                return BadRequest(new 
                { 
                    message = "ID арендатора обязателен",
                    errors = new[] { "Укажите ID арендатора либо в теле запроса (tenantId), либо в query параметре (tenantId)" }
                });
            }

            var createdBooking = await _bookingService.CreateBookingAsync(createBookingDto);
            return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
        }

        /// <summary>
        /// Обновляет бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <param name="updateBookingDto">Данные для обновления</param>
        /// <returns>Обновленное бронирование</returns>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingResponseDto>> UpdateBooking(int id, [FromBody] UpdateBookingDto updateBookingDto)
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

            var updatedBooking = await _bookingService.UpdateBookingAsync(id, updateBookingDto);
            if (updatedBooking == null)
            {
                return NotFound(new { message = $"Бронирование с ID {id} не найдено" });
            }
            return Ok(updatedBooking);
        }

        /// <summary>
        /// Отменяет бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Результат операции</returns>
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Бронирование с ID {id} не найдено" });
            }
            return NoContent();
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

        /// <summary>
        /// Получает список бронирований по идентификатору объекта аренды
        /// </summary>
        /// <param name="propertyId">Идентификатор объекта аренды</param>
        /// <returns>Список бронирований объекта аренды</returns>
        [HttpGet("property/{propertyId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingResponseDto>))]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByProperty(int propertyId)
        {
            var bookings = await _bookingService.GetBookingsByPropertyAsync(propertyId);
            return Ok(bookings);
        }
    }
}