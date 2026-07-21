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
- Play direct MP4, WebM, OGG, HLS, YouTube, and Vimeo sources
- Watch included open-license short movies directly in the built-in player
- Save playback progress automatically for HTML5 video sources
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
- Configure poster, backdrop, MP4, WebM, OGG, HLS, YouTube, and Vimeo URLs
- Record source credit, license information, and an official source page
- View registered users and their activity totals
- Moderate movie reviews
- Review the support queue, publish resolutions, and notify customers

### Operations
- Built-in `/health` endpoint
- Docker multi-stage build
- GitHub Actions build validation
- Free MonsterASP.NET Windows/IIS package and WebDeploy workflow
- Azure App Service deployment workflow and guide
- Render Blueprint through `render.yaml`
- Configurable JSON data path through `STREAMNOVA_DATA_PATH`
- Thread-safe local persistence with no external database dependency
- No external NuGet packages required

## Playback sources

The player resolves media automatically from the URL entered by an administrator:

- Direct MP4, WebM, and OGG files use the browser's HTML5 video player.
- HLS `.m3u8` streams use native browser playback when available and HLS.js otherwise.
- YouTube watch, short, live, and share URLs are converted to privacy-conscious embedded playback.
- Vimeo page and player URLs are converted to embedded playback.
- Unsupported or missing sources display a clear unavailable message instead of fake playback.

The initial catalog includes four playable Blender open movies: **Big Buck Bunny**, **Elephants Dream**, **Sintel**, and **Tears of Steel**. Source and license information is displayed on the watch page. Only media that is owned, licensed, public-domain, open-license, or officially embeddable should be added.

## Technology stack

- .NET 10 LTS
- ASP.NET Core MVC
- C# and Razor Views
- Cookie authentication and role authorization
- PBKDF2 password hashing using built-in .NET cryptography
- Thread-safe JSON persistence
- HTML, CSS, and JavaScript
- HLS.js for adaptive HLS playback where native support is unavailable
- IIS, Docker, and GitHub Actions deployment support

## Demo accounts

| Role | Email | Password |
|---|---|---|
| Administrator | `admin@streamnova.local` | `Admin@123` |
| Viewer | `user@streamnova.local` | `User@123` |

The demo accounts and initial movie catalog are created automatically on first start. Existing databases are upgraded in place to add the open-movie catalog without deleting accounts, watchlists, reviews, or support data.

## Run locally

Install the .NET 10 SDK, open a terminal inside the project, and run:

```bash
dotnet restore
dotnet run
```

Open `http://localhost:5167`.

On Windows, `run.bat` can also be used. Visual Studio users can open `StreamNova.csproj` and press **Ctrl+F5**.

## Free ASP.NET deployment

The recommended no-subscription option is MonsterASP.NET's free ASP.NET hosting. The repository includes:

- IIS in-process hosting configuration
- A framework-dependent publish package
- A downloadable GitHub Actions ZIP artifact
- Automatic WebDeploy publishing after repository secrets are configured

Follow `MONSTERASP_FREE_DEPLOYMENT.md` for the complete setup. After the four hosting secrets are configured, every push to `main` automatically deploys the newest StreamNova version.

## Run with Docker

```bash
docker build -t streamnova .
docker run --rm -p 8080:8080 streamnova
```

Open `http://localhost:8080`. The health endpoint is available at `http://localhost:8080/health`.

## Azure App Service deployment

Azure deployment files remain available for accounts with an active Azure subscription. Follow `AZURE_DEPLOYMENT.md` for the complete setup.

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

The JSON state includes users, movies, watchlists, playback progress, reviews, plans, subscriptions, notifications, support tickets, and a schema version for safe data upgrades. Delete the generated file to reset the demo database. Back it up before a full manual deployment replacement.

## Project structure

```text
StreamNova/
├── Controllers/       # account, browse, subscription, notifications, support, admin
├── Models/            # catalog and platform domain models
├── Services/          # business logic, playback resolver, migrations, recommendations
├── ViewModels/        # UI-specific data contracts
├── Views/             # Razor interfaces
├── wwwroot/           # styling, scripts, and artwork
├── App_Data/          # generated JSON database
├── scripts/           # hosting provisioning scripts
├── .github/workflows/ # build and deployment pipelines
├── Dockerfile
├── render.yaml
├── Program.cs
└── StreamNova.csproj
```

## Brand and content note

StreamNova uses an original name, interface details, and source code. It is inspired by familiar streaming-platform interaction patterns but is not affiliated with Netflix or another streaming service. The project does not include or endorse unauthorized copyrighted movie streams.