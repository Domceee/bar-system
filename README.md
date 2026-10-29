# Bar System

A full-stack bar discovery and nightlife app. Users take a taste survey, get
personalised bar recommendations, reserve tables, plan bar-hopping routes with
friends, build cocktail recipes, and play blackjack.

## Features

- **Bars** – browse, create, edit and delete bars; find bars within a distance
  or matching a taste profile (`/bars`, `/bar-recommendation`)
- **Taste profile** – survey covering budget, drink type, flavour, strength,
  atmosphere, seating and more (`/taste`, `/taste-survey`, `/taste-view`)
- **Reservations** – book, cancel and end table reservations (`/reservations`)
- **Cocktail recipes** – compose recipes from ingredients, preview, optimise
  and publish them (`/recipes`)
- **Friends & messages** – add and invite friends, chat with them (`/friends`)
- **Bar routes** – generate and follow a multi-bar route, using Google Maps
  and OpenWeather data (`/bar-route`)
- **Blackjack** – a mini-game with a chip balance (`/blackjack`)

## Tech stack

| Layer    | Technology                                                   |
|----------|--------------------------------------------------------------|
| Frontend | React 19, TypeScript, Vite, React Router, lucide-react       |
| Backend  | ASP.NET Core Web API (.NET 9), Entity Framework Core 9       |
| Database | PostgreSQL 16 (Docker)                                       |
| External | Google Maps API, OpenWeather API                             |

## Project structure

```
bar-system/
├── backend/            ASP.NET Core API
│   ├── Controllers/    REST endpoints (api/bar, api/reservation, ...)
│   ├── Services/       Business logic + Google Maps / OpenWeather clients
│   ├── Models/ DTOs/   Entities and transfer objects
│   ├── Data/           EF Core DbContext
│   └── Migrations/     EF Core migrations
├── frontend/           React + Vite client
│   └── src/
│       ├── pages/      Route-level pages
│       ├── components/ Forms and shared UI
│       └── api/        Typed API clients
├── docker-compose.yml  PostgreSQL container
└── bar-system.sln
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS) and npm
- [Docker](https://www.docker.com/) (for PostgreSQL)

## Getting started

### 1. Start the database

```bash
docker compose up -d
```

PostgreSQL runs on **localhost:5433** (database `bardb`, user/password
`postgres`/`postgres`).

### 2. Configure API keys

Create `backend/appsettings.local.json` (git-ignored) with your keys:

```json
{
  "GoogleMaps": { "ApiKey": "YOUR_GOOGLE_MAPS_KEY" },
  "OpenWeather": { "ApiKey": "YOUR_OPENWEATHER_KEY" }
}
```

The keys are only needed for route generation and distance features.

### 3. Run the backend

```bash
cd backend
dotnet tool restore
dotnet ef database update
dotnet run
```

The API listens on **http://localhost:5029**. On first start it seeds demo
users, taste profiles, ~40 bars in and around Kaunas, drinks, tables and
ingredients. In development, the OpenAPI document is at `/openapi/v1.json`.

### 4. Run the frontend

```bash
cd frontend
npm install
npm run dev
```

Open **http://localhost:5173**. (The backend CORS policy only allows this
origin.)

## Database tasks

```bash
# Add a new migration (from backend/)
dotnet ef migrations add <MigrationName>
dotnet ef database update

# Reset the database completely (deletes all data)
docker compose down -v
docker compose up -d
dotnet ef database update
```

## Frontend scripts

| Command           | Description                       |
|-------------------|-----------------------------------|
| `npm run dev`     | Start the Vite dev server         |
| `npm run build`   | Type-check and build for prod     |
| `npm run lint`    | Run ESLint                        |
| `npm run preview` | Preview the production build      |
