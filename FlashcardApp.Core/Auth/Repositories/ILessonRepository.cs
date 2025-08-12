using FlashcardApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlashcardApp.Core.Repositories;

public interface ILessonRepository
{
    Task<Lesson> GetByIdAsync(int id);
    Task<IEnumerable<Lesson>> GetByCategoryIdAsync(int categoryId);
    Task AddAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(int id);
}