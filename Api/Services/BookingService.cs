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
        /// <param name="createBookingDto">DTO с данными для создания бронирования (tenantId должен быть заполнен)</param>
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

            // Сравниваем только даты без времени (учитываем UTC)
            var startDateOnly = createBookingDto.StartDate.Kind == DateTimeKind.Utc 
                ? createBookingDto.StartDate.Date 
                : DateTime.SpecifyKind(createBookingDto.StartDate.Date, DateTimeKind.Utc);
            var todayUtc = DateTime.UtcNow.Date;

            if (startDateOnly < todayUtc)
            {
                throw new ArgumentException("Дата начала не может быть в прошлом");
            }

            // Проверка существования объекта аренды
            var property = await _propertyRepository.GetByIdAsync(createBookingDto.PropertyId);
            if (property == null)
            {
                throw new ArgumentException($"Объект аренды с ID {createBookingDto.PropertyId} не найден");
            }

            // Проверка что tenantId указан
            if (!createBookingDto.TenantId.HasValue || createBookingDto.TenantId.Value <= 0)
            {
                throw new ArgumentException("ID арендатора обязателен");
            }

            var tenantId = createBookingDto.TenantId.Value;

            // Проверка существования арендатора
            var tenant = await _userRepository.GetByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new ArgumentException($"Пользователь с ID {tenantId} не найден");
            }

            // Проверка: арендатор не может забронировать свою собственную недвижимость
            if (property.OwnerId == tenantId)
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
            booking.TenantId = tenantId; // Убеждаемся что TenantId установлен
            booking.Status = "Confirmed";
            var createdBooking = await _bookingRepository.AddAsync(booking);
            return _mapper.Map<BookingResponseDto>(createdBooking);
        }

        /// <summary>
        /// Обновляет существующее бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <param name="updateBookingDto">DTO с данными для обновления</param>
        /// <returns>DTO обновленного бронирования или null, если бронирование не найдено</returns>
        /// <exception cref="ArgumentException">Выбрасывается при невалидных данных</exception>
        /// <exception cref="InvalidOperationException">Выбрасывается при попытке изменить недоступное бронирование</exception>
        public async Task<BookingResponseDto?> UpdateBookingAsync(int id, UpdateBookingDto updateBookingDto)
        {
            var existingBooking = await _bookingRepository.GetByIdAsync(id);
            if (existingBooking == null)
            {
                return null;
            }

            // Проверка что бронирование можно изменять (не отменено)
            if (existingBooking.Status == "Cancelled")
            {
                throw new InvalidOperationException("Нельзя изменить отмененное бронирование");
            }

            // Если изменяются даты, нужно проверить доступность
            if (updateBookingDto.StartDate.HasValue || updateBookingDto.EndDate.HasValue)
            {
                var newStartDate = updateBookingDto.StartDate ?? existingBooking.StartDate;
                var newEndDate = updateBookingDto.EndDate ?? existingBooking.EndDate;

                if (newStartDate >= newEndDate)
                {
                    throw new ArgumentException("Дата начала должна быть раньше даты окончания");
                }

                // Проверяем доступность (исключая текущее бронирование)
                var conflictingBookings = await _bookingRepository.FindAsync(b => 
                    b.PropertyId == existingBooking.PropertyId &&
                    b.Id != id &&
                    b.Status == "Confirmed" &&
                    ((b.StartDate <= newEndDate && b.EndDate >= newStartDate)));

                if (conflictingBookings.Any())
                {
                    throw new InvalidOperationException("Объект недоступен на выбранные даты");
                }
            }

            // Обновляем поля
            if (updateBookingDto.StartDate.HasValue)
                existingBooking.StartDate = updateBookingDto.StartDate.Value;
            
            if (updateBookingDto.EndDate.HasValue)
                existingBooking.EndDate = updateBookingDto.EndDate.Value;
            
            if (!string.IsNullOrEmpty(updateBookingDto.Status))
                existingBooking.Status = updateBookingDto.Status;

            // Пересчитываем стоимость, если изменились даты или цена объекта
            if (updateBookingDto.StartDate.HasValue || updateBookingDto.EndDate.HasValue)
            {
                var property = await _propertyRepository.GetByIdAsync(existingBooking.PropertyId);
                if (property != null)
                {
                    var days = (existingBooking.EndDate.Date - existingBooking.StartDate.Date).Days;
                    existingBooking.TotalPrice = property.PricePerDay * days;
                }
            }

            await _bookingRepository.UpdateAsync(existingBooking);
            return _mapper.Map<BookingResponseDto>(existingBooking);
        }

        /// <summary>
        /// Отменяет бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>true, если бронирование отменено, false если не найдено</returns>
        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return false;
            }

            if (booking.Status == "Cancelled")
            {
                return true; // Уже отменено
            }

            booking.Status = "Cancelled";
            await _bookingRepository.UpdateAsync(booking);
            return true;
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

        /// <summary>
        /// Получает список бронирований по идентификатору объекта аренды
        /// </summary>
        /// <param name="propertyId">Идентификатор объекта аренды</param>
        /// <returns>Коллекция DTO бронирований объекта аренды</returns>
        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByPropertyAsync(int propertyId)
        {
            var bookings = await _bookingRepository.GetBookingsByPropertyAsync(propertyId);
            return _mapper.Map<IEnumerable<BookingResponseDto>>(bookings);
        }
    }
}