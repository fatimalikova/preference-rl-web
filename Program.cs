using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var clipsPath = Path.Combine(AppContext.BaseDirectory, "Data", "clips.json");
var clips = JsonSerializer.Deserialize<List<Clip>>(File.ReadAllText(clipsPath))!;
var random = new Random();

app.MapGet("/api/pair", () =>
{
    var a = clips[random.Next(clips.Count)];
    Clip b;
    do { b = clips[random.Next(clips.Count)]; } while (b.ClipId == a.ClipId);

    return Results.Ok(new
    {
        clipA = new { id = a.ClipId, angles = a.Angles },
        clipB = new { id = b.ClipId, angles = b.Angles }
    });
});

app.MapPost("/api/preference", (PreferenceSubmission submission) =>
{
    var prefsPath = Path.Combine(AppContext.BaseDirectory, "Data", "human_preferences.json");
    List<PreferenceSubmission> existing = new();
    if (File.Exists(prefsPath))
        existing = JsonSerializer.Deserialize<List<PreferenceSubmission>>(File.ReadAllText(prefsPath)) ?? new();

    existing.Add(submission);
    File.WriteAllText(prefsPath, JsonSerializer.Serialize(existing, new JsonSerializerOptions { WriteIndented = true }));

    return Results.Ok(new { saved = true, total = existing.Count });
});

app.Run();

record Clip(
    [property: JsonPropertyName("clip_id")] int ClipId,
    [property: JsonPropertyName("angles")] List<double> Angles,
    [property: JsonPropertyName("total_reward")] double TotalReward,
    [property: JsonPropertyName("source")] string Source
);

record PreferenceSubmission(int ClipAId, int ClipBId, int WinnerId);