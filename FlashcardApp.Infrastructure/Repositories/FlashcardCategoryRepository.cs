using FlashcardApp.Core.Models;
using FlashcardApp.Core.Repositories;
using FlashcardApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlashcardApp.Infrastructure.Repositories;

public class FlashcardCategoryRepository : IFlashcardCategoryRepository
{
    private readonly AppDbContext _context;

    public FlashcardCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FlashcardCategory> GetByIdAsync(int id)
    {
        return await _context.FlashcardCategories.FindAsync(id);
    }

    public async Task<FlashcardCategory> GetByNameAndUserIdAsync(string name, string userId)
    {
        return await _context.FlashcardCategories
            .FirstOrDefaultAsync(c => c.Name == name && c.UserId == userId);
    }

    public async Task AddAsync(FlashcardCategory category)
    {
        _context.FlashcardCategories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FlashcardCategory category)
    {
        _context.FlashcardCategories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.FlashcardCategories.FindAsync(id);
        if (category != null)
        {
            _context.FlashcardCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FlashcardCategory>> GetAllByUserIdAsync(string userId)
    {
        return await _context.FlashcardCategories
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<FlashcardCategory>> GetPublicCategoriesAsync()
    {
        return await _context.FlashcardCategories
            .Where(c => c.IsPublic)
            .ToListAsync();
    }
}
