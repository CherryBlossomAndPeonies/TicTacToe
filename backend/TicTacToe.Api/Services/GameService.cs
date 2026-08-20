using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.DataAccess;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Dtos;

namespace TicTacToe.Api.Services;

public class GameService
{
    private readonly IDbContext dbContext;
    private readonly ScoreboardService scoreboardService;

    public GameService(IDbContext dbContext, ScoreboardService scoreboardService)
    {
        this.dbContext = dbContext;
        this.scoreboardService = scoreboardService;
    }

    public async Task<GameDto> CreateGameAsync(GameMode gameMode, CancellationToken cancellationToken)
    {
        var game = new Game
        {
            CurrentPlayer = 'X',
            GameMode = gameMode,
            GameStatus = GameStatus.Active
        };

        dbContext.Set<Game>().Add(game);
        await dbContext.SaveChangesAsync(cancellationToken);
        return game.ToDto();
    }

    public async Task<GameDto?> GetGameAsync(int gameId, CancellationToken cancellationToken)
    {
        var game = await GetGameQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(currentGame => currentGame.GameId == gameId, cancellationToken);

        return game?.ToDto();
    }

    public async Task<GameDto> MakeMoveAsync(int gameId, int cellIndex, CancellationToken cancellationToken)
    {
        if (cellIndex is < 1 or > 9)
        {
            throw new InvalidOperationException("CellIndex must be between 1 and 9.");
        }

        var game = await GetGameQuery()
            .SingleOrDefaultAsync(currentGame => currentGame.GameId == gameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Game {gameId} was not found.");

        if (game.GameStatus != GameStatus.Active)
        {
            throw new InvalidOperationException("The game is already finished.");
        }

        if (GetCell(game.BoardState, cellIndex) is not null)
        {
            throw new InvalidOperationException("That cell is already occupied.");
        }

        var player = game.CurrentPlayer;
        SetCell(game.BoardState, cellIndex, player);
        game.Moves.Add(new GameMove
        {
            CellIndex = cellIndex,
            Player = player,
            PlayedAt = DateTime.UtcNow
        });

        if (HasWinner(game.BoardState, player))
        {
            game.Winner = player;
            game.GameStatus = GameStatus.Completed;
        }
        else if (IsDraw(game.BoardState))
        {
            game.GameStatus = GameStatus.Draw;
        }
        else
        {
            game.CurrentPlayer = player == 'X' ? 'O' : 'X';
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (game.GameStatus != GameStatus.Active)
        {
            await scoreboardService.RecordResultAsync(game.GameId, game.GameStatus, game.Winner, cancellationToken);
        }

        return game.ToDto();
    }

    public async Task<GameDto> UndoAsync(int gameId, CancellationToken cancellationToken)
    {
        var game = await GetGameQuery()
            .SingleOrDefaultAsync(currentGame => currentGame.GameId == gameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Game {gameId} was not found.");

        var lastMove = game.Moves.OrderByDescending(move => move.GameMoveId).FirstOrDefault()
            ?? throw new InvalidOperationException("There are no moves to undo.");

        var previousStatus = game.GameStatus;
        var previousWinner = game.Winner;
        SetCell(game.BoardState, lastMove.CellIndex, null);
        game.Moves.Remove(lastMove);
        game.CurrentPlayer = lastMove.Player;
        game.Winner = null;
        game.GameStatus = GameStatus.Active;

        await dbContext.SaveChangesAsync(cancellationToken);

        if (previousStatus != GameStatus.Active)
        {
            await scoreboardService.RemoveResultAsync(game.GameId, previousStatus, previousWinner, cancellationToken);
        }

        return game.ToDto();
    }

    public async Task<GameDto> ResetAsync(int gameId, CancellationToken cancellationToken)
    {
        var game = await GetGameQuery()
            .SingleOrDefaultAsync(currentGame => currentGame.GameId == gameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Game {gameId} was not found.");

        var previousStatus = game.GameStatus;
        var previousWinner = game.Winner;
        SetCellValues(game.BoardState, null);
        game.Moves.Clear();
        game.CurrentPlayer = 'X';
        game.Winner = null;
        game.GameStatus = GameStatus.Active;

        await dbContext.SaveChangesAsync(cancellationToken);

        if (previousStatus != GameStatus.Active)
        {
            await scoreboardService.RemoveResultAsync(game.GameId, previousStatus, previousWinner, cancellationToken);
        }

        return game.ToDto();
    }

    private IQueryable<Game> GetGameQuery()
    {
        return dbContext.Set<Game>()
            .Include(game => game.BoardState)
            .Include(game => game.Moves);
    }

    private static char? GetCell(BoardState boardState, int cellIndex) => cellIndex switch
    {
        1 => boardState.Cell1,
        2 => boardState.Cell2,
        3 => boardState.Cell3,
        4 => boardState.Cell4,
        5 => boardState.Cell5,
        6 => boardState.Cell6,
        7 => boardState.Cell7,
        8 => boardState.Cell8,
        9 => boardState.Cell9,
        _ => throw new ArgumentOutOfRangeException(nameof(cellIndex))
    };

    private static void SetCell(BoardState boardState, int cellIndex, char? value)
    {
        switch (cellIndex)
        {
            case 1: boardState.Cell1 = value; break;
            case 2: boardState.Cell2 = value; break;
            case 3: boardState.Cell3 = value; break;
            case 4: boardState.Cell4 = value; break;
            case 5: boardState.Cell5 = value; break;
            case 6: boardState.Cell6 = value; break;
            case 7: boardState.Cell7 = value; break;
            case 8: boardState.Cell8 = value; break;
            case 9: boardState.Cell9 = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(cellIndex));
        }
    }

    private static void SetCellValues(BoardState boardState, char? value)
    {
        boardState.Cell1 = value;
        boardState.Cell2 = value;
        boardState.Cell3 = value;
        boardState.Cell4 = value;
        boardState.Cell5 = value;
        boardState.Cell6 = value;
        boardState.Cell7 = value;
        boardState.Cell8 = value;
        boardState.Cell9 = value;
    }

    private static bool HasWinner(BoardState boardState, char player)
    {
        var cells = new[]
        {
            boardState.Cell1, boardState.Cell2, boardState.Cell3,
            boardState.Cell4, boardState.Cell5, boardState.Cell6,
            boardState.Cell7, boardState.Cell8, boardState.Cell9
        };

        var winningLines = new[]
        {
            new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 },
            new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 },
            new[] { 0, 4, 8 }, new[] { 2, 4, 6 }
        };

        return winningLines.Any(line => line.All(index => cells[index] == player));
    }

    private static bool IsDraw(BoardState boardState)
    {
        return new[]
        {
            boardState.Cell1, boardState.Cell2, boardState.Cell3,
            boardState.Cell4, boardState.Cell5, boardState.Cell6,
            boardState.Cell7, boardState.Cell8, boardState.Cell9
        }.All(cell => cell is not null);
    }
}
