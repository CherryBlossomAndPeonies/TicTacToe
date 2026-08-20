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
            BoardState = game.BoardState.ToDto()
        };
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
