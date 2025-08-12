namespace FlashcardApp.Core.Models;

public class Score
{
    public int Id { get; set; }
    public int LobbyId { get; set; }
    public string UserId { get; set; }
    public int Points { get; set; }
    public DateTime CreatedAt { get; set; }
}