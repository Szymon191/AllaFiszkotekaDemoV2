using FlashcardApp.Core.Models;
using FlashcardApp.Core.Repositories;
using FlashcardApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlashcardApp.Infrastructure.Repositories;

public class FlashcardRepository : IFlashcardRepository
{
    private readonly AppDbContext _context;

    public FlashcardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Flashcard> GetByIdAsync(int id)
    {
        return await _context.Flashcards.FindAsync(id);
    }

    public async Task<IEnumerable<Flashcard>> GetByLessonIdAsync(int lessonId)
    {
        return await _context.Flashcards.Where(f => f.LessonId == lessonId).ToListAsync();
    }

    public async Task AddAsync(Flashcard flashcard)
    {
        _context.Flashcards.Add(flashcard);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Flashcard flashcard)
    {
        _context.Flashcards.Update(flashcard);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var flashcard = await _context.Flashcards.FindAsync(id);
        if (flashcard != null)
        {
            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();
        }
    }
}