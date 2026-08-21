export enum GameMode {
  SinglePlayer = 1,
  TwoPlayer = 2,
}

export enum GameStatus {
  Active = 1,
  Draw = 2,
  Completed = 3,
}

export interface Game {
  gameId: number;
  currentPlayer: string;
  boardStateId: number;
  gameMode: GameMode;
  winner: string | null;
  gameStatus: GameStatus;
  winningCells: number[];
  boardState: BoardState;
}

export interface BoardState {
  boardId: number;
  cell1: string | null;
  cell2: string | null;
  cell3: string | null;
  cell4: string | null;
  cell5: string | null;
  cell6: string | null;
  cell7: string | null;
  cell8: string | null;
  cell9: string | null;
}

export interface CreateGameRequest {
  gameMode: GameMode;
}

export interface MakeMoveRequest {
  cellIndex: number;
}
