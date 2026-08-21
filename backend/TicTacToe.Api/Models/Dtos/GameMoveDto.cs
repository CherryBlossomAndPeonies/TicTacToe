namespace TicTacToe.Api.Models.Dtos;

public sealed class GameMoveDto
{
    public int MoveNumber { get; init; }
    public char Player { get; init; }
    public required string Position { get; init; }
}
