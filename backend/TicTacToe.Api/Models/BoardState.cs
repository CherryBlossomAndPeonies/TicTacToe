using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Api.Models;

public class BoardState
{
    public int BoardId { get; set; }
    public char? Cell1 { get; set; }
    public char? Cell2 { get; set; }
    public char? Cell3 { get; set; }
    public char? Cell4 { get; set; }
    public char? Cell5 { get; set; }
    public char? Cell6 { get; set; }
    public char? Cell7 { get; set; }
    public char? Cell8 { get; set; }
    public char? Cell9 { get; set; }
}