import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';

import { Game, GameMode, GameStatus } from '../models/game.model';
import { Gameboard } from './gameboard';

describe('Gameboard', () => {
  let component: Gameboard;
  let fixture: ComponentFixture<Gameboard>;
  let httpTesting: HttpTestingController;

  const activeGame: Game = {
    gameId: 7,
    currentPlayer: 'X',
    boardStateId: 1,
    gameMode: GameMode.TwoPlayer,
    winner: null,
    gameStatus: GameStatus.Active,
    winningCells: [],
    moveHistory: [],
    boardState: {
      boardId: 1,
      cell1: null,
      cell2: null,
      cell3: null,
      cell4: null,
      cell5: null,
      cell6: null,
      cell7: null,
      cell8: null,
      cell9: null,
    },
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Gameboard],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { queryParamMap: { get: () => 'Two Player' } } },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Gameboard);
    component = fixture.componentInstance;
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create a two-player game and render the board', () => {
    fixture.detectChanges();
    const request = httpTesting.expectOne('/api/games');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ gameMode: GameMode.TwoPlayer });
    request.flush(activeGame);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelectorAll('.game-board .board-cell')).toHaveLength(9);
    expect(compiled.querySelector('.turn-indicator')?.textContent).toContain("Player X's turn");
    expect(compiled.querySelector('.empty-history')?.textContent).toContain('No moves yet');
  });

  it('should post a move when an empty cell is selected', () => {
    fixture.detectChanges();
    httpTesting.expectOne('/api/games').flush(activeGame);
    fixture.detectChanges();

    component.makeMove(4);

    const request = httpTesting.expectOne('/api/games/7/moves');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ cellIndex: 5 });
  });
});
