using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;

namespace Api.Repositories
{
    /// <summary>
    /// Репозиторий для работы с бронированиями
    /// </summary>
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор репозитория бронирований
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает бронирование по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Бронирование с информацией об объекте аренды и арендаторе или null, если не найдено</returns>
        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <summary>
        /// Получает все бронирования
        /// </summary>
        /// <returns>Коллекция бронирований с информацией об объектах аренды и арендаторах</returns>
        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        /// <summary>
        /// Находит бронирования по условию
        /// </summary>
        /// <param name="predicate">Условие для поиска</param>
        /// <returns>Коллекция бронирований, удовлетворяющих условию</returns>
        public async Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _context.Bookings
                .Where(predicate)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        /// <summary>
        /// Добавляет новое бронирование
        /// </summary>
        /// <param name="entity">Бронирование для добавления</param>
        /// <returns>Добавленное бронирование с рассчитанной общей стоимостью</returns>
        public async Task<Booking> AddAsync(Booking entity)
        {
            var property = await _context.RentalProperties.FindAsync(entity.PropertyId);
            if (property != null)
            {
                // Расчет количества дней аренды: день заезда считается, день выезда - нет
                // Например: заезд 1 января, выезд 3 января = 2 дня аренды
                var days = (entity.EndDate.Date - entity.StartDate.Date).Days;
                entity.TotalPrice = property.PricePerDay * days;
            }

            await _context.Bookings.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Обновляет данные бронирования
        /// </summary>
        /// <param name="entity">Бронирование с обновленными данными</param>
        public async Task UpdateAsync(Booking entity)
        {
            _context.Bookings.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет бронирование
        /// </summary>
        /// <param name="entity">Бронирование для удаления</param>
        public async Task DeleteAsync(Booking entity)
        {
            _context.Bookings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Проверяет существование бронирования по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>true, если бронирование существует, иначе false</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bookings.AnyAsync(b => b.Id == id);
        }

        /// <summary>
        /// Получает бронирования по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя (арендатора)</param>
        /// <returns>Коллекция бронирований пользователя</returns>
        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.TenantId == userId)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        /// <summary>
        /// Получает бронирования по идентификатору объекта аренды
        /// </summary>
        /// <param name="propertyId">Идентификатор объекта аренды</param>
        /// <returns>Коллекция бронирований объекта аренды</returns>
        public async Task<IEnumerable<Booking>> GetBookingsByPropertyAsync(int propertyId)
        {
            return await _context.Bookings
                .Where(b => b.PropertyId == propertyId)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        /// <summary>
        /// Получает активные бронирования (подтвержденные и не завершенные)
        /// </summary>
        /// <returns>Коллекция активных бронирований</returns>
        public async Task<IEnumerable<Booking>> GetActiveBookingsAsync()
        {
            return await _context.Bookings
                .Where(b => b.Status == "Confirmed" && b.EndDate >= DateTime.Now)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        /// <summary>
        /// Получает бронирования в указанном диапазоне дат
        /// </summary>
        /// <param name="startDate">Начальная дата диапазона</param>
        /// <param name="endDate">Конечная дата диапазона</param>
        /// <returns>Коллекция бронирований, пересекающихся с указанным диапазоном</returns>
        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        /// <summary>
        /// Проверяет доступность объекта аренды на указанный период
        /// </summary>
        /// <param name="propertyId">Идентификатор объекта аренды</param>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="endDate">Дата окончания периода</param>
        /// <returns>true, если объект доступен, иначе false</returns>
        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime startDate, DateTime endDate)
        {
            return !await _context.Bookings
                .AnyAsync(b => b.PropertyId == propertyId &&
                              b.Status == "Confirmed" &&
                              ((b.StartDate <= endDate && b.EndDate >= startDate)));
        }
    }
}