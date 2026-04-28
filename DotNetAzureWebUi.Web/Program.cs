var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages support for the web UI.
builder.Services.AddRazorPages();

var app = builder.Build();

// Use production-safe error handling outside development.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Serve CSS, JavaScript, images, and static files.
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Map Razor Pages such as / and /Privacy.
app.MapRazorPages();

// Simple health endpoint for testing locally, in Azure, and in pipelines.
app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        app = "DotNet Azure Web UI",
        timeUtc = DateTime.UtcNow
    });
});

app.Run();
