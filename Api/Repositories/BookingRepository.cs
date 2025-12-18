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

//crud методы базовые
        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _context.Bookings
                .Where(predicate)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        public async Task<Booking> AddAsync(Booking entity)
        {
            var property = await _context.RentalProperties.FindAsync(entity.PropertyId);
            if (property != null)
            {
                var days = (entity.EndDate - entity.StartDate).Days;
                entity.TotalPrice = property.PricePerDay * days;
            }

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
        {
            return await _context.Bookings.AnyAsync(b => b.Id == id);
        }


        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.TenantId == userId)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByPropertyAsync(int propertyId)
        {
            return await _context.Bookings
                .Where(b => b.PropertyId == propertyId)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetActiveBookingsAsync()
        {
            return await _context.Bookings
                .Where(b => b.Status == "Confirmed" && b.EndDate >= DateTime.Now)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
                .Include(b => b.Property)
                .Include(b => b.Tenant)
                .ToListAsync();
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime startDate, DateTime endDate)
        {
            return !await _context.Bookings
                .AnyAsync(b => b.PropertyId == propertyId &&
                              b.Status == "Confirmed" &&
                              ((b.StartDate <= endDate && b.EndDate >= startDate)));
        }
    }
}