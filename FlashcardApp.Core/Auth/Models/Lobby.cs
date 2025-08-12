namespace FlashcardApp.Core.Models;

public class Lobby
{
    public int Id { get; set; }
    public string CreatorId { get; set; }
    public int? FlashcardCategoryId { get; set; }
    public string GameMode { get; set; }
    public string Status { get; set; }
    public int MaxPlayers { get; set; }
    public DateTime CreatedAt { get; set; }
}
