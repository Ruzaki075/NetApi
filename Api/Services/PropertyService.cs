using AutoMapper;
using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IRentalPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public PropertyService(IRentalPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PropertyResponseDto>>(properties);
        }

        public async Task<PropertyResponseDto?> GetPropertyByIdAsync(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            return property == null ? null : _mapper.Map<PropertyResponseDto>(property);
        }

        public async Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
        {
            var property = _mapper.Map<RentalProperty>(createPropertyDto);
            var createdProperty = await _propertyRepository.AddAsync(property);
            return _mapper.Map<PropertyResponseDto>(createdProperty);
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate)
        {
            var properties = await _propertyRepository.GetAvailablePropertiesAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<PropertyResponseDto>>(properties);
        }
    }
}