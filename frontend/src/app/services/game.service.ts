import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Game } from '../models/game.model';

@Injectable({ providedIn: 'root' })
export class GameService {
	private readonly gamesUrl = '/api/games';

	constructor(private readonly http: HttpClient) {}

	getGame(gameId: number): Observable<Game> {
		return this.http.get<Game>(`${this.gamesUrl}/${gameId}`);
	}
}
