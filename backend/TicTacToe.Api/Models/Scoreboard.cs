namespace TicTacToe.Api.Models;

public class Scoreboard
{
    public int GameId { get; set; }
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }

    public virtual Game Game { get; set; } = null!;
}
