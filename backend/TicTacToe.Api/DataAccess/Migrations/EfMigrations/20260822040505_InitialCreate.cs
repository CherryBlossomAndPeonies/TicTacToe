using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Api.DataAccess.Migrations.EfMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardState",
                columns: table => new
                {
                    BoardId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cell1 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell2 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell3 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell4 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell5 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell6 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell7 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell8 = table.Column<char>(type: "TEXT", nullable: true),
                    Cell9 = table.Column<char>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardState", x => x.BoardId);
                });

            migrationBuilder.CreateTable(
                name: "Scoreboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    WinsX = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    WinsO = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Draws = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scoreboard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CurrentPlayer = table.Column<char>(type: "TEXT", nullable: false),
                    BoardStateId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameMode = table.Column<int>(type: "INTEGER", nullable: false),
                    Winner = table.Column<char>(type: "TEXT", nullable: true),
                    GameStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Game_BoardState_BoardStateId",
                        column: x => x.BoardStateId,
                        principalTable: "BoardState",
                        principalColumn: "BoardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameMove",
                columns: table => new
                {
                    GameMoveId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    CellIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Player = table.Column<char>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMove", x => x.GameMoveId);
                    table.ForeignKey(
                        name: "FK_GameMove_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_BoardStateId",
                table: "Game",
                column: "BoardStateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameMove_GameId",
                table: "GameMove",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameMove");

            migrationBuilder.DropTable(
                name: "Scoreboard");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "BoardState");
        }
    }
}
