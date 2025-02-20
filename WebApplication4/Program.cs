using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var httpClient = new HttpClient();

app.MapGet("/facts", async () =>
{
    string htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "facts.html");
    if (File.Exists(htmlPath))
    {
        string html = await File.ReadAllTextAsync(htmlPath);
        return Results.Text(html, "text/html");
    }
    return Results.NotFound("Файл facts.html не найден.");
});

app.MapGet("/api/fact", async () =>
{
    var response = await httpClient.GetStringAsync("https://uselessfacts.jsph.pl/random.json?language=en");
    using var doc = JsonDocument.Parse(response);
    var fact = doc.RootElement.GetProperty("text").GetString();
    return Results.Json(new { fact });
});

app.MapGet("/api/joke", async () =>
{
    var response = await httpClient.GetStringAsync("https://official-joke-api.appspot.com/random_joke");
    using var doc = JsonDocument.Parse(response);
    var setup = doc.RootElement.GetProperty("setup").GetString();
    var punchline = doc.RootElement.GetProperty("punchline").GetString();
    return Results.Json(new { joke = $"{setup} {punchline}" });
});

app.MapGet("/api/random-number", (int min = 1, int max = 100) =>
{
    int number = new Random().Next(min, max + 1);
    return Results.Json(new { number, min, max });
});

app.MapGet("/api/cat", async () =>
{
    var response = await httpClient.GetStringAsync("https://meowfacts.herokuapp.com/");
    using var doc = JsonDocument.Parse(response);
    var catFact = doc.RootElement.GetProperty("data")[0].GetString();
    return Results.Json(new { catFact });
});

app.Run();
