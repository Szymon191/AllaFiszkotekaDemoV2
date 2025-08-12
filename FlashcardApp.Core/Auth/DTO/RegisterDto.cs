using System.ComponentModel.DataAnnotations;

namespace FlashcardApp.Core.Auth.DTO;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Nickname { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
}