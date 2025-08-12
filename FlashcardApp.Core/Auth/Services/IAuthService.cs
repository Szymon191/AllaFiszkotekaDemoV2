using FlashcardApp.Core.Auth.DTO;

namespace FlashcardApp.Core.Auth.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto registerDto);
    Task<string> LoginAsync(LoginDto loginDto);
}
