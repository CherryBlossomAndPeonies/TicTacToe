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

    public async Task<ScoreboardDto> GetAsync(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await GetScoreboardAsync(gameId, cancellationToken);
        return scoreboard.ToDto();
    }

    public async Task<ScoreboardDto> ResetAsync(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await GetScoreboardAsync(gameId, cancellationToken);
        ResetValues(scoreboard);
        await dbContext.SaveChangesAsync(cancellationToken);
        return scoreboard.ToDto();
    }

    internal async Task RecordResultAsync(int gameId, GameStatus gameStatus, char? winner, CancellationToken cancellationToken)
    {
        var scoreboard = await GetOrCreateScoreboardAsync(gameId, cancellationToken);
        ApplyResult(scoreboard, gameStatus, winner, 1);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Scoreboard> GetOrCreateScoreboardAsync(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await dbContext.Set<Scoreboard>()
            .SingleOrDefaultAsync(currentScoreboard => currentScoreboard.Id == gameId, cancellationToken);

        if (scoreboard is null)
        {
            scoreboard = new Scoreboard { Id = gameId };
            dbContext.Set<Scoreboard>().Add(scoreboard);
        }

        return scoreboard;
    }

    private async Task<Scoreboard> GetScoreboardAsync(int gameId, CancellationToken cancellationToken)
    {
        return await dbContext.Set<Scoreboard>()
            .SingleOrDefaultAsync(currentScoreboard => currentScoreboard.Id == gameId, cancellationToken)
            ?? throw new KeyNotFoundException($"Scoreboard for game {gameId} was not found.");
    }

    private static void ResetValues(Scoreboard scoreboard)
    {
        scoreboard.WinsX = 0;
        scoreboard.WinsO = 0;
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
            scoreboard.WinsX += amount;
        }
        else if (gameStatus == GameStatus.Completed && winner == 'O')
        {
            scoreboard.WinsO += amount;
        }
    }
}

internal static class ScoreboardMappingExtensions
{
    public static ScoreboardDto ToDto(this Scoreboard scoreboard)
    {
        return new ScoreboardDto
        {
            Id = scoreboard.Id,
            WinsX = scoreboard.WinsX,
            WinsO = scoreboard.WinsO,
            Draws = scoreboard.Draws,
        };
    }
}
