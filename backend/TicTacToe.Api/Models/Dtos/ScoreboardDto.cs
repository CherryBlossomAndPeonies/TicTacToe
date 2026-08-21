namespace TicTacToe.Api.Models.Dtos;

public sealed class ScoreboardDto
{
    public int Id { get; init; }
    public int WinsX { get; init; }
    public int WinsO { get; init; }
    public int Draws { get; init; }
}
