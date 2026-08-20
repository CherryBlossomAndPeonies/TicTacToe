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
}
