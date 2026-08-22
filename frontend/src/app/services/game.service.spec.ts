import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { GameMode } from '../models/game.model';
import { GameService } from './game.service';

describe('GameService', () => {
  let service: GameService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [GameService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(GameService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should get a game by id', () => {
    service.getGame(7).subscribe();

    const request = httpTesting.expectOne('/api/games/7');
    expect(request.request.method).toBe('GET');
    request.flush({});
  });

  it('should create a game with the selected mode', () => {
    service.createGame(GameMode.SinglePlayer).subscribe();

    const request = httpTesting.expectOne('/api/games');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ gameMode: GameMode.SinglePlayer });
    request.flush({});
  });

  it.each([
    ['makeMove', '/api/games/7/moves', { cellIndex: 3 }, (gameService: GameService) => gameService.makeMove(7, 3)],
    ['undo', '/api/games/7/undo', {}, (gameService: GameService) => gameService.undo(7)],
    ['reset', '/api/games/7/reset', {}, (gameService: GameService) => gameService.reset(7)],
  ])('should call the %s endpoint', (_name, url, body, call) => {
    call(service).subscribe();

    const request = httpTesting.expectOne(url);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
    request.flush({});
  });
});