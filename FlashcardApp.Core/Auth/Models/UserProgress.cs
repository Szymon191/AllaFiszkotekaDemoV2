namespace FlashcardApp.Core.Models;

public class UserProgress
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int FlashcardId { get; set; }
    public string Status { get; set; }
    public DateTime? LastReviewed { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public DateTime? NextReviewDate { get; set; }
    public int ReviewInterval { get; set; }
    public decimal EaseFactor { get; set; }
}
