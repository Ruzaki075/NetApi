using AutoMapper;
using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    /// <summary>
    /// Сервис для работы с объектами аренды
    /// </summary>
    public class PropertyService : IPropertyService
    {
        private readonly IRentalPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Конструктор сервиса объектов аренды
        /// </summary>
        /// <param name="propertyRepository">Репозиторий для работы с объектами аренды</param>
        /// <param name="userRepository">Репозиторий для работы с пользователями</param>
        /// <param name="mapper">Маппер для преобразования сущностей в DTO</param>
        public PropertyService(IRentalPropertyRepository propertyRepository, IUserRepository userRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получает список всех объектов аренды
        /// </summary>
        /// <returns>Коллекция DTO объектов аренды</returns>
        public async Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PropertyResponseDto>>(properties);
        }

        /// <summary>
        /// Получает объект аренды по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта аренды</param>
        /// <returns>DTO объекта аренды или null, если объект не найден</returns>
        public async Task<PropertyResponseDto?> GetPropertyByIdAsync(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            return property == null ? null : _mapper.Map<PropertyResponseDto>(property);
        }

        /// <summary>
        /// Создает новый объект аренды
        /// </summary>
        /// <param name="createPropertyDto">DTO с данными для создания объекта аренды</param>
        /// <returns>DTO созданного объекта аренды</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если владелец не найден</exception>
        public async Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
        {
            // Проверка существования владельца
            var owner = await _userRepository.GetByIdAsync(createPropertyDto.OwnerId);
            if (owner == null)
            {
                throw new ArgumentException($"Владелец с ID {createPropertyDto.OwnerId} не найден");
            }

            var property = _mapper.Map<RentalProperty>(createPropertyDto);
            var createdProperty = await _propertyRepository.AddAsync(property);
            return _mapper.Map<PropertyResponseDto>(createdProperty);
        }

        /// <summary>
        /// Получает список доступных объектов аренды на указанный период
        /// </summary>
        /// <param name="startDate">Дата начала аренды</param>
        /// <param name="endDate">Дата окончания аренды</param>
        /// <returns>Коллекция DTO доступных объектов аренды</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если дата начала позже даты окончания</exception>
        public async Task<IEnumerable<PropertyResponseDto>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new ArgumentException("Дата начала должна быть раньше даты окончания");
            }

            var properties = await _propertyRepository.GetAvailablePropertiesAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<PropertyResponseDto>>(properties);
        }
    }
}