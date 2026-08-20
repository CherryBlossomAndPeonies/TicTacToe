using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.DataAccess;

public class TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options) : DbContext(options), IDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoardState>()
            .HasKey(boardState => boardState.BoardId);

        modelBuilder.Entity<Game>()
            .HasOne(game => game.BoardState)
            .WithOne()
            .HasForeignKey<Game>(game => game.BoardStateId)
            .IsRequired();

        modelBuilder.Entity<GameMove>()
            .HasOne(gameMove => gameMove.Game)
            .WithMany(game => game.Moves)
            .HasForeignKey(gameMove => gameMove.GameId)
            .IsRequired();

        modelBuilder.Entity<Scoreboard>()
            .HasKey(scoreboard => scoreboard.GameId);

        modelBuilder.Entity<Scoreboard>()
            .HasOne(scoreboard => scoreboard.Game)
            .WithOne(game => game.Scoreboard)
            .HasForeignKey<Scoreboard>(scoreboard => scoreboard.GameId)
            .IsRequired();
    }
}
