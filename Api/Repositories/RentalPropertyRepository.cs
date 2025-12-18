using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;

namespace Api.Repositories
{
    public class RentalPropertyRepository : IRentalPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public RentalPropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<RentalProperty?> GetByIdAsync(int id)
        {
            return await _context.RentalProperties
                .Include(rp => rp.Owner)
                .FirstOrDefaultAsync(rp => rp.Id == id);
        }

        public async Task<IEnumerable<RentalProperty>> GetAllAsync()
        {
            return await _context.RentalProperties
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalProperty>> FindAsync(Expression<Func<RentalProperty, bool>> predicate)
        {
            return await _context.RentalProperties
                .Where(predicate)
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        public async Task<RentalProperty> AddAsync(RentalProperty entity)
        {
            await _context.RentalProperties.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(RentalProperty entity)
        {
            _context.RentalProperties.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RentalProperty entity)
        {
            _context.RentalProperties.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RentalProperties.AnyAsync(rp => rp.Id == id);
        }


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

        public async Task<IEnumerable<RentalProperty>> GetPropertiesByOwnerAsync(int ownerId)
        {
            return await _context.RentalProperties
                .Where(rp => rp.OwnerId == ownerId)
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentalProperty>> GetPropertiesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.RentalProperties
                .Where(rp => rp.PricePerDay >= minPrice && rp.PricePerDay <= maxPrice)
                .Include(rp => rp.Owner)
                .ToListAsync();
        }

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