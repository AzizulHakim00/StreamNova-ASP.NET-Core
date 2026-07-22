# StreamNova global access and automatic publishing

StreamNova now has three deployment targets so one free-hosting hostname cannot make the entire project unavailable.

## Why `runasp.net` may fail in another browser or network

A different Chrome account does not change DNS. Some networks and DNS resolvers may fail to resolve or may filter free shared-hosting subdomains. The application cannot repair a provider-level DNS failure from ASP.NET code.

The application has nevertheless been hardened for free hosting:

- HTTPS redirect and HSTS are no longer forced by default.
- Forwarded proxy headers are supported for Render and other reverse proxies.
- `/health` returns the ASP.NET health status.
- `/api/status` returns a small JSON status response.
- Static assets are compressed and cached.

Use `STREAMNOVA_FORCE_HTTPS=true` only on a host where HTTPS is confirmed and terminated correctly.

## Target 1: MonsterASP.NET backend

Every push to `main` runs `.github/workflows/monsterasp-free-hosting.yml` and deploys through WebDeploy.

Required GitHub repository secrets:

```text
WEBSITE_NAME
SERVER_COMPUTER_NAME
SERVER_USERNAME
SERVER_PASSWORD
```

Free-host URL:

```text
http://streamnova.runasp.net
```

If the hostname does not resolve on a network, test another DNS resolver or use either fallback below.

## Target 2: Render full ASP.NET application

`render.yaml` is a ready-to-use Blueprint with automatic deployment and `/health` monitoring.

One-time setup:

1. Sign in to Render.
2. Select **New → Blueprint**.
3. Connect `AzizulHakim00/StreamNova-ASP.NET-Core`.
4. Apply the Blueprint.
5. Enter the private `TMDB_READ_ACCESS_TOKEN` when Render asks for it.

After setup, every push to `main` deploys automatically. Render supplies a public HTTPS hostname. The free filesystem is temporary, so use a persistent disk or an external database before relying on user data in production.

## Target 3: GitHub Pages public mirror

The `public-site` folder is an installable static streaming catalog. It provides legal full open movies, official trailers, search, filters, favorites, a random-title button, responsive playback and an offline application shell.

One-time setup:

1. Open the GitHub repository.
2. Select **Settings → Pages**.
3. Under **Build and deployment**, choose **GitHub Actions** as the source.
4. Open **Actions → Deploy StreamNova Public Mirror** and run it once if a run is not already active.

Expected public address:

```text
https://azizulhakim00.github.io/StreamNova-ASP.NET-Core/
```

After Pages is enabled, changes in `public-site` pushed to `main` publish automatically.

## TMDB catalog configuration

Store the TMDB Read Access Token only as a hosting environment variable:

```text
TMDB_READ_ACCESS_TOKEN=your-private-read-access-token
```

Never commit the token to GitHub. The static public mirror does not need the token; its legal catalog is included in the repository.

## Permanent branded global address

For the most reliable result, buy a domain and connect it to Render or a paid host with custom-domain and HTTPS support. A custom domain also lets you move hosts later without changing the address your visitors use.
