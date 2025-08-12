using FlashcardApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlashcardApp.Core.Repositories;

public interface IFlashcardRepository
{
    Task<Flashcard> GetByIdAsync(int id);
    Task<IEnumerable<Flashcard>> GetByLessonIdAsync(int lessonId);
    Task AddAsync(Flashcard flashcard);
    Task UpdateAsync(Flashcard flashcard);
    Task DeleteAsync(int id);
}