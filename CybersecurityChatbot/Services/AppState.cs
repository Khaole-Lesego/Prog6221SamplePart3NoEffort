using CybersecurityChatbot.Models;
using System.Text.Json;

namespace CybersecurityChatbot.Services;

public class AppState
{
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
        Logs.Add(new ActivityLog { ActionName = action, Details = details, StartTime = DateTime.Now, Status = "Completed", EndTime = DateTime.Now });
        Save();
    }
}
