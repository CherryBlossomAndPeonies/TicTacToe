-- Migration: Create GameMoves Table
-- Description: Stores game move history so the last move can be undone

CREATE TABLE IF NOT EXISTS GameMoves (
    GameMoveId INTEGER PRIMARY KEY AUTOINCREMENT,
    GameId INTEGER NOT NULL,
    CellIndex INTEGER NOT NULL,
    Player CHAR NOT NULL,
    PlayedAt TEXT NOT NULL,
    FOREIGN KEY (GameId) REFERENCES Games(GameId)
);

CREATE INDEX IF NOT EXISTS idx_game_moves_game_id ON GameMoves(GameId);
