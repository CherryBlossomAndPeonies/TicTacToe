using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.DataAccess;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Dtos;

namespace TicTacToe.Api.Services;

public class ScoreboardService
{
    private readonly IDbContext dbContext;

    public ScoreboardService(IDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ScoreboardDto?> GetAsync(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await dbContext.Set<Scoreboard>()
            .AsNoTracking()
            .SingleOrDefaultAsync(currentScoreboard => currentScoreboard.GameId == gameId, cancellationToken);

        return scoreboard?.ToDto();
    }

    public async Task<ScoreboardDto?> ResetAsync(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await GetOrCreateAsync(gameId, cancellationToken);
        if (scoreboard is null)
        {
            return null;
        }

        ResetValues(scoreboard);
        await dbContext.SaveChangesAsync(cancellationToken);
        return scoreboard.ToDto();
    }

    internal async Task RecordResultAsync(int gameId, GameStatus gameStatus, char? winner, CancellationToken cancellationToken)
    {
        var scoreboard = await GetOrCreateAsync(gameId, cancellationToken);
        if (scoreboard is null)
        {
            return;
        }

        ApplyResult(scoreboard, gameStatus, winner, 1);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task RemoveResultAsync(int gameId, GameStatus gameStatus, char? winner, CancellationToken cancellationToken)
    {
        var scoreboard = await GetOrCreateAsync(gameId, cancellationToken);
        if (scoreboard is null)
        {
            return;
        }

        ApplyResult(scoreboard, gameStatus, winner, -1);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Scoreboard?> GetOrCreateAsync(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await dbContext.Set<Scoreboard>()
            .SingleOrDefaultAsync(currentScoreboard => currentScoreboard.GameId == gameId, cancellationToken);

        if (scoreboard is not null)
        {
            return scoreboard;
        }

        if (!await dbContext.Set<Game>().AnyAsync(game => game.GameId == gameId, cancellationToken))
        {
            return null;
        }

        scoreboard = new Scoreboard { GameId = gameId };
        dbContext.Set<Scoreboard>().Add(scoreboard);
        return scoreboard;
    }

    private static void ResetValues(Scoreboard scoreboard)
    {
        scoreboard.XWins = 0;
        scoreboard.OWins = 0;
        scoreboard.Draws = 0;
    }

    private static void ApplyResult(Scoreboard scoreboard, GameStatus gameStatus, char? winner, int amount)
    {
        if (gameStatus == GameStatus.Draw)
        {
            scoreboard.Draws += amount;
        }
        else if (gameStatus == GameStatus.Completed && winner == 'X')
        {
            scoreboard.XWins += amount;
        }
        else if (gameStatus == GameStatus.Completed && winner == 'O')
        {
            scoreboard.OWins += amount;
        }
    }
}

internal static class ScoreboardMappingExtensions
{
    public static ScoreboardDto ToDto(this Scoreboard scoreboard)
    {
        return new ScoreboardDto
        {
            GameId = scoreboard.GameId,
            XWins = scoreboard.XWins,
            OWins = scoreboard.OWins,
            Draws = scoreboard.Draws
        };
    }
}
