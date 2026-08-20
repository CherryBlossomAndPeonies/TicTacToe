using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models.Dtos;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController(ScoreboardService scoreboardService) : ControllerBase
{
    [HttpGet("{gameId:int}")]
    [ProducesResponseType<ScoreboardDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ScoreboardDto>> Get(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await scoreboardService.GetAsync(gameId, cancellationToken);
        return scoreboard is null ? NotFound() : Ok(scoreboard);
    }

    [HttpPost("{gameId:int}/reset")]
    [ProducesResponseType<ScoreboardDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ScoreboardDto>> Reset(int gameId, CancellationToken cancellationToken)
    {
        var scoreboard = await scoreboardService.ResetAsync(gameId, cancellationToken);
        return scoreboard is null ? NotFound() : Ok(scoreboard);
    }
}
