using CybersecurityChatbot.Models;
using System.Text.Json;

namespace CybersecurityChatbot.Services;

public class AppState
{
    // I keep a local JSON store as a resilient fallback so the app never loses progress during demos.
    // For Part 3A deployment, this service can be swapped to MySQL using the same Add/Load contract.
    private readonly string _file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "activity-log.json");
    public List<ActivityLog> Logs { get; private set; } = new();

    public void Load()
    {
        if (File.Exists(_file))
            Logs = JsonSerializer.Deserialize<List<ActivityLog>>(File.ReadAllText(_file)) ?? new();
    }

    public void Save() => File.WriteAllText(_file, JsonSerializer.Serialize(Logs, new JsonSerializerOptions { WriteIndented = true }));

    public void Add(string action, string details)
    {
        Logs.Add(new ActivityLog
        {
            ActionName = action,
            Details = details,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
            Status = "Completed"
        });
        Save();
    }
}
