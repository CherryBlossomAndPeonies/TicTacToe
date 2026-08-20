-- Migration: Create Games Table
-- Description: Creates the Games table to store TicTacToe game instances

CREATE TABLE IF NOT EXISTS Games (
    GameId INTEGER PRIMARY KEY AUTOINCREMENT,
    CurrentPlayer CHAR NOT NULL,
    BoardStateId INTEGER NOT NULL,
    GameMode INTEGER NOT NULL,
    Winner CHAR,
    GameStatus INTEGER NOT NULL,
    FOREIGN KEY (BoardStateId) REFERENCES BoardStates(BoardId)
);

-- Create index on BoardStateId for faster lookups
CREATE INDEX IF NOT EXISTS idx_games_board_state_id ON Games(BoardStateId);
