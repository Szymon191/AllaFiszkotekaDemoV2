namespace FlashcardApp.Core.Models;

public class Lesson
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
