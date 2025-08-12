namespace FlashcardApp.Core.Models;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string NotificationType { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public string RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    public DateTime CreatedAt { get; set; }
}