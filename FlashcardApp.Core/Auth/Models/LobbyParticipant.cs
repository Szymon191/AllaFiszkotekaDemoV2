namespace FlashcardApp.Core.Models;

public class LobbyParticipant
{
    public int Id { get; set; }
    public int LobbyId { get; set; }
    public string UserId { get; set; }
    public DateTime JoinedAt { get; set; }
}