using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;

namespace Api.Repositories
{
    /// <summary>
    /// Репозиторий для работы с объектами аренды
    /// </summary>
    public class RentalPropertyRepository : IRentalPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор репозитория объектов аренды
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public RentalPropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает объект аренды по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта аренды</param>
        /// <returns>Объект аренды с информацией о владельце или null, если не найден</returns>
        public async Task<RentalProperty?> GetByIdAsync(int id)
        {
            return await _context.RentalProperties
                .Include(rp => rp.Owner)
                .FirstOrDefaultAsync(rp => rp.Id == id);
        }

        /// <summary>
        /// Получает все объекты аренды
        /// </summary>
        /// <returns>Коллекция объектов аренды с информацией о владельцах</returns>
        public async Task<IEnumerable<RentalProperty>> GetAllAsync()
        {
            return await _context.RentalProperties
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        /// <summary>
        /// Находит объекты аренды по условию
        /// </summary>
        /// <param name="predicate">Условие для поиска</param>
        /// <returns>Коллекция объектов аренды, удовлетворяющих условию</returns>
        public async Task<IEnumerable<RentalProperty>> FindAsync(Expression<Func<RentalProperty, bool>> predicate)
        {
            return await _context.RentalProperties
                .Where(predicate)
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        /// <summary>
        /// Добавляет новый объект аренды
        /// </summary>
        /// <param name="entity">Объект аренды для добавления</param>
        /// <returns>Добавленный объект аренды</returns>
        public async Task<RentalProperty> AddAsync(RentalProperty entity)
        {
            await _context.RentalProperties.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Обновляет данные объекта аренды
        /// </summary>
        /// <param name="entity">Объект аренды с обновленными данными</param>
        public async Task UpdateAsync(RentalProperty entity)
        {
            _context.RentalProperties.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет объект аренды
        /// </summary>
        /// <param name="entity">Объект аренды для удаления</param>
        public async Task DeleteAsync(RentalProperty entity)
        {
            _context.RentalProperties.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Проверяет существование объекта аренды по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта аренды</param>
        /// <returns>true, если объект существует, иначе false</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RentalProperties.AnyAsync(rp => rp.Id == id);
        }

        /// <summary>
        /// Получает доступные объекты аренды на указанный период
        /// </summary>
        /// <param name="startDate">Дата начала периода</param>
        /// <param name="endDate">Дата окончания периода</param>
        /// <returns>Коллекция доступных объектов аренды</returns>
        public async Task<IEnumerable<RentalProperty>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate)
        {
            var bookedPropertyIds = await _context.Bookings
                .Where(b => b.Status == "Confirmed" &&
                           ((b.StartDate <= endDate && b.EndDate >= startDate)))
                .Select(b => b.PropertyId)
                .Distinct()
                .ToListAsync();

            return await _context.RentalProperties
                .Where(rp => !bookedPropertyIds.Contains(rp.Id))
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        /// <summary>
        /// Получает объекты аренды по идентификатору владельца
        /// </summary>
        /// <param name="ownerId">Идентификатор владельца</param>
        /// <returns>Коллекция объектов аренды владельца</returns>
        public async Task<IEnumerable<RentalProperty>> GetPropertiesByOwnerAsync(int ownerId)
        {
            return await _context.RentalProperties
                .Where(rp => rp.OwnerId == ownerId)
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        /// <summary>
        /// Получает объекты аренды в указанном диапазоне цен
        /// </summary>
        /// <param name="minPrice">Минимальная цена за день</param>
        /// <param name="maxPrice">Максимальная цена за день</param>
        /// <returns>Коллекция объектов аренды в указанном диапазоне цен</returns>
        public async Task<IEnumerable<RentalProperty>> GetPropertiesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.RentalProperties
                .Where(rp => rp.PricePerDay >= minPrice && rp.PricePerDay <= maxPrice)
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        /// <summary>
        /// Ищет объекты аренды по поисковому запросу
        /// </summary>
        /// <param name="searchTerm">Поисковый запрос</param>
        /// <returns>Коллекция объектов аренды, соответствующих поисковому запросу</returns>
        public async Task<IEnumerable<RentalProperty>> SearchPropertiesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _context.RentalProperties
                .Where(rp => rp.Title.Contains(searchTerm) ||
                            rp.Description.Contains(searchTerm) ||
                            rp.Address.Contains(searchTerm))
                .Include(rp => rp.Owner)
                .ToListAsync();
        }
    }
}