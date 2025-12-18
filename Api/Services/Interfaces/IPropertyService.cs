using Api.DTOs;

namespace Api.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync();
        Task<PropertyResponseDto?> GetPropertyByIdAsync(int id);
        Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto);
        Task<IEnumerable<PropertyResponseDto>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate);
    }
}