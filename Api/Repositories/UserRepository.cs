using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;

namespace Api.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор репозитория пользователей
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Пользователь или null, если не найден</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Получает всех пользователей
        /// </summary>
        /// <returns>Коллекция пользователей</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Находит пользователей по условию
        /// </summary>
        /// <param name="predicate">Условие для поиска</param>
        /// <returns>Коллекция пользователей, удовлетворяющих условию</returns>
        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <param name="entity">Пользователь для добавления</param>
        /// <returns>Добавленный пользователь</returns>
        public async Task<User> AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        /// <param name="entity">Пользователь с обновленными данными</param>
        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="entity">Пользователь для удаления</param>
        public async Task DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Проверяет существование пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>true, если пользователь существует, иначе false</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        /// <summary>
        /// Получает пользователя по email
        /// </summary>
        /// <param name="email">Email пользователя</param>
        /// <returns>Пользователь или null, если не найден</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Получает пользователя по телефону
        /// </summary>
        /// <param name="phone">Номер телефона пользователя</param>
        /// <returns>Пользователь или null, если не найден</returns>
        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }

        /// <summary>
        /// Получает пользователей с их объектами аренды
        /// </summary>
        /// <returns>Коллекция пользователей с загруженными объектами аренды</returns>
        public async Task<IEnumerable<User>> GetUsersWithPropertiesAsync()
        {
            return await _context.Users
                .Include(u => u.Properties)
                .ToListAsync();
        }

        /// <summary>
        /// Получает список арендодателей (пользователей с объектами аренды)
        /// </summary>
        /// <returns>Коллекция арендодателей с их объектами аренды</returns>
        public async Task<IEnumerable<User>> GetLandlordsAsync()
        {
            return await _context.Users
                .Where(u => u.Properties.Any())
                .Include(u => u.Properties)
                .ToListAsync();
        }

        /// <summary>
        /// Получает список арендаторов (пользователей с бронированиями)
        /// </summary>
        /// <returns>Коллекция арендаторов с их бронированиями</returns>
        public async Task<IEnumerable<User>> GetTenantsAsync()
        {
            return await _context.Users
                .Where(u => u.Bookings.Any())
                .Include(u => u.Bookings)
                .ToListAsync();
        }
    }
}