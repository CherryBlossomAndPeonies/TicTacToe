import { Routes } from '@angular/router';
import { Gameboard } from './gameboard/gameboard';
import { Home } from './home/home';

export const routes: Routes = [
	{ path: '', component: Home },
	{ path: 'gameboard', component: Gameboard },
	{ path: '**', redirectTo: '' },
];
