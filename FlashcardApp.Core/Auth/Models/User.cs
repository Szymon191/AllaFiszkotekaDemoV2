using Microsoft.AspNetCore.Identity;

namespace FlashcardApp.Core.Auth.Models;

public class User : IdentityUser
{
    public string Nickname { get; set; }
    public int Points { get; set; }
    public int StreakCount { get; set; }
    public int ConsecutiveDays { get; set; }
    public string SubscriptionType { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}