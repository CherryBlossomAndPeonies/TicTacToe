import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-gameboard',
  imports: [],
  templateUrl: './gameboard.html',
  styleUrl: './gameboard.scss',
})
export class Gameboard {
  readonly selectedMode: string;
  cells: Array<'X' | 'O' | ''> = ['', '', '', '', '', '', '', '', ''];
  currentPlayer: 'X' | 'O' = 'X';

  constructor(readonly route: ActivatedRoute) {
    this.selectedMode = this.route.snapshot.queryParamMap.get('mode') ?? 'Two Player';
  }

  makeMove(cellIndex: number) {
    if (this.cells[cellIndex]) {
      return;
    }

    this.cells[cellIndex] = this.currentPlayer;
    this.currentPlayer = this.currentPlayer === 'X' ? 'O' : 'X';
  }

  resetBoard() {
    this.cells = ['', '', '', '', '', '', '', '', ''];
    this.currentPlayer = 'X';
  }
}
