# StreamNova

> Discover. Watch. Manage. Beyond the ordinary.

StreamNova is an original Netflix-inspired streaming and content-operations platform built with ASP.NET Core MVC and C#. It transforms a basic movie manager into a multi-module web application with secure accounts, personalized discovery, membership plans, customer support, notifications, playback tracking, reviews, and administrator analytics.

## Functional features

### Viewer experience
- Register and sign in with secure PBKDF2 password hashing
- Browse a responsive streaming-style catalog
- Search by title, genre, or description
- Filter movies by genre
- Open movie details and similar-title recommendations
- Receive personalized recommendations based on watchlist, playback, and review activity
- Add and remove titles from **My List**
- Play direct public MP4 videos or use the built-in demo player
- Save playback progress automatically
- Resume a movie from the saved timestamp
- See a personalized **Continue Watching** rail
- Rate movies from 1 to 5 stars
- Create, update, and delete personal reviews
- Update the profile display name and password

### Membership and account operations
- Compare Mobile, Standard, and Premium plans
- Activate or switch a subscription
- Cancel a subscription while retaining the current renewal date
- Track subscription status and simulated renewal dates
- Receive subscription notifications
- Open a centralized notification center
- Mark individual or all notifications as read

### Customer support
- Create support tickets for account, playback, billing, general, and content-request issues
- Select ticket priority
- Track open and resolved requests
- Read administrator responses inside the support center
- Receive a notification when a ticket is opened or resolved

### Administrator experience
- Role-protected administrator dashboard
- Catalog, user, watchlist, playback, and review statistics
- Active subscription and simulated monthly recurring revenue metrics
- Open support-ticket metrics
- Add, edit, and delete movies
- Set featured, trending, and new-release flags
- Configure poster, backdrop, and direct MP4 URLs
- View registered users and their activity totals
- Moderate movie reviews
- Review the support queue, publish resolutions, and notify customers

### Operations
- Built-in `/health` endpoint
- Docker multi-stage build
- GitHub Actions build validation
- Render Blueprint through `render.yaml`
- Configurable JSON data path through `STREAMNOVA_DATA_PATH`
- Thread-safe local persistence with no external database dependency
- No external NuGet packages required

## Technology stack

- .NET 10 LTS
- ASP.NET Core MVC
- C# and Razor Views
- Cookie authentication and role authorization
- PBKDF2 password hashing using built-in .NET cryptography
- Thread-safe JSON persistence
- HTML, CSS, and JavaScript
- Docker

## Demo accounts

| Role | Email | Password |
|---|---|---|
| Administrator | `admin@streamnova.local` | `Admin@123` |
| Viewer | `user@streamnova.local` | `User@123` |

The demo accounts and initial movie catalog are created automatically on first start.

## Run locally

Install the .NET 10 SDK, open a terminal inside the project, and run:

```bash
dotnet restore
dotnet run
```

Open `http://localhost:5167`.

On Windows, `run.bat` can also be used. Visual Studio users can open `StreamNova.csproj` and press **Ctrl+F5**.

## Run with Docker

```bash
docker build -t streamnova .
docker run --rm -p 8080:8080 streamnova
```

Open `http://localhost:8080`. The health endpoint is available at `http://localhost:8080/health`.

## Deploy on Render

The repository contains `render.yaml` for Docker deployment.

1. Sign in to Render.
2. Select **New → Blueprint**.
3. Connect `AzizulHakim00/StreamNova-ASP.NET-Core`.
4. Apply the Blueprint.

The free Render filesystem is temporary. For persistent production data, attach a disk and set:

```text
STREAMNOVA_DATA_PATH=/var/data/streamnova.json
```

## Data storage

Runtime data is stored by default in:

```text
App_Data/streamnova.json
```

The JSON state now includes users, movies, watchlists, playback progress, reviews, plans, subscriptions, notifications, and support tickets. Delete the generated file to reset the demo database.

## Project structure

```text
StreamNova/
├── Controllers/       # account, browse, subscription, notifications, support, admin
├── Models/            # catalog and platform domain models
├── Services/          # business logic and recommendation engine
├── ViewModels/        # UI-specific data contracts
├── Views/             # Razor interfaces
├── wwwroot/           # styling, scripts, and artwork
├── App_Data/          # generated JSON database
├── .github/workflows/
├── Dockerfile
├── render.yaml
├── Program.cs
└── StreamNova.csproj
```

## Brand note

StreamNova uses an original name, interface details, and source code. It is inspired by familiar streaming-platform interaction patterns but is not affiliated with Netflix or another streaming service.
