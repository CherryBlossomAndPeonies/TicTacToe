using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models.Dtos;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GameController(GameService gameService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<GameDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<GameDto>> CreateGame(
        CreateGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = await gameService.CreateGameAsync(request.GameMode, cancellationToken);
        return CreatedAtAction(nameof(GetGame), new { gameId = game.GameId }, game);
    }

    [HttpGet("{gameId:int}")]
    [ProducesResponseType<GameDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetGame(int gameId, CancellationToken cancellationToken)
    {
        var game = await gameService.GetGameAsync(gameId, cancellationToken);

        return game is null ? NotFound() : Ok(game);
    }

    [HttpPost("{gameId:int}/moves")]
    [ProducesResponseType<GameDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> MakeMove(
        int gameId,
        MakeMoveRequest request,
        CancellationToken cancellationToken)
    {
        return await ExecuteGameActionAsync(
            () => gameService.MakeMoveAsync(gameId, request.CellIndex, cancellationToken));
    }

    [HttpPost("{gameId:int}/undo")]
    public async Task<ActionResult<GameDto>> Undo(int gameId, CancellationToken cancellationToken)
    {
        return await ExecuteGameActionAsync(() => gameService.UndoAsync(gameId, cancellationToken));
    }

    [HttpPost("{gameId:int}/reset")]
    public async Task<ActionResult<GameDto>> Reset(int gameId, CancellationToken cancellationToken)
    {
        return await ExecuteGameActionAsync(() => gameService.ResetAsync(gameId, cancellationToken));
    }

    private static async Task<ActionResult<GameDto>> ExecuteGameActionAsync(
        Func<Task<GameDto>> action)
    {
        try
        {
            return new OkObjectResult(await action());
        }
        catch (KeyNotFoundException exception)
        {
            return new NotFoundObjectResult(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return new BadRequestObjectResult(new { error = exception.Message });
        }
    }
}
