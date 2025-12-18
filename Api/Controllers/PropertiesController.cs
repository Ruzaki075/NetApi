using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services.Interfaces;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyResponseDto>> GetProperty(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null) return NotFound();
            return Ok(property);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyResponseDto>> CreateProperty(CreatePropertyDto createPropertyDto)
        {
            var createdProperty = await _propertyService.CreatePropertyAsync(createPropertyDto);
            return CreatedAtAction(nameof(GetProperty), new { id = createdProperty.Id }, createdProperty);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetAvailableProperties(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var properties = await _propertyService.GetAvailablePropertiesAsync(startDate, endDate);
            return Ok(properties);
        }
    }
}