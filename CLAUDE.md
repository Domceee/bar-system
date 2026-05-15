# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Stack

- **Backend**: ASP.NET Core 9 Web API (`backend/`, `net9.0`, nullable enabled, implicit usings). EF Core 9 with Npgsql for PostgreSQL.
- **Frontend**: React 19 + TypeScript + Vite (`frontend/`), React Router 7, no UI library, plain `fetch`.
- **DB**: PostgreSQL 16 via `docker-compose.yml` — published on host port **5433** (container internal 5432), db `bardb`, user/pass `postgres/postgres`.
- **Solution**: `bar-system.sln` ties the backend project.

## Common commands

```powershell
# Start the database (required before running backend)
docker-compose up -d

# Backend (from backend/)
dotnet run                                  # http://localhost:5029 (https 7079)
dotnet ef migrations add <Name>             # new migration
dotnet ef database update                   # apply migrations

# Frontend (from frontend/)
npm install
npm run dev      # vite dev server on http://localhost:5173
npm run build    # tsc -b && vite build
npm run lint     # eslint .
```

There is no test project in this repo.

## Local config

`backend/Program.cs` loads `appsettings.local.json` (optional, gitignored) on top of `appsettings.json`. Put real `OpenWeather:ApiKey` and `GoogleMaps:ApiKey` there — the committed files have empty strings. The connection string defaults to the docker-compose Postgres on port 5433.

## Architecture

### Backend layering (`backend/`)

- `Controllers/` — thin ASP.NET controllers (`BarController`, `ReservationController`, `UserController`) under route `api/[controller]`. Controllers depend on service interfaces from `Services/Interfaces/` plus the two external-API wrappers directly.
- `Services/` — business logic (`BarService`, `UserService`) registered as scoped. External integrations are typed `HttpClient` wrappers registered via `AddHttpClient<T>`:
  - `GoogleMapsInterface` — Places API; used by `BarController.requestBarsWithinDistance` to find nearby bars and cross-reference with seeded bars by coordinate.
  - `OpenWeatherInterface` — weather lookup used by the bar recommendation flow.
- `Data/AppDbContext.cs` — `DbSet`s for `Users`, `Bars`, `Drinks`, `Tables`, `Reservations`, `TasteProfiles`, `TasteAnswers`. EF Core migrations live in `Migrations/`.
- `Models/` — entities. Bar/Drink attributes are modelled as C# enums (`BarDesign`, `BarAtmosphere`, `BarSeating`, `DrinkType`, `DrinkFlavor`, `DrinkStrength`, `DrinkFlavorBalance`) and serialized as integers.
- `DTOs/` — request/response shapes (e.g. `CreateBarDto`, `UpdateBarDto`, `BarsWithinDistanceRequest`).
- `Program.cs` — also performs **dev seeding on startup**: if `Bars` is empty it inserts ~41 Kaunas-area bars with their drinks and a handful of tables. Adding new seed data requires either wiping the DB or doing a manual insert/migration — the seed block only runs when the table is empty.
- CORS is locked to `http://localhost:5173` (`frontend` policy).

### Frontend (`frontend/src/`)

- `api/` — one module per backend controller (`barApi.ts`, `reservationApi.ts`, `userApi.ts`). All hit `http://localhost:5029/api/...` directly (hard-coded base URL — change here if backend port changes).
- `pages/` — route-level screens: `MainPage`, `BarList`, `BarRecPage` (recommendation flow using geolocation + within-distance endpoint), `ReservationList`, `TasteSurvey`.
- `components/` — shared UI (currently `BarForm.tsx`).
- `types/` — TS mirrors of backend DTOs/entities. Backend enums are represented as numeric unions / enums on this side.

### Bar recommendation flow (cross-cutting)

`POST /api/bar/within-distance` is the integration point: frontend sends `{ lat, lon, distanceMeters }`, backend calls Google Places, intersects results with seeded bars by coordinate match (`BarService.findBarsWithSameCoordinates`), and may enrich with `OpenWeatherInterface`. When changing this, touch all of: `BarController`, `GoogleMapsInterface`, `BarService`, `types/barRec.ts`, `BarRecPage.tsx`.

## Conventions

- Backend: primary-constructor controllers (`public class XController(IDep dep) : ControllerBase`), `async/await` everywhere, services return DTOs not entities.
- Enum properties on models are stored as ints in Postgres — when adding a new enum value, append to the end to avoid renumbering.
- Frontend API helpers use a shared `handleResponse<T>` that throws on non-OK; pages handle errors locally.
