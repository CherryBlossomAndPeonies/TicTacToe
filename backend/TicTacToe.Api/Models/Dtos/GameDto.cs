namespace TicTacToe.Api.Models.Dtos;

public sealed class GameDto
{
    public int GameId { get; init; }
    public char CurrentPlayer { get; init; }
    public int BoardStateId { get; init; }
    public GameMode GameMode { get; init; }
    public char? Winner { get; init; }
    public GameStatus GameStatus { get; init; }
    public IReadOnlyList<int> WinningCells { get; init; } = Array.Empty<int>();
    public IReadOnlyList<GameMoveDto> MoveHistory { get; init; } = Array.Empty<GameMoveDto>();
    public required BoardStateDto BoardState { get; init; }
}
