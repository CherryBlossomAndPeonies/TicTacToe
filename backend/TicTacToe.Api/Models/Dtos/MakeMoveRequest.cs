using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Api.Models.Dtos;

public class MakeMoveRequest
{
    [Range(1, 9)]
    public int CellIndex { get; init; }
}
