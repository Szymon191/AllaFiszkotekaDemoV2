namespace FlashcardApp.Core.DTO;

public class FlashcardDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Word { get; set; }
    public string Translation { get; set; }
    public string ExampleUsage { get; set; }
    public string[] Tags { get; set; }
    public string Difficulty { get; set; }
}