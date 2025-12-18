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
            => await _context.RentalProperties.FindAsync(id);

        public async Task<IEnumerable<RentalProperty>> GetAllAsync()
            => await _context.RentalProperties.ToListAsync();

        public async Task<IEnumerable<RentalProperty>> FindAsync(Expression<Func<RentalProperty, bool>> predicate)
            => await _context.RentalProperties.Where(predicate).ToListAsync();

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
            => await _context.RentalProperties.AnyAsync(rp => rp.Id == id);

        // Реализация уникального метода из IRentalPropertyRepository
        public async Task<IEnumerable<RentalProperty>> GetPropertiesByOwnerAsync(int ownerId)
            => await _context.RentalProperties
                .Where(rp => rp.OwnerId == ownerId)
                .ToListAsync();
    }
}