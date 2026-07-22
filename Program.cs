using StreamNova.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
var forceHttps = builder.Configuration.GetValue<bool>("Hosting:ForceHttps") ||
    string.Equals(Environment.GetEnvironmentVariable("STREAMNOVA_FORCE_HTTPS"), "true", StringComparison.OrdinalIgnoreCase);

builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
        options.Cookie.Name = "StreamNova.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = forceHttps
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.SameAsRequest;
    });
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddMemoryCache();
builder.Services.AddResponseCompression(options => options.EnableForHttps = true);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddHttpClient<TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("StreamNova/2.0 (+https://github.com/AzizulHakim00/StreamNova-ASP.NET-Core)");
});
builder.Services.AddHttpClient<TmdbShelfService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("StreamNova/2.0 (+https://github.com/AzizulHakim00/StreamNova-ASP.NET-Core)");
});

builder.Services.AddSingleton<JsonDatabase>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<UserMovieService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<SupportService>();
builder.Services.AddScoped<RecommendationService>();

var app = builder.Build();

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    if (forceHttps)
    {
        app.UseHsts();
    }
}

if (forceHttps)
{
    app.UseHttpsRedirection();
}

app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        const int oneWeek = 60 * 60 * 24 * 7;
        context.Context.Response.Headers.CacheControl = $"public,max-age={oneWeek}";
    }
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapGet("/api/status", () => Results.Ok(new
{
    service = "StreamNova",
    status = "ok",
    utc = DateTimeOffset.UtcNow,
    version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"
}));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<JsonDatabase>();
    var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();
    await database.InitializeAsync(passwordService);
}

app.Run();
