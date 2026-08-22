import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { ScoreboardService } from './scoreboard.service';

describe('ScoreboardService', () => {
  let service: ScoreboardService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ScoreboardService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ScoreboardService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should get a scoreboard by game id', () => {
    service.getScoreboard(7).subscribe();

    const request = httpTesting.expectOne('/api/scoreboard/7');
    expect(request.request.method).toBe('GET');
    request.flush({ id: 7, winsX: 1, winsO: 2, draws: 3 });
  });

  it('should reset a scoreboard by game id', () => {
    service.resetScoreboard(7).subscribe();

    const request = httpTesting.expectOne('/api/scoreboard/7/reset');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});
    request.flush({ id: 7, winsX: 0, winsO: 0, draws: 0 });
  });
});