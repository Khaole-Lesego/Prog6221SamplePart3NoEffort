namespace CybersecurityChatbot.Models;
public class ActivityLog
{
    public string ActionName { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime? EndTime { get; set; }
    public string Status { get; set; } = "Started";
}
