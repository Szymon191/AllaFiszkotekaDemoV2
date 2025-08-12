namespace FlashcardApp.Core.Models;

public class FlashcardCategory
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
}