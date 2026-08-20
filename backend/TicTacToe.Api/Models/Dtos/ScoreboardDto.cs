namespace TicTacToe.Api.Models.Dtos;

public sealed class ScoreboardDto
{
    public int GameId { get; init; }
    public int XWins { get; init; }
    public int OWins { get; init; }
    public int Draws { get; init; }
}
