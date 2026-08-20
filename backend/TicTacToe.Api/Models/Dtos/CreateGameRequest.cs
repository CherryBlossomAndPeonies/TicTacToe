using System.ComponentModel.DataAnnotations;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Models.Dtos;

public class CreateGameRequest
{
    [EnumDataType(typeof(GameMode))]
    public GameMode GameMode { get; init; } = GameMode.TwoPlayer;
}
