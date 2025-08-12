using FlashcardApp.Core.Models;
using System.Threading.Tasks;

namespace FlashcardApp.Core.Repositories;

public interface IFlashcardCategoryRepository
{
    Task<FlashcardCategory> GetByIdAsync(int id);
    Task<FlashcardCategory> GetByNameAndUserIdAsync(string name, string userId);
    Task AddAsync(FlashcardCategory category);
    Task UpdateAsync(FlashcardCategory category);
    Task DeleteAsync(int id);
    Task<IEnumerable<FlashcardCategory>> GetAllByUserIdAsync(string userId);
    Task<IEnumerable<FlashcardCategory>> GetPublicCategoriesAsync();
}