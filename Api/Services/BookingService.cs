using AutoMapper;
using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    /// <summary>
    /// Сервис для работы с бронированиями
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Конструктор сервиса бронирований
        /// </summary>
        /// <param name="bookingRepository">Репозиторий для работы с бронированиями</param>
        /// <param name="propertyRepository">Репозиторий для работы с объектами аренды</param>
        /// <param name="userRepository">Репозиторий для работы с пользователями</param>
        /// <param name="mapper">Маппер для преобразования сущностей в DTO</param>
        public BookingService(IBookingRepository bookingRepository, IRentalPropertyRepository propertyRepository, IUserRepository userRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получает список всех бронирований
        /// </summary>
        /// <returns>Коллекция DTO бронирований</returns>
        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
        }

        /// <summary>
        /// Получает бронирование по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>DTO бронирования или null, если бронирование не найдено</returns>
        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : _mapper.Map<BookingResponseDto>(booking);
        }

        /// <summary>
        /// Создает новое бронирование
        /// </summary>
        /// <param name="createBookingDto">DTO с данными для создания бронирования</param>
        /// <returns>DTO созданного бронирования</returns>
        /// <exception cref="ArgumentException">Выбрасывается при невалидных данных</exception>
        /// <exception cref="InvalidOperationException">Выбрасывается, если объект недоступен для бронирования</exception>
        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            // Валидация дат
            if (createBookingDto.StartDate >= createBookingDto.EndDate)
            {
                throw new ArgumentException("Дата начала должна быть раньше даты окончания");
            }

            if (createBookingDto.StartDate < DateTime.Today)
            {
                throw new ArgumentException("Дата начала не может быть в прошлом");
            }

            // Проверка существования объекта аренды
            var property = await _propertyRepository.GetByIdAsync(createBookingDto.PropertyId);
            if (property == null)
            {
                throw new ArgumentException($"Объект аренды с ID {createBookingDto.PropertyId} не найден");
            }

            // Проверка существования арендатора
            var tenant = await _userRepository.GetByIdAsync(createBookingDto.TenantId);
            if (tenant == null)
            {
                throw new ArgumentException($"Пользователь с ID {createBookingDto.TenantId} не найден");
            }

            // Проверка: арендатор не может забронировать свою собственную недвижимость
            if (property.OwnerId == createBookingDto.TenantId)
            {
                throw new InvalidOperationException("Нельзя забронировать свою собственную недвижимость");
            }

            // Проверка доступности объекта на указанный период
            var isAvailable = await _bookingRepository.IsPropertyAvailableAsync(
                createBookingDto.PropertyId,
                createBookingDto.StartDate,
                createBookingDto.EndDate);

            if (!isAvailable)
            {
                throw new InvalidOperationException("Объект аренды недоступен для бронирования на выбранные даты");
            }

            var booking = _mapper.Map<Booking>(createBookingDto);
            booking.Status = "Confirmed";
            var createdBooking = await _bookingRepository.AddAsync(booking);
            return _mapper.Map<BookingResponseDto>(createdBooking);
        }

        /// <summary>
        /// Получает список бронирований по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Коллекция DTO бронирований пользователя</returns>
        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByUserAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserAsync(userId);
            return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
        }
    }
}