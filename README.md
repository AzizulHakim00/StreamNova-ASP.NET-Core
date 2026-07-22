# StreamNova

> Discover. Watch. Manage. Beyond the ordinary.

StreamNova is a legal streaming-discovery and content-operations platform built with ASP.NET Core MVC and C#. It combines a full account-based application, a live TMDB movies/TV/anime catalog, open-license full movies, official trailers, legal provider discovery and a globally deployable public mirror.

## Viewer experience

- Register and sign in with secure PBKDF2 password hashing
- Browse a responsive streaming-style catalog
- Search and filter local titles by title, genre or description
- Explore live TMDB movies, TV shows and anime
- View cast, genres, ratings, seasons and episode counts
- Play official YouTube trailers inside StreamNova
- Find regional authorised streaming, rental and purchase providers
- Add titles to **My List** and receive personalised recommendations
- Save and resume HTML5 playback progress
- See a personalised **Continue Watching** rail
- Rate and review titles
- Manage profile information and password

## Legal playable catalog

The built-in player supports:

- Direct MP4, WebM and OGG media
- HLS `.m3u8` adaptive streams
- YouTube and Vimeo embeds
- Fullscreen and mobile playback
- Playback labels, source credits and licence information

The included full open-movie catalog now contains:

1. Big Buck Bunny
2. Elephants Dream
3. Sintel
4. Tears of Steel
5. Spring
6. Sprite Fright
7. Cosmos Laundromat
8. Coffee Run
9. Charge
10. WING IT!

The commercial demonstration catalog uses official promotional trailers and official rights-holder pages. Only owned, licensed, public-domain, open-license or officially embeddable media should be entered by an administrator.

## Live Movies / TV / Anime discovery

The `/discover` area uses the TMDB API for:

- Weekly trending titles
- Popular movies and TV
- Japanese animation discovery
- Multi-type search
- Poster and backdrop artwork
- Cast, metadata, seasons and episode counts
- Official trailers
- Country-specific legal provider availability

Set this private hosting environment variable to activate it:

```text
TMDB_READ_ACCESS_TOKEN=your-read-access-token
```

Never commit the token to GitHub. See `TMDB_LEGAL_CATALOG_SETUP.md`.

## Accounts and operations

### Membership

- Mobile, Standard and Premium plan comparison
- Subscription activation, switching and cancellation
- Simulated renewal dates and notifications

### Customer support

- Account, playback, billing, content and general support tickets
- Priority selection and ticket tracking
- Administrator resolutions and customer notifications

### Administration

- Role-protected dashboard
- Catalog, user, playback, watchlist, review, subscription and support statistics
- Movie creation, editing and deletion
- Featured, trending and new-release controls
- Legal source URL, licence, credit and official-page management
- Review moderation and support operations

## Reliability and global access

StreamNova has three complementary deployment targets:

### MonsterASP.NET backend

`.github/workflows/monsterasp-free-hosting.yml` builds and WebDeploys every push to `main` automatically. It validates deployment secrets, publishes the IIS package and attempts a public `/health` check.

```text
http://streamnova.runasp.net
```

HTTPS redirect is optional instead of forced, improving compatibility with free shared hosting. Enable it only where HTTPS is correctly configured:

```text
STREAMNOVA_FORCE_HTTPS=true
```

### Render full application

`render.yaml` defines an auto-deploying Docker Blueprint with `/health` monitoring. Create a Blueprint from this repository once; all later pushes to `main` deploy automatically.

### GitHub Pages public mirror

`public-site` is a standalone installable PWA with:

- 10 full legal open movies
- 6 official trailers
- Search and category filters
- Local favorites
- Surprise-me playback
- Responsive modal player
- Offline application shell

`.github/workflows/public-mirror-pages.yml` publishes it automatically after GitHub Pages is configured to use GitHub Actions.

Expected address:

```text
https://azizulhakim00.github.io/StreamNova-ASP.NET-Core/
```

Read `GLOBAL_ACCESS_SETUP.md` for the exact one-time setup and DNS explanation.

## Health and diagnostics

```text
GET /health
GET /api/status
```

`/health` is used by hosting platforms. `/api/status` returns a small JSON status payload with UTC time and application version.

## Technology stack

- .NET 10 and ASP.NET Core MVC
- C# and Razor Views
- Cookie authentication and role authorization
- Built-in PBKDF2 cryptography
- Thread-safe JSON persistence
- TMDB API and JustWatch-attributed provider data
- HTML, CSS and JavaScript
- HLS.js adaptive playback fallback
- IIS WebDeploy, Docker, Render and GitHub Pages
- GitHub Actions CI/CD

## Demo accounts

| Role | Email | Password |
|---|---|---|
| Administrator | `admin@streamnova.local` | `Admin@123` |
| Viewer | `user@streamnova.local` | `User@123` |

## Run locally

```bash
dotnet restore
dotnet run
```

Open `http://localhost:5167`. On Windows, `run.bat` can also be used.

## Docker

```bash
docker build -t streamnova .
docker run --rm -p 8080:8080 streamnova
```

Open `http://localhost:8080`.

## Data storage

The default database is:

```text
App_Data/streamnova.json
```

Override it with:

```text
STREAMNOVA_DATA_PATH=/your/path/streamnova.json
```

The JSON state includes users, movies, watchlists, progress, reviews, plans, subscriptions, notifications, support tickets and a schema version. Back it up before replacing a production deployment.

## Project structure

```text
StreamNova/
├── Controllers/       # viewer, account, discovery, support and admin endpoints
├── Models/            # platform domain models
├── Services/          # persistence, playback, TMDB, migrations and recommendations
├── ViewModels/        # UI-specific contracts
├── Views/             # Razor interfaces
├── wwwroot/           # ASP.NET styles, scripts and artwork
├── public-site/       # globally deployable PWA mirror
├── App_Data/          # generated JSON database
├── .github/workflows/ # build and deployment pipelines
├── Dockerfile
├── render.yaml
├── Program.cs
└── StreamNova.csproj
```

## Brand and content policy

StreamNova uses an original name, interface and source code. It is not affiliated with Netflix, TMDB, JustWatch or another streaming service. This product uses the TMDB API but is not endorsed or certified by TMDB. StreamNova does not include, scrape, proxy or endorse unauthorised copyrighted streams.
