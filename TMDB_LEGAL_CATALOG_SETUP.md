# TMDB legal catalog setup

StreamNova can display a live catalog of movies, TV shows and anime using TMDB metadata. It plays official YouTube trailers, lists TV seasons and cast, and shows authorised streaming, rental and purchase providers for a selected country.

Commercial movies and episodes are not copied or proxied by StreamNova. Full in-app playback is reserved for media you own, properly license, or that is public-domain/open-license.

## 1. Create a TMDB token

1. Create or sign in to a TMDB account.
2. Open account **Settings → API**.
3. Request an API key for a non-commercial developer project.
4. Copy the **API Read Access Token**. Use the long Bearer token, not your account password.

Never commit this token to GitHub and never paste it into a public issue.

## 2. Add the token to MonsterASP.NET

1. Open the MonsterASP.NET control panel.
2. Go to **Websites → Manage streamnova.runasp.net**.
3. Open **Scripting → Environment Variables**.
4. Add this key:

```text
TMDB_READ_ACCESS_TOKEN
```

5. Paste the TMDB API Read Access Token as the value.
6. Save the environment variable.
7. Restart the website or application pool.

The application also accepts `TMDB_API_TOKEN` or the ASP.NET configuration key `Tmdb__ReadAccessToken`, but `TMDB_READ_ACCESS_TOKEN` is the recommended name for this project.

## 3. Test the catalog

Open:

```text
https://streamnova.runasp.net/discover
```

You should see live sections for:

- Trending movies and TV
- Popular movies
- Popular TV series
- Popular Japanese animation
- Search across movies, TV and anime
- Official trailers
- Cast and season metadata
- Country-specific legal provider availability

## 4. Local development

In PowerShell:

```powershell
$env:TMDB_READ_ACCESS_TOKEN="YOUR_TOKEN"
dotnet run
```

In Windows Command Prompt:

```bat
set TMDB_READ_ACCESS_TOKEN=YOUR_TOKEN
dotnet run
```

On Linux/macOS:

```bash
export TMDB_READ_ACCESS_TOKEN="YOUR_TOKEN"
dotnet run
```

## Attribution

The discovery pages include TMDB attribution and the required notice:

> This product uses the TMDB API but is not endorsed or certified by TMDB.

Provider availability is supplied through TMDB's JustWatch partnership and is attributed to JustWatch in the interface.
