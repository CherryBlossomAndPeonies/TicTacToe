using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Dtos;

namespace TicTacToe.Api.Services;

public static class GameMappingExtensions
{
    public static GameDto ToDto(this Game game)
    {
        return new GameDto
        {
            GameId = game.GameId,
            CurrentPlayer = game.CurrentPlayer,
            BoardStateId = game.BoardStateId,
            GameMode = game.GameMode,
            Winner = game.Winner,
            GameStatus = game.GameStatus,
            WinningCells = GetWinningCells(game.BoardState, game.Winner),
            BoardState = game.BoardState.ToDto()
        };
    }

    private static IReadOnlyList<int> GetWinningCells(BoardState boardState, char? winner)
    {
        if (winner is null)
        {
            return Array.Empty<int>();
        }

        var cells = new[]
        {
            boardState.Cell1, boardState.Cell2, boardState.Cell3,
            boardState.Cell4, boardState.Cell5, boardState.Cell6,
            boardState.Cell7, boardState.Cell8, boardState.Cell9
        };

        var winningLines = new[]
        {
            new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 },
            new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, new[] { 3, 6, 9 },
            new[] { 1, 5, 9 }, new[] { 3, 5, 7 }
        };

        return winningLines.FirstOrDefault(line => line.All(cellIndex => cells[cellIndex - 1] == winner))
            ?? Array.Empty<int>();
    }

    private static BoardStateDto ToDto(this BoardState boardState)
    {
        return new BoardStateDto
        {
            BoardId = boardState.BoardId,
            Cell1 = boardState.Cell1,
            Cell2 = boardState.Cell2,
            Cell3 = boardState.Cell3,
            Cell4 = boardState.Cell4,
            Cell5 = boardState.Cell5,
            Cell6 = boardState.Cell6,
            Cell7 = boardState.Cell7,
            Cell8 = boardState.Cell8,
            Cell9 = boardState.Cell9
        };
    }
}
