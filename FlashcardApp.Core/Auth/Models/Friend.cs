namespace FlashcardApp.Core.Models;

public class Friend
{
    public int Id { get; set; }
    public string UserId1 { get; set; }
    public string UserId2 { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
