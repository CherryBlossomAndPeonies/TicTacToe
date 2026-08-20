-- Migration: Create BoardState Table
-- Description: Creates the BoardState table to store the nine cells of a TicTacToe board

CREATE TABLE IF NOT EXISTS BoardStates (
    BoardId INTEGER PRIMARY KEY AUTOINCREMENT,
    Cell1 CHAR,
    Cell2 CHAR,
    Cell3 CHAR,
    Cell4 CHAR,
    Cell5 CHAR,
    Cell6 CHAR,
    Cell7 CHAR,
    Cell8 CHAR,
    Cell9 CHAR
);
