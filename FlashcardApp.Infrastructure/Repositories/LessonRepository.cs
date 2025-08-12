using FlashcardApp.Core.Models;
using FlashcardApp.Core.Repositories;
using FlashcardApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlashcardApp.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson> GetByIdAsync(int id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public async Task<IEnumerable<Lesson>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Lessons.Where(l => l.CategoryId == categoryId).ToListAsync();
    }

    public async Task AddAsync(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
}