namespace TicTacToe.Api.Models;

/// <summary>
/// Represents a TicTacToe game
/// </summary>
public class Game
{
    public Game()
    {
        BoardState = new BoardState();
        Scoreboard = new Scoreboard();
    }

    public int GameId { get; set; }

    /// <summary>
    /// Current player ('X' or 'O')
    /// </summary>
    public char CurrentPlayer { get; set; }

    /// <summary>
    /// Foreign key to Board table
    /// </summary>
    public int BoardStateId { get; set; }

    /// <summary>
    /// Game mode: 1 = Single Player, 2 = Two Player
    /// </summary>
    public GameMode GameMode { get; set; }

    /// <summary>
    /// Winner of the game ('X', 'O', or null if no winner yet)
    /// </summary>
    public char? Winner { get; set; }

    /// <summary>
    /// Current game status: 1 = Active, 2 = Draw, 3 = Completed
    /// </summary>
    public GameStatus GameStatus { get; set; }

    public virtual BoardState BoardState { get; set; }
    public virtual ICollection<GameMove> Moves { get; set; } = new List<GameMove>();
    public virtual Scoreboard Scoreboard { get; set; }
}

/// <summary>
/// Game mode enumeration
/// </summary>
public enum GameMode
{
    SinglePlayer = 1,
    TwoPlayer = 2
}

/// <summary>
/// Game status enumeration
/// </summary>
public enum GameStatus
{
    Active = 1,
    Draw = 2,
    Completed = 3
}
