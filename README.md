# Tic Tac Toe

A full-stack Tic Tac Toe game with an ASP.NET Core API, SQLite persistence, and an Angular web client. The application supports two-player games, a single-player mode with a computer opponent, move history, undo/reset actions, and game score tracking.

## Project Overview

The repository is split into two applications:

- `backend/`: ASP.NET Core Web API targeting .NET 10, with Entity Framework Core and SQLite.
- `frontend/`: Angular 21 single-page application that communicates with the API through the Angular development proxy.

In development, the API creates and migrates the local `tictactoe.db` SQLite database automatically.

## Tech Stack

- C# and ASP.NET Core Web API
- .NET 10
- Entity Framework Core 10 with SQLite
- OpenAPI in the ASP.NET Core development environment
- Angular 21 and TypeScript
- RxJS and Angular HttpClient
- Vitest through the Angular CLI for frontend unit tests
- xUnit for backend tests

## Features Implemented

- Start a new Tic Tac Toe game.
- Two-player mode with alternating X and O turns.
- Single-player mode with a computer opponent.
- Server-side validation for cell indexes, occupied cells, turns, and completed games.
- Win and draw detection with winning-cell highlighting.
- Move history display.
- Undo the latest move, or the latest human/computer turn in single-player mode.
- Reset the current game board.
- Per-game scoreboard with X wins, O wins, and draws.
- Reset the scoreboard for a game.
- Responsive Angular game interface with accessible labels, status text, and keyboard-operable buttons.
- CORS configuration for the local Angular origin.

## Prerequisites

Install the following before running the project:

- .NET 10 SDK
- Node.js and npm
- A modern browser

Install frontend dependencies once:

```bash
cd frontend
npm install
cd ..
```

## Run the Backend Locally

From the repository root:

```bash
dotnet run --project backend/TicTacToe.Api/TicTacToe.Api.csproj --launch-profile https
```

The API is then available at:

- HTTPS: `https://localhost:7038`
- HTTP: `http://localhost:5078`

The default development connection string is `Data Source=tictactoe.db`. The database file is created in the API process working directory when migrations run. The HTTPS development certificate may need to be trusted on a new machine:

```bash
dotnet dev-certs https --trust
```

## Run the Frontend Locally

Start the backend first, then open another terminal at the repository root:

```bash
cd frontend
npm start
```

Open `http://localhost:4200/`. The Angular development proxy forwards `/api` requests to `https://localhost:7038` and is configured to accept the local development certificate.

For a production build:

```bash
cd frontend
npm run build
```

## API Endpoint Summary

All endpoints use the `/api` prefix and return JSON.

| Method | Route                            | Description                                                                                       | Success       |
| ------ | -------------------------------- | ------------------------------------------------------------------------------------------------- | ------------- |
| `POST` | `/api/games`                     | Create a game. Body: `{ "gameMode": 1 }` for single-player or `{ "gameMode": 2 }` for two-player. | `201 Created` |
| `GET`  | `/api/games/{gameId}`            | Get a game, board state, and move history.                                                        | `200 OK`      |
| `POST` | `/api/games/{gameId}/moves`      | Make a move. Body: `{ "cellIndex": 1 }` through `{ "cellIndex": 9 }`.                             | `200 OK`      |
| `POST` | `/api/games/{gameId}/undo`       | Undo the latest eligible move or turn.                                                            | `200 OK`      |
| `POST` | `/api/games/{gameId}/reset`      | Reset the board and move history for a game.                                                      | `200 OK`      |
| `GET`  | `/api/scoreboard/{gameId}`       | Get the scoreboard for a game.                                                                    | `200 OK`      |
| `POST` | `/api/scoreboard/{gameId}/reset` | Reset the scoreboard counters for a game.                                                         | `200 OK`      |

Game actions can return `400 Bad Request` for invalid operations and `404 Not Found` when the requested game does not exist. The game creation endpoint returns a `Location` header pointing to the new game resource.

## Run Tests

Run all backend unit and end-to-end tests from the repository root:

```bash
dotnet test backend/TicTacToe.sln
```

Run frontend unit tests once without watch mode:

```bash
cd frontend
npm test -- --watch=false
```

Run frontend tests interactively during development:

```bash
cd frontend
npm test
```

The backend test project uses xUnit and Playwright. The frontend test project uses Angular's unit-test builder with Vitest and includes component rendering and HTTP integration-point tests.

## AI Tools and Prompt Summary

AI assistance was used as a development aid for repository exploration, implementation, test coverage, and documentation review. The main prompt themes were:

- Inspect the existing Angular and ASP.NET Core structure before making focused changes.
- Add component rendering tests and service HTTP contract tests while preserving existing APIs.
- Verify commands and diagnose working-directory issues when running frontend and .NET tests.
- Document the implemented features, local setup, API contract, assumptions, limitations, and future work.

All generated changes were reviewed against the source code and validated with the available test commands. AI output is not treated as a substitute for code review or security testing.

## Design Decisions

- Keep the frontend and backend as separate applications with a small JSON API boundary.
- Use Angular standalone components and the Angular development proxy for local integration.
- Keep game rules on the server so validation and game state are authoritative.
- Use SQLite and Entity Framework Core migrations to provide a low-friction local setup without requiring a separate database server.
- Represent board positions as indexes `1` through `9` in API requests while exposing named board-state cells in responses.
- Persist move history and scoreboard values so the UI can render completed games and accumulated results.
- Use focused unit tests for service HTTP contracts and component rendering, with backend tests covering game behavior and integration paths.

## Clarifications and Assumptions

- `GameMode.SinglePlayer` is represented by `1`; `GameMode.TwoPlayer` is represented by `2`.
- Scoreboards are keyed by game ID. They are not a global account-level leaderboard.
- The local SQLite database is intended for development and test use, not concurrent production hosting.
- The API allows the configured Angular development origin `http://localhost:4200` through CORS.
- No authentication, authorization, user accounts, or cross-device game identity is implemented.
- The test commands assume the required .NET SDK, Node.js packages, and browser dependencies are installed.

## Known Limitations

- No authentication or player profiles are available.
- SQLite is a local file database and is not configured for production-scale concurrent access.
- The frontend currently uses hard-coded display strings in templates rather than fully applying the translation map.
- The computer opponent uses a lightweight tactical strategy and is not a full minimax AI.
- API error handling is intentionally simple and does not expose a formal problem-details contract.
- The HTTPS development certificate may require machine-specific setup.

## Future Improvements

- Add real-time online multiplayer using SignalR or another WebSocket-based transport.
- Add authentication, player identities, and global or per-user scoreboards.
- Replace the single-player heuristic with configurable difficulty levels and a stronger game-playing algorithm.
- Add formal API documentation and generated client models from the OpenAPI contract.
- Introduce production database configuration, structured migrations, and deployment settings.
- Centralize frontend translations and add language selection.
- Expand accessibility testing, visual regression coverage, and API contract tests.
- Extend the GitHub Actions workflow with build, coverage, and deployment checks.
