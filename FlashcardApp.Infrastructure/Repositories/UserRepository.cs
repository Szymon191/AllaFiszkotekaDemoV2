using FlashcardApp.Core.Auth.Models;
using FlashcardApp.Core.Repositories;
using FlashcardApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlashcardApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        _logger.LogInformation("Adding user: {@User}", user);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}