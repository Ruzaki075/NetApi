using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;

namespace Api.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(int id)
            => await _context.Bookings.FindAsync(id);

        public async Task<IEnumerable<Booking>> GetAllAsync()
            => await _context.Bookings.ToListAsync();

        public async Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate)
            => await _context.Bookings.Where(predicate).ToListAsync();

        public async Task<Booking> AddAsync(Booking entity)
        {
            await _context.Bookings.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Booking entity)
        {
            _context.Bookings.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Booking entity)
        {
            _context.Bookings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Bookings.AnyAsync(b => b.Id == id);

        // Реализация уникального метода из IBookingRepository
        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
            => await _context.Bookings
                .Where(b => b.TenantId == userId)
                .ToListAsync();
    }
}