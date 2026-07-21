# StreamNova

> Discover. Watch. Manage. Beyond the ordinary.

StreamNova is an original Netflix-inspired streaming catalog and movie-management platform built with ASP.NET Core MVC and C#. It transforms a basic desktop movie manager into a responsive web application with secure accounts, personalized viewing, community reviews, and an administrator operations dashboard.

## Functional features

### Viewer experience
- Register and sign in with secure PBKDF2 password hashing
- Browse a responsive streaming-style catalog
- Search by title, genre, or description
- Filter movies by genre
- Open movie details and similar-title recommendations
- Add and remove titles from **My List**
- Play direct public MP4 videos or use the built-in demo player
- Save playback progress automatically
- Resume a movie from the saved timestamp
- See a personalized **Continue Watching** rail
- Rate movies from 1 to 5 stars
- Create, update, and delete personal reviews
- Update the profile display name
- Change the account password
- View account activity statistics

### Administrator experience
- Role-protected administrator dashboard
- Catalog, user, watchlist, playback, and review statistics
- Add, edit, and delete movies
- Set featured, trending, and new-release flags
- Configure poster, backdrop, and direct MP4 URLs
- View registered users and their activity totals
- Moderate any movie review

### Operations
- Built-in `/health` endpoint
- Docker multi-stage build
- GitHub Actions build validation
- Render Blueprint through `render.yaml`
- Configurable JSON data path through `STREAMNOVA_DATA_PATH`
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

The demo accounts and initial movie catalog are created automatically on the first application start.

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

The repository contains `render.yaml` for one-click Docker deployment.

1. Sign in to Render.
2. Select **New → Blueprint**.
3. Connect `AzizulHakim00/StreamNova-ASP.NET-Core`.
4. Apply the Blueprint.

The free Render filesystem is temporary. For permanent production data, attach a persistent disk and set:

```text
STREAMNOVA_DATA_PATH=/var/data/streamnova.json
```

## Data storage

By default, runtime data is stored in:

```text
App_Data/streamnova.json
```

Delete that generated file to reset the demo database. The file is excluded from Git so passwords, account activity, reviews, and local changes are not committed.

## Add a playable title

1. Sign in with the administrator account.
2. Open **Admin → Movies**.
3. Add or edit a movie.
4. Enter a direct public `.mp4` URL in **MP4 video URL**.
5. Save the movie and open its playback page.

When a movie has no direct video URL, StreamNova uses a functional simulated player while still saving and resuming viewing progress.

## Project structure

```text
StreamNova/
├── Controllers/
├── Models/
├── Services/
├── ViewModels/
├── Views/
├── wwwroot/
├── App_Data/
├── .github/workflows/
├── Dockerfile
├── render.yaml
├── Program.cs
└── StreamNova.csproj
```

## Brand note

StreamNova uses an original name, interface details, and source code. It is inspired by familiar streaming-platform interaction patterns but is not affiliated with Netflix or another streaming service.
