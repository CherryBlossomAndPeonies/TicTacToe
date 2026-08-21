import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Scoreboard } from '../models/scoreboard.model';

@Injectable({ providedIn: 'root' })
export class ScoreboardService {
	private readonly scoreboardUrl = '/api/scoreboard';

	constructor(private readonly http: HttpClient) {}

	getScoreboard(gameId: number): Observable<Scoreboard> {
		return this.http.get<Scoreboard>(`${this.scoreboardUrl}/${gameId}`);
	}

	resetScoreboard(gameId: number): Observable<Scoreboard> {
		return this.http.post<Scoreboard>(`${this.scoreboardUrl}/${gameId}/reset`, {});
	}
}