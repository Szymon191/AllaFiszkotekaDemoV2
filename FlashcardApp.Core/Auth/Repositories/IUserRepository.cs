using FlashcardApp.Core.Auth.Models;

namespace FlashcardApp.Core.Repositories;

public interface IUserRepository
{
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
}
