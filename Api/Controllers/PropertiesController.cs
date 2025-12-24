using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services.Interfaces;

namespace Api.Controllers
{
    /// <summary>
    /// Контроллер для управления объектами аренды
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        /// <summary>
        /// Конструктор контроллера объектов аренды
        /// </summary>
        /// <param name="propertyService">Сервис для работы с объектами аренды</param>
        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>
        /// Получает список всех объектов аренды
        /// </summary>
        /// <returns>Список объектов аренды</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PropertyResponseDto>))]
        public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Получает объект аренды по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта аренды</param>
        /// <returns>Данные объекта аренды</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropertyResponseDto>> GetProperty(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound(new { message = $"Объект аренды с ID {id} не найден" });
            }
            return Ok(property);
        }

        /// <summary>
        /// Создает новый объект аренды
        /// </summary>
        /// <param name="createPropertyDto">Данные для создания объекта аренды</param>
        /// <returns>Созданный объект аренды</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PropertyResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyResponseDto>> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
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

            var createdProperty = await _propertyService.CreatePropertyAsync(createPropertyDto);
            return CreatedAtAction(nameof(GetProperty), new { id = createdProperty.Id }, createdProperty);
        }

        /// <summary>
        /// Получает список доступных объектов аренды на указанный период
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="endDate">Дата окончания периода</param>
        /// <returns>Список доступных объектов аренды</returns>
        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PropertyResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetAvailableProperties(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var properties = await _propertyService.GetAvailablePropertiesAsync(startDate, endDate);
            return Ok(properties);
        }
    }
}