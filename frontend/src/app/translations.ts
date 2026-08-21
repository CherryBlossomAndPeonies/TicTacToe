export const translations = {
  app: {
    title: 'Tic Tac Toe',
  },
  home: {
    boardPreviewLabel: 'Tic tac toe board preview',
    gameModesLabel: 'Game modes',
    modes: {
      twoPlayer: 'Two Player',
      playOnline: 'Play Online',
      scoreBoard: 'Score Board',
    },
  },
  gameboard: {
    yourTurn: 'Your turn',
    boardLabel: 'Tic tac toe game board',
    updatingGame: 'Updating game...',
    gameDrawn: 'Game drawn',
    playerWins: (player: string) => `Player ${player} wins`,
    playerTurn: (player: string) => `Player ${player}'s turn`,
    playerCell: (player: string, cell: number) => `Player ${player} at cell ${cell}`,
    emptyCell: (cell: number) => `Empty cell ${cell}`,
    serviceError: 'Unable to reach the game service.',
    resetBoard: 'Reset Board',
    undoMove: 'Undo Move',
  },
  scoreboard: {
    allGames: 'All games',
    title: 'Score Board',
    scoresLabel: 'Game scores',
    xWins: 'X wins',
    oWins: 'O wins',
    draws: 'Draws',
    reset: 'Reset Scoreboard',
    loadError: 'Unable to load the scoreboard.',
    resetError: 'Unable to reset the scoreboard.',
  },
} as const;
