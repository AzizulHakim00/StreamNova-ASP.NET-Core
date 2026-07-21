using System.Text.RegularExpressions;

namespace StreamNova.Services;

public static partial class VideoSourceResolver
{
    public const string None = "none";
    public const string Direct = "direct";
    public const string Hls = "hls";
    public const string YouTube = "youtube";
    public const string Vimeo = "vimeo";

    public static VideoPlaybackSource Resolve(string? rawUrl)
    {
        if (string.IsNullOrWhiteSpace(rawUrl) ||
            !Uri.TryCreate(rawUrl.Trim(), UriKind.Absolute, out var uri) ||
            uri.Scheme is not ("http" or "https"))
        {
            return VideoPlaybackSource.Unavailable;
        }

        var host = uri.Host.ToLowerInvariant();
        if (host is "youtu.be" or "youtube.com" or "www.youtube.com" or "m.youtube.com")
        {
            var id = ExtractYouTubeId(uri);
            return id is null
                ? VideoPlaybackSource.Unavailable
                : new VideoPlaybackSource(YouTube, $"https://www.youtube.com/embed/{id}?rel=0&playsinline=1", null);
        }

        if (host is "vimeo.com" or "www.vimeo.com" or "player.vimeo.com")
        {
            var id = uri.AbsolutePath
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault(segment => segment.All(char.IsDigit));

            return string.IsNullOrWhiteSpace(id)
                ? VideoPlaybackSource.Unavailable
                : new VideoPlaybackSource(Vimeo, $"https://player.vimeo.com/video/{id}", null);
        }

        var path = uri.AbsolutePath.ToLowerInvariant();
        if (path.EndsWith(".m3u8", StringComparison.Ordinal))
        {
            return new VideoPlaybackSource(Hls, uri.ToString(), "application/vnd.apple.mpegurl");
        }

        var mimeType = path.EndsWith(".webm", StringComparison.Ordinal)
            ? "video/webm"
            : path.EndsWith(".ogv", StringComparison.Ordinal) || path.EndsWith(".ogg", StringComparison.Ordinal)
                ? "video/ogg"
                : "video/mp4";

        return new VideoPlaybackSource(Direct, uri.ToString(), mimeType);
    }

    private static string? ExtractYouTubeId(Uri uri)
    {
        string? candidate = null;
        if (uri.Host.Equals("youtu.be", StringComparison.OrdinalIgnoreCase))
        {
            candidate = uri.AbsolutePath.Trim('/').Split('/')[0];
        }
        else
        {
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 && segments[0] is "embed" or "shorts" or "live")
            {
                candidate = segments[1];
            }
            else
            {
                var query = uri.Query.TrimStart('?')
                    .Split('&', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split('=', 2))
                    .FirstOrDefault(parts => parts.Length == 2 && parts[0] == "v");
                candidate = query is null ? null : Uri.UnescapeDataString(query[1]);
            }
        }

        return candidate is not null && YouTubeIdPattern().IsMatch(candidate)
            ? candidate
            : null;
    }

    [GeneratedRegex("^[A-Za-z0-9_-]{6,20}$")]
    private static partial Regex YouTubeIdPattern();
}

public sealed record VideoPlaybackSource(string Kind, string? Url, string? MimeType)
{
    public static VideoPlaybackSource Unavailable { get; } = new(VideoSourceResolver.None, null, null);
}