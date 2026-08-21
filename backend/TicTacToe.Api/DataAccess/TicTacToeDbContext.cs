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
            .ToTable("Scoreboard")
            .HasKey(scoreboard => scoreboard.Id);

        modelBuilder.Entity<Scoreboard>()
            .Property(scoreboard => scoreboard.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Scoreboard>()
            .Property(scoreboard => scoreboard.WinsX)
            .HasDefaultValue(0);

        modelBuilder.Entity<Scoreboard>()
            .Property(scoreboard => scoreboard.WinsO)
            .HasDefaultValue(0);

        modelBuilder.Entity<Scoreboard>()
            .Property(scoreboard => scoreboard.Draws)
            .HasDefaultValue(0);
    }
}
