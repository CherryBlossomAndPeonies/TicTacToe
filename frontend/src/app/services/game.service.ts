import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
	CreateGameRequest,
	Game,
	GameMode,
	MakeMoveRequest,
} from '../models/game.model';

@Injectable({ providedIn: 'root' })
export class GameService {
	private readonly gamesUrl = '/api/games';

	constructor(private readonly http: HttpClient) {}

	getGame(gameId: number): Observable<Game> {
		return this.http.get<Game>(`${this.gamesUrl}/${gameId}`);
	}

	createGame(gameMode: GameMode): Observable<Game> {
		const request: CreateGameRequest = { gameMode };
		return this.http.post<Game>(this.gamesUrl, request);
	}

	makeMove(gameId: number, cellIndex: number): Observable<Game> {
		const request: MakeMoveRequest = { cellIndex };
		return this.http.post<Game>(`${this.gamesUrl}/${gameId}/moves`, request);
	}

	undo(gameId: number): Observable<Game> {
		return this.http.post<Game>(`${this.gamesUrl}/${gameId}/undo`, {});
	}

	reset(gameId: number): Observable<Game> {
		return this.http.post<Game>(`${this.gamesUrl}/${gameId}/reset`, {});
	}
}
