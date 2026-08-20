namespace TicTacToe.Api.Models;

public class GameMove
{
    public int GameMoveId { get; set; }
    public int GameId { get; set; }
    public int CellIndex { get; set; }
    public char Player { get; set; }
    public DateTime PlayedAt { get; set; }

    public virtual Game Game { get; set; } = null!;
}
