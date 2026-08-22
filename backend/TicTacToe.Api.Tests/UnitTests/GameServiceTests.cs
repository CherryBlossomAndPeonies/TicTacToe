using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.DataAccess;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Dtos;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Tests;

public sealed class GameServiceTests : IDisposable
{
    private readonly SqliteConnection connection;
    private readonly TicTacToeDbContext dbContext;
    private readonly GameService gameService;

    public GameServiceTests()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        dbContext = new TicTacToeDbContext(
            new DbContextOptionsBuilder<TicTacToeDbContext>()
                .UseSqlite(connection)
                .Options);
        dbContext.Database.EnsureCreated();
        gameService = new GameService(dbContext, new ScoreboardService(dbContext));
    }

    [Fact]
    public async Task MakeMoveAsync_ValidMove_UpdatesBoardAndHistory()
    {
        var game = await CreateGameAsync();

        var result = await gameService.MakeMoveAsync(game.GameId, 1, CancellationToken.None);

        Assert.Equal('X', result.BoardState.Cell1);
        Assert.Equal('O', result.CurrentPlayer);
        Assert.Single(result.MoveHistory);
        Assert.Equal("Row 1, Column 1", result.MoveHistory[0].Position);
    }

    [Fact]
    public async Task MakeMoveAsync_InvalidCell_Throws()
    {
        var game = await CreateGameAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            gameService.MakeMoveAsync(game.GameId, 10, CancellationToken.None));
    }

    [Fact]
    public async Task MakeMoveAsync_AlternatesTurns()
    {
        var game = await CreateGameAsync();

        var afterX = await gameService.MakeMoveAsync(game.GameId, 1, CancellationToken.None);
        var afterO = await gameService.MakeMoveAsync(game.GameId, 2, CancellationToken.None);

        Assert.Equal('O', afterX.CurrentPlayer);
        Assert.Equal('X', afterO.CurrentPlayer);
    }

    [Fact]
    public async Task MakeMoveAsync_RowWin_CompletesGame()
    {
        var result = await PlayMovesAsync(GameMode.TwoPlayer, 1, 4, 2, 5, 3);

        Assert.Equal(GameStatus.Completed, result.GameStatus);
        Assert.Equal('X', result.Winner);
    }

    [Fact]
    public async Task MakeMoveAsync_ColumnWin_CompletesGame()
    {
        var result = await PlayMovesAsync(GameMode.TwoPlayer, 1, 2, 4, 5, 7);

        Assert.Equal(GameStatus.Completed, result.GameStatus);
        Assert.Equal('X', result.Winner);
    }

    [Fact]
    public async Task MakeMoveAsync_DiagonalWin_CompletesGame()
    {
        var result = await PlayMovesAsync(GameMode.TwoPlayer, 1, 2, 5, 3, 9);

        Assert.Equal(GameStatus.Completed, result.GameStatus);
        Assert.Equal('X', result.Winner);
    }

    [Fact]
    public async Task MakeMoveAsync_FullBoardWithoutWinner_IsDraw()
    {
        var result = await PlayMovesAsync(GameMode.TwoPlayer, 1, 2, 3, 5, 4, 6, 8, 7, 9);

        Assert.Equal(GameStatus.Draw, result.GameStatus);
        Assert.Null(result.Winner);
    }

    [Fact]
    public async Task ResetAsync_ClearsBoardHistoryAndStatus()
    {
        var game = await CreateGameAsync();
        await gameService.MakeMoveAsync(game.GameId, 1, CancellationToken.None);

        var result = await gameService.ResetAsync(game.GameId, CancellationToken.None);

        Assert.Equal(game.GameId, result.GameId);
        Assert.Equal(GameStatus.Active, result.GameStatus);
        Assert.Equal('X', result.CurrentPlayer);
        Assert.Empty(result.MoveHistory);
        Assert.Null(result.BoardState.Cell1);
    }

    [Fact]
    public async Task UndoAsync_TwoPlayer_RemovesOnlyLatestMoveAndRestoresItsTurn()
    {
        var game = await CreateGameAsync();
        await gameService.MakeMoveAsync(game.GameId, 1, CancellationToken.None);
        await gameService.MakeMoveAsync(game.GameId, 2, CancellationToken.None);

        var result = await gameService.UndoAsync(game.GameId, CancellationToken.None);

        Assert.Equal('O', result.CurrentPlayer);
        Assert.Equal('X', result.BoardState.Cell1);
        Assert.Null(result.BoardState.Cell2);
        Assert.Single(result.MoveHistory);
    }

    [Fact]
    public async Task UndoAsync_SinglePlayer_RemovesHumanAndComputerMoves()
    {
        var game = await CreateGameAsync(GameMode.SinglePlayer);
        await gameService.MakeMoveAsync(game.GameId, 1, CancellationToken.None);

        var result = await gameService.UndoAsync(game.GameId, CancellationToken.None);

        Assert.Equal('X', result.CurrentPlayer);
        Assert.Empty(result.MoveHistory);
        Assert.All(new[]
        {
            result.BoardState.Cell1, result.BoardState.Cell2, result.BoardState.Cell3,
            result.BoardState.Cell4, result.BoardState.Cell5, result.BoardState.Cell6,
            result.BoardState.Cell7, result.BoardState.Cell8, result.BoardState.Cell9
        }, Assert.Null);
    }

    [Fact]
    public async Task MakeMoveAsync_CompletedGame_RejectsAnotherMove()
    {
        var result = await PlayMovesAsync(GameMode.TwoPlayer, 1, 4, 2, 5, 3);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            gameService.MakeMoveAsync(result.GameId, 6, CancellationToken.None));
    }

    [Fact]
    public async Task MakeMoveAsync_CompletedWin_UpdatesScoreboardOnce()
    {
        var result = await PlayMovesAsync(GameMode.TwoPlayer, 1, 4, 2, 5, 3);
        var scoreboard = await new ScoreboardService(dbContext).GetAsync(result.GameId, CancellationToken.None);

        Assert.Equal(1, scoreboard.WinsX);
        Assert.Equal(0, scoreboard.WinsO);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public async Task MakeMoveAsync_SinglePlayer_TakesCenterWhenAvailable()
    {
        var game = await CreateGameAsync(GameMode.SinglePlayer);

        var result = await gameService.MakeMoveAsync(game.GameId, 1, CancellationToken.None);

        Assert.Equal('O', result.BoardState.Cell5);
    }

    [Fact]
    public async Task MakeMoveAsync_SinglePlayer_BlocksWinningXMove()
    {
        var game = await CreateGameAsync(GameMode.SinglePlayer);
        await AddMoveAsync(game, 1, 'X');
        await AddMoveAsync(game, 5, 'O');
        game.CurrentPlayer = 'X';
        await dbContext.SaveChangesAsync();

        var result = await gameService.MakeMoveAsync(game.GameId, 2, CancellationToken.None);

        Assert.Equal('O', result.BoardState.Cell3);
    }

    [Fact]
    public async Task MakeMoveAsync_SinglePlayer_MakesWinningMove()
    {
        var game = await CreateGameAsync(GameMode.SinglePlayer);
        await AddMoveAsync(game, 2, 'X');
        await AddMoveAsync(game, 5, 'O');
        await AddMoveAsync(game, 6, 'X');
        await AddMoveAsync(game, 3, 'O');
        game.CurrentPlayer = 'X';
        await dbContext.SaveChangesAsync();

        var result = await gameService.MakeMoveAsync(game.GameId, 8, CancellationToken.None);

        Assert.Equal('O', result.BoardState.Cell7);
        Assert.Equal('O', result.Winner);
    }

    private async Task<Game> CreateGameAsync(GameMode mode = GameMode.TwoPlayer)
    {
        var result = await gameService.CreateGameAsync(mode, CancellationToken.None);
        return await dbContext.Set<Game>().SingleAsync(game => game.GameId == result.GameId);
    }

    private async Task<GameDto> PlayMovesAsync(GameMode mode, params int[] cells)
    {
        var game = await CreateGameAsync(mode);
        var result = game.ToDto();
        foreach (var cell in cells)
        {
            result = await gameService.MakeMoveAsync(game.GameId, cell, CancellationToken.None);
        }

        return result;
    }

    private async Task AddMoveAsync(Game game, int cellIndex, char player)
    {
        SetCell(game.BoardState, cellIndex, player);
        game.Moves.Add(new GameMove { CellIndex = cellIndex, Player = player });
        await dbContext.SaveChangesAsync();
    }

    private static void SetCell(BoardState boardState, int cellIndex, char value)
    {
        typeof(BoardState).GetProperty($"Cell{cellIndex}")!.SetValue(boardState, value);
    }

    public void Dispose()
    {
        dbContext.Dispose();
        connection.Dispose();
    }
}
