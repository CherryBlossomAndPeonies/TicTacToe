import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Game, GameMode, GameStatus } from '../models/game.model';
import { GameService } from '../services/game.service';
import { Scoreboard } from '../models/scoreboard.model';
import { ScoreboardService } from '../services/scoreboard.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-gameboard',
  imports: [RouterLink],
  templateUrl: './gameboard.html',
  styleUrl: './gameboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Gameboard implements OnInit {
  readonly selectedMode: string;
  cells: Array<'X' | 'O' | ''> = ['', '', '', '', '', '', '', '', ''];
  currentPlayer: 'X' | 'O' = 'X';
  game: Game | null = null;
  winningCells: number[] = [];
  errorMessage = '';
  isLoading = false;
  isGameActive = true;
  showCompletionPopup = false;
  scoreboard: Scoreboard = { id: 0, winsX: 0, winsO: 0, draws: 0 };

  get hasMoves(): boolean {
    return this.cells.some(cell => cell === 'X' || cell === 'O');
  }

  constructor(
    readonly route: ActivatedRoute,
    private readonly gameService: GameService,
    private readonly scoreboardService: ScoreboardService,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) {
    this.selectedMode = this.route.snapshot.queryParamMap.get('mode') ?? 'Two Player';
  }

  ngOnInit() {
    this.createGame();
  }

  makeMove(cellIndex: number) {
    if (!this.game || this.isLoading || this.game.gameStatus !== 1) {
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';

    if (this.game.gameMode === GameMode.SinglePlayer) {
      this.cells = this.cells.map((cell, index) => index === cellIndex ? 'X' : cell);
      this.currentPlayer = 'O';
      this.changeDetectorRef.markForCheck();
    }

    this.gameService.makeMove(this.game!.gameId, cellIndex + 1).subscribe({
      next: game => this.updateGame(game),
      error: (res) => this.handleError(res),
    });
  }

  resetBoard() {
    this.isLoading = true;
    this.errorMessage = '';

    if (this.game) {
      this.gameService.reset(this.game!.gameId).subscribe({
        next: game => this.updateGame(game),
        error: (res) => this.handleError(res),
      })
    }
  }

  undoMove() {
    this.isLoading = true;
    this.errorMessage = '';
    
    if (this.game) {
      this.gameService.undo(this.game!.gameId).subscribe({
        next: game => this.updateGame(game),
        error: (res) => this.handleError(res),
      })
    }
  }

  private createGame() {
    this.isLoading = true;
    this.errorMessage = '';
    this.gameService.createGame(this.getGameMode()).subscribe({
      next: game => this.updateGame(game),
      error: (error) => this.handleError(error),
    });
  }

  private updateGame(game: Game) {
    if (this.game?.gameId !== game.gameId) {
      this.scoreboard = { id: 0, winsX: 0, winsO: 0, draws: 0 };
    }

    this.game = game;
    this.winningCells = game.winningCells ?? [];
    this.isGameActive = game.gameStatus === GameStatus.Active;
    this.showCompletionPopup = !this.isGameActive;

    if (!this.isGameActive) {
      this.loadScoreboard(game.gameId);
    }

    this.currentPlayer = game.currentPlayer as 'X' | 'O';
    this.cells = [
      game.boardState.cell1,
      game.boardState.cell2,
      game.boardState.cell3,
      game.boardState.cell4,
      game.boardState.cell5,
      game.boardState.cell6,
      game.boardState.cell7,
      game.boardState.cell8,
      game.boardState.cell9,
    ].map(cell => cell as 'X' | 'O' | '');
    this.isLoading = false;
    this.changeDetectorRef.markForCheck();
  }

  closeCompletionPopup() {
    this.showCompletionPopup = false;
    this.changeDetectorRef.markForCheck();
  }

  private loadScoreboard(gameId: number) {
    this.scoreboardService.getScoreboard(gameId).subscribe({
      next: scoreboard => {
        this.scoreboard = scoreboard;
        this.changeDetectorRef.markForCheck();
      },
      error: () => undefined,
    });
  }

  private handleError(response: HttpErrorResponse) {
    this.isLoading = false;
    this.errorMessage = response.error;
    if (this.game) {
      this.updateGame(this.game);
    }
    this.changeDetectorRef.markForCheck();
  }

  private getGameMode(): GameMode {
    return this.selectedMode === 'Two Player' ? GameMode.TwoPlayer : GameMode.SinglePlayer;
  }
}
